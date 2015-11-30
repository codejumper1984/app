#ifndef __COMMON_H__
#define  __COMMON_H__

enum eNetResult{
	X2NET_SUCCESS = 0,
	X2NET_FAILED,
	X2NET_PIPEINITFAILED,
	X2NET_PIPEFULL,
};

enum eNetHanldeType
{
	NETHANDLE_LISTENSOCKET = 0,
	NETHANDLE_CLIENTSOCKET ,
};

union NetHandle
{
	long long data;
	struct {
		short type;
		union{;
		unsigned short tag;
		unsigned short port;
		};
		int slot;
	};
};


#endif
