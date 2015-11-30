#ifndef __X2SERVER_H__
#define __X2SERVER_H__
#include "common.h"
#include "X2Event.h"
#include "X2MsgManager.h"
class X2Server
{
	public:

		X2Server(unsigned short unPort);
		virtual int Init();
		virtual int Start() = 0;
		virtual int Stop();

	protected:

		int Listen();
		int StopListen();
		int Send(NetHandle handle, X2Msg* pMsg);

		virtual void LogicFunc();
		void LoopFunc();

		void OnEvent(NetEvent& evt);

		virtual void OnConnected(NetHandle netHandle,NetHandle listenHandle) = 0;
		virtual void OnListenSuccess(NetHandle listenHandle) = 0;
		virtual void OnListenFail(unsigned short port) = 0;
		virtual void OnReceived(NetHandle netHandle,X2Msg* pMsg) = 0;
		virtual void OnSended(NetHandle netHandle, X2Msg* pMsg) = 0;
		virtual void OnDisConnected(NetHandle netHandle,NetHandle listHandle, long flag) = 0;
		virtual void OnStop(bool bStopped) = 0;
		virtual void OnStopListenSuccess(NetHandle& handle) = 0;
		virtual void OnStopListenFail(NetHandle& handle) = 0;
		NetHandle _litenHandle;
		short _unPort;
};
#endif
