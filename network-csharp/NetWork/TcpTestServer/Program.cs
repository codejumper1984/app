using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetWork;

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
    }
}
