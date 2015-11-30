#include <errno.h>
#include <arpa/inet.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include<sys/socket.h>
#include<netinet/in.h>
#include "../include/X2PThread.h"
#include "../include/X2MsgManager.h"
#include "../include/X2ByteStream.h"
class MyThread1:public X2PThread
{
	public:
		MyThread1(ushort unPort):_nSendLen(0),_nRecLen(0),stream(_codeBuff,sizeof(_codeBuff))
		{
			_unPort = unPort;
			for( int i = 0; i < 100;i++)
				recBuff[i] = i;
		}

		int _nSendLen;
		int _nRecLen;

		int sockid;
			char recBuff[100];
		bool Send()
		{
			int nSendedLen = 0;
			X2HelloMsg msg;
			stream.clear();
			msg.Serilize(stream);
			char* pBuff = _codeBuff;
			int nSendLen = stream.Distance();

			while( nSendedLen < nSendLen )
			{
				int nNeedLen = nSendLen  - nSendedLen;
				int nLen = send(sockid, pBuff,nNeedLen,0);
				if(nLen > 0)
				{
					nSendedLen += nLen;
					pBuff += nLen;
					_nSendLen += nLen;
				}
				else
				{
					return false;
				}
			}
			printf("send len:%d\n",_nSendLen);
			return true;
		}

		bool Recv()
		{
			char recBuff[100] ={0};
			int nLen = recv(sockid, recBuff, sizeof(recBuff),0);
			if(nLen <= 0)
			{
				printf("read failed errno:%d",errno);
				return false;
			}
			else
				_nRecLen += nLen;
			printf("recv len:%d\n",_nRecLen);
			return true;
		}

		void* RunFunc()
		{
			sockid = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
			sockaddr_in addr;
			addr.sin_port = htons(_unPort);
			addr.sin_family = AF_INET;
			inet_aton("127.0.0.1",&(addr.sin_addr));
			printf("begin to connect\n");
			if(!connect(sockid,(sockaddr*)&addr,sizeof(addr)))
			{
				printf("connected succcss\n");
				bool bCont = true;
				while(bCont)
				{
					if( Send() &&  Recv() )
					{
					}
					else
					{
						bCont = false;
					}
				}

			}
			else
			{
				printf("connected failed %d\n",errno);
			}
			CloseSock();
			Stop();
		}
		void CloseSock()
		{
			close(sockid);
		}
		ushort _unPort;
		char _codeBuff[1024];
		X2ByteStream stream;
};

int main(int argc, char* argv[])
{
	MyThread1 thread(11111);
	thread.Start();
	thread.Join();
	return 0;
}
