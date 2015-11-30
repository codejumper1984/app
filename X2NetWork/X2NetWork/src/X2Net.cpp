#include <stddef.h>
#include "../include/X2Net.h"
#include "../include/IO.h"
#define DEF_MSGQUE_SIZE 1024
#define DEF_AIO_SLOTNUM 4 * 1024
#define DEF_MSGQUE_RESERVELEN 32
#ifdef EPOLL_CONTROL
#include "../include/X2EpollSocket.h"
#include "../include/X2EpollControl.h"
#endif
X2Net X2Net::instance;

X2Net& X2Net::Instance()
{
	return instance;
}

eNetResult X2Net::PushServerEvent(ServerEvent& evt)
{
	return _msgQueueFromServer.PushData(&evt,sizeof(evt));
}

eNetResult X2Net::GetNextNetEvent(NetEvent& evt)
{
	return _msgQueueToServer.PopData(&evt,sizeof(evt));
}

X2Net::X2Net():_msgQueueFromServer(DEF_MSGQUE_SIZE,DEF_MSGQUE_RESERVELEN),_msgQueueToServer(DEF_MSGQUE_SIZE,DEF_MSGQUE_RESERVELEN),_pAioSlot(NULL),_pDecoder(NULL)
{

}

eNetResult X2Net::Init()
{
	_pDecoder = new X2Decoder;
	_pAioSlot = new NetAIO* [DEF_AIO_SLOTNUM];
	for(int i = 0;i < DEF_AIO_SLOTNUM;i++)
	{
		_freeSlotList.push_back(i);
#ifdef EPOLL_CONTROL
		_pAioSlot[i] = new X2EpollSocket();
#else
		_pAioSlot[i] = NULL;
#endif
	}
#ifdef EPOLL_CONTROL
		_pAIOController = new X2EpollControl();
		Start();
#else
		_pAIOController = NULL;
#endif
		X2MsgManager::RegisterMsg();
	return X2NET_SUCCESS;
}

int X2Net::ProcessServerEvent()
{
	int procCot = 0;
	ServerEvent evt;
	while ( X2NET_SUCCESS == _msgQueueFromServer.PopData(&evt, sizeof( evt )) )
	{
		OnServerEvent(evt);
		procCot++;
	}
	return procCot;
}

eNetResult X2Net::OnServerSend(NetHandle handle, X2Msg*pMsg)
{
	if(handle.slot < DEF_AIO_SLOTNUM && handle.slot >= 0)
	{
		NetAIO* pNetAIO = _pAioSlot[handle.slot];
		if(NULL != pNetAIO)
		{
			if( X2NET_SUCCESS == pNetAIO->Send(pMsg) )
			{
				return X2NET_SUCCESS;
			}
		}
	}
	OnSendFailed(handle,pMsg);
	return X2NET_FAILED;
}

void X2Net::OnServerEvent( ServerEvent& evt )
{
	switch(evt.type)
	{
	case SERVEREVENT_STOPLISTEN:
		{
			short port = (short)evt.data;
			OnServerStopListen(port);
		}
		break;
	case SERVEREVENT_LISTEN:
		{
			short port = (short)evt.data;
			OnServerListen(port);
		}
		break;
	case SERVEREVENT_SEND:
		{
			OnServerSend(evt.handle1,(X2Msg*)evt.data);
		}
		break;

	}
}


void* X2Net::RunFunc()
{
	ProcessServerEvent();
	if(NULL != _pAIOController)
		_pAIOController->RunControlling();
	return NULL;

}

NetAIO* X2Net::NewAIO()
{
	if(_freeSlotList.size() > 0)
	{
		int nSlot = _freeSlotList.front();
		NetAIO* pNetAIO = _pAioSlot[nSlot];
		if(NULL == pNetAIO)
		{
			pNetAIO = NULL;
		}
		pNetAIO->Handle.type = NETHANDLE_CLIENTSOCKET ; 
		pNetAIO->Handle.slot = nSlot;
		pNetAIO->SetListenner(this);
		pNetAIO->SetController(_pAIOController);
		_freeSlotList.pop_front();
		return pNetAIO;
	}
	return	NULL;
}

NetAIO* X2Net::NewAIO(unsigned short port)
{
	NetAIO* pNetAIO = NULL;
	if(_freeSlotList.size() > 0)
	{
		int nSlot = _freeSlotList.front();
		pNetAIO = _pAioSlot[nSlot];
		if(NULL != pNetAIO)
		{
			pNetAIO->Handle.type = NETHANDLE_LISTENSOCKET ; 
			pNetAIO->Handle.slot = nSlot;
			pNetAIO->SetPort(port);
			pNetAIO->SetListenner(this);
			pNetAIO->SetController(_pAIOController);
			if( X2NET_FAILED == pNetAIO->Init())
			{
				pNetAIO->Release();
				return NULL;
			}
			_freeSlotList.pop_front();
		}
	}
	return	pNetAIO;
}

void X2Net::ReleaseAIO(NetAIO* pAIO)
{
	if( NULL != pAIO )
	{
		int slot = pAIO->Handle.slot;
		pAIO->Release();
		_freeSlotList.push_back(slot);
	}
}

eNetResult X2Net::OnServerStopListen(short port)
{
	ListenPortHandleMap::iterator ite = _listenPortHandleMap.find(port);
	if( ite != _listenPortHandleMap.end())
	{
		NetHandle handle = ite->second;
		NetAIO* pAIO = _pAioSlot[handle.slot];
		short port = handle.port;
		pAIO->Close();
		ReleaseAIO(pAIO);
		return X2NET_SUCCESS;
	}
	else
	{
		return X2NET_FAILED;
	}
}

eNetResult X2Net::OnServerListen(short port)
{
	ListenPortHandleMap::iterator ite = _listenPortHandleMap.find(port);
	if( ite == _listenPortHandleMap.end())
	{
		NetAIO* p = NewAIO(port);
		if( X2NET_FAILED == p->Listen())
		{
			ReleaseAIO(p);
			return X2NET_FAILED;
		}
		_listenPortHandleMap[port] = p->Handle;
		return X2NET_SUCCESS;
	}
	else
	{
		NetHandle handle;
		handle.port = port;
		OnListenFail(handle);
		return X2NET_FAILED;
	}
}

void  X2Net::OnListenSuccess(NetHandle& handle)
{
	NetEvent evt;
	evt.type = NETEVENT_LISTEN_SUCCESS;
	evt.handle1 = handle;
	_msgQueueToServer.PushData(&evt,sizeof(evt));

}

void X2Net::OnListenFail(NetHandle& handle)
{
	NetEvent evt;
	evt.type = NETEVENT_LISTEN_FAIL;
	evt.data = handle.port;
	_msgQueueToServer.PushData(&evt,sizeof(evt));
}

void X2Net::OnSendSuccess(NetHandle& netHandle,X2Msg* pMsg)
{
	NetEvent evt;
	evt.type = NETEVENT_SEND_SUCCESS;
	evt.data = (long)pMsg;
	evt.handle1 = netHandle;
	_msgQueueToServer.PushData(&evt,sizeof(evt));
}

void X2Net::OnSendFailed(NetHandle& netHandle,X2Msg* pMsg)
{
	NetEvent evt;
	evt.type = NETEVENT_SEND_FAIL;
	evt.data = (long)pMsg;
	evt.handle1 = netHandle;
	_msgQueueToServer.PushData(&evt,sizeof(evt));

}

int X2Net::OnRead(NetHandle& netHandle,const char* pData, int nDataSize)
{
	X2ByteStream stream((char*)pData,nDataSize);
	int nLen = _pDecoder->Decode(stream);
	X2Msg* pMsg =_pDecoder->GetDecodeMsg(); 
	while(NULL != pMsg)
	{
		NetEvent evt;
		evt.type = NETEVENT_READ_SUCCESS;
		evt.data = (long)pMsg;
		evt.handle1 = netHandle;
		_msgQueueToServer.PushData(&evt,sizeof(evt));
	}
	return nLen;

}

void X2Net::OnClosed(NetHandle& netHandle,bool bPositive)
{
	NetEvent evt;
	evt.type = NETEVENT_CLOSE_SUCCESS;
	evt.handle1 = netHandle;
	if(bPositive)
		evt.data = 1;
	else
		evt.data = 0;
	_msgQueueToServer.PushData(&evt,sizeof(evt));
}

void X2Net::OnAccepted(NetHandle& netHandle,NetHandle& listenHandle,eNetResult eRes)
{
	NetEvent evt;
	if(eRes == X2NET_SUCCESS)
	{
		evt.type = NETEVENT_ACCEPTED_SUCCESS;
	}
	evt.handle1 = netHandle;
	evt.handle2= listenHandle;
	_msgQueueToServer.PushData(&evt,sizeof(evt));
}

void X2Net::OnStopListenFail(NetHandle& netHandle)
{
	NetEvent evt;
	evt.type = NETEVENT_STOPLISTEN_SUCCES;
	evt.handle1 = netHandle;
	_msgQueueToServer.PushData(&evt,sizeof(evt));

}

void X2Net::OnStopListenSuccess(NetHandle& netHandle)
{
	NetEvent evt;
	evt.type = NETEVENT_STOPLISTEN_FAIL;
	evt.handle1 = netHandle;
	_msgQueueToServer.PushData(&evt,sizeof(evt));
}
