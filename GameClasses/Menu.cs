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
        List<bool> isButtonDynamic; //bool for checking if button is looking for a value to update its name string with
        List<string> buttonName; //the name of a button after modifications made to update it with an observable value
        int selectedMenuIndex = 0;
        Vector2 menuPosition;
        float spacing;
        List<Vector2> btnPosList;
        float menuButtonWidth = 256; //it's just a fake width for now

        string title;

        Color regularColor = Color.Black, highlightedColor = Color.Yellow, pressedColor = Color.Lime;
        bool isMouseHeld;

        public Menu() {
            menuList = new List<MenuButton>();
            isButtonDynamic = new List<bool>();
            buttonName = new List<string>();
            btnPosList = new List<Vector2>();
            spacing = 32.0f;
        }

        public void SetPosition(Vector2 _pos){
            menuPosition = _pos;
        }

        public void SetTitle(string _title) {
            title = _title;
        }

        public void Add(MenuButton _menuButton) {
            //Addding menu button
            menuList.Add(_menuButton);
            //Adding button's name string
            buttonName.Add(_menuButton.name);
            //Adding button dynamic condition for button and checking if it is dyanmic
            isButtonDynamic.Add(false);
            if (_menuButton.name.IndexOf("%o") > -1) { //the string "%o" will let the program know that there's an observable value it needs to check for
                isButtonDynamic[isButtonDynamic.Count - 1] = true;
            }
            //Adding a position for the new button
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

        public void UpdateValues(string[] _observableArr) {
            int currentObserveIndex = 0;
            for (int i = 0; i < menuList.Count; i++) {
                if (isButtonDynamic[i]) {
                    buttonName[i] = menuList[i].name.Replace("%o", _observableArr[currentObserveIndex]);
                }
            }
        }

        public void DrawMenu(SpriteBatch _spriteBatch, SpriteFont _font){
            if (title != null)_spriteBatch.DrawString(_font, title, new Vector2(btnPosList[0].X, btnPosList[0].Y - (spacing * 1.5f)), Color.Black);
            for (int i = 0; i < menuList.Count; i++) {
                Color colorToUse = (i == selectedMenuIndex) ? highlightedColor : regularColor; ;
                if (isMouseHeld && colorToUse == highlightedColor) {
                    colorToUse = pressedColor;
                }
                _spriteBatch.DrawString(_font, buttonName[i], btnPosList[i], colorToUse);
            }
        }
    }
}
