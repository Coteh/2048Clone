using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _2048Clone {
    public class GameSaver {
        private static GameSaver instance;

        StreamReader reader;
        StreamWriter writer;

        public static GameSaver Instance {
            get {
                if (instance == null) {
                    instance = new GameSaver();
                }
                return instance;
            }
        }

        public int[] ReadHighscores(int _amountOfGameModes) {
            int[] highScoresToReturn = new int[_amountOfGameModes];
            try {
                int index = 0;
                using (reader = new StreamReader("highscores.txt")) {
                    while (!reader.EndOfStream && index < _amountOfGameModes) {
                        highScoresToReturn[index] = ConvertToInt(reader.ReadLine());
                        index++;
                    }
                }
            } catch {
                
            }
            return highScoresToReturn;
        }

        public void SaveHighScores(int[] _scores) {
            try {
                using (writer = new StreamWriter("highscores.txt")) {
                    for (int i = 0; i < _scores.Length; i++) {
                        writer.Write(_scores[i]);
                        if (i < _scores.Length - 1) {
                            writer.Write("\n");
                        }
                    }
                }
            } catch {

            }
        }

        int ConvertToInt(string _value) {
            int value;
            if (!int.TryParse(_value, out value)) { value = 0; }
            return value;
        }
    }
}
