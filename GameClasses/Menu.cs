using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameClasses {
    public delegate void MenuAction();

    public struct MenuButton {
        public string name;
        public MenuAction menuAction;
    }

    public class Menu {
        List<MenuButton> menuList;
        int selectedMenuIndex = 0;
        Vector2 position;

        Color regularColor = Color.Black, highlightedColor = Color.Yellow;

        public int ListCount {
            get {
                return menuList.Count;
            }
        }

        public Menu() {
            menuList = new List<MenuButton>();
        }

        public void SetPosition(Vector2 _pos){
            position = _pos;
        }

        public void Add(MenuButton _menuButton) {
            menuList.Add(_menuButton);
        }

        public void AddMultiple(MenuButton[] _menuButtonArr) {
            for (int i = 0; i < _menuButtonArr.Length; i++) {
                menuList.Add(_menuButtonArr[i]);
            }
        }

        public void Select() {
            menuList[selectedMenuIndex].menuAction();
        }

        public void Move(int _value) {
            selectedMenuIndex += _value;
            if (selectedMenuIndex < 0) {
                selectedMenuIndex = menuList.Count - 1;
            } else if (selectedMenuIndex > menuList.Count - 1) {
                selectedMenuIndex = 0;
            }
        }

        public void DrawMenu(SpriteBatch _spriteBatch, SpriteFont _font){
            for (int i = 0; i < menuList.Count; i++) {
                Color colorToUse = (i == selectedMenuIndex) ? highlightedColor : regularColor;
                _spriteBatch.DrawString(_font, menuList[i].name, new Vector2(position.X, position.Y + (32 * i)), colorToUse);
            }
        }
    }
}
