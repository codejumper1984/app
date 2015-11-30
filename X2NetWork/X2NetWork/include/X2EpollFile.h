#ifndef __X2EPOLLFILE_H__
#define __X2EPOLLFILE_H__
#include<sys/epoll.h>

class X2EpollFile
{
	public:
		X2EpollFile(int nMaxEvent);
		~X2EpollFile();
		enum X2EpollFileResult
		{
			X2EPOLLFILE_SUCCESS,
			X2EPOLLFILE_FAILED,
			X2EPOLLFILE_CREATEFAILEd,
			X2EPOLLFILE_ADDFAILEd,
			X2EPOLLFILE_DELFAILEd,
			X2EPOLLFILE_MODFAILEd,
		};
		X2EpollFileResult Create(int nMaxEvent = 0);
		X2EpollFileResult Add(int fd, int events,void* pData);
		X2EpollFileResult Mod(int fd, int events,void* pData);
		X2EpollFileResult Del(int fd);
		int Wait(int nWaitTime,struct epoll_event*,int nEventNum);
		X2EpollFileResult Close();
		int ID()
		{
			return _nEpollID;
		}
	private:
		int _nEpollID;
		int _nMaxPoolEvent;
};
#endif
