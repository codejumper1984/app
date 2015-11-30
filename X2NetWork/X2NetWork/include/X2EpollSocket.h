#ifndef __X2_EPOLLSOCKET_H__
#define __X2_EPOLLSOCKET_H__
#include<stdint.h>
#include<list>
#include "IO.h"
#include "X2MsgManager.h"
#define MAX_LISTEN_NUM 128
#define X2_MAX_IP_LEN 128
enum X2EpollSocketResult
{
    EPOLLSOCK_SUCCESS = 0,
    EPOLLSOCK_READFULL = 1,
    EPOLLSOCK_CLOSE,
    EPOLLSOCK_SENDFULL,
    EPOLLSOCK_FAILED,
    EPOLLSOCK_INITERROR,
	EPOLLSOCK_GETFLERROR,
	EPOLLSOCK_SETFLERROR,
	EPOLLSOCK_BINDERROR,
	EPOLLSOCK_READEMPTY,
};

enum SOCKET_TYPE
{
	TCP,
};

struct SendMeta
{
	X2Msg* pMsg;
	int nSendLen;
};

class X2EpollSocket: public NetAIO
{
	public:

        X2EpollSocket(unsigned short port = 0);

		eNetResult Listen();
		eNetResult Accept(NetIO*);

		eNetResult Send(X2Msg* pMsg);
		eNetResult Read(const char* pData, uint32_t dataSize);

		eNetResult Init();
		eNetResult Release();
		eNetResult Close();

		void SetAddress(const char* lpSzAddr);
		void SetPort(unsigned short unPort);
		void SetIdentifier(int id);
		int GetIdentifier()
		{
			return _nSocket;
		}

		int OnSend();
		int OnRead();
		int OnAccept(NetIO* pNetIO);
		int OnAccepted();
		int OnError();
		int OnClose(bool bPositive);


		int ReleaseReadBuffer();

		typedef std::list<SendMeta> MsgQueue;
		int DataLen()
		{
			return _nRecvBuffLen;
		}
		const char* DataBuffer()
		{
			return _ReadBuffer;
		}
        bool IsReadBuffFull();
	private:
        eNetResult InitSocket();

		MsgQueue _sendQueue;

        char _pszIP[X2_MAX_IP_LEN];
        unsigned short _unPort;
        int _nSocket;

        char* _ReadBuffer;
        int _nRecvBuffLen;

        char* _SendBuffer;
		X2ByteStream _stream;
};
#endif
