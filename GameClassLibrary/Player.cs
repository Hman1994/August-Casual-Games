using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClassLibrary
{
    public class Player : Sprite
    {
        static int ID = 1;
        int PlayerID;
        public Character PlayerChar;    
     
        public int score;
        Game game;

        public Player() : base()
        {

        }

        public Player(Character character, Vector2 pos, Color c, Game g) : base(character._texture, pos, c) //create the player
        {
            PlayerID = ID++;
            PlayerChar = character;                  
            game = g;
        }

        public void Move(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.W) && _position.Y > 0) //check the move direction and update position
            {
                _position -= new Vector2(0, PlayerChar.movementSpeed); 
                
            }
            if (state.IsKeyDown(Keys.S) && _position.Y < 550)
            {
                _position += new Vector2(0,PlayerChar.movementSpeed);
            
            }
            if (state.IsKeyDown(Keys.D) && _position.X < 760)
            {
                _position += new Vector2(PlayerChar.movementSpeed, 0);
             
            }
            if (state.IsKeyDown(Keys.A) && _position.X > 0)
            {
                _position -= new Vector2(PlayerChar.movementSpeed, 0);
              
            }
           
        }

        public void Collect(Collectable c)
        {
            score += c.Value;
        }

        public void Draw(SpriteBatch sp, SpriteFont sf)
        {
            sp.Begin();
            sp.Draw(PlayerChar._texture, _position, pColor); //draw the player
            //sp.Draw(healthBar, new Rectangle((int)_position.X , (int)_position.Y + _texture.Height + 2, healthBar.Width, healthBar.Height), Color.Red); //draw the negative HealthBar
            //sp.Draw(healthBar, new Rectangle((int)_position.X , (int)_position.Y + _texture.Height + 2, (int)(healthBar.Width *), healthBar.Height), Color.Green); //calculate and draw the positive HealthBar above           
            sp.End();
        }
    }
}
