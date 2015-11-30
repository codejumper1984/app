#include <string.h>
#include "../include/Serializer.h"

Serializer:: Serializer(const char* pBuff, int nDataLen):_pBuff(pBuff),_pNext(pBuff), _nDataLen(nDataLen),_nLetDataLen(nDataLen)
{
}

Serializer::~Serializer(void)
{
}

template <class T> bool Serializer::ReadValue(T &val)
{
	if(_nLetDataLen >= sizeof(val))
	{
		val = (T*)_pNext;
		_pNext += sizeof(val);
		_nLetDataLen -= sizeof(val);
		return true;
	}
	return false;
}
template <class T> bool Serializer::WriteValue(T val)
{
	if(_nLetDataLen >= sizeof(val))
	{
		(T*)_pNext = val;
		_pNext += sizeof(val);
		_nLetDataLen -= sizeof(val);
		return true;
	}
	return false;
}
