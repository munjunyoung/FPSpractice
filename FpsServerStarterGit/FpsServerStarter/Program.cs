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
        private readonly Socket serverSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static void Main(string[] args)
        {
            var newProgram = new Program();

            newProgram.Start();
        }

        //start ServerSocket , connection waiting
        private void Start()
        {
            //IPEndPoint : 클래스 호트스 및 호스트서비스에 연결하는 응용프로그램 로컬또는 원격포트 정보를 포함
            // 매개변수 : 지정된 주소와 포트번호 
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 23000));
            //대기중인 queue 갯수 
            serverSocket.Listen(10);
            //들어오는 연결시도를 허용하는 비동기 작업
            serverSocket.BeginAccept(OnAcceptCallBack, serverSocket);

            Console.WriteLine("Server Start! \n");
            while(true)
            {
                Thread.Sleep(1);
            }
        }

        //IAsynceResult 비동기 작업의 상태를 나타냄
        private void OnAcceptCallBack(IAsyncResult ar)
        {
            var newSocket = (Socket)ar.AsyncState;
            var clientSokcet = newSocket.EndAccept(ar);

            var newClient = new Client(clientSokcet);
            Console.WriteLine("New Client Connected\n");
            newClient.BeginReceive();
            serverSocket.BeginAccept(OnAcceptCallBack, serverSocket);
            
        }
    }
}
