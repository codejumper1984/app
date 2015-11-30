#include<string.h>
#include<stdlib.h>
#include "../include/X2Pipe.h"


X2Pipe::X2Pipe(unsigned long lnBuffLen,unsigned long lnReservedSize): _lnBuffLen(lnBuffLen),_lnReservedSize(lnReservedSize),_pBuff(NULL),_pHead(NULL),_pTail(NULL)
{
	if(_lnBuffLen >= 0 )
	{
		_pBuff = new char[_lnBuffLen];
		_pHead = _pBuff;
		_pTail = _pBuff;
	}
}

eNetResult X2Pipe::Init()
{
	if(_lnBuffLen >= 0 )
	{
		_pBuff = new char[_lnBuffLen];
		_pHead = _pBuff;
		_pTail = _pBuff;
		return X2NET_SUCCESS;
	}
	else
	{
		return X2NET_FAILED;
	}
}

X2Pipe::~X2Pipe()
{
	if(NULL != _pBuff)
	{
		delete[] _pBuff;
		_pBuff = NULL;
		_pTail = NULL;
		_pHead = NULL;
		_lnBuffLen = 0;
	}
}

long X2Pipe::Size()
{
	if(_pTail >= _pHead)
	{
		return (_pTail - _pHead);
	}
	else
	{
		return  _lnBuffLen + _pTail - _pHead;
	}
}

long X2Pipe::Capacity()
{
	if(_pTail >= _pHead)
	{
		return (_pHead- _pTail ) + _lnBuffLen - _lnReservedSize;
	}
	else
	{ 
		return ( _pHead - _pTail ) - _lnReservedSize;
	}

}

eNetResult X2Pipe::PushData(const void* pSrc,unsigned  long lnDataSize)
{
    const char* pData = (const char*)pSrc;
	if(_pTail >= _pHead )
	{
		if( (_pHead- _pTail) + _lnBuffLen - _lnReservedSize  < lnDataSize)
		{
			return X2NET_FAILED;
		}

		if( (_pBuff - _pTail) +_lnBuffLen >= lnDataSize)
		{
			memcpy(_pTail, pData, lnDataSize);
		}
		else
		{
			long tailDataLen = _pBuff +_lnBuffLen - _pTail;
			memcpy(_pTail, pData, tailDataLen);
			memcpy(_pBuff, pData + tailDataLen, lnDataSize -tailDataLen);
		}
	}
	else
	{
		if( (_pHead - _pTail) - _lnReservedSize < lnDataSize)
		{
			return X2NET_FAILED;
		}
		memcpy(_pTail, pData, lnDataSize);
	}

	EnlargeTail(lnDataSize);

	return X2NET_SUCCESS;
}

eNetResult X2Pipe::PopData(void* pDest,unsigned long lnDataSize)
{
    char* pData = (char*)pDest;
	if(_pTail >= _pHead )
	{
		if(_pTail - _pHead < lnDataSize)
			return X2NET_FAILED;
		memcpy(pData, _pHead,lnDataSize);
	}
	else
	{
		if( (_pTail- _pHead) + _lnBuffLen < lnDataSize)
			return X2NET_FAILED;
		long fCpLen = _pTail- _pHead +_lnBuffLen  ;
		memcpy(pData,_pHead, fCpLen);
		memcpy(pData+fCpLen,_pBuff, lnDataSize - fCpLen);
	}
	EnlargeHead(lnDataSize);
	return X2NET_SUCCESS;
}

eNetResult X2Pipe::SetPos(int nIdx)
{
	if(Size() == 0)
	{
		_pTail = _pBuff + (nIdx % _lnBuffLen);
		_pHead = _pTail;
		return X2NET_SUCCESS;
	}
	else
	{
		return X2NET_FAILED;
	}
}
