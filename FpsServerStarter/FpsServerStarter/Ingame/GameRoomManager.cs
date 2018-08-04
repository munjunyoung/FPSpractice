using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FpsServerStarter.Ingame
{
    static class GameRoomManager
    {
        public static List<GameRoom> _gameRoomList = new List<GameRoom>();

        /// <summary>
        /// add Gaemroom in List
        /// </summary>
        /// <param name="room"></param>
        public static void AddGameRoom(GameRoom room)
        {
            _gameRoomList.Add(room);
            room.gameRoomNumber = _gameRoomList.Count;
            Console.WriteLine("INGAME MANAGER : Add GameRoom ! .. this gameroom in people count : " + _gameRoomList[0].ClientListInGameRoom.Count);
        }

        /// <summary>
        /// Delete GameRoom in List
        /// </summary>
        /// <param name="room"></param>
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
