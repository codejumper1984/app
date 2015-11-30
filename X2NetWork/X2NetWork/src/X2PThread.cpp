#ifdef EPOLL_CONTROL
#include "../include/X2PThread.h"

X2PThread::X2PThread(ThreadLogicFunc LogicFunc):_threadFunc(LogicFunc),_thread_id(0),_bStop(false)
{

}

void X2PThread::OnStop()
{
}

void* X2PThread::ThreadFunc(void* pData)
{
    X2PThread* pThread = (X2PThread*)pData;
    if ( NULL != pThread )
    {
		while(!pThread->_bStop)
		{
			if( pThread->_threadFunc)
			{
				pThread->_threadFunc(pData);
			}
			else
			{
				pThread->RunFunc();
			}
		}
		pThread->OnStop();
    }
}

void* X2PThread::RunFunc()
{

}

int X2PThread::Start()
{
	if( pthread_create(&_thread_id, NULL, ThreadFunc, this) )
		return X2THREAD_CREATEFAILED;
	return X2THREAD_SUCCESS;
}

int X2PThread::Join()
{
	if( pthread_join(_thread_id,NULL) )
		return X2THREAD_JOINFAILED;
	return X2THREAD_SUCCESS;
}

int X2PThread::Cancel()
{

}

int X2PThread::Stop()
{
	_bStop = true;
	return 0;
}
#endif
