using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using _2048Clone;
using System.IO;

namespace Tests {
    [TestFixture]
    public class GameSaverTest {
        GameSaver gameSaver;
        int size = 4;
        int[] expectedScores = { 666, 9001, 1337, 8008 };

        [OneTimeSetUp]
        public void Before() {
            gameSaver = GameSaver.Instance;

            Console.WriteLine("Creates a singleton instance of GameSaver that all tests in this section will use.");
        }

        [Test]
        public void TestHighscoresRead() {
            Console.WriteLine("Should read highscores properly.");

            int[] highscores = gameSaver.ReadHighscores(size);
            
            Assert.AreEqual(size, highscores.Length);
            for (int i = 0; i < size; i++) {
                Assert.AreEqual(expectedScores[i], highscores[i]);
            }
        }

        [Test]
        public void TestHighscoresSave() {
            Console.WriteLine("Should save highscores properly.");

            int[] loadedScores;

            string tempPath = Path.Combine(Path.GetTempPath(), "2048Clone_Tests");
            string tempFileName = Path.Combine(tempPath, "highscores.txt");

            //Create the temp folder "2048Clone_Tests"
            Directory.CreateDirectory(tempPath);

            //Assign the new temporary location to GameSaver
            gameSaver.HighScoreFileName = tempFileName;

            //Write new highscores to temporary location
            Console.WriteLine("Writing fake scores to temporary location: \"" + tempFileName + "\".");
            gameSaver.SaveHighScores(expectedScores);

            //Load new highscores from temporary location
            Console.WriteLine("Reading from this temporary location now to confirm that the save worked.");
            loadedScores = gameSaver.ReadHighscores(4);

            //Check if it's right
            Assert.AreEqual(size, loadedScores.Length);
            for (int i = 0; i < size; i++) {
                Assert.AreEqual(expectedScores[i], loadedScores[i]);
            }

            //Set the highscore filename back to normal (for other tests)
            gameSaver.HighScoreFileName = GameSaver.DEFAULT_HIGHSCORE_FILENAME;
        }
    }
}
