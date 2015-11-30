#ifdef EPOLL_CONTROL
#include <arpa/inet.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <fcntl.h>
#include <netinet/in.h>
#include <string.h>
#include <sys/socket.h>
#include "../include/X2EpollSocket.h"

#define MAX_RECVBUFF_LEN 1024
#define MAX_SENDBUFF_LEN 512


X2EpollSocket::X2EpollSocket(unsigned short port): _unPort(port),_SendBuffer(NULL),_ReadBuffer(NULL),_nRecvBuffLen(0),_stream(_SendBuffer,MAX_SENDBUFF_LEN)
{
}

eNetResult X2EpollSocket::Init()
{
	return InitSocket();
}

int X2EpollSocket::OnAccepted()
{
	if(NULL != _pAIOController)
		_pAIOController->NewReadAIO(this);
}

eNetResult X2EpollSocket::Accept(NetIO* pNetIO)
{
	sockaddr_in addr;
	socklen_t len;
	int sockid = accept(_nSocket, (sockaddr*)&addr, &len );
	eNetResult ret;
	if(sockid < 0)
	{
		ret = X2NET_FAILED;
	}
	else
	{
		char* pIP = inet_ntoa(addr.sin_addr);
		pNetIO->SetPort(addr.sin_port);
		pNetIO->SetAddress(pIP);
		pNetIO->SetIdentifier(sockid);
		ret = pNetIO->Init();
	}

	if(NULL != _pAIOListenner)
	{
		_pAIOListenner->OnAccepted(pNetIO->Handle,Handle,ret);
	}
	return ret;

}

eNetResult X2EpollSocket::Listen()
{

	struct sockaddr_in addr;
	memset(&addr,0,sizeof(addr));
	addr.sin_port = htons(_unPort);
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_family = AF_INET;

	if(  bind(_nSocket,(struct sockaddr*)&addr,sizeof(addr))  == 0)
	{
		if(  listen(_nSocket,MAX_LISTEN_NUM )  == 0)
		{
			this->Handle.port = _unPort;
			if(NULL != _pAIOController)
				_pAIOController->NewReadAIO(this);
			if(NULL != _pAIOListenner)
				_pAIOListenner->OnListenSuccess(this->Handle);
			return X2NET_SUCCESS;
		}
	}
	if(NULL != _pAIOListenner)
		_pAIOListenner->OnListenFail(this->Handle);
	return X2NET_FAILED;
}

int X2EpollSocket::OnAccept(NetIO* pNetIO)
{
	return Accept(pNetIO);
}

eNetResult X2EpollSocket::Send(X2Msg* pMsg)
{
	if(_nSocket < 0)
		return X2NET_FAILED;
	SendMeta meta;
	meta.pMsg = pMsg;
	meta.nSendLen = 0;
	_sendQueue.push_back(meta);
	if(_sendQueue.size() == 1 )
	{
		if( NULL != _pAIOController)
			_pAIOController->ReadAIO(this);
	}
	return X2NET_SUCCESS;
}

eNetResult X2EpollSocket::Read(const char* pData, uint32_t dataSize)
{
	return X2NET_SUCCESS;
}

int X2EpollSocket::ReleaseReadBuffer()
{
	_nRecvBuffLen = 0;
}

int X2EpollSocket::OnRead()
{
    if(IsReadBuffFull())
    {
        return EPOLLSOCK_READFULL;
    }

    while(true)
    {
        char* pReadHead = _ReadBuffer + _nRecvBuffLen;
        //int nReadLen = recv(_nSocket,_ReadBuffer, MAX_RECVBUFF_LEN - _nRecvBuffLen,0);
        int nReadLen = read(_nSocket,_ReadBuffer, MAX_RECVBUFF_LEN - _nRecvBuffLen);
        if(nReadLen > 0)
        {
            if(IsReadBuffFull() )
            {
				int nLeftLen = _pAIOListenner->OnRead(Handle,_ReadBuffer,_nRecvBuffLen);
				_nRecvBuffLen = nReadLen;
                // several continue reading lead to full
				if(IsReadBuffFull())
					return EPOLLSOCK_READFULL;
            }
            _nRecvBuffLen += nReadLen;
        }
        else if(nReadLen < 0 )
        {
            if( errno == EAGAIN ) 
            {
				if(NULL != _pAIOListenner)
				{
					int nLeftLen = _pAIOListenner->OnRead(Handle,_ReadBuffer,_nRecvBuffLen);
					_nRecvBuffLen = nReadLen;
				}
                return EPOLLSOCK_SUCCESS;
            }
            else if(errno == EINTR )
            {
				continue;
                //return EPOLLSOCK_FAILED;
            }
            else
            {
                return EPOLLSOCK_FAILED;
            }
        }
        else if(nReadLen == 0 )
        {
            return EPOLLSOCK_READEMPTY;
        }
    }
	return EPOLLSOCK_SUCCESS;
}

int X2EpollSocket::OnError()
{
	return 0;
}

int X2EpollSocket::OnClose(bool bPositive)
{
	if(NULL != _pAIOListenner )
	{
		_pAIOListenner->OnClosed(this->Handle,bPositive);
	}
	return X2NET_SUCCESS;
}

eNetResult X2EpollSocket::Close()
{
	if( 0 == _pAIOController->RemoveAIO(this))
		return X2NET_SUCCESS;
	return X2NET_FAILED;
}

int X2EpollSocket::OnSend()
{
	bool bCont = true;
	int ret = EPOLLSOCK_SUCCESS;
	while( _sendQueue.size() > 0 )
	{
		SendMeta& meta = _sendQueue.front();
		if(meta.nSendLen == 0 && _stream.Distance() == 0)
		{
			meta.pMsg->Serilize(_stream);
		}
		int nNeedSendLen = _stream.Distance() - meta.nSendLen;
		char* pData = _stream.Begin() + meta.nSendLen;
		while( nNeedSendLen > 0 )
		{
			int nSendLen = 0;
			if( ( nSendLen = send(_nSocket, pData, nNeedSendLen,0) ) > 0)
			{
				nNeedSendLen -= nSendLen;
				meta.nSendLen += nSendLen;
				pData += nSendLen;
			}
			else
			{
				if( errno == EAGAIN ) 
				{
					bCont = false;
					break;
				}
				else if(errno == EINTR )
				{
					continue;
				}
				else
				{
					ret = EPOLLSOCK_FAILED;
					bCont = false;
					break;
				}
			}
		}

		if( 0 == nNeedSendLen)
		{
			if(NULL != _pAIOListenner)
			{
				_pAIOListenner->OnSendSuccess(Handle,meta.pMsg);
			}
			_sendQueue.pop_front();
		}


		if(!bCont)
		{
			break;
		}
	}
	if(_sendQueue.size() == 0 )
		_pAIOController->ReadAIO(this);
	return ret;
}

eNetResult X2EpollSocket::Release()
{
	if(NULL != _ReadBuffer)
		delete[] _ReadBuffer;
	_ReadBuffer = NULL;
	ReleaseReadBuffer();
	_sendQueue.clear();
	return X2NET_SUCCESS;
}

eNetResult X2EpollSocket::InitSocket()
{
	if( Handle.type == NETHANDLE_LISTENSOCKET )
	{
		_nSocket = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
	}
	else if( Handle.type == NETHANDLE_CLIENTSOCKET )
	{
		_ReadBuffer = new char[MAX_RECVBUFF_LEN];
		_SendBuffer = new char[MAX_SENDBUFF_LEN];
	}
	else
	{
		_nSocket = -1;
	}

	if(_nSocket < 0 )
	{
		return X2NET_FAILED;
	}
	else
	{
		int flags = fcntl(_nSocket,F_GETFL,0);
		if(flags == -1 )
		{
			return X2NET_FAILED;
		}
		if( fcntl(_nSocket,F_SETFL, flags | O_NONBLOCK ) == -1 )
		{
			return X2NET_FAILED;
		}
		return X2NET_SUCCESS;
	}
}

bool X2EpollSocket::IsReadBuffFull()
{
    return _nRecvBuffLen == MAX_RECVBUFF_LEN;
}

void X2EpollSocket::SetIdentifier(int id)
{
	_nSocket = id;
}

void X2EpollSocket::SetAddress(const char* lpSzAddr)
{
	strncpy(_pszIP,lpSzAddr,X2_MAX_IP_LEN);
}

void X2EpollSocket::SetPort(unsigned short port)
{
	_unPort = port;
}
#endif
