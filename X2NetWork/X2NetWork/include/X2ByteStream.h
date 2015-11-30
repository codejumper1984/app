#ifndef __X2BYTESTREM_H__
#define __X2BYTESTREM_H__
#include <string.h>
class X2ByteStream
{
public:
	X2ByteStream(char* pData, int nDataSize);

	void clear()
	{
		_pHead = _pData;
	}

	bool Skip(int nLen)
	{
		if(nLen <= Capacity())
		{
			_pHead += nLen;
			return true;
		}
		else
		{
			return false;
		}
	}
	bool Align();
	char* Head()
	{
		return _pHead;
	}
	char* Begin()
	{
		return _pData;
	}
	int Capacity()
	{
		return _nEnd - (_pHead - _pData);
	}

	int Distance()
	{
		return _pHead - _pData;
	}

	template <class T> void BasicWrite(T val)
	{
		if( Capacity()>= sizeof(T))
		{
			memcpy(&_pHead,(const char*)&val,sizeof(val));
			_pHead += sizeof(val);
		}
	}

	template <class T> void BasicRead(T& val)
	{
		if( Capacity()>= sizeof(T))
		{
			memcpy(&val,_pHead,sizeof(T));
			_pHead += sizeof(val);
		}
	}

	X2ByteStream& operator >> ( unsigned char& val)
	{
		BasicRead(val);
		return *this;
	}

	X2ByteStream& operator << ( unsigned char val)
	{
		BasicWrite(val);
		return *this;
	}

	X2ByteStream& operator >> ( char& val)
	{
		BasicRead(val);
		return *this;
	}

	X2ByteStream& operator << ( char val)
	{
		BasicWrite(val);
		return *this;
	}


	X2ByteStream& operator >> ( short& val)
	{
		BasicRead(val);
		return *this;
	}

	X2ByteStream& operator << ( short val)
	{
		BasicWrite(val);
		return *this;
	}

	X2ByteStream& operator >> ( int& val)
	{
		BasicRead(val);
		return *this;
	}

	X2ByteStream& operator << ( int val)
	{
		BasicWrite(val);
		return *this;
	}

	X2ByteStream& operator >> ( float& val)
	{
		BasicRead(val);
		return *this;
	}

	X2ByteStream& operator << ( float val)
	{
		BasicWrite(val);
		return *this;
	}
	
	X2ByteStream& operator >> ( double& val)
	{
		BasicRead(val);
		return *this;
	}

	X2ByteStream& operator << ( double val)
	{
		BasicWrite(val);
		return *this;
	}

	X2ByteStream& operator << ( X2ByteStream& stream)
	{
		strncpy(_pHead,stream.Head(),stream.Capacity());
		_pHead += stream.Capacity();
		return *this;
	}

	X2ByteStream& operator >> ( X2ByteStream& stream)
	{
		strncpy(stream.Head(),_pHead,stream.Capacity());
		_pHead += stream.Capacity();
		return *this;
	}

private:
	char* _pData;
	char* _pHead;
	int _nEnd;
	const int _nDataLen;
};

//template<class T>  X2ByteStream& operator >>( X2ByteStream& stream, T& val);


#endif
