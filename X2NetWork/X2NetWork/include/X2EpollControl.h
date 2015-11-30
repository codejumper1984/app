#ifdef EPOLL_CONTROL
#ifndef __X2EPOLLCONTRO_H__
#define __X2EPOLLCONTRO_H__
#define MAX_EPOLL_NUM 1024
#define INIT_EPOLL_EVENT 1024
#include<sys/epoll.h>
#include<list>
#include<map>
#include "X2EpollFile.h"
#include "IO.h"
class X2EpollListener;
enum X2EPollResult
{
	EPOLLCTL_SUCCESS = 0,
	EPOLLCTL_FAILED,
	EPOLLCTL_CRTPOLLFAILED,
	EPOLLCTL_CRTEVENTFAILED,
	EPOLLCTL_LISTENEREXIT,
};

class X2EpollControl:public AIOController{
	public:
		X2EpollControl();
		~X2EpollControl();
		eNetResult NewReadAIO(NetAIO* pAIO);
		eNetResult NewWriteAIO(NetAIO* pAIO);
		eNetResult ReadAIO(NetAIO* pAIO);
		eNetResult WriteAIO(NetAIO* pAIO);
		eNetResult ReadWriteAIO(NetAIO* pAIO);
		eNetResult RemoveAIO(NetAIO* pAIO);

		int Init();
		int RunControlling();
		int EpollID()
		{
			return _nEpollID;
		}
	private:
		epoll_event* GetEpollEvent(int idx);
		int _nEpollID;
		epoll_event* _pWaitEvent;
		X2EpollFile _epollFile;
		int Wait(int nWaitTime);
};
#endif
#endif
