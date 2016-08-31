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
    public class MenuItem
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private Texture2D _texture;

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        private bool _choosen;

        public bool Choosen
        {
            get { return _choosen; }
            set { _choosen = value; }
        }

        public MenuItem(int id, string label, Vector2 startPosition, Texture2D texture)
        {
            _id = id;
            _text = label;
            _position = startPosition;
            _texture = texture;
            _selected = false;

        }

        public MenuItem(Vector2 startPosition)
        {
            _position = startPosition;
            _selected = false;
        }

        public Rectangle Target
        {
            get { return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height); }
        }

        public void update(GameTime t)
        {
            MouseState m = Mouse.GetState();
            if (m.LeftButton == ButtonState.Pressed)
                if (Target.Contains(new Point(m.X, m.Y)))
                    _choosen = true;
        }

        internal void draw(SpriteBatch sp, SpriteFont font)
        {
            Vector2 textSize = font.MeasureString(_text);
            Vector2 textPos = _position + new Vector2(_texture.Width - textSize.X, _texture.Height * 2) / 2;

            if (_selected)
                sp.Draw(_texture, _position, Color.Red);
            else
                sp.Draw(_texture, _position, Color.Green);

            sp.DrawString(font, _text, textPos, Color.Red);
        }
    }
}
