using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameClasses {
    public class InputHelper {
        /*INSTANCE*/
        static InputHelper instance;

        /*FIELDS*/
        private GamePadState currGamePadState;
        private GamePadState prevGamePadState;
#if(!XBOX)
        private MouseState currMouseState;
        private MouseState prevMouseState;
        private KeyboardState currKeyboardState;
        private KeyboardState prevKeyboardState;
#endif
        private PlayerIndex index;

        /*PROPERTIES*/
        public PlayerIndex Index {
            get { return index; }
            set { index = value; }
        }

        public static InputHelper Instance {
            get {
                if (instance == null) instance = new InputHelper(PlayerIndex.One);
                return instance;
            }
        }

        public GamePadState CurrGamePadState {
            get { return currGamePadState; }
        }


        /*CONSTRUCTOR*/
        private InputHelper(PlayerIndex _playerIndex) {
            this.Index = _playerIndex;
        }

        /*METHODS*/
        public void Update() {
            if (prevGamePadState == null && currGamePadState == null) {
                prevGamePadState = currGamePadState = GamePad.GetState(index);
            } else {
                prevGamePadState = currGamePadState;
                currGamePadState = GamePad.GetState(index);
            }
            if (prevMouseState == null && currMouseState == null) {
                prevMouseState = currMouseState = Mouse.GetState();
            } else {
                prevMouseState = currMouseState;
                currMouseState = Mouse.GetState();
            }
            if (prevKeyboardState == null && currKeyboardState == null) {
                prevKeyboardState = currKeyboardState = Keyboard.GetState();
            } else {
                prevKeyboardState = currKeyboardState;
                currKeyboardState = Keyboard.GetState();
            }
        }
        public void UpdateMouseOnly() {
            if (prevMouseState == null && currMouseState == null) {
                prevMouseState = currMouseState = Mouse.GetState();
            } else {
                prevMouseState = currMouseState;
                currMouseState = Mouse.GetState();
            }
        }
        //Gamepad Methods
        public bool CheckForGamepadPress(Buttons _button) {
            return (currGamePadState.IsButtonDown(_button) && prevGamePadState.IsButtonUp(_button));
        }
        public bool CheckForGamepadHold(Buttons _button) {
            return (currGamePadState.IsButtonDown(_button) && prevGamePadState.IsButtonDown(_button));
        }
        public bool CheckForGamepadRelease(Buttons _button) {
            return (currGamePadState.IsButtonUp(_button) && prevGamePadState.IsButtonDown(_button));
        }
        //Keyboard Methods
        public bool CheckForKeyboardPress(Keys _key) {
            return (currKeyboardState.IsKeyDown(_key) && prevKeyboardState.IsKeyUp(_key));
        }
        public bool CheckForKeyboardHold(Keys _key) {
            return (currKeyboardState.IsKeyDown(_key) && prevKeyboardState.IsKeyDown(_key));
        }
        public bool CheckForKeyboardRelease(Keys _key) {
            return (currKeyboardState.IsKeyUp(_key) && prevKeyboardState.IsKeyDown(_key));
        }
        public bool CheckForCtrlHold() {
            return (CheckForKeyboardHold(Keys.LeftControl) || CheckForKeyboardHold(Keys.RightControl));
        }
        //Mouse Methods
        public bool CheckForLeftClick() {
            return (currMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released);
        }
        public bool CheckForLeftHold() {
            return (currMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Pressed);
        }
        public bool CheckForRightClick() {
            return (currMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released);
        }
        public bool CheckForRightHold() {
            return (currMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Pressed);
        }
        public bool CheckForLeftRelease() {
            return (currMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed);
        }
        public int CheckMouseScrollChange() {
            return currMouseState.ScrollWheelValue;
        }
        public Vector2 GetMousePosition() {
            return new Vector2(currMouseState.X, currMouseState.Y);
        }
        public Vector2 GetDeltaMousePosition() {
            return new Vector2(currMouseState.X - prevMouseState.X, currMouseState.Y - prevMouseState.Y);
        }
    }
}
