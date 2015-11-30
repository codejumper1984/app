#ifndef __X2DECODER_H__
#define __X2DECODER_H__
#include <list>
#include "X2MsgManager.h"
#include "X2ByteStream.h"
class X2Decoder
{
public:
	int Decode(X2ByteStream& stream);
	X2Msg* GetDecodeMsg();
private:
	typedef std::list<X2Msg*> DecodedMsgList;
	DecodedMsgList _decodedMsgList;
};

#endif