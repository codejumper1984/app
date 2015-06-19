using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using NetWork.Util;

namespace NetWork.Link
{
    public class AsyncLink:Link
    {
        struct AsyncWriteData{
            public byte[] data;
        }

        LinkedList<AsyncWriteData> m_lMsgBack = new LinkedList<AsyncWriteData>();

        SafeCircleList<AsyncWriteData> m_CircleList = new SafeCircleList<AsyncWriteData>();

        String m_strIP = String.Empty;
        ushort m_unPort = 0;
        Socket m_socket = null;

        public void Read(byte[] data)
        {

        }

        void WriteCallBack(IAsyncResult ar)
        {
            if(!m_CircleList.IsEmpty())
            {
                m_CircleList.DeQueue();
                if(!m_CircleList.IsEmpty())
                {
                    AsyncWriteData data = m_CircleList.Front();
                    Write_Socket(data.data);
                }
            }

        }

        private void Write_Socket(byte[]data)
        {
            m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, WriteCallBack, null);
        }

        public void Write(byte[] data)
        {
            if (m_CircleList.IsEmpty())
            {
                Write_Socket(data);
            }
            AsyncWriteData writeData = new AsyncWriteData();
            writeData.data = data;
            if (m_CircleList.IsFull())
            {
                m_lMsgBack.AddFirst(writeData);
            }
            else
            {
                LinkedListNode<AsyncWriteData> node = m_lMsgBack.First;
                while(node != null)
                {
                    if(!m_CircleList.IsFull())
                    {
                        m_CircleList.EnQueue(node.Value);
                        m_lMsgBack.RemoveFirst();
                        node = node.Next;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!m_CircleList.IsFull())
                    m_CircleList.EnQueue(writeData);
            }
        }

        public void Update()
        {

        }

        public void Close()
        {

        }

        public EventCallBack ConnectCallBack
        {
            get;
            set;
        }

        public bool IsConnected
        {
            get
            {
                return m_socket != null && m_socket.Connected;
            }
        }
        public EventCallBack DisconnectedCallBack
        {
            get;
            set;
        }

        public EventCallBack SendCallBack
        {
            get;
            set;
        }

        public EventCallBack ReadCallBack
        {
            get;
            set;
        }


        public void AsyncConnectCallBack(IAsyncResult ar)
        {
            try
            {
                m_socket.EndConnect(ar);
            }
            catch (Exception e)
            {
                LinkCallBackData callBackData = new LinkCallBackData();
                callBackData.CallBackMsg = eLinkCallBackMsg.Connect_ConnectFailed;
                callBackData.Data = e;
                if(ConnectCallBack != null)
                    ConnectCallBack(callBackData);
            }
        }

        public void Connect(String strIP,ushort port)
        {
            m_strIP = strIP;
            m_unPort = port;
            IPAddress ipAddress;
            if( IPAddress.TryParse(strIP, out ipAddress))
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.BeginConnect(new IPEndPoint(ipAddress, m_unPort),AsyncConnectCallBack,null);
            }
            else
            {
                LinkCallBackData callBackData = new LinkCallBackData();
                callBackData.CallBackMsg = eLinkCallBackMsg.Connect_IPFormatError;
                callBackData.Data = strIP;
                ConnectCallBack(callBackData);
            }
        }

        public void Send(byte[] data)
        {

        }

    }
}
