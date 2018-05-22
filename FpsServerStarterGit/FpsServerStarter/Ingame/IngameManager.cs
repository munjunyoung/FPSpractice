using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FpsServerStarter.Ingame
{
    static class IngameManager
    {
        public static List<GameRoom> _gameRoomList = new List<GameRoom>();

        //add Gaemroom in List
        public static void AddGameRoom(GameRoom room)
        {
            _gameRoomList.Add(room);
            Console.WriteLine("INGAME MANAGER : Add GameRoom ! .. this gameroom in people count : " + _gameRoomList[0].gameRoomClientList.Count);
        }

        public static void DellGameRoom(GameRoom room)
        {
            _gameRoomList.Remove(room);
            Console.WriteLine("INGAME MANAGER : Remove GameRoom");
        }

        public static void FindGameRoom()
        {

        }
    }
}
