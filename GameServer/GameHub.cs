using GameClassLibrary;
using Microsoft.AspNet.SignalR;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Threading.Tasks;

namespace GameServer
{
    public class GameHub : Hub
    {

        static Timer t;
        Random random = new Random();
        static List<Vector2> PosistionsOfCollectables = new List<Vector2>();
        static List<Vector2> playerOnePositionBarriers = new List<Vector2>();
        static string playerOneChar;
        static string playerOne;
        static string playerTwo;
        static int c = 0;
        static bool gameRunning = false;


        #region StartGame send once

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public void SendPlayer(string charType)
        {
            if (playerOneChar == null)
            {
                SendCollectables();
                playerOneChar = charType;
            }
            else
            {
                SendCollectables();
                Clients.Caller.otherStartpoint(new Vector2(700, 200));
                Clients.Caller.sendPlayer(playerOne, playerOneChar);
                Clients.Others.sendPlayer(Context.ConnectionId, charType);
                gameRunning = true;
                GameStart(gameRunning);
                playerOneChar = null;
            }
        }

        public void SendBarriers(List<Vector2> positions)
        {
            if (playerOnePositionBarriers.Count == 0)
            {
                playerOnePositionBarriers = positions;
                playerOne = Context.ConnectionId;
            }
            else
            {
                playerTwo = Context.ConnectionId;
                Clients.Caller.sendBarriers(playerOne, playerOnePositionBarriers);
                Clients.Others.sendBarriers(playerTwo, positions);
                playerOnePositionBarriers.Clear();
            }
        }

        public void SendCollectables()
        {
            if (PosistionsOfCollectables.Count == 0 && c == 0)
            {
                c++;
                int temp = random.Next(3, 10);
                for (int i = 0; i < temp; i++) //create collectables
                {
                    PosistionsOfCollectables.Add(new Vector2(random.Next(50, 700), random.Next(50, 500)));
                }

            }
            else
            {
                while (c != 1)
                { }
                c = 0;
                Clients.All.sendPositionCollectables(PosistionsOfCollectables);

                PosistionsOfCollectables.Clear();
            }

        }

        #endregion

        #region Updating Methodes

        public void UpdatePosition(Vector2 newPlayerPositon)
        {
            Clients.Others.updatePosition(newPlayerPositon);
        }

        #endregion



        public void NewSuperCollectable()
        {
            t = new Timer(random.Next(2000, 10000));
            t.Elapsed += T_Elapsed;
            if (gameRunning)
                t.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Clients.All.newSuperCollectable(new Vector2(random.Next(50, 700), random.Next(50, 500)));
        }

        public void GameStart(bool g)
        {
            gameRunning = g;
            NewSuperCollectable();
        }

       
    }
}