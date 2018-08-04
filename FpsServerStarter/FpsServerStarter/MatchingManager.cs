using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FpsLib;
using FpsServerStarter.Ingame;
using Newtonsoft.Json;
namespace FpsServerStarter
{

    static class MatchingManager
    {
        public static List<Client> _queueClientList = new List<Client>();
        /// <summary>
        /// Client Matching Process
        /// </summary>
        /// <param name="c"></param>
        public static void MatchingProcess(Client c)
        {
            //When there is a game room that has not started
            foreach (GameRoom g in GameRoomManager._gameRoomList)
            {
                if (!g.startGame)
                {
                    g.AddClientInGameRoom(c);
                    break;
                }
            }
            QueueAddClient(c);
        }
        /// <summary>
        /// add Client to queueClientList
        /// </summary>
        /// <param name="c"></param>
        public static void QueueAddClient(Client c)
        {
            _queueClientList.Add(c);
            Console.WriteLine("[MATCHING MANAGER] : Matching people Count : " + _queueClientList.Count);
            // when one person is present
            if (_queueClientList.Count > 1)
                CreateGameRoom();
            else
                Console.WriteLine("[MATCHING MANAGER] : No people..");
        }

        /// <summary>
        /// Remove Client to queueClientList
        /// </summary>
        /// <param name="c"></param>
        public static void QueueRemoveClient(Client c)
        {
            _queueClientList.Remove(c);
            Console.WriteLine("[MATCHING MANAGER] : client Mathcing remove.. Count : " + _queueClientList.Count);
        }

        /// <summary>
        ///  Create GameRoom
        /// </summary>
        public static void CreateGameRoom()
        {
            var room = new GameRoom();

            GameRoomManager.AddGameRoom(room);
            foreach (Client cl in _queueClientList)
            {
                cl.ingameState = true;
                room.AddClientInGameRoom(cl);
            }
            
            _queueClientList.Clear();
            Console.WriteLine("[MATCHING MANAGER] : Create Room! Queue List all Remove .. show Count : " + _queueClientList.Count);
        }
    }
}
