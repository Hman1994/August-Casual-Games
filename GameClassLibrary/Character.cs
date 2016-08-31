using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClassLibrary
{
    public class Character : Sprite
    {
        string ID;             
        public int movementSpeed;
        public int strength;

        public Character(string id, Texture2D tex, int speed, int str) : base(tex, Vector2.Zero)
        {
            ID = id;                 
            movementSpeed = speed;
            strength = str;

        }

    }
}
