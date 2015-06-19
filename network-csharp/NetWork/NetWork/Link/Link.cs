using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace NetWork.Link
{
    public enum eLinkCallBackMsg
    {
        Connect_IPFormatError = 0,
        Connect_ConnectFailed 
    }

    public class LinkCallBackData
    {
        public eLinkCallBackMsg CallBackMsg
        {
            get;
            set;
        }
        public Object Data
        {
            get;set;
        }

    }

    public delegate void EventCallBack(LinkCallBackData callBackData);

    public interface Link
    {
        bool IsConnected
        {
            get;
        }

        void Connect(String strIp, ushort unPort);
        void Close();
        void Write(byte[] data);
        void Read(byte[] data);

        EventCallBack DisconnectedCallBack
        {
            get;
            set;
        }

        EventCallBack ConnectCallBack
        {
            get;
            set;
        }

        EventCallBack SendCallBack
        {
            get;
            set;
        }

        EventCallBack ReadCallBack
        {
            get;
            set;
        }
    }


}
