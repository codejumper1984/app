#include<stdlib.h>
#include "../include/X2NetInterface.h"
#include "../include/X2Server.h"
X2Server::X2Server(unsigned short unPort): _unPort(unPort)
{
}

int X2Server::Init()
{
	X2NetInterface::Init();
    return 0;
}

int X2Server::StopListen()
{
	return X2NetInterface::StopListen(_litenHandle);
}

int X2Server::Listen()
{
	return X2NetInterface::Listen(_unPort);
}

int X2Server::Start()
{
	return 0;
}

int X2Server::Stop()
{
	return 0;
}


void X2Server::LoopFunc()
{
	NetEvent evt;
	while( X2NET_SUCCESS == X2NetInterface::GetNextNetEvent(evt) )
	{
		OnEvent(evt);
	}
	LogicFunc();
}

void X2Server::OnConnected(NetHandle netHandle,NetHandle listenHandle)
{

}

void X2Server::OnReceived(NetHandle netHandle,X2Msg* pMsg)
{
}

void X2Server::OnStop(bool bStopped)
{

}

void X2Server::OnSended(NetHandle netHandle, X2Msg* pMsg)
{

}


void X2Server::OnEvent(NetEvent& evt)
{
	switch(evt.type)
	{
	case NETEVENT_LISTEN_SUCCESS :
		{
			NetHandle listenHandle;
			listenHandle = evt.handle1;
			OnListenSuccess(listenHandle);
		}
		break;
	case NETEVENT_LISTEN_FAIL :
		{
			OnListenFail(evt.data);
		}
		break;
	case NETEVENT_SEND_SUCCESS:
		{
			OnSended(evt.handle1,(X2Msg*)evt.data);
		}
		break;
	case NETEVENT_READ_SUCCESS:
		{
			OnReceived(evt.handle1,(X2Msg*)evt.data);
		}
		break;
	case NETEVENT_ACCEPTED_SUCCESS:
		{
			OnConnected(evt.handle1,evt.handle2);
		}
		break;
	case NETEVENT_CLOSE_SUCCESS:
		{
			OnDisConnected(evt.handle1,evt.handle2,evt.data);
		}
		break;
	case NETEVENT_STOPLISTEN_SUCCES:
		{
			OnStopListenSuccess(evt.handle1);
		}
		break;
	case NETEVENT_STOPLISTEN_FAIL:
		{
			OnStopListenFail(evt.handle1);
		}
		break;
	}
}

int X2Server::Send(NetHandle netHandle, X2Msg* pMsg)
{
	return X2NetInterface::Send(netHandle,pMsg);
}

void X2Server::LogicFunc()
{
}

void X2Server::OnDisConnected(NetHandle netHandle,NetHandle listenHandle, long flag)
{
}

void X2Server::OnListenFail(unsigned short port)
{
}

void X2Server::OnListenSuccess(NetHandle listenHandle)
{
	_litenHandle = listenHandle;
}

void X2Server::OnStopListenSuccess( NetHandle& handle )
{

}

void X2Server::OnStopListenFail( NetHandle& handle )
{

}
