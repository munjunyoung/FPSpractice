using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FpsServerStarter
{
    static class GameManager
    {
        private static List<Client> clientsList = new List<Client>();
        

        //Add client to List
        public static void AddClient(Client c)
        {
            clientsList.Add(c);
            Console.WriteLine("GAME MANAGER : add New Client");
            ShowConnectedClientsCount();
        }

        //Remvod Client to list
        public static void RemoveClient(Client c)
        {
            if (c.ingameState)//인게임 상태일때
                return;
            clientsList.Remove(c);
            Console.WriteLine("GAME MANAGER : client [" + c.Sock.RemoteEndPoint + "] was removed.");
            c.Sock.Close();
            ShowConnectedClientsCount();
        }
        
        public static void MatchingRequest(Client c)
        {
            MatchingManager.MatchingProcess(c);
        }

        public static void QueueCancelRequest(Client c)
        {
            MatchingManager.QueueRemoveClient(c);
        }

        public static void ShowConnectedClientsCount()
        {
            Console.WriteLine("# of connected clients : " + clientsList.Count);
        }
    }
}
