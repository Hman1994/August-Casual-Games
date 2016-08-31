using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameClassLibrary;
using System.Collections.Generic;
using System;
using Microsoft.AspNet.SignalR.Client;
using System.Timers;
using Game1;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

// Alan Hardiman - S00146801 - Computing in Games Development - Year 3

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        float timer;
        int timecounter;
  
        string clientID;
        Color playerColor = Color.Green;
        Color enemyColor = Color.Red;

        enum currentDisplay { Selection, Game, Score };
        currentDisplay currentState = currentDisplay.Selection;

        enum endGameStatuses { Win, Lose, Draw }
        endGameStatuses gameOutcome = endGameStatuses.Draw;

        Player player;
        Player Enemy;

        Menu menu;
        string[] menuOptions = new string[] { "Fast", "Normal", "Strong" };

        Vector2 startVector = new Vector2(50, 250);

        KeyboardState oldState, newState;

        static Random random = new Random();
        bool isGameStarted = false;

    
        List<Collectable> Collectables = new List<Collectable>();
        List<Barrier> Barriers = new List<Barrier>();
        List<Collectable> pickUp = new List<Collectable>();
        List<Barrier> destroyBarrier = new List<Barrier>();
       

        Texture2D backgroundTexture;
        Texture2D menuBackground;
        Texture2D[] textures;
        Texture2D collectibleTexture;
        Texture2D supercollectibleTexture;
        Texture2D[] barrierTexture;      
        Texture2D textureManyCollectibles;
        Texture2D homeTexture;
        Texture2D awayTexture;
        Texture2D announcementTexture;
        SpriteFont message;

        static IHubProxy proxy;
        HubConnection connection = new HubConnection("http://localhost:5553/");
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
  
            oldState = Keyboard.GetState();
            graphics.PreferredBackBufferWidth = 800; //set the size of the window
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();


            proxy = connection.CreateHubProxy("GameHub");
            clientID = connection.ConnectionId;
            Action<string, string> RecivePlayer = recivePlayerMessage;
            Action<string, Vector2[]> ReciveBarriersPositions = reciveBarriers;
            Action<Vector2> ReciveNewPosition = reciveNewPlayerPosition;
        
            Action<Vector2> ReciveNewSuperCollectable = reciveSupercollectable;
            Action<List<Vector2>> ReciveCollectablePositions = reciveCollectablePositions;
            Action<Vector2> ReciveDiffrentStartposition = reciveDiffrentStartposition;

            proxy.On("sendPositionCollectables", ReciveCollectablePositions);
            proxy.On("sendBarriers", ReciveBarriersPositions);
            proxy.On("otherStartpoint", ReciveDiffrentStartposition);
            proxy.On("sendPlayer", RecivePlayer);
            proxy.On("updatePosition", ReciveNewPosition);         
            proxy.On("newSuperCollectable", ReciveNewSuperCollectable);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            IsMouseVisible = true;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            message = Content.Load<SpriteFont>("SpriteFont\\message"); //load spriteFont

          
            backgroundTexture = Content.Load<Texture2D>("Background"); //load all textures
            menuBackground = Content.Load<Texture2D>("Sprites\\menuBackground");
            textures = new Texture2D[] { Content.Load<Texture2D>("Sprites\\tennisball"), Content.Load<Texture2D>("Sprites\\soccerball"), Content.Load<Texture2D>("Sprites\\football") };
            barrierTexture = new Texture2D[] { Content.Load<Texture2D>("Sprites\\Barrier"), Content.Load<Texture2D>("Sprites\\brokenbarrier") };
            collectibleTexture = Content.Load<Texture2D>("Sprites\\collectible1");
            supercollectibleTexture = Content.Load<Texture2D>("Sprites\\collectible2");           
            homeTexture = Content.Load<Texture2D>("Sprites\\home sign");
            awayTexture = Content.Load<Texture2D>("Sprites\\guest sign");
            announcementTexture = Content.Load<Texture2D>("Sprites\\announcement");
            textureManyCollectibles = Content.Load<Texture2D>("Sprites\\manyCollectibles");
            
            Console.WriteLine("////////////////Connecting///////////////////");
            connection.Start().Wait();
            Console.WriteLine("////////////////Connected///////////////////");

          
            for (int i = 0; i < 4; i++) //create barriers 
            {
                Barriers.Add(new Barrier(clientID, barrierTexture, new Vector2(random.Next(50, graphics.GraphicsDevice.Viewport.Width - 50), random.Next(50, graphics.GraphicsDevice.Viewport.Height - 50)), playerColor));
            }

            menu = new Menu(new Vector2(300, 250), menuOptions, message, textures); //create the menu

            menu.Active = true; //set menu active

       
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState(); //set the current keyboardState

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            if (currentState == currentDisplay.Selection)
            {
                menu.CheckMouse();

                player = createPlayer(clientID, menu.MenuAction, playerColor);

                if (player != null)
                {
                    proxy.Invoke("SendPlayer", menu.MenuAction);

                    sendBarriers(Barriers);
                }

                menu.MenuAction = null; //reset the selection
            }

            
            if (currentState == currentDisplay.Game) //if the game is running
            {
                if (isGameStarted)
                {
                    player.Move(newState); //this checks for the movement of the player 
                    proxy.Invoke("UpdatePosition", player._position);

                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    timecounter += (int)timer;
                    if (timer >= 1.0F) timer = 0F;

                  
                    foreach (var item in Collectables)
                    {
                        if (player.CollisiionDetection(item.Rectangle))
                        {
                            pickUp.Add(item);
                            item.IsVisible = false;
                            player.Collect(item);
                        }

                        if (Enemy.CollisiionDetection(item.Rectangle))
                        {
                            pickUp.Add(item);
                            item.IsVisible = false;
                            Enemy.Collect(item);
                        }
                    }
                    
                    foreach (var item in destroyBarrier)
                    {
                        Barriers.Remove(item);
                    }
                    foreach (var item in pickUp)
                    {
                        Collectables.Remove(item);
                    }
                 

                    destroyBarrier.Clear();
                    pickUp.Clear();
                

                    // end game criteria / conditions

                    if (Collectables.Count == 0)
                        currentState = currentDisplay.Score;

                    if (player.score >= 300)
                    {
                        currentState = currentDisplay.Score;
                    }

                    if (timecounter == 180)
                    {
                        currentState = currentDisplay.Score;
                    }

                    if (currentState == currentDisplay.Score)
                    {
                        isGameStarted = false;
                        proxy.Invoke("StartGame", isGameStarted);
                        if (player.score > Enemy.score)
                            gameOutcome = endGameStatuses.Win;
                        if (player.score < Enemy.score)
                            gameOutcome = endGameStatuses.Lose;
                        if (player.score == Enemy.score)
                            gameOutcome = endGameStatuses.Draw;
                    }

                }
            }          

           
            base.Update(gameTime);

            oldState = newState;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            if (currentState == currentDisplay.Selection)
               
             menu.Draw(spriteBatch); // handles the menu drawing             

            #region Draw the Game
            if (currentState == currentDisplay.Game) //if game is started
            {
                spriteBatch.Begin();
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 800, 600), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f); //draw the background
                if (Enemy != null)
                    spriteBatch.DrawString(message, "Score: " + Enemy.score.ToString(), new Vector2(500, 0), enemyColor);
                spriteBatch.Draw(homeTexture, new Vector2(0, 0), Color.Wheat);
                spriteBatch.Draw(awayTexture, new Vector2(650, 0), Color.Wheat);
                spriteBatch.DrawString(message, "Score: " + player.score.ToString(), new Vector2(200, 0), playerColor);                
                spriteBatch.End();

                if (Enemy != null)
                {
                    Enemy.Draw(spriteBatch);
                    spriteBatch.Begin();
                    spriteBatch.DrawString(message, "Opponent has joined the game", new Vector2(400, 100), Color.Black);
                    spriteBatch.End();

                }

                if(timecounter >= 10) // timed announcement is here, FYI time is handled in seconds not minutes
                {
                    spriteBatch.Begin();               
                    spriteBatch.Draw(announcementTexture, new Vector2(300, 100), Color.Wheat);
                    spriteBatch.End();
                }

                if (timecounter >= 120) // code here displays message if only one minute left in the game
                {
                    spriteBatch.Begin();
                    spriteBatch.DrawString(message, "Only one minute left", new Vector2(450, 25), Color.Black);
                    spriteBatch.End();
                }

                if (Enemy == null)
                {
                    spriteBatch.Begin();
                    spriteBatch.DrawString(message, "Waiting on opponent to join", new Vector2(400, 0), Color.Black);
                    spriteBatch.End();

                }
                player.Draw(spriteBatch, message); //draw the player

                foreach (var item in Collectables)
                {
                    item.Draw(spriteBatch); 
                  
                }

                if(Collectables.Count > 10)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(textureManyCollectibles, new Vector2(200, 15), Color.Wheat);                
                    spriteBatch.End();

                }

                          
                foreach (var item in Barriers)
                {
                    item.Draw(spriteBatch); // barriers are drawn here 
                }
            }

   // draws the post game screen, displaying the score, winner and loser 
     
            if (currentState == currentDisplay.Score)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(menuBackground, new Rectangle(0, 0, 800, 600), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f); //draw the background
                spriteBatch.End();
                player._position = new Vector2(300, 400);
                Enemy._position = new Vector2(450, 400);

                player.Draw(spriteBatch);
                Enemy.Draw(spriteBatch);
                Vector2 fontPos = new Vector2(player._texture.Width / 2, -10);
                Vector2 namePos = new Vector2(player._texture.Width / 2, player._texture.Height + 10);

                spriteBatch.Begin();
                spriteBatch.DrawString(message, gameOutcome.ToString(), new Vector2(330, 150), Color.Gold, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
                spriteBatch.DrawString(message, player.score.ToString(), fontPos + player._position, playerColor, 0, message.MeasureString(player.score.ToString()) / 2, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(message, Enemy.score.ToString(), fontPos + Enemy._position, enemyColor, 0, message.MeasureString(Enemy.score.ToString()) / 2, 1, SpriteEffects.None, 0);

                spriteBatch.DrawString(message, "You", namePos + player._position, playerColor, 0, message.MeasureString("You") / 2, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(message, "Enemy", namePos + Enemy._position, enemyColor, 0, message.MeasureString("Enemy") / 2, 1, SpriteEffects.None, 0);
                spriteBatch.End();

            }

            #endregion

            base.Draw(gameTime);
        }

        //server information is handled below

        private void reciveSupercollectable(Vector2 obj)
        {
            {
                Collectables.Add(new SuperCollectable(supercollectibleTexture, obj));
                     
            }
            
        }

        private void reciveNewPlayerPosition(Vector2 obj)
        {
            Enemy._position = obj;
        }

        private void reciveBarriers(string arg1, Vector2[] arg2)
        {
            foreach (var item in arg2)
            {
                Barriers.Add(new Barrier(arg1, barrierTexture, item, enemyColor));
            }
        }

        private void recivePlayerMessage(string arg1, string arg2)
        {
            Enemy = createPlayer(arg1, arg2, enemyColor);
            isGameStarted = true;
       
        }

        private Player createPlayer(string id, string type, Color c)
        {
            Player temp = null;
            if (type != null)
            {

                switch (type.ToUpper()) //creates the player character 
                {
                    case "FAST":
                        currentState = currentDisplay.Game;
                        temp = new Player(new Character(id, textures[0], 7, 3), startVector, c, this);
                        break;
                    case "NORMAL":
                        currentState = currentDisplay.Game;
                        temp = new Player(new Character(id, textures[1], 5, 4), startVector, c, this);
                        break;
                    case "STRONG":
                        currentState = currentDisplay.Game;
                        temp = new Player(new Character(id, textures[2], 3, 5), startVector, c, this);
                        break;
                    default:
                        break;
                }
            }


            return temp;
        }

        private void reciveCollectablePositions(List<Vector2> obj)
        {
            foreach (var item in obj)
            {
                Collectables.Add(new Collectable(collectibleTexture, item));
            }
        }

        private void reciveDiffrentStartposition(Vector2 obj)
        {
            player._position = obj;
        }

        public bool OutsideScreen(Sprite obj)
        {
            if (!obj.Rectangle.Intersects(Window.ClientBounds))
            {
                return true;
            }
            else
                return false;
        }

        private void sendBarriers(List<Barrier> barriers)
        {
            List<Vector2> temp = new List<Vector2>();
            foreach (var item in barriers)
            {
                temp.Add(item._position);
            }

            proxy.Invoke("SendBarriers", temp);
        }

        
    }
}
