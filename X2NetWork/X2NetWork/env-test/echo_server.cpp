#include <unistd.h>
#include <stdio.h>
#include <string.h>
#include "../include/X2PThreadServer.h"

class EchoServer: public X2PThreadServer
{
	public:
		int _nRecLen;
		int _nSendLen;
		void LogicFunc()
		{
			//printf("LogicFunc\n");
			sleep(1);
		}

		void OnConnected(NetHandle listenHandle, NetHandle netHandle)
		{
			printf("OnConnected:%d port:%d\n",netHandle.slot,netHandle.port);
		}

		void OnDisConnected(NetHandle netHandle,NetHandle listenHandle, long flag)
		{
			printf("OnConnected:%d form listen port:%d\n",netHandle.slot,listenHandle.port);
		}

		void OnListenFail(unsigned short port)
		{
		}
		void OnListenSuccess(NetHandle listenHandle)
		{
			_litenHandle = listenHandle;
			printf("Listen Success:%d\n",_litenHandle.port);
		}
		void OnReceived(NetHandle netHandle,X2Msg* pMsg)
		{
			printf("OnReceived:%d from %d\n",pMsg->MsgType(), netHandle.slot);
		}
		void OnSended(NetHandle netHandle,X2Msg* pMsg) 
		{
		}

		void OnStop(bool bStopped)
		{
		}
		void OnStopListenSuccess(NetHandle& handle)
		{
		}
		void OnStopListenFail(NetHandle& handle)
		{
		}
};

int main(int argc, char* argv[])
{
	unsigned short port = 11111;
	EchoServer server;
	server.SetPort(port);
	server.Start();
	server.Join();
	return 0;
}
