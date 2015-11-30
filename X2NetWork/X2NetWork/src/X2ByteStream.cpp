#include <string.h>
#include "../include/X2ByteStream.h"

X2ByteStream::X2ByteStream(char* pData, int nDataSize):_pData(pData), _pHead(pData),_nDataLen(nDataSize),_nEnd(nDataSize)
{
}

bool X2ByteStream::Align()
{
	int nCopyLen = Capacity();
	if(nCopyLen > 0)
		memmove((void*)_pData,_pHead,nCopyLen);
	_pHead = _pData;
	_nEnd = nCopyLen;
	return nCopyLen;
}