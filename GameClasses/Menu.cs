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
        Vector2 menuPosition;
        float spacing;
        List<Vector2> btnPosList;
        float menuButtonWidth = 256; //it's just a fake width for now

        Color regularColor = Color.Black, highlightedColor = Color.Yellow, pressedColor = Color.Lime;
        bool isMouseHeld;

        public Menu() {
            menuList = new List<MenuButton>();
            btnPosList = new List<Vector2>();
            spacing = 32.0f;
        }

        public void SetPosition(Vector2 _pos){
            menuPosition = _pos;
        }

        public void Add(MenuButton _menuButton) {
            menuList.Add(_menuButton);
            btnPosList.Add(new Vector2(menuPosition.X, menuPosition.Y + (spacing * btnPosList.Count)));
        }

        public void AddMultiple(MenuButton[] _menuButtonArr) {
            for (int i = 0; i < _menuButtonArr.Length; i++) {
                Add(_menuButtonArr[i]);
            }
        }

        public void Select() {
            if (menuList.Count > 0) menuList[selectedMenuIndex].menuAction();
        }

        public void Move(int _value) {
            selectedMenuIndex += _value;
            if (selectedMenuIndex < 0) {
                selectedMenuIndex = menuList.Count - 1;
            } else if (selectedMenuIndex > menuList.Count - 1) {
                selectedMenuIndex = 0;
            }
        }

        public bool Update(Vector2 _mousePos, bool _isMouseHeld, bool _isMouseReleased) {
            if (_mousePos.X >= menuPosition.X && _mousePos.X <= menuPosition.X + menuButtonWidth) {
                for (int i = 0; i < menuList.Count; i++) {
                    if (_mousePos.Y >= btnPosList[i].Y && _mousePos.Y <= btnPosList[i].Y + spacing) {
                        selectedMenuIndex = i;
                        isMouseHeld = _isMouseHeld;
                        if (_isMouseReleased) {
                            return true;
                        }
                    }
                }
            }
            if (_isMouseReleased) isMouseHeld = false;
            return false;
        }

        public void DrawMenu(SpriteBatch _spriteBatch, SpriteFont _font){
            for (int i = 0; i < menuList.Count; i++) {
                Color colorToUse = (i == selectedMenuIndex) ? highlightedColor : regularColor; ;
                if (isMouseHeld && colorToUse == highlightedColor) {
                    colorToUse = pressedColor;
                }
                _spriteBatch.DrawString(_font, menuList[i].name, btnPosList[i], colorToUse);
            }
        }
    }
}
