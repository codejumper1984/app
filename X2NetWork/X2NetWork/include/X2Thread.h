#ifndef __X2THREADSERVER_H__
#define  __X2THREADSERVER_H__
class X2Thread
{
	virtual int Start() = 0;
	virtual int Stop() = 0;
	virtual int Join() = 0;
	virtual int Cancel() = 0;
};

#endif
