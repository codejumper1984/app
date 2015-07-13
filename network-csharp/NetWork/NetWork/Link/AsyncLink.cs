using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using NetWork.Util;
using NetWork.Msg;

namespace NetWork.Link
{
    public class AsyncLink : Link
    {
        struct AsyncWriteData{
            public byte[] data;
        }

        public AsyncLink()
        {
        }

        // Send Control Datas

        LinkedList<AsyncWriteData> m_lMsgBack = new LinkedList<AsyncWriteData>();

        SafeCircleList<AsyncWriteData> m_CircleList = new SafeCircleList<AsyncWriteData>();

        SafeCircleList<int> m_AReadFlag = new SafeCircleList<int>();


        //Receive Buffer
        BinaryStream m_InputBufferStream = new BinaryStream();

        SimpleDecoder decoder = new SimpleDecoder();

        String m_strIP = String.Empty;

        ushort m_unPort = 0;

        Socket m_socket = null;

        //接收到数据后的回调，回调线程调用
        private void ReceiveCallBack(IAsyncResult ar)
        {
            int nReadLen = m_socket.EndReceive(ar);
            if (SendCallBack != null)
            {
                decoder.Decode(m_InputBufferStream);
                if(ReadCallBack != null)
                {
                    LinkCallBackData sendCallBack = new LinkCallBackData();
                    sendCallBack.CallBackMsg = eLinkCallBackMsg.Receive_Finished;
                    sendCallBack.Flag = nReadLen;
                    for (int i = 0; i < decoder.DecodedMsg.Count; i++)
                    {
                        sendCallBack.Data = decoder.DecodedMsg[i];
                        ReadCallBack(sendCallBack);
                    }
                }
                decoder.DecodedMsg.Clear();
            }
            if(m_socket != null && m_socket.Connected)
                Receive();
        }

        public int Receive(byte[] inputData)
        {
            return 0;
        }

        private int Receive()
        {
            m_socket.BeginReceive(m_InputBufferStream.Data,m_InputBufferStream.End,m_InputBufferStream.Capacity(), SocketFlags.None, ReceiveCallBack, null);
            return 0;
        }

        void WriteCallBack(IAsyncResult ar)
        {
            m_socket.EndSend(ar);
            if(!m_CircleList.IsEmpty())
            {
                m_CircleList.DeQueue();
                if(!m_CircleList.IsEmpty())
                {
                    AsyncWriteData data = m_CircleList.Front();
                    Write_Socket(data.data);
                }
                else
                {
                    //Async Call Over
                    AReadSleep();
                }
            }
            else
            {
                // Impossible
                AReadSleep();
            }
        }

        private void Write_Socket(byte[] data)
        {
            m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, WriteCallBack, null);
        }

        public void Send(byte[] data)
        {
            AsyncWriteData writeData = new AsyncWriteData();
            writeData.data = data;
            if (m_CircleList.IsEmpty())
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
                // first add data to queue and then call async send
                Write_Socket(data);
            }
            else
            {
                if (m_CircleList.IsFull())
                {
                    m_lMsgBack.AddFirst(writeData);
                }
                else
                {
                    if (!m_CircleList.IsFull())
                        m_CircleList.EnQueue(writeData);
                }

            }
        }

        public void Close()
        {
            if(m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
        }

        public bool IsConnected
        {
            get
            {
                return m_socket != null && m_socket.Connected;
            }
        }

        public EventCallBack ConnectCallBack
        {
            get;
            set;
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
                if(ConnectCallBack != null)
                {
                    LinkCallBackData callBackData = new LinkCallBackData();
                    callBackData.CallBackMsg = eLinkCallBackMsg.Connect_IPFormatError;
                    callBackData.Data = strIP;
                    ConnectCallBack(callBackData);
                }
                Close();
            }
        }

        private bool AReadAwake()
        {
            if (m_AReadFlag.IsEmpty())
            {
                m_AReadFlag.EnQueue(1);
                return true;
            }
            return false;
        }

        private bool AReadSleep()
        {
            return m_AReadFlag.DeQueue();
        }

        private bool IsAReadSleep()
        {
            return m_AReadFlag.IsEmpty();
        }

        public void ProcessUnSendMsg()
        {
            if(!m_CircleList.IsEmpty())
            {
                if(IsAReadSleep())
                {
                    AReadAwake(); // must before downside
                    AsyncWriteData data = m_CircleList.Front();
                    Write_Socket(data.data);
                }
            }

        }

    }
}
