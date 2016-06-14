using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using _2048Clone;

namespace Tests {
    [TestFixture]
    public class GameSaverTest {
        GameSaver gameSaver;

        [OneTimeSetUp]
        public void Before() {
            gameSaver = GameSaver.Instance;

            Console.WriteLine("Creates a singleton instance of GameSaver that all tests in this section will use.");
        }

        [Test]
        public void TestHighscoresRead() {
            Console.WriteLine("Should read highscores properly.");

            Assert.Fail("Test not implemented yet.");
        }

        [Test]
        public void TestHighscoresSave() {
            Console.WriteLine("Should save highscores properly.");

            Assert.Fail("Test not implemented yet.");
        }
    }
}
