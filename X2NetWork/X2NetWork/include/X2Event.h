#ifndef __X2NETWORKEVENT_H__
#define __X2NETWORKEVENT_H__

enum eNetEvent
{
	NETEVENT_SUCCESS = 0,
	NETEVENT_LISTEN_SUCCESS,
	NETEVENT_LISTEN_FAIL,
	NETEVENT_SEND_SUCCESS,
	NETEVENT_SEND_FAIL,
	NETEVENT_READ_SUCCESS,
	NETEVENT_ACCEPTED_SUCCESS,
	NETEVENT_CLOSE_SUCCESS,
	NETEVENT_STOPLISTEN_SUCCES,
	NETEVENT_STOPLISTEN_FAIL,
};
enum eServerEvent
{
	SERVEREVENT_LISTEN,
	SERVEREVENT_STOP,
	SERVEREVENT_STOPLISTEN,
	SERVEREVENT_SEND,
};


struct NetEvent
{
	NetHandle handle1;
	NetHandle handle2;
    long data;
    short type;
};

struct ServerEvent
{
	NetHandle handle1;
	NetHandle handle2;
    long data;
    short type;
};
#endif

