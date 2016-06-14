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

            Console.WriteLine("GameBoard instance created");
        }

        [Test]
        public void TestGameBoardNew2048Game() {
            Console.WriteLine("This should initialize all the data needed for a new 2048 game.");

            Assert.Fail("Test not implemented yet.");
        }

        [Test]
        public void TestGameBoardReplayGame() {
            Console.WriteLine("Calling NewGame a second time should wipe the last game's state out entirely and present a new game.");

            Assert.Fail("Test not implemented yet.");
        }

        [Test]
        public void TestGameBoardNew3072Game() {
            Console.WriteLine("This should initialize all the data needed for a new 3072 game.");

            Assert.Fail("Test not implemented yet.");
        }

        [Test]
        public void TestGameBoardNewInvalidGame() {
            Console.WriteLine("Undefined behaviour shouldn't occur if invalid data is passed for the GameBoardMode struct instance.");

            Assert.Fail("Test not implemented yet.");
        }
    }
}
