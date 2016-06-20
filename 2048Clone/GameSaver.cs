using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace _2048Clone {
    public class GameSaver {
        private static GameSaver instance;

        StreamReader reader;
        StreamWriter writer;

        public const string DEFAULT_HIGHSCORE_FILENAME = "highscores.txt";
        string highScoreFilename = DEFAULT_HIGHSCORE_FILENAME;

        static string projectDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

        public static GameSaver Instance {
            get {
                if (instance == null) {
                    instance = new GameSaver();
                }
                return instance;
            }
        }

        public string HighScoreFileName {
            get {
                return highScoreFilename;
            } set {
                highScoreFilename = value;
            }
        }

        public int[] ReadHighscores(int _amountOfGameModes) {
            int[] highScoresToReturn = new int[_amountOfGameModes];
            try {
                int index = 0;
                string path = Path.Combine(projectDirectory, highScoreFilename);
                string localPath = new Uri(path).LocalPath;
                using (reader = new StreamReader(localPath)) {
                    while (!reader.EndOfStream && index < _amountOfGameModes) {
                        highScoresToReturn[index] = ConvertToInt(reader.ReadLine());
                        index++;
                    }
                }
            } catch (FileNotFoundException) {
                File.Create(highScoreFilename);
            } catch (Exception e) {
                throw e;
            }
            return highScoresToReturn;
        }

        public void SaveHighScores(int[] _scores) {
            try {
                string path = Path.Combine(projectDirectory, highScoreFilename);
                string localPath = new Uri(path).LocalPath;
                using (writer = new StreamWriter(localPath)) {
                    for (int i = 0; i < _scores.Length; i++) {
                        writer.Write(_scores[i]);
                        if (i < _scores.Length - 1) {
                            writer.Write("\n");
                        }
                    }
                }
            } catch (Exception e) {
                throw e;
            }
        }

        int ConvertToInt(string _value) {
            int value;
            if (!int.TryParse(_value, out value)) { value = 0; }
            return value;
        }
    }
}
