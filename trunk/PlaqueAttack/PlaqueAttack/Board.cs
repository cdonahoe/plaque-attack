using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlaqueAttack
{
    public class Board
    {

        Block[,] blockArray;
        Random rand;
        int numBlocks;
        public bool gameLost = false;

        public Board(int cols, int rows) 
        {
            blockArray = new Block[cols, rows];
            rand = new Random();
            numBlocks = 0;
        }

        /// <summary>
        /// Attempts to insert block in c, r. Returns true if insertion was successful.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool AddBlock(Block block, int c, int r)
        {
            if (blockArray[c, r] == null)
            {
                blockArray[c, r] = block;
                numBlocks++;
                return true;
            }
            return false;
        }

        public Block[,] GetBoard()
        {
            return blockArray;
        }

        public int GetNumBlocks()
        {
            return numBlocks;
        }

        /// <summary>
        /// Attempts the value of c, r to null.
        /// Returns false if alreaey null
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        public bool ClearTile(int c, int r)
        {
            if (blockArray[c, r] != null)
            {
                blockArray[c, r] = null;
                numBlocks--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to move the value in startC, startR to endC, endR.
        /// Returns false if startC, startR is null or if endC, endR is not null
        /// </summary>
        /// <param name="startC"></param>
        /// <param name="startR"></param>
        /// <param name="endC"></param>
        /// <param name="endR"></param>
        public bool MoveBlock(int startC, int startR, int endC, int endR)
        {
            if (blockArray[startC, startR] != null && blockArray[endC, endR] == null)
            {
                AddBlock(blockArray[startC, startR], endC, endR);
                ClearTile(startC, startR);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see is if adjacent tiles are occupied by blocks (u, d, l, r).
        /// Tile c, r can be empty.
        /// Returns false if there are no adjacent blocks.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool HasAdjacent(int r, int c)
        {
            if (r > 0 && blockArray[c, r - 1] != null) return true; // Up
            if (c > 0 && blockArray[c - 1, r] != null) return true; // Left
            if (r < blockArray.GetLength(1) - 1 && blockArray[c, r + 1] != null) return true; // Down
            if (c < blockArray.GetLength(0) - 1 && blockArray[c + 1, r] != null) return true; // Right
            return false;
        }

        /// <summary>
        /// Checks if the row is full.
        /// </summary>
        /// <returns>True if row is full</returns>
        public bool CheckIfBlockage(int row)
        {
            for (int i = 0; i < blockArray.GetLength(0); i++)
            {
                if (blockArray[i, row] == null) return false;
            }
            return true;
        }

        /// <summary>
        /// Places a block in a random valid location on the board
        /// </summary>
        /// <returns>Position the block was placed in.</returns>
        public Vector2 PlaceBlock(Block block)
        {
            // Start by choosing a row
            // Random right now, should favor middle rows more
            int row = rand.Next(blockArray.GetLength(1));
            // Choose a side at random, then find the nearest open space
            int side = rand.Next(2);
            // Left side
            int col;
            if (side == 0)
            {
                col = 0;
                while (blockArray[col, row] != null)
                {
                    col++;
                }
                blockArray[col, row] = block;
            }
            else
            {
                col = blockArray.GetLength(0) - 1;
                while (blockArray[col, row] != null)
                {
                    col--;
                }
                blockArray[col, row] = block;
            }
            // Now that the block is placed, check if the row is full (game lost)
            if (CheckIfBlockage(row) == true)
            {
                // Game Over!!
                GameLost();
            }
            numBlocks++;
            return new Vector2(col, row);
        }

        public void GameLost()
        {
            gameLost = true;
        }

        /// <summary>
        /// Sets all values in the board to null
        /// </summary>
        public void ClearBoard()
        {
            blockArray = new Block[blockArray.GetLength(0), blockArray.GetLength(1)];
            numBlocks = 0;
        }

    }
}
