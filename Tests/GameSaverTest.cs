using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using _2048Clone;
using System.IO;
using System.Reflection;

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

            string tempFilePath = Path.Combine(Path.GetTempPath(), "2048Clone_Test_HighscoresRead");
            string tempFileName = Path.Combine(tempFilePath, "highscores.txt");

            //Create the temp folder
            Directory.CreateDirectory(tempFilePath);

            //Grab the embedded highscore textfile resource and write it to a temporary location
            Assembly assem = Assembly.GetExecutingAssembly();
            Stream stream = assem.GetManifestResourceStream("Tests.highscores.txt");
            StreamReader streamReader = new StreamReader(stream);
            StreamWriter streamWriter = new StreamWriter(tempFileName);
            streamWriter.Write(streamReader.ReadToEnd());
            streamWriter.Flush();
            streamWriter.Close();
            streamReader.Close();
            Console.WriteLine("Fake highscores saved to temporary location: \"" + tempFileName + "\".");

            //Set the highscore filename
            gameSaver.HighScoreFileName = tempFileName;

            //Read highscores
            Console.WriteLine("Reading fake highscores from the temporary location.");
            int[] highscores = gameSaver.ReadHighscores(size);

            //Check if it's right
            Assert.AreEqual(size, highscores.Length);
            for (int i = 0; i < size; i++) {
                Assert.AreEqual(expectedScores[i], highscores[i]);
            }
        }

        [Test]
        public void TestHighscoresSave() {
            Console.WriteLine("Should save highscores properly.");

            int[] loadedScores;

            string tempPath = Path.Combine(Path.GetTempPath(), "2048Clone_Test_HighscoresSave");
            string tempFileName = Path.Combine(tempPath, "highscores.txt");

            //Create the temp folder
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
