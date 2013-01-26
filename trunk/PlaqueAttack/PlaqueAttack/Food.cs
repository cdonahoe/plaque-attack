using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PlaqueAttack.Utilities;

namespace PlaqueAttack
{
    class Food
    {
        FoodTypes type; // This should probably be an en enum
        int numBlocks;
        int numColors;
        float speed;
        Random rand;

        public enum FoodTypes
        {
            Banana,
            Salad,
            Hamburger,
            Pizza,
            IceCream,
        }
        /// <summary>
        /// Creates an instance of Food.
        /// </summary>
        /// <param name="type">Type of food, e.g. banana</param>
        /// <param name="numBlocks">Number of blocks this food will emit</param>
        /// <param name="numColors">Number of different color blocks possible</param>
        /// <param name="speed">How quickly the blocks will spawn</param>
        public Food(FoodTypes type, int numBlocks, int numColors, float speed)
        {
            rand = new Random();
            this.type = type;
            this.numBlocks = numBlocks;
            this.numColors = numColors;
            this.speed = speed;
        }

        /// <summary>
        /// Creates food with hardcoded values based on type
        /// </summary>
        /// <param name="type"></param>
        public Food(FoodTypes type)
        {
            rand = new Random();
            if (type == FoodTypes.Banana)
            {
                numBlocks = 100;
                numColors = 2;
                speed = 40;
            }
            if (type == FoodTypes.Salad)
            {
                numBlocks = 120;
                numColors = 3;
                speed = 30;
            }
            if (type == FoodTypes.Hamburger)
            {
                numBlocks = 150;
                numColors = 4;
                speed = 20;
            }
            if (type == FoodTypes.Pizza)
            {
                numBlocks = 170;
                numColors = 6;
                speed = 20;
            }
            if (type == FoodTypes.IceCream)
            {
                numBlocks = 200;
                numColors = 8;
                speed = 15;
            }
        }

        /// <summary>
        /// Spawns a block to the board.
        /// </summary>
        /// <returns>False when the last block has been spawned</returns>
        public bool spawnToBoard(Board board, List<TransformAnimation> animationUpdater)
        {
            // Choose a random block color based on num available
            int blockColor = rand.Next(numColors);
            Block b = new Block((Block.BlockColor) blockColor, new Vector2(0,-50));
            Vector2 endPos = Game.TransformGridToScreen(board.PlaceBlock(b));
            Vector2 startPos = new Vector2(endPos.X, -40);
            TransformAnimation tran = new TransformAnimation(b, TimeSpan.FromSeconds(0.5), startPos, endPos, TransformAnimation.AnimationCurve.Smooth);
            animationUpdater.Add(tran);

            numBlocks--;
            //Console.WriteLine("Food: " + numBlocks);
            //Console.WriteLine("Board: " + board.GetNumBlocks());
            if (numBlocks == 0) return false;
            return true;
        }

        public int GetSpeed()
        {
            return (int)speed;
        }
    }
}
