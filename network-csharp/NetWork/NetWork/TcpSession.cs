using System;
using System.Net.Sockets;
using System.Net;

namespace NetWork
{
    class TcpSession
    {
        NetWork.Link.Link m_link = null;
        public void Connect()
        {
            m_link = NetWork.Link.AyncTcpLinkFactory.Instance().CreateLink();
        }
    }
}
