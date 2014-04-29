using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048Clone {
    public class Assets {
        public const int AMOUNT_OF_TILE_SPRITES = 8;

        public static Texture2D[] TileImagesArr = new Texture2D[AMOUNT_OF_TILE_SPRITES];
        public static int TileSpriteWidth, TileSpriteHeight;

        public static SpriteFont daFont;
    }
}
