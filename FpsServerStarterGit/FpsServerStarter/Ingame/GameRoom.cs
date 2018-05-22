using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FpsLib;

namespace FpsServerStarter.Ingame
{
    class GameRoom
    {
        public int gameRoomNumber = 0;
        public bool startGame = false;
        public int peopleCount = 0;
        public List<Client> gameRoomClientList = new List<Client>();


        public GameRoom()
        {

            startGame = false;
        }

        public void AddClientInGameRoom(Client c)
        {
            var setPos = new PositionInfo(new Random().Next(-45,45), 1, new Random().Next(-45, 45));
            c.startPosition = setPos;
            c.BeginSend(setPos);
            gameRoomClientList.Add(c);
            
            Console.WriteLine("GAME ROOM : GameRoonClientList Count = " + gameRoomClientList.Count);
            
        }

        public void CountDown()
        {
            Console.WriteLine("GAME ROOM" + gameRoomNumber + " : Start Game");
            startGame = true;
        }

        public void IngameProcessData()
        {
            
        }

        
    }
}
