using System;
using System.Net.Sockets;
using System.Net;

namespace NetWork
{
    class TcpSession
    {
        Socket sSocket = null;
        String strIPAddress = String.Empty;
        int nPort;
        public bool Connect()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(strIPAddress);
            IPEndPoint endPoint = new IPEndPoint(ipAddress, nPort);
            socket.Connect(endPoint);

            return true;
        }
    }
}
