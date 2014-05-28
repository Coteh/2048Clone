using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048Clone {
    public class BoardHelper {
        private static Random randy = new Random();

        /// <summary>
        /// Generates a new block depending on the game mode
        /// </summary>
        /// <param name="_gameMode">The game mode or combination of game modes</param>
        /// <param name="_blockType">The type of block to spawn. '0' spawns: 2 in Twos mode, 3 in Threes mode and '1' spawns: 4 in Twos mode, 6 in Threes mode</param>
        /// <returns></returns>
        public static int GenerateNewBlock(GameModeState _gameMode, int _blockType) {
            //Depending on the game mode, we will generate the appropriate block
            int splitChance = -1; //if on both Twos and Threes mode at the same time, we will use this to generate a block from either set
            if (_gameMode == (GameModeState.Threes | GameModeState.Twos)) {
                splitChance = randy.Next(0,10); //0-4 will be a block from Twos and 5-10 will be a block from Threes
            }
            if (_gameMode == GameModeState.Twos || (splitChance >= 0 && splitChance < 5)) {
                if (_blockType < 1) {
                    return 2;
                } else {
                    return 4;
                }
            } else if (_gameMode == GameModeState.Threes || (splitChance >= 5 && splitChance <= 10)) {
                if (_blockType < 1) {
                    return 3;
                } else {
                    return 6;
                }
            }
            return 2; //this shouldn't happen though.
        }
    }

    public delegate Color TileColorMethod(int _tileValue);

    public struct TileColorSet {
        public string name;
        public TileColorMethod colorAction;
    }

    public class TileColorHolder {
        private static TileColorHolder instance;
        private List<Tuple<int, Color>> colorList;
        TileColorSetCollection tileColorSets;

        public string CurrTileColorSetName {
            get {
                return tileColorSets.CurrTileColorSet.name;
            }
        }

        public static TileColorHolder Instance {
            get {
                if (instance == null) {
                    instance = new TileColorHolder();
                }
                return instance;
            }
        }

        private TileColorHolder() {
            ClearColorList();
            tileColorSets = new TileColorSetCollection();
        }

        void ClearColorList() {
            colorList = new List<Tuple<int, Color>>();
        }

        public void AddTileColorSet(TileColorSet _set) {
            tileColorSets.Add(_set);
        }

        public void GoToNextTileColorSet() {
            tileColorSets.GoToNext();
            //clear the current color cache so we can allow the new colors in
            ClearColorList();
        }

        public Color GetColor(int _tileValue) {
            for (int i = 0; i < colorList.Count; i++) {
                if (colorList[i].Item1 == _tileValue) { //if tile value already exists in the list
                    return colorList[i].Item2; //return its color
                }
            }
            //If no match has been found, create new color and save it to the list
            Color newColor = tileColorSets.CurrTileColorSet.colorAction(_tileValue);
            colorList.Add(new Tuple<int, Color>(_tileValue, newColor));
            return newColor; //return the new color afterwards
        }
    }

    public class TileColorSetCollection{
        private List<TileColorSet> tileColorSetList;
        int currTileColorSetIndex;

        public TileColorSet CurrTileColorSet {
            get {
                return tileColorSetList[currTileColorSetIndex];
            }
        }

        public TileColorSetCollection() {
            tileColorSetList = new List<TileColorSet>();
        }

        public void Add(TileColorSet _set) {
            tileColorSetList.Add(_set);
        }

        public void GoToNext() {
            currTileColorSetIndex++;
            if (currTileColorSetIndex > tileColorSetList.Count - 1) {
                currTileColorSetIndex = 0;
            }
        }
    }

    public class TileColorHelper {
        public static Color CreateClassicColors(int _tileValue) {
            return new Color(200 - _tileValue, 150 - _tileValue, 100 + _tileValue);
        }

        public static Color CreateModernColors(int _tileValue) {
            return new Color(100 + _tileValue, 100 + _tileValue, 215 - _tileValue);
        }

        public static Color CreateColorfulColors(int _tileValue) {
            Color colorToReturn = Color.White;
            switch (_tileValue) {
                case 3:
                case 2:
                    colorToReturn = Color.Green;
                    break;
                case 6:
                case 4:
                    colorToReturn = Color.Turquoise;
                    break;
                case 12:
                case 8:
                    colorToReturn = Color.LightBlue;
                    break;
                case 24:
                case 16:
                    colorToReturn = Color.AliceBlue;
                    break;
                case 48:
                case 32:
                    colorToReturn = Color.Blue;
                    break;
                case 96:
                case 64:
                    colorToReturn = Color.Purple;
                    break;
                case 192:
                case 128:
                    colorToReturn = Color.Pink;
                    break;
                case 384:
                case 256:
                    colorToReturn = Color.Yellow;
                    break;
                case 768:
                case 512:
                    colorToReturn = Color.Orange;
                    break;
                case 1536:
                case 1024:
                    colorToReturn = Color.OrangeRed;
                    break;
                case 3072:
                case 2048:
                    colorToReturn = Color.Red;
                    break;
                default:
                    colorToReturn = new Color(255 - _tileValue, 0, 0);
                    break;
            }
            return colorToReturn;
        }
    }
}
