#ifndef __SERIALIZER_H__
#define __SERIALIZER_H__
class Serializer
{
public:
	Serializer(const char* pBuff, int nDataLen);
	~Serializer(void);
	template <class T> bool WriteValue(T val);
	template <class T> bool ReadValue(T& val);
private:
	const char* _pNext;
	const char* _pBuff;
	int _nDataLen;
	int _nLetDataLen;
};
#endif