#ifdef EPOLL_CONTROL
#include<unistd.h>
#include "../include/X2EpollFile.h"

X2EpollFile::X2EpollFile(int nMaxPoolEvent):_nEpollID(-1),_nMaxPoolEvent(nMaxPoolEvent)
{
}
X2EpollFile::~X2EpollFile()
{
	if(0 != _nEpollID)
		close(_nEpollID);
	_nEpollID = 0;
	_nMaxPoolEvent = 0;
}

X2EpollFile::X2EpollFileResult X2EpollFile::Close()
{
	if( close(_nEpollID) )
		return X2EPOLLFILE_FAILED;
	_nEpollID = 0;
	return X2EPOLLFILE_SUCCESS;

}

X2EpollFile::X2EpollFileResult X2EpollFile::Create(int nMaxEvent)
{
	if(nMaxEvent != 0 )
	{
		_nMaxPoolEvent = nMaxEvent;
	}
	_nEpollID = epoll_create(_nMaxPoolEvent);
	if(_nEpollID > 0)
		return X2EPOLLFILE_SUCCESS;
	else
		return X2EPOLLFILE_FAILED;
}

X2EpollFile::X2EpollFileResult X2EpollFile::Add(int fd, int events,void* pData)
{
    epoll_event event;
    event.events = events;
    event.data.ptr = pData;

    if( epoll_ctl(_nEpollID, EPOLL_CTL_ADD, fd, &event))
    {
		return X2EPOLLFILE_FAILED;
    }

	return X2EPOLLFILE_SUCCESS;
}

X2EpollFile::X2EpollFileResult X2EpollFile::Mod(int fd, int events,void* pData)
{
    epoll_event event;
    event.events = events;
    event.data.ptr = pData;

    if( epoll_ctl(_nEpollID, EPOLL_CTL_MOD, fd, &event))
    {
		return X2EPOLLFILE_FAILED;
    }
	return X2EPOLLFILE_SUCCESS;
}

int X2EpollFile::Wait(int nWaitTime, struct epoll_event* pEvent,int nEventNum)
{
	return epoll_wait(_nEpollID,pEvent, nEventNum, nWaitTime);
}
X2EpollFile::X2EpollFileResult X2EpollFile::Del(int fd)
{
    epoll_event event;
    if( epoll_ctl(_nEpollID,EPOLL_CTL_DEL, fd, &event))
    {
		return X2EPOLLFILE_FAILED;
    }
	return X2EPOLLFILE_SUCCESS;
}
#endif