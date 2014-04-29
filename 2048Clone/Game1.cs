#region Using Statements
using System;
using System.Collections.Generic;
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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputHelper inputHelper;

        GameBoard gameBoard;

        delegate void AdditionalDraws(SpriteBatch _spriteBatch);
        AdditionalDraws drawSomeMoreStuff;

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
            gameBoard = new GameBoard();
            updateMethod = GameUpdate;
            drawSomeMoreStuff = OverlayDraw;

            base.Initialize();
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
            if (gameBoard.IsGameOver) {
                if (drawSomeMoreStuff != GameOverDraw) drawSomeMoreStuff = GameOverDraw;
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
                drawSomeMoreStuff = OverlayDraw;
                gameBoard = new GameBoard();
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
            drawSomeMoreStuff(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void OverlayDraw(SpriteBatch _spriteBatch) {

        }

        void GameOverDraw(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawString(Assets.daFont, "Game Over! Press R to try again", Vector2.Zero, Color.Black);
        }
    }
}
