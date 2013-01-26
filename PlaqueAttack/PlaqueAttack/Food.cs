using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaqueAttack
{
    class Food
    {
        FoodTypes type; // This should probably be an en enum
        int numBlocks;
        int numColors;
        float speed;

        public enum FoodTypes
        {
            Banana,
            Salad,
            Hamburger,
            Cake,
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
            this.type = type;
            this.numBlocks = numBlocks;
            this.numColors = numColors;
            this.speed = speed;
        }
    }
}
