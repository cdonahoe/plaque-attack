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
        private bool alive;
        private Vector2 gridLoc; // Location in the grid (r, c)
        private Vector2 loc; // Location on the screen (x, y)
        private Rectangle bounds;
        static int width = 40;
        static int height = 40;

        public enum BlockColor
        {
            Yellow,
            Blue,
            Purple,
            Red,
            Brown,
            Green,
            Magenta,
            Orange
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
            alive = true;
            loc = location;
            bounds = new Rectangle((int)loc.X, (int)loc.Y, width, height);
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }

        public bool IsActive()
        {
            return active;
        }

        public bool isAlive()
        {
            return alive;
        }

        public void setGridLoc(int r, int c) {
            gridLoc = new Vector2(r, c);
        }
        public void SetLoc(Vector2 location)
        {
            loc = location;
        }


        //checks for collision 
        public void playerBlockCollision(Player player)
        {
            //checks if player hit kill button
            if (player.kill == true)
            {
                if (player.barNumber == 1)
                {
                    //checks if player bar intersects block
                    if (player.bar1.Intersects(bounds))
                    {
                        //checks if bar and block colors match
                        if (player.color1.Equals(this.color))
                        {
                            this.alive = false;
                        }

                    }
                }

                if (player.barNumber == 2)
                {
                    if (player.bar1.Intersects(bounds))
                    {
                        //checks if bar and block colors match
                        if (player.color1.Equals(this.color))
                        {
                            this.alive = false;
                        }
                    }
                    if (player.bar2.Intersects(bounds))
                    {
                        if (player.color2.Equals(this.color))
                        {
                            this.alive = false;
                        }
                    }

                }
            }

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
