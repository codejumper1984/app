#include "../include/X2NetInterface.h"
#include "../include/X2Net.h"


eNetResult X2NetInterface::Init()
{
	return X2Net::Instance().Init();
}

eNetResult X2NetInterface::StopListen(NetHandle netHandle)
{
	ServerEvent evt;
	evt.type = SERVEREVENT_STOPLISTEN;
	evt.handle1 = netHandle;
	return X2Net::Instance().PushServerEvent(evt);
}

eNetResult X2NetInterface::Listen(short unPort)
{
	ServerEvent evt;
	evt.type = SERVEREVENT_LISTEN;
	evt.data = unPort;
	return X2Net::Instance().PushServerEvent(evt);
}

eNetResult X2NetInterface::GetNextNetEvent(NetEvent& evt)
{
	return X2Net::Instance().GetNextNetEvent(evt);
}

eNetResult X2NetInterface::Send(NetHandle netHandle,X2Msg* pMsg)
{
	ServerEvent evt;
	evt.type = SERVEREVENT_SEND;
	evt.data = (long)pMsg;
	evt.handle1 = netHandle;
	return X2Net::Instance().PushServerEvent(evt);

}
