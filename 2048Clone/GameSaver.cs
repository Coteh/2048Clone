using System;
using System.IO;

namespace _2048Clone {
    public class GameSaver {
        private static GameSaver instance;

        StreamReader reader;
        StreamWriter writer;

        public const string DEFAULT_HIGHSCORE_FILENAME = "highscores.txt";
        string highScoreFilename = DEFAULT_HIGHSCORE_FILENAME;

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
                using (reader = new StreamReader(highScoreFilename)) {
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
                using (writer = new StreamWriter(highScoreFilename)) {
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
