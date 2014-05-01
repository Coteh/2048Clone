using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048Clone {
    public class GameBoard {
        int[,] board;
        Vector2 position;
        float scale;

        int score;
        bool isGameOver, isMoved, reached2048;

        public readonly Vector2 UP = new Vector2(0, -1);
        public readonly Vector2 DOWN = new Vector2(0, 1);
        public readonly Vector2 RIGHT = new Vector2(1, 0);
        public readonly Vector2 LEFT = new Vector2(-1, 0);

        Random randy = new Random();

        public int Score {
            get { return score; }
        }

        public bool IsGameOver {
            get { return isGameOver; }
        }

        public bool Reached2048 {
            get { return reached2048; }
        }

        public GameBoard(Vector2 _pos) {
            //GameBoard Initalization
            board = new int[4,4];
            position = _pos;
            scale = 1.0f;
            score = 0;
            isGameOver = false;
            isMoved = false;
            //SPAWNING FIRST BLOCK
            SpawnNewBlock();
        }

        public void BeginMove(Vector2 _direction) {
            isMoved = false;
            if (!isGameOver) {
                if (CanMove(_direction)) {
                    try {
                        if (_direction == UP) {
                            MoveUp();
                        } else if (_direction == DOWN) {
                            MoveDown();
                        } else if (_direction == LEFT) {
                            MoveLeft();
                        } else if (_direction == RIGHT) {
                            MoveRight();
                        }
                    } catch (Exception) {

                    }
                } else {
                    if (IsFilled()) {
                        Console.WriteLine("Game over!");
                        isGameOver = true;
                    }
                    return;
                }
                if (isMoved) SpawnNewBlock();
            }
        }

        void MoveRight() {
            for (int j = board.GetLength(1) - 1; j >= 0; j--){ //start from the bottom
                MoveRow(j, RIGHT);
                bool isMerge = false;
                for (int i = board.GetLength(0) - 2; i >= 0; i--) { //start from the very right that isn't touching a wall
                    if (CanMerge(i, j, RIGHT)) {
                        Merge(ref board[i + 1, j], ref board[i, j]);
                        isMerge = true;
                    }
                }
                if (isMerge) {
                    MoveRow(j, RIGHT);
                }
            }
        }

        void MoveLeft() {
            for (int j = board.GetLength(1) - 1; j >= 0; j--){ //start from the bottom
                MoveRow(j, LEFT);
                bool isMerge = false;
                for (int i = 1; i <= board.GetLength(0) - 1; i++) { //start from the very left that isn't touching a wall
                    if (CanMerge(i, j, LEFT)) {
                        Merge(ref board[i - 1, j], ref board[i, j]);
                        isMerge = true;
                    }
                }
                if (isMerge) {
                    MoveRow(j, LEFT);
                }
            }
        }

        void MoveUp() {
            for (int i = board.GetLength(0) - 1; i >= 0; i--) { //start from the very right
                MoveCol(i, UP);
                bool isMerge = false;
                for (int j = 1; j <= board.GetLength(1) - 1; j++) { //start from the top that isn't touching wall
                    if (CanMerge(i, j, UP)) {
                        Merge(ref board[i, j - 1], ref board[i, j]);
                        isMerge = true;
                    }
                }
                if (isMerge) {
                    MoveCol(i, UP);
                }
            }
        }

        void MoveDown() {
            for (int i = board.GetLength(0) - 1; i >= 0; i--) { //start from the very right
                MoveCol(i, DOWN);
                bool isMerge = false;
                for (int j = board.GetLength(1) - 2; j >= 0; j--) { //start from the bottom that isn't touching wall
                    if (CanMerge(i, j, DOWN)) {
                        Merge(ref board[i, j + 1], ref board[i, j]);
                        isMerge = true;
                    }
                }
                if (isMerge) {
                    MoveCol(i, DOWN);
                }
            }
        }

        void MoveRow(int _rowIndex, Vector2 _direction) {
            int rowStart = 1, rowEnd = board.GetLength(0) - 1, increment = 1;
            if (_direction == RIGHT) {
                rowStart = board.GetLength(0) - 2; rowEnd = 0; increment = -1;
            }
            for (int i = rowStart; (i >= rowEnd && increment < 0) || (i <= rowEnd && increment > 0); i += increment) { //start from the very right that isn't touching a wall
                if (!IsEmptySpace(board[i, _rowIndex])) {
                    int dist = 0;
                    for (int d = 0; (i + (d * _direction.X) + (_direction.X) <= board.GetLength(0) - 1 
                        && i + (d * _direction.X) + (_direction.X) >= 0) 
                        && d < board.GetLength(0) - 1 
                        && IsEmptySpace(board[i + (d * (int)_direction.X) + ((int)_direction.X), _rowIndex]); d++) {
                        dist++;
                    }
                    if (dist > 0) {
                        board[i + (dist * (int)_direction.X), _rowIndex] = board[i, _rowIndex];
                        board[i, _rowIndex] = 0;
                        isMoved = true;
                    }
                }
            }
        }

        void MoveCol(int _colIndex, Vector2 _direction) {
            int colStart = 1, colEnd = board.GetLength(1) - 1, increment = 1;
            if (_direction == DOWN) {
                colStart = board.GetLength(1) - 2; colEnd = 0; increment = -1;
            }
            for (int i = colStart; (i >= colEnd && increment < 0) || (i <= colEnd && increment > 0); i += increment) { //start from the very right that isn't touching a wall
                if (!IsEmptySpace(board[_colIndex, i])) {
                    int dist = 0;
                    for (int d = 0; (i + (d * _direction.Y) + (_direction.Y) <= board.GetLength(0) - 1
                        && i + (d * _direction.Y) + (_direction.Y) >= 0)
                        && d < board.GetLength(0) - 1
                        && IsEmptySpace(board[_colIndex, i + (d * (int)_direction.Y) + ((int)_direction.Y)]); d++) {
                        dist++;
                    }
                    if (dist > 0) {
                        board[_colIndex, i + (dist * (int)_direction.Y)] = board[_colIndex, i];
                        board[_colIndex, i] = 0;
                        isMoved = true;
                    }
                }
            }
        }

        /// <summary>
        /// Tests if a move is possible.
        /// </summary>
        /// <returns>Returns whether it can or not.</returns>
        bool CanMove(Vector2 _direction) {
            //First check to see if board is filled
            if (!IsFilled()) { return true; }
            //Then check to see if we can merge any blocks
            for (int i = 0; i < board.GetLength(0); i++) {
                for (int j = 0; j < board.GetLength(1); j++) {
                    if (CanMerge(i, j, UP) || CanMerge(i, j, DOWN) || CanMerge(i, j, LEFT) || CanMerge(i, j, RIGHT)) {
                        return true;
                    }
                }
            }
            return false; //if there's no other possibilities left, return false
        }

        /// <summary>
        /// Checks if entire board is filled.
        /// </summary>
        /// <returns>Returns true for filled, and false for not filled.</returns>
        bool IsFilled() {
            return (getEmptyCellCount() == 0);
        }

        /// <summary>
        /// Checks for empty space.
        /// </summary>
        /// <param name="_value"></param>
        /// <returns>True for empty space in specified value.</returns>
        bool IsEmptySpace(int _value) {
            return (_value <= 0);
        }

        /// <summary>
        /// Gets the amount of empty cells in the board.
        /// </summary>
        /// <returns>Int representing the amount of empty cells there are.</returns>
        int getEmptyCellCount() {
            int amountOfEmptyCells = 0;
            foreach (int val in board) {
                if (val == 0) { amountOfEmptyCells++; }
            }
            return amountOfEmptyCells;
        }

        /// <summary>
        /// Gets the next available empty spot on the board.
        /// </summary>
        /// <param name="_direction">The direction the board should be searched from.</param>
        /// <returns>Vector2 for X and Y of next empty index.</returns>
        Vector2 getNextEmptyIndex(int _direction) {
            //_direction value of -1 or less has the loop below going from bottom right of board to top left
            //and _direction value of 0 or greater is default, which is top left to bottom right
            int iStart = 0, iEnd = board.GetLength(0), jStart = 0, jEnd = board.GetLength(1), increment = 1;
            if (_direction < 0) {
                iStart = board.GetLength(0) - 1; jStart = board.GetLength(1) - 1; iEnd = jEnd = 0; increment = -1;
            }
            if (!IsFilled()) {
                for (int i = iStart; (i < iEnd && increment > 0) || (i > iEnd && increment < 0); i+=increment) {
                    for (int j = jStart; (j < jEnd && increment > 0) || (j > jEnd && increment < 0); j+=increment) {
                        if (IsEmptySpace(board[i,j])){
                            return new Vector2(i,j);
                        }
                    }
                }
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the next available empty block from the top left to the bottom right of the board.
        /// </summary>
        /// <returns>Vector2 for X and Y of next empty index.</returns>
        Vector2 getNextEmptyIndex() {
            return getNextEmptyIndex(0);
        }

        /// <summary>
        /// Can the block merge with the one next to it?
        /// </summary>
        /// <param name="_x">X index of block</param>
        /// <param name="_y">Y index of block</param>
        /// <param name="_direction">The block that's next to this block in which direction?</param>
        /// <returns>Returns true if it can merge.</returns>
        bool CanMerge(int _x, int _y, Vector2 _direction) {
            //If empty tile, don't merge
            if (IsEmptySpace(board[_x, _y])) return false;
            //Checking to see if next tile over (in specified direction) is the same
            try {
                if ((_x + _direction.X < 0 || _x + _direction.X > board.GetLength(0) - 1)
                    || (_y + _direction.Y < 0 || _y + _direction.Y > board.GetLength(1) - 1)) {
                    return false;
                }
                if (_direction == UP) {
                    return CanMergeUp(_x, _y);
                } else if (_direction == DOWN) {
                    return CanMergeDown(_x, _y);
                } else if (_direction == LEFT) {
                    return CanMergeLeft(_x, _y);
                } else if (_direction == RIGHT) {
                    return CanMergeRight(_x, _y);
                }
            } catch (Exception) {
                return false;
            }
            return false;
        }

        int getUpBlock(int _x, int _y) {
            return board[_x, _y - 1];
        }
        int getDownBlock(int _x, int _y) {
            return board[_x, _y + 1];
        }
        int getLeftBlock(int _x, int _y) {
            return board[_x - 1, _y];
        }
        int getRightBlock(int _x, int _y) {
            return board[_x + 1, _y];
        }

        bool CanMergeRight(int _x, int _y) {
            return (board[_x, _y] == getRightBlock(_x, _y));
        }

        bool CanMergeLeft(int _x, int _y) {
            return (board[_x, _y] == getLeftBlock(_x, _y));
        }

        bool CanMergeUp(int _x, int _y) {
            return (board[_x, _y] == getUpBlock(_x, _y));
        }

        bool CanMergeDown(int _x, int _y) {
            return (board[_x, _y] == getDownBlock(_x, _y));
        }

        /// <summary>
        /// _block1 is mixed with _block2, and _block2 is then set to 0
        /// Note that you will have to run a check seperately to see if they are the same block before running this
        /// </summary>
        /// <param name="_block1">The block to be mixed</param>
        /// <param name="_block2">The mixee</param>
        void Merge(ref int _block1, ref int _block2) {
            _block1 += _block2;
            _block2 = 0;
            score += _block1;
            if (_block1 == 2048) { //if this block is 2048
                reached2048 = true; //we've reached 2048!
            }
            isMoved = true; //if we only merged a block and not moved anything, this will let the random block generator spawn a block
        }

        void SpawnNewBlock() {
            //Determining random tile placement
            int randomX = 0, randomY = 0, attemptsToRandomize = 0;
            do {
                //Just some basic randomness right now, no AI or anything
                randomX = randy.Next(0, 3);
                randomY = randy.Next(0, 3);
                attemptsToRandomize++;
                //If more than three attempts done to randomize, just get the next available empty space
                if (attemptsToRandomize > 3) {
                    Vector2 nextEmptyIndex = getNextEmptyIndex(randy.Next(-1, 1));
                    randomX = (int)nextEmptyIndex.X;
                    randomY = (int)nextEmptyIndex.Y;
                }
            } while (!IsEmptySpace(board[randomX, randomY]) && !IsFilled());
            //Check if board tile is empty before going on
            if (!IsEmptySpace(board[randomX,randomY])){
                return;
            }
            //Determining random tile type
            int randomTileTypeChance = randy.Next(0,10); //0-4 will be a 2 block and 5-10 will be a 4 block
            if (randomTileTypeChance < 5){
                board[randomX, randomY] = BoardHelper.tileTypeArr[0];
            } else {
                board[randomX, randomY] = BoardHelper.tileTypeArr[1];
            }
            return;
        }

        public void Draw(SpriteBatch _spriteBatch) {
            for (int i = 0; i < board.GetLength(0); i++) {
                for (int j = 0; j < board.GetLength(1); j++) {
                    if (board[i, j] != 0) {
                        _spriteBatch.Draw(Assets.TileImagesArr[BoardHelper.ConvertValueToIndex(board[i,j])], new Vector2((i * (Assets.TileSpriteWidth * scale)) + position.X, (j * (Assets.TileSpriteHeight * scale)) + position.Y), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                        _spriteBatch.DrawString(Assets.daFont, "" + board[i, j], new Vector2((i * (Assets.TileSpriteWidth * scale)) + ((Assets.TileSpriteWidth * scale) / 3) + position.X, (j * (Assets.TileSpriteHeight * scale)) + ((Assets.TileSpriteHeight * scale) / 3) + position.Y), Color.Black);
                    }
                }
            }
        }
    }
}
