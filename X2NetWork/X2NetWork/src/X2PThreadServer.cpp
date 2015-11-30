#ifdef EPOLL_CONTROL
#include "../include/X2PThreadServer.h"
void* X2PThreadServer::RunFunc()
{
	this->LoopFunc();
}
#endif
