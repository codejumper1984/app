#ifdef EPOLL_CONTROL
#include<stdlib.h>
#include<errno.h>
#include "../include/X2Net.h"
#include "../include/X2EpollControl.h"


X2EpollControl::X2EpollControl():_nEpollID(-1),_epollFile(-1),_pWaitEvent(NULL)
{
	Init();
}

X2EpollControl::~X2EpollControl()
{
	_epollFile.Close();
	if(_pWaitEvent)
		delete[] _pWaitEvent;
    _pWaitEvent = NULL;
	_nEpollID = -1;
}

eNetResult X2EpollControl::NewReadAIO(NetAIO* pAIO)
{
	_epollFile.Add(pAIO->GetIdentifier(),EPOLLIN,pAIO);
}

eNetResult X2EpollControl::NewWriteAIO(NetAIO* pAIO)
{
	_epollFile.Add(pAIO->GetIdentifier(),EPOLLOUT,pAIO);
}

eNetResult X2EpollControl::RemoveAIO(NetAIO* pAIO)
{
	_epollFile.Del(pAIO->GetIdentifier());
}

eNetResult X2EpollControl::X2EpollControl::ReadAIO(NetAIO* pAIO)
{
	_epollFile.Mod(pAIO->GetIdentifier(),EPOLLIN,pAIO);
}


eNetResult X2EpollControl::X2EpollControl::WriteAIO(NetAIO* pAIO)
{
	_epollFile.Mod(pAIO->GetIdentifier(),EPOLLOUT,pAIO);
}

eNetResult X2EpollControl::ReadWriteAIO(NetAIO* pAIO)
{
	_epollFile.Mod(pAIO->GetIdentifier(),EPOLLOUT | EPOLLOUT, pAIO);
}


int X2EpollControl::Init()
{
	_epollFile.Create(MAX_EPOLL_NUM);
	_nEpollID = _epollFile.ID();

	if(_nEpollID <= 0)
		return EPOLLCTL_CRTPOLLFAILED; 
    _pWaitEvent = new epoll_event[MAX_EPOLL_NUM];
	if(NULL == _pWaitEvent)
		return EPOLLCTL_CRTEVENTFAILED;
	return EPOLLCTL_SUCCESS;
}

epoll_event* X2EpollControl::GetEpollEvent(int idx)
{
    if(idx >= 0 && idx <MAX_EPOLL_NUM)
        return _pWaitEvent + idx;
    return NULL;
}

int X2EpollControl::Wait(int nWaitTime)
{

	int nCount = _epollFile.Wait(nWaitTime,_pWaitEvent, MAX_EPOLL_NUM);
	if(nCount < 0 )
	{
		if(errno != EINTR)
		{
			return nCount;
		}
	}
	else
		return nCount;

}

int X2EpollControl::RunControlling()
{
	int nEvtCot =Wait(1);
	for( int i = 0; i <nEvtCot;i++)
	{
		epoll_event* pEvent = GetEpollEvent(i);
		if( NULL != pEvent)
		{
			NetAIO* pAIO = (NetAIO*)pEvent->data.ptr;
			if(pAIO != NULL)
			{
				if(pAIO->Handle.type == NETHANDLE_LISTENSOCKET)
				{
					NetAIO* pAcceptedAIO = X2Net::Instance().NewAIO();
					if( X2NET_SUCCESS == pAIO->OnAccept(pAcceptedAIO))
					{
						pAcceptedAIO->OnAccepted();
					}
				}
				else if(pAIO->Handle.type == NETHANDLE_CLIENTSOCKET)
				{
					if( pEvent->events & EPOLLIN)
					{
						pAIO->OnRead();
					}

					if( pEvent->events & EPOLLOUT)
					{
						pAIO->OnSend();
					}

					if( pEvent->events & EPOLLERR)
					{
						pAIO->OnError();
						pAIO->Close();
						pAIO->OnClose(true);
					}
				}
			}
			
		}
	}
}

#endif
