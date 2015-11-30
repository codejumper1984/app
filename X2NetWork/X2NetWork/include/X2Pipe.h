#ifndef __X2PIPE_H__
#define __X2PIPE_H__
#include "../include/common.h"
class X2Pipe
{

	public:
		X2Pipe(unsigned long nBuffLen,unsigned long nReservedSize = 1);
		~X2Pipe();
		eNetResult PushData(const void* pSrc, unsigned long DataSize);
		eNetResult PopData(void* pDest, unsigned long DataSize);
		eNetResult Init();
		void Clear()
		{
			_pTail = _pHead;
		}
		long Capacity();
		long Size();
		eNetResult SetPos(int nIdx);
	protected:
		inline void EnlargeTail(unsigned long size)
		{
			_pTail = _pBuff + (_pTail + size - _pBuff) % _lnBuffLen;
		}

		inline void EnlargeHead(long size)
		{
			_pHead = _pBuff + (_pHead + size - _pBuff ) % _lnBuffLen;
		}
	private:
		char* _pBuff;
		char*_pHead;
		char*_pTail;
		unsigned long _lnBuffLen;
		unsigned long _lnReservedSize;
};
#endif
