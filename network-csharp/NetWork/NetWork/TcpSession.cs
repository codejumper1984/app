using System;
using System.Net.Sockets;
using System.Net;
using NetWork.Link;

namespace NetWork
{
    public class TcpSessionManager
    {
        public static TcpSession CreateSession()
        {
            TcpSession session = new TcpSession();
            return session;
        }
    }

    public class TcpSession
    {
        NetWork.Link.Link m_link = null;
        public String strServerIp
        {
            get;
            private set;
        }
        public ushort unServerPort
        {
            get;
            private set;
        }
        public void Connect(String strIp, ushort unPort)
        {
            strServerIp = strIp;
            unServerPort = unPort;
            m_link = NetWork.Link.AyncTcpLinkFactory.Instance().CreateLink();
            m_link.ConnectCallBack = OnConnectResultAsync;
            m_link.SendCallBack = OnWriteResultAsync;
            m_link.ReadCallBack = OnReceiveResultAsnc;
            m_link.Connect(strIp, unPort);
        }

        public void Close()
        {
            if(m_link != null)
                m_link.Close();
            m_link = null;
        }

        const int nMaxMsgBufferLen = 1024 * 1024;
        byte[] m_MsgBuffer = new byte[nMaxMsgBufferLen];
        public virtual void OnReceiveResultAsnc(LinkCallBackData callBackData)
        {
            Console.WriteLine(String.Format("OnConnectResultAsync:{0}",callBackData.CallBackMsg )) ;
        }

        public virtual void OnConnectResultAsync(LinkCallBackData callBackData)
        {
            Console.WriteLine(String.Format("OnConnectResultAsync:{0}",callBackData.CallBackMsg )) ;
        }

        public virtual void OnWriteResultAsync(LinkCallBackData callBackData)
        {
        }

        public void Send(byte[] message)
        {
            m_link.Send(message);
        }

    }
}
