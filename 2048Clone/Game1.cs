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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputHelper inputHelper;

        GameBoard gameBoard;
        Vector2 boardPos;

        Rectangle topHUDRect;
        Rectangle popupRect;

        bool checkForGameOver, checkFor2048;

        delegate void AdditionalDraws(SpriteBatch _spriteBatch);
        AdditionalDraws drawSomeMoreStuff;
        event AdditionalDraws DrawSomeMoreStuff {
            add {
                if (drawSomeMoreStuff == null || !drawSomeMoreStuff.GetInvocationList().Contains(value)) {
                    drawSomeMoreStuff += value;
                }
            }
            remove {
                drawSomeMoreStuff -= value;
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
            inputHelper = InputHelper.Instance;
            Assets.pixel = new Texture2D(GraphicsDevice, 1, 1);
            Assets.pixel.SetData(new[] { Color.White });
            topHUDRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 16);
            popupRect = new Rectangle(0, GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 8);
            boardPos = new Vector2(GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 8);
            NewGame();
            updateMethod = GameUpdate;
            drawSomeMoreStuff = OverlayDraw;

            base.Initialize();
        }

        void NewGame() {
            checkForGameOver = checkFor2048 = true;
            gameBoard = new GameBoard(boardPos);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            for (int i = 0; i < Assets.AMOUNT_OF_TILE_SPRITES; i++) {
                //Load tile images into the static variables
                Assets.TileImagesArr[i] = Content.Load<Texture2D>(@"" + Math.Pow(2,i + 1));
            }
            Assets.TileSpriteHeight = Assets.TileImagesArr[0].Width;
            Assets.TileSpriteWidth = Assets.TileImagesArr[0].Height;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            updateMethod();

            base.Update(gameTime);
        }

        void GameUpdate() {
            UpdateGameInput();

            //listens for game over
            if (checkForGameOver) {
                if (gameBoard.IsGameOver) {
                    DrawSomeMoreStuff -= GoalDraw; //remove the goal draw if it's there
                    DrawSomeMoreStuff += GameOverDraw;
                    checkForGameOver = false;
                    checkFor2048 = false; //it's game over, so no point checking for 2048 anymore either
                    return;
                }
            }
            //listens for 2048
            if (checkFor2048) {
                if (gameBoard.Reached2048) {
                    DrawSomeMoreStuff += GoalDraw;
                    checkFor2048 = false;
                }
            }
        }

        void UpdateGameInput() {
            inputHelper.Update();
            if (inputHelper.CheckForKeyboardPress(Keys.Right)) {
                gameBoard.BeginMove(gameBoard.RIGHT);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Left)) {
                gameBoard.BeginMove(gameBoard.LEFT);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Up)) {
                gameBoard.BeginMove(gameBoard.UP);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Down)) {
                gameBoard.BeginMove(gameBoard.DOWN);
            } else if (inputHelper.CheckForKeyboardPress(Keys.R)) {
                DrawSomeMoreStuff -= GameOverDraw;
                if (gameBoard.Reached2048) {
                    DrawSomeMoreStuff -= GoalDraw;
                }
                NewGame();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Space)) {
                if (gameBoard.Reached2048) {
                    DrawSomeMoreStuff -= GoalDraw;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            gameBoard.Draw(spriteBatch);
            spriteBatch.DrawLine(boardPos, new Vector2(boardPos.X, boardPos.Y + (Assets.TileSpriteHeight * 4)), Color.Black, 2.0f);
            spriteBatch.DrawLine(boardPos, new Vector2(boardPos.X + (Assets.TileSpriteWidth * 4), boardPos.Y), Color.Black, 2.0f);
            spriteBatch.DrawLine(new Vector2(boardPos.X, boardPos.Y + (Assets.TileSpriteHeight * 4)), new Vector2(boardPos.X + (Assets.TileSpriteWidth * 4), boardPos.Y + (Assets.TileSpriteHeight * 4)), Color.Black, 2.0f);
            spriteBatch.DrawLine(new Vector2(boardPos.X + (Assets.TileSpriteWidth * 4), boardPos.Y), new Vector2(boardPos.X + (Assets.TileSpriteWidth * 4), boardPos.Y + (Assets.TileSpriteHeight * 4)), Color.Black, 2.0f);
            drawSomeMoreStuff(spriteBatch); //all the extra draw commands (such as OverlayDraw, GameOverDraw, etc.) are called through here.
            spriteBatch.End();

            base.Draw(gameTime);
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
        }
    }

    public static class RectangleExtender {
        public static Vector2 Location(this Rectangle _rect) {
            return new Vector2(_rect.X, _rect.Y);
        }
    }
}
