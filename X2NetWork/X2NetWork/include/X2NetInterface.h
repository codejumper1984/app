#ifndef __X2NETINTERFACE_H__
#define __X2NETINTERFACE_H__
#include "common.h"
#include "X2Event.h"
#include "X2MsgManager.h"
class X2NetInterface
{
public:
	static eNetResult Init();
	static eNetResult Listen(short unPort);
	static eNetResult Send(NetHandle netHandle,X2Msg* pMsg);
	static eNetResult GetNextNetEvent(NetEvent& evt);
	static eNetResult StopListen(NetHandle netHandle);
};
#endif
