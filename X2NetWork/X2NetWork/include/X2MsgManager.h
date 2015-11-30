#ifndef __X2DMSGMANAGER_H__
#define __X2DMSGMANAGER_H__
#include <list>
#include "X2Msg.h"

template<class T> class X2MsgPoll
{
public:
	T* NextMsg();
	void PutMsg(T*);
	void Init();
	static X2MsgPoll<T>& GetInstance();
private:
	typedef std::list<T*> MsgList;
	MsgList _MsgPoll;
	static X2MsgPoll<T> g_Instance;
};

class X2MsgManager
{
public:
	static X2Msg* GetMsg(short type);
	static void PutMsg(X2Msg* p);
	static void RegisterMsg();
};
#endif
