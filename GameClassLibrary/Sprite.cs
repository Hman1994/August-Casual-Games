using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClassLibrary
{
    public class Sprite
    {
        public Texture2D _texture{ get; set; }
        public Vector2 _position { get; set; }
        private Rectangle rectangle;
        public bool IsVisible;
        protected Color pColor = Color.White;

        public virtual Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            }

            set
            {
                rectangle = value;
            }
        }
        

        public Sprite()
        {}

        public Sprite(Texture2D _tex, Vector2 _pos)
        {
            _texture = _tex;
            _position = _pos;
            IsVisible = true;
            rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
        }

        public Sprite(Texture2D _tex, Vector2 _pos, Color c)
        {
            _texture = _tex;
            _position = _pos;
            pColor = c;
            IsVisible = true;
            rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
        }

        public virtual void Draw(SpriteBatch sp)
        {
            sp.Begin();
            if (_texture != null && IsVisible)
                sp.Draw(_texture, _position, pColor);
            sp.End();
        }

        public virtual void Draw(SpriteBatch sp, Color c)
        {
            sp.Begin();
            if (_texture != null && IsVisible)
            {
                sp.Draw(_texture, _position, c);
            }
        }

        public virtual void Draw(SpriteBatch sp, float layer)
        {
            sp.Begin();
            if (_texture != null)
                sp.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, layer);
            sp.End();
        }

        public bool CollisiionDetection(Rectangle otherRec)
        {
            if (this.Rectangle.Intersects(otherRec))
                return true;
            else
                return false;
        }
    }
}
