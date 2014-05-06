#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using GameClasses;
#endregion

namespace _2048Clone {
    /// <summary>
    /// 2048Clone
    /// MonoGame/XNA game by James Cote
    /// 
    /// About
    /// ------
    /// Basically a clone of the smash hit, 2048. If you know how to play the original,
    /// then you should have no problem with this. Here's a basic rundown though.
    /// 
    /// Controls
    /// ---------
    /// Arrow keys to move board, try to mix similar blocks together to score points!
    /// R to restart
    /// Space to close some of the popup messages (such as the one when you get 2048)
    /// </summary>
    public class Game1 : Game {

        public enum ScreenState { TitleScreen, InGame }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputHelper inputHelper;
        ScreenState screenState;
        ScreenState SetScreenState {
            set {
                screenState = value;
                switch (screenState) {
                    case ScreenState.TitleScreen:
                        titleMenu = new Menu();
                        titleMenu.SetPosition(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));
                        MenuButton playGameBtn, playBigGameBtn, exitGameBtn;
                        playGameBtn.name = "Play 4x4 Game";
                        playGameBtn.menuAction = StartRegularGame;
                        playBigGameBtn.name = "Play 8x8 Game";
                        playBigGameBtn.menuAction = StartBigGame;
                        exitGameBtn.name = "Exit Game";
                        exitGameBtn.menuAction = Exit;
                        titleMenu.AddMultiple(new MenuButton[] { playGameBtn, playBigGameBtn, exitGameBtn });
                        updateMethod = TitleScreenUpdate;
                        drawCalls = TitleScreenDraw;
                        break;
                    case ScreenState.InGame:
                        NewGame();
                        updateMethod = GameUpdate;
                        drawCalls = GameDraw;
                        DrawCallEvent += OverlayDraw;
                        break;
                    default:
                        break;
                }
            }
        }

        Menu titleMenu;

        GameBoard gameBoard;
        Vector2 boardPos;

        /*Game settings
         THESE ARE SET WHEN STARTING A NEW GAME*/
        int gridWidthToSet = 4, gridHeightToSet = 4;

        /*GUI Elements*/
        Rectangle topHUDRect;
        Rectangle popupRect;

        /*Condition checking*/
        bool checkForGameOver, checkFor2048;

        delegate void DrawCalls(SpriteBatch _spriteBatch);
        DrawCalls drawCalls;
        event DrawCalls DrawCallEvent {
            add {
                if (drawCalls == null || !drawCalls.GetInvocationList().Contains(value)) {
                    drawCalls += value;
                }
            }
            remove {
                drawCalls -= value;
            }
        }

        delegate void UpdateMode();
        UpdateMode updateMethod;

        public Game1()
            : base() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            //Loading dependecies
            inputHelper = InputHelper.Instance;
            Assets.TileWidth = 64;
            Assets.TileHeight = 64;
            Assets.pixel = new Texture2D(GraphicsDevice, 1, 1);
            Assets.pixel.SetData(new[] { Color.White });
            topHUDRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 16);
            popupRect = new Rectangle(0, GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 8);
            boardPos = new Vector2(GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 8);
            gameBoard = new GameBoard(boardPos);
            //Starting game at specified screen
            SetScreenState = ScreenState.TitleScreen;

            base.Initialize();
        }

        void NewGame() {
            checkForGameOver = checkFor2048 = true;
            gameBoard.NewGame(gridWidthToSet, gridHeightToSet);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.daFont = Content.Load<SpriteFont>(@"Fonts/ComicSans");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            inputHelper.Update();
            updateMethod();

            base.Update(gameTime);
        }

        void GameUpdate() {
            UpdateGameInput();

            //listens for game over
            if (checkForGameOver) {
                if (gameBoard.IsGameOver) {
                    DrawCallEvent -= GoalDraw; //remove the goal draw if it's there
                    DrawCallEvent += GameOverDraw;
                    checkForGameOver = false;
                    checkFor2048 = false; //it's game over, so no point checking for 2048 anymore either
                    return;
                }
            }
            //listens for 2048
            if (checkFor2048) {
                if (gameBoard.Reached2048) {
                    DrawCallEvent += GoalDraw;
                    checkFor2048 = false;
                }
            }
        }

        void UpdateGameInput() {
            if (inputHelper.CheckForKeyboardPress(Keys.Right) || inputHelper.CheckForGamepadPress(Buttons.DPadRight) || inputHelper.CheckForGamepadPress(Buttons.LeftThumbstickRight)) {
                gameBoard.BeginMove(gameBoard.RIGHT);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Left) || inputHelper.CheckForGamepadPress(Buttons.DPadLeft) || inputHelper.CheckForGamepadPress(Buttons.LeftThumbstickLeft)) {
                gameBoard.BeginMove(gameBoard.LEFT);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Up) || inputHelper.CheckForGamepadPress(Buttons.DPadUp) || inputHelper.CheckForGamepadPress(Buttons.LeftThumbstickUp)) {
                gameBoard.BeginMove(gameBoard.UP);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Down) || inputHelper.CheckForGamepadPress(Buttons.DPadDown) || inputHelper.CheckForGamepadPress(Buttons.LeftThumbstickDown)) {
                gameBoard.BeginMove(gameBoard.DOWN);
            } else if (inputHelper.CheckForKeyboardPress(Keys.R) || inputHelper.CheckForGamepadPress(Buttons.Y) 
                || (inputHelper.CheckForGamepadHold(Buttons.LeftShoulder) && inputHelper.CheckForGamepadPress(Buttons.RightShoulder))) {
                DrawCallEvent -= GameOverDraw;
                if (gameBoard.Reached2048) {
                    DrawCallEvent -= GoalDraw;
                }
                NewGame();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Space)) {
                if (gameBoard.Reached2048) {
                    DrawCallEvent -= GoalDraw;
                }
            } else if (inputHelper.CheckForKeyboardPress(Keys.Escape) || inputHelper.CheckForGamepadPress(Buttons.Back)) {
                SetScreenState = ScreenState.TitleScreen;
            }
        }

        void TitleScreenUpdate() {
            UpdateTitleScreenInput();
        }

        void UpdateTitleScreenInput() {
            if (inputHelper.CheckForKeyboardPress(Keys.Enter) || inputHelper.CheckForGamepadPress(Buttons.Start) || inputHelper.CheckForGamepadPress(Buttons.A)) {
                if (titleMenu.ListCount > 0) titleMenu.Select();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Escape) || inputHelper.CheckForGamepadPress(Buttons.Back)) {
                Exit();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Down) || inputHelper.CheckForGamepadPress(Buttons.DPadDown)) {
                titleMenu.Move(1);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Up) || inputHelper.CheckForGamepadPress(Buttons.DPadUp)) {
                titleMenu.Move(-1);
            }
        }

        void StartRegularGame() {
            gridWidthToSet = gridHeightToSet = 4;
            Assets.TileWidth = Assets.TileHeight = 128;
            SetScreenState = ScreenState.InGame;
        }

        void StartBigGame() {
            gridWidthToSet = gridHeightToSet = 8;
            Assets.TileWidth = Assets.TileHeight = 64;
            SetScreenState = ScreenState.InGame;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            drawCalls(spriteBatch); //all the extra draw commands (such as OverlayDraw, GameOverDraw, etc.) are called through here.
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void TitleScreenDraw(SpriteBatch _spriteBatch) {
            GraphicsDevice.Clear(Color.SeaGreen);
            _spriteBatch.DrawString(Assets.daFont, "Welcome to 2048!", Vector2.Zero, Color.Black);
            titleMenu.DrawMenu(_spriteBatch, Assets.daFont);
        }

        void GameDraw(SpriteBatch _spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            gameBoard.Draw(spriteBatch);
            spriteBatch.DrawLine(boardPos, new Vector2(boardPos.X, boardPos.Y + (Assets.TileHeight * gameBoard.GetBoardHeight)), Color.Black, 2.0f);
            spriteBatch.DrawLine(boardPos, new Vector2(boardPos.X + (Assets.TileWidth * gameBoard.GetBoardWidth), boardPos.Y), Color.Black, 2.0f);
            spriteBatch.DrawLine(new Vector2(boardPos.X, boardPos.Y + (Assets.TileHeight * gameBoard.GetBoardHeight)), new Vector2(boardPos.X + (Assets.TileWidth * gameBoard.GetBoardWidth), boardPos.Y + (Assets.TileHeight * gameBoard.GetBoardHeight)), Color.Black, 2.0f);
            spriteBatch.DrawLine(new Vector2(boardPos.X + (Assets.TileWidth * gameBoard.GetBoardWidth), boardPos.Y), new Vector2(boardPos.X + (Assets.TileWidth * gameBoard.GetBoardWidth), boardPos.Y + (Assets.TileHeight * gameBoard.GetBoardHeight)), Color.Black, 2.0f);
        }

        void OverlayDraw(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(topHUDRect, Color.Orange);
            _spriteBatch.DrawString(Assets.daFont, "Score: " + gameBoard.Score, Vector2.Zero, Color.Black);
        }

        void GameOverDraw(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(popupRect, Color.Red, 2.0f);
            _spriteBatch.DrawString(Assets.daFont, "Game Over! Press R to try again", new Vector2(popupRect.X,popupRect.Y), Color.Black);
        }

        void GoalDraw(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(popupRect, Color.Lime, 2.0f);
            _spriteBatch.DrawString(Assets.daFont, "Congratulations! You made it to 2048! Press Space to remove this message or R to restart.", new Vector2(popupRect.X, popupRect.Y), Color.Black);
        }
    }

    public static class SpriteBatchExtender {
        public static void DrawLine(this SpriteBatch _spriteBatch, Vector2 _pos1, Vector2 _pos2, Color _color, float _stroke) {
            _spriteBatch.Draw(Assets.pixel, _pos1, null, _color, (float)Math.Atan2(_pos2.Y - _pos1.Y, _pos2.X - _pos1.X), new Vector2(0f, Assets.pixel.Height / 2), new Vector2(Vector2.Distance(_pos1, _pos2), _stroke), SpriteEffects.None, 0f);
        }
        public static void DrawRect(this SpriteBatch _spriteBatch, Rectangle _rect, Color _color) {
            _spriteBatch.Draw(Assets.pixel, _rect, _color);
        }
        public static void DrawRect(this SpriteBatch _spriteBatch, Rectangle _rect, Color _color, float _lineStroke) {
            _spriteBatch.Draw(Assets.pixel, _rect, _color);
            _spriteBatch.DrawLine(_rect.Location(), new Vector2(_rect.X + _rect.Width, _rect.Y), Color.Black, _lineStroke);
            _spriteBatch.DrawLine(new Vector2(_rect.X, _rect.Y + _rect.Height), new Vector2(_rect.X + _rect.Width, _rect.Y + _rect.Height), Color.Black, _lineStroke);
            _spriteBatch.DrawLine(_rect.Location(), new Vector2(_rect.X, _rect.Y + +_rect.Height), Color.Black, _lineStroke);
            _spriteBatch.DrawLine(new Vector2(_rect.X + _rect.Width, _rect.Y), new Vector2(_rect.X + _rect.Width, _rect.Y + _rect.Height), Color.Black, _lineStroke);
        }
    }

    public static class RectangleExtender {
        public static Vector2 Location(this Rectangle _rect) {
            return new Vector2(_rect.X, _rect.Y);
        }
    }
}
