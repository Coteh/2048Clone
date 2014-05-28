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
        GameSaver gameSaver;
        ScreenState screenState;
        ScreenState SetScreenState {
            set {
                screenState = value;
                switch (screenState) {
                    case ScreenState.TitleScreen:
                        updateMethod = TitleScreenUpdate;
                        drawCalls = TitleScreenDraw;
                        UpdateEvent += UpdateTitleMenuInput;
                        menuDraw = titleMenu.DrawMenu;
                        break;
                    case ScreenState.InGame:
                        NewGame();
                        updateMethod = GameUpdate;
                        drawCalls = GameDraw;
                        DrawCallEvent += OverlayDraw;
                        menuDraw = null;
                        break;
                    default:
                        break;
                }
            }
        }

        Menu titleMenu, pauseMenu, settingsMenu;
        TileColorHolder colorHolder;

        GameBoard gameBoard;
        Vector2 boardPos;

        /*Game settings
         THESE ARE SET WHEN STARTING A NEW GAME*/
        const int AMOUNT_OF_GAME_MODES = 4;
        GameBoardMode[] gameModeArr;
        int selectedModeIndex;
        int tileHeight, tileWidth;

        /*GUI Elements*/
        Rectangle topHUDRect;
        Rectangle popupRect;

        /*Condition checking*/
        bool checkForGameOver, checkFor2048, checkFor3072;

        /*Checkerboard background*/
        const int CHECKERBOARD_TILE_SIZE = 32;

        /*Misc.*/
        Texture2D gameLogo;

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
        event UpdateMode UpdateEvent {
            add {
                if (updateMethod == null || !updateMethod.GetInvocationList().Contains(value)) {
                    updateMethod += value;
                }
            }
            remove {
                updateMethod -= value;
            }
        }

        delegate void MenuDraw(SpriteBatch _spriteBatch, SpriteFont _font);
        MenuDraw menuDraw;
        event MenuDraw MenuDrawEvent {
            add {
                if (menuDraw == null || !menuDraw.GetInvocationList().Contains(value)) {
                    menuDraw += value;
                }
            }
            remove {
                menuDraw -= value;
            }
        }

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
            gameSaver = GameSaver.Instance;
            Assets.pixel = new Texture2D(GraphicsDevice, 1, 1);
            Assets.pixel.SetData(new[] { Color.White });
            topHUDRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 16);
            popupRect = new Rectangle(0, GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 8);
            boardPos = new Vector2(GraphicsDevice.Viewport.Width / 16, GraphicsDevice.Viewport.Height / 8);
            gameBoard = new GameBoard(boardPos);
            //Loading game modes
            gameModeArr = new GameBoardMode[AMOUNT_OF_GAME_MODES];
            gameModeArr[0].name = "4x4";
            gameModeArr[0].boardConfig.gridWidth = gameModeArr[0].boardConfig.gridHeight = 4;
            gameModeArr[0].boardConfig.tileWidth = gameModeArr[0].boardConfig.tileHeight = 128;
            gameModeArr[0].boardConfig.gameMode = GameModeState.Twos;
            gameModeArr[1].name = "8x8";
            gameModeArr[1].boardConfig.gridWidth = gameModeArr[1].boardConfig.gridHeight = 8;
            gameModeArr[1].boardConfig.tileWidth = gameModeArr[1].boardConfig.tileHeight = 64;
            gameModeArr[1].boardConfig.gameMode = GameModeState.Twos;
            gameModeArr[2].name = "3s (3072) - 4x4";
            gameModeArr[2].boardConfig.gridWidth = gameModeArr[2].boardConfig.gridHeight = 4;
            gameModeArr[2].boardConfig.tileWidth = gameModeArr[2].boardConfig.tileHeight = 128;
            gameModeArr[2].boardConfig.gameMode = GameModeState.Threes;
            gameModeArr[3].name = "Duo 2s and 3s - 6x6";
            gameModeArr[3].boardConfig.gridWidth = gameModeArr[3].boardConfig.gridHeight = 6;
            gameModeArr[3].boardConfig.tileWidth = gameModeArr[3].boardConfig.tileHeight = 80;
            gameModeArr[3].boardConfig.gameMode = GameModeState.Twos | GameModeState.Threes;
            int[] highScoresLoaded = gameSaver.ReadHighscores(AMOUNT_OF_GAME_MODES);
            for (int i = 0; i < AMOUNT_OF_GAME_MODES; i++) {
                gameModeArr[i].highScore = highScoresLoaded[i];
            }
            //Loading title screen menu
            titleMenu = new Menu();
            titleMenu.SetPosition(new Vector2((GraphicsDevice.Viewport.Width / 2) - (GraphicsDevice.Viewport.Width / 8), (GraphicsDevice.Viewport.Height / 2) + (GraphicsDevice.Viewport.Height / 6)));
            MenuButton playGameBtn, playBigGameBtn, playThreesBtn, playDuoButton, settingsBtn, exitGameBtn;
            playGameBtn.name = "Play 4x4 Game";
            playGameBtn.menuAction = StartRegularGame;
            playBigGameBtn.name = "Play 8x8 Game";
            playBigGameBtn.menuAction = StartBigGame;
            playThreesBtn.name = "Play 3s Game (3072)";
            playThreesBtn.menuAction = StartThreesGame;
            playDuoButton.name = "Play Duo 2s and 3s Game";
            playDuoButton.menuAction = StartDuoGame;
            settingsBtn.name = "Settings";
            settingsBtn.menuAction = GoToSettings;
            exitGameBtn.name = "Exit Game";
            exitGameBtn.menuAction = Exit;
            titleMenu.AddMultiple(new MenuButton[] { playGameBtn, playBigGameBtn, playThreesBtn, playDuoButton, settingsBtn, exitGameBtn });
            //Loading pause menu
            pauseMenu = new Menu();
            pauseMenu.SetPosition(new Vector2((GraphicsDevice.Viewport.Width / 2) - (GraphicsDevice.Viewport.Width / 8), GraphicsDevice.Viewport.Height / 2));
            pauseMenu.SetTitle("PAUSED");
            MenuButton resumeBtn, returnBtn;
            resumeBtn.name = "Resume";
            resumeBtn.menuAction = HidePauseMenu;
            returnBtn.name = "Return to Title";
            returnBtn.menuAction = ReturnToTitleScreen;
            pauseMenu.AddMultiple(new MenuButton[] { resumeBtn, returnBtn });
            //Creating tile color sets
            TileColorSet modern, classic, colorful;
            modern.name = "Modern";
            modern.colorAction = TileColorHelper.CreateModernColors;
            classic.name = "Classic";
            classic.colorAction = TileColorHelper.CreateClassicColors;
            colorful.name = "Colorful";
            colorful.colorAction = TileColorHelper.CreateColorfulColors;
            colorHolder = TileColorHolder.Instance;
            colorHolder.AddTileColorSet(modern);
            colorHolder.AddTileColorSet(classic);
            colorHolder.AddTileColorSet(colorful);
            //Loading settings menu
            settingsMenu = new Menu();
            settingsMenu.SetPosition(new Vector2((GraphicsDevice.Viewport.Width / 2) - (GraphicsDevice.Viewport.Width / 8), (GraphicsDevice.Viewport.Height / 2) + (GraphicsDevice.Viewport.Height / 4)));
            settingsMenu.SetTitle("Settings");
            MenuButton tileColorBtn, clearHighscoresBtn, backBtn;
            tileColorBtn.name = "Change Tile Color Theme -%o-";
            tileColorBtn.menuAction = colorHolder.GoToNextTileColorSet;
            clearHighscoresBtn.name = "Clear High Scores";
            clearHighscoresBtn.menuAction = ClearHighscores;
            backBtn.name = "Back";
            backBtn.menuAction = ReturnToTitleScreen;
            settingsMenu.AddMultiple(new MenuButton[] { tileColorBtn, clearHighscoresBtn, backBtn });
            //Starting game at specified screen
            SetScreenState = ScreenState.TitleScreen;

            base.Initialize();
        }

        void NewGame() {
            checkForGameOver = true;
            if (gameModeArr[selectedModeIndex].boardConfig.gameMode == GameModeState.Twos) {
                checkFor2048 = true;
            } else if (gameModeArr[selectedModeIndex].boardConfig.gameMode == GameModeState.Threes) {
                checkFor3072 = true;
            } else if (gameModeArr[selectedModeIndex].boardConfig.gameMode == (GameModeState.Twos | GameModeState.Threes)) {
                checkFor2048 = checkFor3072 = true;
            }
            gameBoard.NewGame(gameModeArr[selectedModeIndex]);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.daFont = Content.Load<SpriteFont>(@"Fonts/ComicSans");

            gameLogo = Content.Load<Texture2D>(@"Images/Logo");
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
            if (updateMethod != null) updateMethod();
            base.Update(gameTime);
        }

        void GameUpdate() {
            UpdateGameInput();

            //listens for game over
            if (checkForGameOver) {
                if (gameBoard.IsGameOver) {
                    DrawCallEvent -= GoalDraw2048; //remove the 2048 goal draw if it's there
                    DrawCallEvent -= GoalDraw3072; //remove the 3072 goal draw if it's there
                    DrawCallEvent += GameOverDraw;
                    if (gameBoard.HighScore > gameModeArr[selectedModeIndex].highScore) { //if user got a new highscore
                        gameModeArr[selectedModeIndex].highScore = gameBoard.HighScore; //save the new highscore to the appropriate struct
                        int[] highScoresToSave = new int[AMOUNT_OF_GAME_MODES]; //gather all highscores using this array
                        for (int i = 0; i < AMOUNT_OF_GAME_MODES; i++) {
                            highScoresToSave[i] = gameModeArr[i].highScore;
                        }
                        gameSaver.SaveHighScores(highScoresToSave); //save all highscores to file
                    }
                    checkForGameOver = false;
                    checkFor2048 = false; //it's game over, so no point checking for 2048 anymore either
                    return;
                }
            }
            //listens for 2048
            if (checkFor2048) {
                if (gameBoard.Reached2048) {
                    DrawCallEvent += GoalDraw2048;
                    checkFor2048 = false;
                }
            }
            //listens for 3072
            if (checkFor3072) {
                if (gameBoard.Reached3072) {
                    DrawCallEvent += GoalDraw3072;
                    checkFor3072 = false;
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
                DrawCallEvent -= GoalDraw2048;
                DrawCallEvent -= GoalDraw3072;
                NewGame();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Space) || inputHelper.CheckForGamepadPress(Buttons.A)) {
                DrawCallEvent -= GoalDraw2048;
                DrawCallEvent -= GoalDraw3072;
            } else if (inputHelper.CheckForKeyboardPress(Keys.Escape) || inputHelper.CheckForGamepadPress(Buttons.Back)) {
                ReturnToTitleScreen();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Enter) || inputHelper.CheckForGamepadPress(Buttons.Start)) {
                ShowPauseMenu();
            }
        }

        void TitleScreenUpdate() {
            
        }

        void UpdateTitleMenuInput() {
            if (titleMenu.Update(inputHelper.GetMousePosition(), inputHelper.CheckForLeftHold(), inputHelper.CheckForLeftRelease())) {
                titleMenu.Select();
            }
            if (inputHelper.CheckForKeyboardPress(Keys.Enter) || inputHelper.CheckForGamepadPress(Buttons.Start) || inputHelper.CheckForGamepadPress(Buttons.A)) {
                titleMenu.Select();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Escape) || inputHelper.CheckForGamepadPress(Buttons.Back)) {
                Exit();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Down) || inputHelper.CheckForGamepadPress(Buttons.DPadDown)) {
                titleMenu.Move(1);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Up) || inputHelper.CheckForGamepadPress(Buttons.DPadUp)) {
                titleMenu.Move(-1);
            }
        }

        void UpdatePauseMenuInput() {
            if (pauseMenu.Update(inputHelper.GetMousePosition(), inputHelper.CheckForLeftHold(), inputHelper.CheckForLeftRelease())) {
                pauseMenu.Select();
            }
            if (inputHelper.CheckForKeyboardPress(Keys.Enter) || inputHelper.CheckForGamepadPress(Buttons.Start) || inputHelper.CheckForGamepadPress(Buttons.A)) {
                pauseMenu.Select();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Down) || inputHelper.CheckForGamepadPress(Buttons.DPadDown)) {
                pauseMenu.Move(1);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Up) || inputHelper.CheckForGamepadPress(Buttons.DPadUp)) {
                pauseMenu.Move(-1);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Escape) || inputHelper.CheckForGamepadPress(Buttons.Back)) {
                ReturnToTitleScreen();
            }
        }

        void UpdateSettingsMenuInput() {
            if (settingsMenu.Update(inputHelper.GetMousePosition(), inputHelper.CheckForLeftHold(), inputHelper.CheckForLeftRelease())) {
                settingsMenu.Select();
                settingsMenu.UpdateValues(new string[] { colorHolder.CurrTileColorSetName });
            }
            if (inputHelper.CheckForKeyboardPress(Keys.Enter) || inputHelper.CheckForGamepadPress(Buttons.Start) || inputHelper.CheckForGamepadPress(Buttons.A)) {
                settingsMenu.Select();
            } else if (inputHelper.CheckForKeyboardPress(Keys.Down) || inputHelper.CheckForGamepadPress(Buttons.DPadDown)) {
                settingsMenu.Move(1);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Up) || inputHelper.CheckForGamepadPress(Buttons.DPadUp)) {
                settingsMenu.Move(-1);
            } else if (inputHelper.CheckForKeyboardPress(Keys.Escape) || inputHelper.CheckForGamepadPress(Buttons.Back)) {
                ReturnToTitleScreen();
            }
        }

        void StartRegularGame() {
            selectedModeIndex = 0;
            AdjustDrawTiles();
            SetScreenState = ScreenState.InGame;
        }

        void StartBigGame() {
            selectedModeIndex = 1;
            AdjustDrawTiles();
            SetScreenState = ScreenState.InGame;
        }

        void StartThreesGame() {
            selectedModeIndex = 2;
            AdjustDrawTiles();
            SetScreenState = ScreenState.InGame;
        }

        void StartDuoGame() {
            selectedModeIndex = 3;
            AdjustDrawTiles();
            SetScreenState = ScreenState.InGame;
        }

        void AdjustDrawTiles() {
            tileWidth = gameModeArr[selectedModeIndex].boardConfig.tileWidth;
            tileHeight = gameModeArr[selectedModeIndex].boardConfig.tileHeight;
        }

        void ShowPauseMenu() {
            UpdateEvent -= GameUpdate;
            UpdateEvent += UpdatePauseMenuInput;
            MenuDrawEvent += pauseMenu.DrawMenu;
        }

        void HidePauseMenu() {
            UpdateEvent -= UpdatePauseMenuInput;
            UpdateEvent += GameUpdate;
            MenuDrawEvent -= pauseMenu.DrawMenu;
        }

        void ReturnToTitleScreen() {
            SetScreenState = ScreenState.TitleScreen;
        }

        void GoToSettings() {
            UpdateEvent -= UpdateTitleMenuInput;
            settingsMenu.UpdateValues(new string[] {colorHolder.CurrTileColorSetName});
            UpdateEvent += UpdateSettingsMenuInput;
            menuDraw = settingsMenu.DrawMenu;
        }

        void ClearHighscores() {
            int[] highScoresToSave = new int[AMOUNT_OF_GAME_MODES]; //int array are all 0s when initalized
            for (int i = 0; i < gameModeArr.Length; i++) {
                gameModeArr[i].highScore = highScoresToSave[i]; //wipe the highscore out
            }
            gameSaver.SaveHighScores(highScoresToSave); //save the cleared highscores to text file
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            bool useDarkGray = false;
            for (int x = 0; x < (GraphicsDevice.Viewport.Width / CHECKERBOARD_TILE_SIZE) + 1; x++) {
                for (int y = 0; y < (GraphicsDevice.Viewport.Height / CHECKERBOARD_TILE_SIZE) + 1; y++) {
                    Color colorToUse = (useDarkGray) ? Color.DarkGray : Color.Gray;
                    spriteBatch.DrawRect(new Rectangle(x * CHECKERBOARD_TILE_SIZE, y * CHECKERBOARD_TILE_SIZE, CHECKERBOARD_TILE_SIZE, CHECKERBOARD_TILE_SIZE), colorToUse);
                    useDarkGray = !useDarkGray;
                }
            }
            drawCalls(spriteBatch); //all the extra draw commands (such as OverlayDraw, GameOverDraw, etc.) are called through here.
            if (menuDraw != null) menuDraw(spriteBatch, Assets.daFont);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void TitleScreenDraw(SpriteBatch _spriteBatch) {
            GraphicsDevice.Clear(Color.SeaGreen);
            _spriteBatch.DrawString(Assets.daFont, "Welcome to 2048Clone!", Vector2.Zero, Color.Black);
            _spriteBatch.Draw(gameLogo, new Vector2(GraphicsDevice.Viewport.Width / 6, 0), Color.White);
            _spriteBatch.DrawString(Assets.daFont, "2014 James Cote", new Vector2(GraphicsDevice.Viewport.Width / 2 - GraphicsDevice.Viewport.Width / 7, GraphicsDevice.Viewport.Height - 28), Color.Black);
        }

        void GameDraw(SpriteBatch _spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            gameBoard.Draw(spriteBatch);
            spriteBatch.DrawLine(boardPos, new Vector2(boardPos.X, boardPos.Y + (tileHeight * gameBoard.GetBoardHeight)), Color.Black, 2.0f);
            spriteBatch.DrawLine(boardPos, new Vector2(boardPos.X + (tileWidth * gameBoard.GetBoardWidth), boardPos.Y), Color.Black, 2.0f);
            spriteBatch.DrawLine(new Vector2(boardPos.X, boardPos.Y + (tileHeight * gameBoard.GetBoardHeight)), new Vector2(boardPos.X + (tileWidth * gameBoard.GetBoardWidth), boardPos.Y + (tileHeight * gameBoard.GetBoardHeight)), Color.Black, 2.0f);
            spriteBatch.DrawLine(new Vector2(boardPos.X + (tileWidth * gameBoard.GetBoardWidth), boardPos.Y), new Vector2(boardPos.X + (tileWidth * gameBoard.GetBoardWidth), boardPos.Y + (tileHeight * gameBoard.GetBoardHeight)), Color.Black, 2.0f);
        }

        void OverlayDraw(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(topHUDRect, Color.Orange);
            _spriteBatch.DrawString(Assets.daFont, "Score: " + gameBoard.Score, Vector2.Zero, Color.Black);
            _spriteBatch.DrawString(Assets.daFont, "Highscore: " + gameBoard.HighScore, new Vector2(GraphicsDevice.Viewport.Width - 200,0), Color.Black);
        }

        void GameOverDraw(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(popupRect, Color.Red, 2.0f);
            _spriteBatch.DrawString(Assets.daFont, "Game Over! Press R to try again", new Vector2(popupRect.X,popupRect.Y), Color.Black);
        }

        void GoalDraw2048(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(popupRect, Color.Lime, 2.0f);
            _spriteBatch.DrawString(Assets.daFont, "Congratulations! You made it to 2048! Press Space to remove this message or R to restart.", new Vector2(popupRect.X, popupRect.Y), Color.Black);
        }

        void GoalDraw3072(SpriteBatch _spriteBatch) {
            _spriteBatch.DrawRect(popupRect, Color.DeepSkyBlue, 2.0f);
            _spriteBatch.DrawString(Assets.daFont, "Congratulations! You made it to 3072! Press Space to remove this message or R to restart.", new Vector2(popupRect.X, popupRect.Y), Color.White);
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
