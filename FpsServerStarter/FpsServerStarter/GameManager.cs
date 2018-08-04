using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FpsLib;
namespace FpsServerStarter
{
    static class GameManager
    {
        private static List<Client> clientsList = new List<Client>();

        /// <summary>
        /// Add client to List
        /// </summary>
        public static void AddClient(Client c, LoginInfo data)
        {
            c.loginState = true;
            c.LoginID = data.Id;
            clientsList.Add(c);
            
            Console.WriteLine("[GAME MANAGER] : Add New Client ! ID : [ " + c.LoginID + " ]" );
            ShowConnectedClientsCount();
        }

        /// <summary>
        /// Remove Client to list
        /// </summary>
        /// <param name="c"></param>
        public static void RemoveClient(Client c)
        {
            if (c.ingameState)//인게임 상태일때
                return;
            clientsList.Remove(c);
            Console.WriteLine("[GAME MANAGER] : client [" + c.SockTcp.RemoteEndPoint + "] was removed.");
            c.SockTcp.Close();
            ShowConnectedClientsCount();
        }

        /// <summary>
        /// Show Login People Number 
        /// </summary>
        public static void ShowConnectedClientsCount()
        {
            Console.WriteLine("[GAME MANAGER] : Number of connected clients [ " + clientsList.Count + " ]");
        }
    }
}
