#include <stdio.h>
#include "include/X2ByteStream.h"
int main(int argc,char* argv[])
{
	int a = 3;
	char* pData = (char*)&a;
	const int DataLen = sizeof(a);
	X2ByteStream stream(pData,DataLen);
	int val;
	stream >> val;
	printf("%d\n", val);
	return 0;
}