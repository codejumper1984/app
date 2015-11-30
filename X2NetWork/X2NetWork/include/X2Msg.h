#ifndef __X2MSG_H__
#define __X2MSG_H__
#include "X2ByteStream.h"

struct X2MsgHead
{
	short unType;
	short unMsgLen;
};

class Serializable
{
public:
	virtual void Deserilize(X2ByteStream& stream) = 0;
	virtual void Serilize(X2ByteStream& stream) = 0;
};

class X2Msg:public Serializable
{
public:
	const static int INIT_POLL_SIZE = 16;
	virtual ~X2Msg()
	{

	}
	void Serilize(X2ByteStream& stream)
	{
		stream << head.unType;
		char* pLenAddress = stream.Head();
		stream << head.unMsgLen;
		char* pBodyBegin = stream.Head();
		SerilizeMsg(stream);
		char* pBodyEnd = stream.Head();
		X2ByteStream lenStream(pLenAddress,sizeof(head.unMsgLen));
		short MsgLen = pBodyEnd - pBodyBegin;
		lenStream << MsgLen;
	}
	void Deserilize(X2ByteStream& stream)
	{
		stream >> head.unType;
		stream >> head.unMsgLen;
		DeserilizeMsg(stream);
	}
	short MsgLen()
	{
		return head.unMsgLen;
	}

	short MsgType()
	{
		return head.unType;
	}
protected:
	virtual void DeserilizeMsg(X2ByteStream& strem) = 0;
	virtual void SerilizeMsg(X2ByteStream& strem) = 0;
	X2MsgHead head;
};
#endif