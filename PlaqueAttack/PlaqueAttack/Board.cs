using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaqueAttack
{
    class Board
    {

        Block[,] blockArray;

        public Board(int rows, int cols) 
        {
            blockArray = new Block[rows, cols];
        }

        /// <summary>
        /// Attempts to insert block in x, y. Returns true if insertion was successful.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool addBlock(Block block, int r, int c)
        {
            if (blockArray[r, c] == null)
            {
                blockArray[r, c] = block;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the value of r, c to null, even if already null.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public void clearTile(int r, int c)
        {
            blockArray[r, c] = null;
        }

        /// <summary>
        /// Attempts to move the value in startR, startC to endR, endC.
        /// Returns false if startR, startC is null or if endR, endC is not null
        /// </summary>
        /// <param name="startR"></param>
        /// <param name="startC"></param>
        /// <param name="endR"></param>
        /// <param name="endC"></param>
        public bool moveBlock(int startR, int startC, int endR, int endC)
        {
            if (blockArray[startR, startC] != null && blockArray[endR, endC] == null)
            {
                addBlock(blockArray[startR, startC], endR, endC);
                clearTile(startR, startC);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see is if adjacent tiles are occupied by blocks (u, d, l, r).
        /// Tile r, c can be empty.
        /// Returns false if there are no adjacent blocks.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool hasAdjacent(int r, int c)
        {
            if (r > 0 && blockArray[r - 1, c] != null) return true; // Up
            if (c > 0 && blockArray[r, c - 1] != null) return true; // Left
            if (r < blockArray.GetLength(0) - 1 && blockArray[r + 1, c] != null) return true; // Down
            if (c < blockArray.GetLength(1) - 1 && blockArray[r, c + 1] != null) return true; // Right
            return false;
        }
    }
}
