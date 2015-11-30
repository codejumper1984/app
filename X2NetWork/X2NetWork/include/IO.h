#ifndef __IO_H__
#define __IO_H__
#include "common.h"
#include "X2MsgManager.h"
class AIOListenner
{
	public:
		virtual void OnListenSuccess(NetHandle& netHandle) = 0;
		virtual void OnListenFail(NetHandle& netHandle) = 0;
		virtual void OnSendSuccess(NetHandle& netHandle,X2Msg* pMsg) = 0;
		virtual void OnSendFailed(NetHandle& netHandle,X2Msg* pMsg) = 0;
		virtual int OnRead(NetHandle& netHandle,const char* pData, int nDataSize) = 0;
		virtual void OnAccepted(NetHandle& netHandle,NetHandle& listenHandle,eNetResult) = 0;
		virtual void OnClosed(NetHandle& netHandle,bool bPositive ) = 0;
		virtual void OnStopListenFail(NetHandle& netHandle) = 0;
		virtual void OnStopListenSuccess(NetHandle& netHandle) = 0;
};

class NetIO
{
public:
	NetHandle Handle;
	virtual eNetResult Listen() = 0;
	virtual eNetResult Close() = 0;
	//virtual eNetResult Send(const char* pData, int nDataSize) = 0;
	virtual eNetResult Send(X2Msg* pMsg) = 0;
	virtual eNetResult Accept(NetIO*) = 0;
	virtual void SetAddress(const char* lpSzAddr) = 0;
	virtual void SetPort(unsigned short unPort) = 0;
	virtual void SetIdentifier(int id) = 0;
	virtual int GetIdentifier() = 0;
	virtual eNetResult Init() = 0;
	virtual eNetResult Release() = 0;
};

class AIOController;
class NetAIO: public  NetIO
{
public:
	void SetListenner(AIOListenner* pListener)
	{
		_pAIOListenner = pListener;
	}
	void SetController(AIOController* pController)
	{
		_pAIOController = pController;
	}
	virtual int OnSend() = 0;
	virtual int OnError() = 0;
	virtual int OnClose(bool bPositive) = 0;
	virtual	int OnRead() = 0;
	virtual	int OnAccept(NetIO* pNetIO) = 0;
	virtual	int OnAccepted() = 0;
protected:
	AIOListenner* _pAIOListenner;
	AIOController* _pAIOController;
};

class AIOController
{
	public:
		virtual eNetResult NewReadAIO(NetAIO* pAIO) = 0;
		virtual eNetResult NewWriteAIO(NetAIO* pAIO) = 0;
		virtual eNetResult ReadAIO(NetAIO* pAIO) = 0;
		virtual eNetResult WriteAIO(NetAIO* pAIO) = 0;
		virtual eNetResult ReadWriteAIO(NetAIO* pAIO) = 0;
		virtual eNetResult RemoveAIO(NetAIO* pAIO) = 0;
		virtual int RunControlling() = 0;
};


#endif
