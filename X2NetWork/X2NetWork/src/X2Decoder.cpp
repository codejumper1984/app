#include <stdlib.h>
#include "../include/X2MsgManager.h"
#include "../include/X2Decoder.h"
X2Msg* X2Decoder::GetDecodeMsg()
{
	X2Msg* pMsg = NULL;
	if(_decodedMsgList.size() > 0)
	{
		pMsg = _decodedMsgList.front();
		_decodedMsgList.pop_front();
	}
	return pMsg;
}

int X2Decoder::Decode(X2ByteStream& stream)
{
	while(stream.Capacity() >= sizeof(X2MsgHead))
	{
		X2MsgHead* pMsgHead = (X2MsgHead*)stream.Head();
		int nNeedLen =pMsgHead->unMsgLen + sizeof(X2MsgHead);
		if( nNeedLen >= stream.Capacity())
		{
			X2Msg* pMsg = X2MsgManager::GetMsg(pMsgHead->unType);
			if(NULL != pMsgHead)
			{
				pMsg->Deserilize(stream);
				_decodedMsgList.push_back(pMsg);
			}
			else
			{
				stream.Skip(nNeedLen);
			}
		}
		else
		{
			break;
		}
	}
	stream.Align();
	return stream.Capacity();
}
