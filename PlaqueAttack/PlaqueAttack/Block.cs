using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlaqueAttack
{
    class Block
    {
        private BlockColor color;
        private bool active;
        private Vector2 gridLoc; // Location in the grid (r, c)
        private Vector2 loc; // Location on the screen (x, y)

        public enum BlockColor
        {
            // These are placeholders!
            Red,
            Blue,
            Cyan,
            Magenta,
            Yellow,
            Green,
            Orange,
            Purple,
        }

        /// <summary>
        /// Creates an instance of Block
        /// </summary>
        /// <param name="color">Color of this block (type Block.BlockColor)</param>
        /// <param name="location">x, y starting location of the block</param>
        public Block(BlockColor color, Vector2 location)
        {
            this.color = color;
            active = false;
            loc = location;
        }

        public void setActive(bool active)
        {
            this.active = active;
        }

        public bool isActive()
        {
            return active;
        }

        public void setGridLoc(int r, int c) {
            gridLoc = new Vector2(r, c);
        }

        public void getGridLoc()
        {
            //return 
        }

        public void Draw()
        {

        }
    }
}
