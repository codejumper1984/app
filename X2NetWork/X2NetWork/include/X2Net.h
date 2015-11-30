#ifndef  __X2NET_H__
#define __X2NET_H__
#include <list>
#include <map>
#include "../include/X2Pipe.h"
#include "../include/X2Event.h"
#include "../include/IO.h"
#include "../include/X2Decoder.h"
#ifdef EPOLL_CONTROL
#include "../include/X2PThread.h"
class X2Net: AIOListenner, X2PThread
#else
class X2Net: AIOListenner
#endif
{
public:
	static X2Net& Instance();
	eNetResult Init();

	eNetResult PushServerEvent(ServerEvent& evt);
	eNetResult GetNextNetEvent(NetEvent& evt);

	void OnListenSuccess(NetHandle& netHandle);
	void OnListenFail(NetHandle& netHandle);
	void OnStopListenFail(NetHandle& netHandle);
	void OnStopListenSuccess(NetHandle& netHandle);
	void OnSendSuccess(NetHandle& netHandle,X2Msg* pMsg);
	void OnSendFailed(NetHandle& netHandle,X2Msg* pMsg);
	int OnRead(NetHandle& netHandle,const char* pData, int nDataSize);
	void OnAccepted(NetHandle& netHandle,NetHandle& listenHandle,eNetResult);
	void OnClosed(NetHandle& netHandle,bool bPositive);

	NetAIO* NewAIO(unsigned short port);
	NetAIO* NewAIO();
	void ReleaseAIO(NetAIO* pAIO);

private:
	X2Net();
	short NextNetOpSlot();
	void OnListenDup();

	X2Pipe _msgQueueFromServer;
	X2Pipe _msgQueueToServer;

	int ProcessServerEvent();
	void OnServerEvent(ServerEvent& evt);
	eNetResult OnServerListen(short port);
	eNetResult OnServerStopListen(short port);
	eNetResult OnServerSend(NetHandle handle, X2Msg* pMsg);

	void* RunFunc();

	typedef std::map<int,NetHandle> ListenPortHandleMap;
	typedef std::list<int> FreeSlotList;

	ListenPortHandleMap _listenPortHandleMap;
	FreeSlotList _freeSlotList;
	NetAIO** _pAioSlot;
	AIOController* _pAIOController;
	X2Decoder* _pDecoder;
	static X2Net instance;
};
#endif
