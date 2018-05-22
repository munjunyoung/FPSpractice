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

        //Client Matching Process
        public static void MatchingProcess(Client c)
        {
            //When there is a game room that has not started
            foreach (GameRoom g in IngameManager._gameRoomList)
            {
                if (!g.startGame)
                {
                    g.AddClientInGameRoom(c);
                    c.ingameState = true;
                    break;
                }
            }
            QueueAddClient(c);
        }

        //add Client to queueClientList
        public static void QueueAddClient(Client c)
        {
            _queueClientList.Add(c);
            Console.WriteLine("MATCHING MANAGER : Matching people Count : " + _queueClientList.Count);

            if (_queueClientList.Count > 1)
                CreateGameRoom();
            else
                Console.WriteLine("MathcingManager : No people..");
        }

        //Remove Client to queueClientList
        public static void QueueRemoveClient(Client c)
        {
            _queueClientList.Remove(c);
            Console.WriteLine("MATCHING MANAGER : client Mathcing remove.. Count : " + _queueClientList.Count);
        }

        //Create GameRoom
        public static void CreateGameRoom()
        {
            var room = new GameRoom();
            
            foreach (Client cl in _queueClientList)
            {
                cl.BeginSend(new StartGamePermission());
                cl.ingameState = true;
                room.AddClientInGameRoom(cl);
            }
            IngameManager.AddGameRoom(room);
            _queueClientList.Clear();
            Console.WriteLine("MATCHING MANAGER : Create Room! Queue List all Remove .. show Count : " + _queueClientList.Count);
        }
    }
}
