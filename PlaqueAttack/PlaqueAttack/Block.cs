using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlaqueAttack
{
    public class Block
    {
        private BlockColor color;
        private bool active;
        private Vector2 gridLoc; // Location in the grid (r, c)
        private Vector2 loc; // Location on the screen (x, y)
        //public Rectangle bounds;
        static int width = 40;
        static int height = 40;
        public int x;
        public int y;

        public enum BlockColor
        {
            Yellow,
            Blue,
            Green,
            Purple,
            Red,
            Brown,
            Magenta,
            Orange,
            Black = 8,
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
            x = (int)gridLoc.X;
            y = (int)gridLoc.Y;
            //bounds = new Rectangle((int)loc.X, (int)loc.Y, width, height);
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }

        public bool IsActive()
        {
            return active;
        }

        public void setGridLoc(int r, int c) {
            gridLoc = new Vector2(r, c);
        }
        public void SetLoc(Vector2 location)
        {
            loc = location;
        }

        public BlockColor GetColor()
        {
            return color;
        }

        public Vector2 getGridLoc()
        {
            return gridLoc;
        }
        public Vector2 GetLoc()
        {
            return loc;
        }

        public void Draw()
        {

        }
    }
}
