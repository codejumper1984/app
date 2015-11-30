#ifdef EPOLL_CONTROL
#ifndef __X2THREADSERVER_H__
#define  __X2THREADSERVER_H__
#include "X2PThread.h"
#include "X2Server.h"
class X2PThreadServer: public X2Server, public X2PThread
{
public:
	X2PThreadServer(unsigned short port = 0):X2Server(port)
	{

	}
	void SetPort(unsigned port)
	{
		_unPort = port;
	}

	int Start()
	{
		Init();
		if( Listen() )
			return -1;
		return ((X2PThread*)this)->Start();
	}
	void* RunFunc();

};

#endif
#endif
