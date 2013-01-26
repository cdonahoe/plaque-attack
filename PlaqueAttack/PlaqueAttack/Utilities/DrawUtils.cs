using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace PlaqueAttack.Utilities
{
    /// <summary>
    /// Provides a set of primitive drawing utilities.
    /// </summary>
    static class DrawUtils
    {
        /// <summary>
        /// Creates a circle outline texture with a notch to the right indicating direction.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>A white circle texture.</returns>
        public static Texture2D CreateCircleWithDirectionNotch(GraphicsDevice device, int radius)
        {
            var outerRadius = radius * 2 + 2; // so circle doesn't go out of bounds
            var texture = new Texture2D(device, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            // Draw the circle.
            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            // Draw a small notch to indicate the heading of the circle.
            for (var i = 3 * radius / 2; i < 2 * radius + 2; i++)
            {
                var x = i;
                var y = radius;

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a circle outline texture.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>A white circle texture.</returns>
        public static Texture2D CreateCircle(GraphicsDevice device, int radius)
        {
            var outerRadius = radius * 2 + 2; // so circle doesn't go out of bounds
            var texture = new Texture2D(device, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            // Draw the circle.
            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a progress bar texture.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="width">The width of the progress bar.</param>
        /// <param name="height">The height of the progress bar.</param>
        /// <param name="segments">The progress pieces, which add up to 1.</param>
        /// <returns>A segmented progress bar.</returns>
        public static Texture2D CreateProgressBar(GraphicsDevice device, int width, int height, 
            params Tuple<double, Color>[] segments)
        {
            Debug.Assert(width > 0 && height > 0, "Width and height of progress bar must be positive.");

            var texture = new Texture2D(device, width, height);
            var data = new Color[width * height];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            for (var i = 0; i < height; ++i)
            {
                data[i * width] = Color.Gray;
                data[i * width + width - 1] = Color.Gray;
            }

            var progressAccumulator = 0.0;
            var progressIndex = 0;

            var numCols = 0;
            if (segments.Length > 0)
            {
                progressAccumulator += segments[progressIndex].Item1;
                numCols = (int)(progressAccumulator * width);
            }

            for (var i = 1; i < width - 1; ++i)
            {
                data[i] = Color.Gray;
                data[(height - 1) * width + i] = Color.Gray;

                while (i > numCols)
                {
                    ++progressIndex;
                    if (segments.Length <= progressIndex)
                        break;

                    progressAccumulator += segments[progressIndex].Item1;
                    numCols = (int)(progressAccumulator * width);
                }

                if (segments.Length <= progressIndex)
                    continue;

                for (var j = 1; j < height - 1; ++j)
                {
                    data[j * width + i] = segments[progressIndex].Item2;
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a rectangle outline texture.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="width">The width of the progress bar.</param>
        /// <param name="height">The height of the progress bar.</param>
        /// <param name="borderColor">The color for the border of the rectangle.</param>
        /// <param name="fillColor">The color for the fill of the rectangle.</param>
        /// <returns>A white rectangle outline texture.</returns>
        public static Texture2D CreateFilledRectangle(GraphicsDevice device, int width, int height, 
            Color borderColor, Color fillColor)
        {
            Debug.Assert(width > 0 && height > 0, "Width and height of rectangle must be positive.");

            var texture = new Texture2D(device, width, height);
            var data = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var color = (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                                ? borderColor
                                : fillColor;

                    data[y*width + x] = color;
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a rectangle outline texture.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="width">The width of the progress bar.</param>
        /// <param name="height">The height of the progress bar.</param>
        /// <returns>A white rectangle outline texture.</returns>
        public static Texture2D CreateRectangle(GraphicsDevice device, int width, int height)
        {
            Debug.Assert(width > 0 && height > 0, "Width and height of rectangle must be positive.");

            var texture = new Texture2D(device, width, height);
            var data = new Color[width * height];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            for (var i = 0; i < height; ++i)
            {
                data[i * width] = Color.White;
                data[i * width + width - 1] = Color.White;
            }

            for (var i = 0; i < width; ++i)
            {
                data[i] = Color.White;
                data[(height - 1) * width + i] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a circle texture filled in.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="border">The color of the border of the circle</param>
        /// <param name="fillCenter">The fill color for the center circle.</param>
        /// <param name="fillOuter">The fill color for the outer region of the inner part of the circle</param>
        /// <returns>A white circle texture.</returns>
        public static Texture2D CreateFilledCircle(GraphicsDevice device, int radius, 
            Color border, Color fillCenter, Color fillOuter)
        {
            var outerRadius = radius * 2 + 2; // so circle doesn't go out of bounds
            var texture = new Texture2D(device, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            var fillC = fillCenter.ToVector4();
            var fillO = fillOuter.ToVector4();

            // Draw the circle.)
            for (double angle = 0; angle <= 2 * Math.PI; angle += angleStep)
            {
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y1 = (int)Math.Round(radius + radius * Math.Sin(angle));
                var y2 = (int)Math.Round(radius - radius * Math.Sin(angle));

                data[y1 * outerRadius + x + 1] = border;
                data[y2 * outerRadius + x + 1] = border;

                for (var i = y2 + 1; i < y1; ++i)
                {
                    var dx = (i - outerRadius / 2);
                    var dy = (x - outerRadius / 2);
                    var dist = (float)Math.Sqrt(dx*dx + dy*dy);
                    var interp = dist/outerRadius;
                    data[i * outerRadius + x + 1] = new Color(interp * fillO + (1 - interp) * fillC);
                }

                    
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a circle texture filled in horizontally from the left by progress percent.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="progress">The percent of the circle filled in, in the range 0 to 1.</param>
        /// <returns>A white circle texture.</returns>
        public static Texture2D CreateProgressCircle(GraphicsDevice device, int radius, double progress)
        {
            var outerRadius = radius * 2 + 2; // so circle doesn't go out of bounds
            var texture = new Texture2D(device, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            // Work out the angle beyond which we fill in the circle.
            var angleBoundary = Math.PI - Math.Acos(1 - progress);

            // Draw the circle.)
            for (double angle = 0; angle < Math.PI; angle += angleStep)
            {
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y1 = (int)Math.Round(radius + radius * Math.Sin(angle));
                var y2 = (int)Math.Round(radius - radius * Math.Sin(angle));

                data[y1 * outerRadius + x + 1] = Color.White;
                data[y2 * outerRadius + x + 1] = Color.White;

                if (angle < angleBoundary)
                    continue;

                for (var i = y2 + 1; i < y1; ++i)
                    data[ i * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates an equilateral triangle outline texture.
        /// </summary>
        /// <param name="device">The XNA graphics context.</param>
        /// <param name="side">The side length of each triangle.</param>
        /// <returns>A white equilateral triangle texture.</returns>
        public static Texture2D CreateEquilateralTriangle(GraphicsDevice device, int side)
        {
            var height = (int) Math.Ceiling(Math.Sqrt(3) / 2 * side);

            var texture = new Texture2D(device, side, height);
            var data = new Color[side * height];

            // Color the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Draw the triangle's base.
            for (var i = (height - 1) * side; i < data.Length; i++)
                data[i] = Color.White;

            // Draw the triangle's two slanted slides.
            double currLftX = 0;
            double currRgtX = side - 1;
            var xDiff = 1 / Math.Sqrt(3);
            for (var j = height - 2; j >= 0; j--)
            {
                currLftX += xDiff;
                currRgtX -= xDiff;
                data[j * side + (int)(currLftX + 0.5)] = Color.White;
                data[j * side + (int)(currRgtX + 0.5)] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        /*
        public static void DrawLineSegment(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, int lineWidth) 
        {
          var angle = (float) Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
          var length = Vector2.Distance(point1, point2);

          spriteBatch.Draw(Assets.Get<Texture2D>("Spacer"), point1, null, color, 
            angle, Vector2.Zero, new Vector2(length, lineWidth), 
            SpriteEffects.None, 0f); 
        }
         */
    }
}
