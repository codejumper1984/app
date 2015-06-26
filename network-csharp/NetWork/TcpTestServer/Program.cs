using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetWork;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TcpTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread server_thread = new Thread(RunServer);
            server_thread.Start();
            Thread client_thread = new Thread(RunClient);
            client_thread.Start();
            server_thread.Join();
            client_thread.Join();
            Console.WriteLine("Test Finished");
            String strLine = Console.ReadLine();
        }

        static void RunClient()
        {
            Console.WriteLine("Client Begining...");
            TcpSession session = TcpSessionManager.CreateSession();
            session.Connect(server_ip, server_port);
            byte[] sendbytes = new byte[1];
            sendbytes[0] = 1;
            Random randgen = new Random();
            for(int i = 0; i < 100;i++)
            {
                session.Send(sendbytes);
                Thread.Sleep(randgen.Next(0, max_sleep_time));
            }
            session.Close();
            Console.WriteLine("Client Ending");
        }

        const int max_sleep_time = 10;//ms
        const ushort server_port = 40001;
        const string server_ip = "127.0.0.1";
        const int rec_buf_max_len = 1024;
        const int listen_queue_len = 100;
        static void RunServer()
        {
            Console.WriteLine("Server Begining...");
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(server_ip), server_port));
            socket.Listen(listen_queue_len);
            Socket client_socket = socket.Accept();
            byte[] read_bufer = new byte[rec_buf_max_len];
            int nTotalByte = 0;
            int nReadLen = client_socket.Receive(read_bufer);
            while(nReadLen != 0)
            {
                nTotalByte += nReadLen;
                nReadLen = client_socket.Receive(read_bufer);
            }
            Console.WriteLine(String.Format("Total Read Bytes:{0}",nTotalByte));
            Console.WriteLine("Server End");
        }
    }
}
