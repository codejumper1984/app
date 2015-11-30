#include <stdlib.h>
#include "../include/X2MsgDef.h"
#include "../include/X2MsgManager.h"


template<class T> X2MsgPoll<T> X2MsgPoll<T>::g_Instance;

template<class T> void X2MsgPoll<T>::Init()
{
	for( int i = 0; i < T::INIT_POLL_SIZE;i++)
	{
		T* pT = new T;
		_MsgPoll.push_back(pT);
	}

}

template<class T> X2MsgPoll<T>& X2MsgPoll<T>::GetInstance()
{
	return g_Instance;
}

template<class T> void X2MsgPoll<T>::PutMsg(T* pMsg)
{
	_MsgPoll.push_back(pMsg);
}

template<class T> T* X2MsgPoll<T>::NextMsg()
{
	if( _MsgPoll.size() > 0 )
	{
		T* p = _MsgPoll.front();
		_MsgPoll.pop_front();
		return p;
	}
	else
		return new T;
}

void X2MsgManager::PutMsg(X2Msg* pMsg)
{
	switch(pMsg->MsgType())
	{
	case X2MSG_REQTEST:
		return X2MsgPoll<X2Msg_ReqTest>::GetInstance().PutMsg((X2Msg_ReqTest*)pMsg);
		break;

	case X2MSG_RESTEST:
		return X2MsgPoll<X2Msg_ResTest>::GetInstance().PutMsg((X2Msg_ResTest*)pMsg);
		break;
	}

}

X2Msg* X2MsgManager::GetMsg(short type)
{
	switch(type)
	{
	case X2MSG_REQTEST:
		return X2MsgPoll<X2Msg_ReqTest>::GetInstance().NextMsg();
		break;

	case X2MSG_RESTEST:
		return X2MsgPoll<X2Msg_ResTest>::GetInstance().NextMsg();
		break;
	}

}

void X2MsgManager::RegisterMsg()
{
	X2MsgPoll<X2Msg_ReqTest>::GetInstance().Init();
	X2MsgPoll<X2Msg_ResTest>::GetInstance().Init();
}

