#ifdef EPOLL_CONTROL
#ifndef __X2PTHREAD_H__
#define __X2PTHREAD_H__
#include <pthread.h>
class X2PThread
{
	public:
		enum X2PThreadResult
		{
			X2THREAD_SUCCESS = 0,
			X2THREAD_CREATEFAILED,
			X2THREAD_JOINFAILED,
		};
		typedef void* (*ThreadLogicFunc)(void*);
		X2PThread(ThreadLogicFunc LogicFunc= NULL);
		int Start();
		int Join();
		int Stop();
		int Cancel();
        virtual void* RunFunc();
		virtual void OnStop();
	private:
		pthread_t _thread_id;
		ThreadLogicFunc _threadFunc;
		static void* ThreadFunc(void*);
		bool _bStop;
};
#endif
#endif
