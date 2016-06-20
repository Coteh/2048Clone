using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using _2048Clone;

namespace GameClasses {
    [TestFixture]
    public class GameBoardTest {
        GameBoard gameBoard;

        [OneTimeSetUp]
        public void Before() {
            gameBoard = new GameBoard(Vector2.Zero);

            Console.WriteLine("Creates an instance of GameBoard that all tests in this section will use.");
        }

        [Test]
        public void TestGameBoardNew2048Game() {
            Console.WriteLine("This should initialize all the data needed for a new 2048 game.");

            GameBoardConfig gameBoardConfig;
            gameBoardConfig.gridWidth = gameBoardConfig.gridHeight = 4;
            gameBoardConfig.tileWidth = gameBoardConfig.tileHeight = 128;
            gameBoardConfig.gameMode = GameModeState.Twos;

            GameBoardMode gameBoardMode;
            gameBoardMode.name = "4x4";
            gameBoardMode.boardConfig = gameBoardConfig;
            gameBoardMode.highScore = 0;

            gameBoard.NewGame(gameBoardMode);

            Assert.AreEqual(0, gameBoard.Score);
            Assert.AreEqual(0, gameBoard.HighScore);
            Assert.AreEqual(false, gameBoard.IsGameOver);
            Assert.AreEqual(false, gameBoard.Reached2048);
            Assert.AreEqual(false, gameBoard.Reached3072);
            Assert.AreEqual(4, gameBoard.GetBoardWidth);
            Assert.AreEqual(4, gameBoard.GetBoardHeight);
        }

        [Test]
        public void TestGameBoardReplayGame() {
            Console.WriteLine("Calling NewGame a second time should wipe the last game's state out entirely and present a new game.");

            Assert.Fail("Test not implemented yet.");
        }

        [Test]
        public void TestGameBoardNew3072Game() {
            Console.WriteLine("This should initialize all the data needed for a new 3072 game.");

            GameBoardConfig gameBoardConfig;
            gameBoardConfig.gridWidth = gameBoardConfig.gridHeight = 4;
            gameBoardConfig.tileWidth = gameBoardConfig.tileHeight = 128;
            gameBoardConfig.gameMode = GameModeState.Threes;

            GameBoardMode gameBoardMode;
            gameBoardMode.name = "3s Game";
            gameBoardMode.boardConfig = gameBoardConfig;
            gameBoardMode.highScore = 0;

            gameBoard.NewGame(gameBoardMode);

            Assert.AreEqual(0, gameBoard.Score);
            Assert.AreEqual(0, gameBoard.HighScore);
            Assert.AreEqual(false, gameBoard.IsGameOver);
            Assert.AreEqual(false, gameBoard.Reached2048);
            Assert.AreEqual(false, gameBoard.Reached3072);
            Assert.AreEqual(4, gameBoard.GetBoardWidth);
            Assert.AreEqual(4, gameBoard.GetBoardHeight);
        }

        [Test]
        public void TestGameBoardNewDuo2sAnd3sGame() {
            Console.WriteLine("This should initialize all the data needed for a new duo 2s and 3s game.");

            GameBoardConfig gameBoardConfig;
            gameBoardConfig.gridWidth = gameBoardConfig.gridHeight = 6;
            gameBoardConfig.tileWidth = gameBoardConfig.tileHeight = 80;
            gameBoardConfig.gameMode = GameModeState.Twos | GameModeState.Threes;

            GameBoardMode gameBoardMode;
            gameBoardMode.name = "2s and 3s Game";
            gameBoardMode.boardConfig = gameBoardConfig;
            gameBoardMode.highScore = 0;

            gameBoard.NewGame(gameBoardMode);

            Assert.AreEqual(0, gameBoard.Score);
            Assert.AreEqual(0, gameBoard.HighScore);
            Assert.AreEqual(false, gameBoard.IsGameOver);
            Assert.AreEqual(false, gameBoard.Reached2048);
            Assert.AreEqual(false, gameBoard.Reached3072);
            Assert.AreEqual(6, gameBoard.GetBoardWidth);
            Assert.AreEqual(6, gameBoard.GetBoardHeight);
        }

        [Test]
        public void TestGameBoardNewInvalidGame() {
            Console.WriteLine("Undefined behaviour shouldn't occur if invalid data is passed for the GameBoardMode struct instance.");

            Console.WriteLine("Making all the data 0");

            GameBoardConfig gameBoardConfig;
            gameBoardConfig.gridWidth = gameBoardConfig.gridHeight = 0;
            gameBoardConfig.tileWidth = gameBoardConfig.tileHeight = 0;
            gameBoardConfig.gameMode = 0;

            GameBoardMode gameBoardMode;
            gameBoardMode.name = null;
            gameBoardMode.boardConfig = gameBoardConfig;
            gameBoardMode.highScore = 0;

            gameBoard.NewGame(gameBoardMode);

            Assert.AreEqual(0, gameBoard.Score);
            Assert.AreEqual(0, gameBoard.HighScore);
            Assert.AreEqual(false, gameBoard.IsGameOver);
            Assert.AreEqual(false, gameBoard.Reached2048);
            Assert.AreEqual(false, gameBoard.Reached3072);
            Assert.AreEqual(0, gameBoard.GetBoardWidth);
            Assert.AreEqual(0, gameBoard.GetBoardHeight);
        }
    }
}
