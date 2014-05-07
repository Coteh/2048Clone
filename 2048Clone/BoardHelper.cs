using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048Clone {
    public class BoardHelper {
        /// <summary>
        /// Generates a new block depending on the game mode
        /// </summary>
        /// <param name="_gameMode">The game mode or combination of game modes</param>
        /// <param name="_blockType">The type of block to spawn. '0' spawns: 2 in Twos mode, 3 in Threes mode and '1' spawns: 4 in Twos mode, 6 in Threes mode</param>
        /// <returns></returns>
        public static int GenerateNewBlock(GameMode _gameMode, int _blockType) {
            //Depending on the game mode, we will generate the appropriate block
            int splitChance = -1; //if on both Twos and Threes mode at the same time, we will use this to generate a block from either set
            Random randy;
            if (_gameMode == (GameMode.Threes | GameMode.Twos)) {
                randy = new Random(); //create the random object here because this is the only time we're going to use it in this method
                splitChance = randy.Next(0,10); //0-4 will be a block from Twos and 5-10 will be a block from Threes
            }
            if (_gameMode == GameMode.Twos || (splitChance >= 0 && splitChance < 5)) {
                if (_blockType < 1) {
                    return 2;
                } else {
                    return 4;
                }
            } else if (_gameMode == GameMode.Threes || (splitChance >= 5 && splitChance <= 10)) {
                if (_blockType < 1) {
                    return 3;
                } else {
                    return 6;
                }
            }
            return 2; //this shouldn't happen though.
        }
    }

    public class TileColorHolder {
        private List<Tuple<int, Color>> colorList;

        public TileColorHolder() {
            colorList = new List<Tuple<int, Color>>();
        }

        public Color GetColor(int _tileValue) {
            for (int i = 0; i < colorList.Count; i++) {
                if (colorList[i].Item1 == _tileValue) { //if tile value already exists in the list
                    return colorList[i].Item2; //return its color
                }
            }
            //If no match has been found, create new color and save it to the list
            Color newColor = TileColorHelper.CreateNewTileColor(_tileValue);
            colorList.Add(new Tuple<int, Color>(_tileValue, newColor));
            return newColor; //return the new color afterwards
        }
    }

    public class TileColorHelper {
        public static Color CreateNewTileColor(int _tileValue) {
            return new Color(200 - _tileValue, 150 - _tileValue, 100 + _tileValue);
        }
    }
}
