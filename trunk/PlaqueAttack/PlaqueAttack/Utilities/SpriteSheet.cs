using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace PlaqueAttack.Utilities
{
    /// <summary>
    /// Represents the individual sprites as rectangular clips of a sprite sheet.
    /// </summary>
    [Serializable]
    public class SpriteSheet
    {
        public enum SheetDirection
        {
            LeftToRight,
            TopToBottom,
        }

        /// <summary>
        /// Our frame clips.
        /// </summary>
        protected Rectangle[] Frames;

        /// <summary>
        /// Create a sprite sheet.
        /// </summary>
        /// <param name="frameCount">The number of frames in the sprite sheet.</param>
        /// <param name="frameWidth">The width in pixels of a frame.</param>
        /// <param name="frameHeight">The height in pixels of a frame.</param>
        /// <param name="xOffset">The beginning x-coord of the first frame in the sprite sheet.</param>
        /// <param name="yOffset">The beginning y-coord of the first frame in the sprite sheet.</param>
        /// <param name="direction">The direction the sheet is clipped in.</param>
        public SpriteSheet(int frameCount, int frameWidth, int frameHeight, int numFrameWidth, int numFrameHeight,
            int xOffset, int yOffset, SheetDirection direction = SheetDirection.LeftToRight)
        {
            Debug.Assert(frameCount > 0, "Sprite sheets must have a positive frame count.");
            Debug.Assert(frameWidth > 0, "Frames must have a positive width.");
            Debug.Assert(frameHeight > 0, "Frames must have a positive height.");

            Frames = new Rectangle[frameCount];
            for (var i = 0; i < frameCount; i++)
            {
                Frames[i] = new Rectangle(xOffset, yOffset, frameWidth, frameHeight);
                if (direction == SheetDirection.TopToBottom)
                {
                    yOffset += frameHeight;
                }
                else
                {
                    xOffset += frameWidth;
                    if (xOffset == frameWidth * numFrameWidth)
                    {
                        xOffset = 0;
                        yOffset += frameHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Create a sprite sheet from another sprite sheet.
        /// </summary>
        public SpriteSheet(SpriteSheet sheet)
        {
            Frames = sheet.Frames;
        }

        /// <summary>
        /// Private constructor for serialization.
        /// </summary>
        private SpriteSheet()
        {
        }

        /// <summary>
        /// Return the frame at the specified index.
        /// </summary>
        /// <param name="index">The index of the frame.</param>
        /// <returns>The frame.</returns>
        public Rectangle GetFrame(int index)
        {
            Debug.Assert(index >= 0 && index < Frames.Length, "Frame index is out of bounds.");

            return Frames[index];
        }
    }
}
