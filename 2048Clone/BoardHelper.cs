using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048Clone {
    //public class BoardHelper {

    //}

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
