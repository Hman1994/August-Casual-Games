using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClassLibrary
{
    public class Barrier : Sprite
    {
        public string createdClientID;
        int health;
        Texture2D[] textures;

        public Barrier(string id, Texture2D tex, Vector2 pos) : base(tex, pos)
        {
            createdClientID = id;
            health = 15;
        }

        public Barrier(string id, Texture2D[] tex, Vector2 pos, Color c) : base(tex[0], pos, c)
        {
            createdClientID = id;
            textures = tex;
            health = 15;
        }

        public override void Draw(SpriteBatch sp)
        {
            if (health <= 8)
            {
                sp.Begin();
                sp.Draw(textures[0], _position, pColor);
                sp.Draw(textures[1], _position, Color.White);
                sp.End();
            }
            else
                base.Draw(sp);
        }

    }
}
