#ifndef __X2MSGDEF_H__
#define __X2MSGDEF_H__
#include "X2ByteStream.h"
#include "X2Msg.h"

enum X2MSGTYPE
{
	X2MSG_REQTEST = 0,
	X2MSG_RESTEST,
};


class X2Msg_ResTest: public X2Msg
{
public:
	X2Msg_ResTest():pData(NULL)
	{
		head.unType = X2MSG_RESTEST;
	}
	~X2Msg_ResTest()
	{
		if(NULL != pData)
			delete[] pData;
		pData = NULL;
	}
	char* pData;
	int nDataLen;
protected:

	void DeserilizePData(X2ByteStream& stream)
	{
		int nDataLen;
		stream >> nDataLen;
		pData = new char[nDataLen];
		X2ByteStream byteStream(pData,nDataLen);
		stream >> byteStream;

	}

	void SerilizeMsg(X2ByteStream& stream)
	{
		SerilizePData(stream);
	}

	void DeserilizeMsg(X2ByteStream& stream)
	{
		DeserilizePData(stream);
	}

	void SerilizePData(X2ByteStream& stream)
	{
		X2ByteStream byteStream(pData,nDataLen);
		stream << byteStream;
	}

};

class X2Msg_ReqTest: public X2Msg
{
public:
	X2Msg_ReqTest():pData(NULL)
	{
		head.unType = X2MSG_REQTEST;
	}
	~X2Msg_ReqTest()
	{
		if(NULL != pData)
			delete[] pData;
		pData = NULL;
	}
	char* pData;
	int nDataLen;
protected:

	void DeserilizePData(X2ByteStream& stream)
	{
		int nDataLen;
		stream >> nDataLen;
		pData = new char[nDataLen];
		X2ByteStream byteStream(pData,nDataLen);
		stream >> byteStream;

	}

	void SerilizeMsg(X2ByteStream& stream)
	{
		SerilizePData(stream);
	}

	void DeserilizeMsg(X2ByteStream& stream)
	{
		DeserilizePData(stream);
	}

	void SerilizePData(X2ByteStream& stream)
	{
		X2ByteStream byteStream(pData,nDataLen);
		stream << byteStream;
	}

};
#endif