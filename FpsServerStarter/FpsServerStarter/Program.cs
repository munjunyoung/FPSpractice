using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace FpsServerStarter
{
    internal class Program
    {
        const int PortNumber = 23000;
        private readonly Socket TcpServerSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        private static void Main(string[] args)
        {
            var newProgram = new Program();
            newProgram.TcpStart();
        }
        /// <summary>
        /// Start ServerSocket , connection waiting
        /// </summary>
        private void TcpStart()
        {
            //IPEndPoint : 클래스 호트스 및 호스트서비스에 연결하는 응용프로그램 로컬또는 원격포트 정보를 포함
            // 매개변수 : 지정된 주소와 포트번호 
            TcpServerSocket.Bind(new IPEndPoint(IPAddress.Any, PortNumber));
            //대기중인 queue 갯수 
            TcpServerSocket.Listen(10);
            //들어오는 연결시도를 허용하는 비동기 작업
            TcpServerSocket.BeginAccept(OnAcceptTcpCallBack, TcpServerSocket);
            Console.WriteLine("TCP Server Start! \n");

            while(true)
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Callback
        /// </summary>
        /// <param name="ar"></param>
        private void OnAcceptTcpCallBack(IAsyncResult ar)
        {
            var newSocket = (Socket)ar.AsyncState;
            var clientSokcet = newSocket.EndAccept(ar);
           
            var newClient = new Client(clientSokcet);
            Console.WriteLine("New Client Connected\n");
            newClient.BeginReceive();
            TcpServerSocket.BeginAccept(OnAcceptTcpCallBack, TcpServerSocket);
        }
    }
}
