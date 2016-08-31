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
    public class Menu
    {
        #region Properties

        bool _active;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        private MenuItem[] _menuItems;

        private SpriteFont _spFont;

        public string MenuAction;

        private int _selectedID = 0;

        #endregion

        #region Constructor

        public Menu(Vector2 StartingPosition, string[] menuItemText, SpriteFont spFont, Texture2D[] menuItemTexture)
        {
            _spFont = spFont;
            int itemGap = 50;

            _menuItems = new MenuItem[menuItemText.Length];
            for (int i = 0; i < _menuItems.Length; i++)
            {
                _menuItems[i] = new MenuItem(i, menuItemText[i], StartingPosition, menuItemTexture[i]);
                StartingPosition += new Vector2(_menuItems[0].Texture.Width + itemGap, 0);
            }
        }


        public void CheckMouse()
        {
            foreach (var item in _menuItems)
            {
                MouseState m = Mouse.GetState();
                if (m.LeftButton == ButtonState.Pressed)
                    if (item.Target.Contains(new Point(m.X, m.Y)))
                        MenuAction = item.Text.ToUpper();
            }
        }

        public void Draw(SpriteBatch sp)
        {
            if (_active)
            {
                sp.Begin();
                foreach (MenuItem n in _menuItems)
                {
                    n.draw(sp, _spFont);
                }
                sp.End();
            }
        }

        #endregion
    }
}
