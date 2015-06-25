using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetWork;
using System.Net.Sockets;
using System.Net;

namespace TcpTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpSession session = TcpSessionManager.CreateSession();
            session.Connect("127.0.0.1", 40001);
            String strLine = Console.ReadLine();
        }
        const ushort server_port = 40001;
        const string server_ip = "127.0.0.1";
        static void RunServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(server_ip), server_port));
            socket.Listen();
            socket.Accept();
        }
    }
}
