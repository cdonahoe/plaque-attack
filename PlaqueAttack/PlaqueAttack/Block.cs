using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaqueAttack
{
    class Block
    {
        BlockColor color;
        bool active;
        int x;
        int y;

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

        public Block(BlockColor color, int r, int c)
        {
            this.color = color;
            active = false;
        }

        public void setActive(bool active)
        {
            this.active = active;
        }

        public bool isActive()
        {
            return active;
        }

        public void Draw()
        {

        }
    }
}
