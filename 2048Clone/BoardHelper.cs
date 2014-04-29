using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048Clone {
    public class BoardHelper {

        //Array representing which numbers get tile colors
        public static int[] tileTypeArr = { 2, 4, 8, 16, 32, 64, 128, 256 };

        public static int ConvertValueToIndex(int _value){
            for (int i = 0; i < tileTypeArr.Length; i++) {
                if (_value == tileTypeArr[i]) {
                    return i;
                }
            }
            return 0;
        }

        public static int ConvertIndexToValue(int _index) {
            return tileTypeArr[_index];
        }
    }
}
