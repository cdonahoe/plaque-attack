//////////////////////////////////////////////////////////////////////////
////License:  The MIT License (MIT)
////Copyright (c) 2010 David Amador (http://www.david-amador.com)
////
////Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
////
////The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
////
////THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaqueAttack.Utilities
{
    static class Resolution
    {
        private static GraphicsDeviceManager _device;

        static private int _width = 800;
        static private int _height = 600;
        static private int _vWidth = 1024;
        static private int _vHeight = 768;
        static private bool _dirtyMatrix = true;

        static public int Width
        {
            get { return _width; }
        }

        static public int Height
        {
            get { return _height; }
        }

        public static bool IsFullscreen { get; private set; }

        public static int ViewportX { get; private set; }

        public static int ViewportY { get; private set; }

        public static int ViewportWidth { get; private set; }

        public static int ViewportHeight { get; private set; }

        public static Matrix Matrix { get; private set; }

        static public void Init(ref GraphicsDeviceManager device)
        {
            _width = device.PreferredBackBufferWidth;
            _height = device.PreferredBackBufferHeight;
            _device = device;
            _dirtyMatrix = true;
            ApplyResolutionSettings();
        }


        static public Matrix GetTransformationMatrix()
        {
            if (_dirtyMatrix) RecreateScaleMatrix();
            
            return Matrix;
        }

        static public void SetResolution(int width, int height, bool fullScreen)
        {
            _width = width;
            _height = height;

            IsFullscreen = fullScreen;

           ApplyResolutionSettings();
        }

        static public void SetVirtualResolution(int width, int height)
        {
            _vWidth = width;
            _vHeight = height;

            _dirtyMatrix = true;
        }

        static private void ApplyResolutionSettings()
       {

#if XBOX360
           _fullScreen = true;
#endif

           // If we aren't using a full screen mode, the height and width of the window can
           // be set to anything equal to or smaller than the actual screen size.
           if (IsFullscreen == false)
           {
               if ((_width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                   && (_height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
               {
                   _device.PreferredBackBufferWidth = _width;
                   _device.PreferredBackBufferHeight = _height;
                   _device.IsFullScreen = IsFullscreen;
                   _device.ApplyChanges();
               }
           }
           else
           {
               // If we are using full screen mode, we should check to make sure that the display
               // adapter can handle the video mode we are trying to set.  To do this, we will
               // iterate through the display modes supported by the adapter and check them against
               // the mode we want to set.
               foreach (var dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
               {
                   // Check the width and height of each mode against the passed values
                   if ((dm.Width == _width) && (dm.Height == _height))
                   {
                       // The mode is supported, so set the buffer formats, apply changes and return
                       _device.PreferredBackBufferWidth = _width;
                       _device.PreferredBackBufferHeight = _height;
                       _device.IsFullScreen = IsFullscreen;
                       _device.ApplyChanges();
                   }
               }
           }

           _dirtyMatrix = true;

           _width =   _device.PreferredBackBufferWidth;
           _height = _device.PreferredBackBufferHeight;
       }

        /// <summary>
        /// Sets the device to use the draw pump
        /// Sets correct aspect ratio
        /// </summary>
        static public void BeginDraw()
        {
            // Start by reseting viewport to (0,0,1,1)
            FullViewport();
            // Clear to Black
            _device.GraphicsDevice.Clear(Color.Black);
            // Calculate Proper Viewport according to Aspect Ratio
            ResetViewport();
            // and clear that
            // This way we are gonna have black bars if aspect ratio requires it and
            // the clear color on the rest
            //_device.GraphicsDevice.Clear(Color.CornflowerBlue);
            _device.GraphicsDevice.Clear(Color.Black);
        }

        static private void RecreateScaleMatrix()
        {
            _dirtyMatrix = false;
            Matrix = Matrix.CreateScale(
                           (float)_device.GraphicsDevice.Viewport.Width / _vWidth,
                           (float)_device.GraphicsDevice.Viewport.Width / _vWidth,
                           1f);
        }


        static public void FullViewport()
        {
            var vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = _width;
            vp.Height = _height;
            ViewportX = 0;
            ViewportY = 0;
            ViewportWidth = _width;
            ViewportHeight = _height;
            _device.GraphicsDevice.Viewport = vp;
        }

        /// <summary>
        /// Get virtual Mode Aspect Ratio
        /// </summary>
        /// <returns>aspect ratio</returns>
        static public float GetVirtualAspectRatio()
        {
            return (float)_vWidth / _vHeight;
        }

        static public void ResetViewport()
        {
            var targetAspectRatio = GetVirtualAspectRatio();
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            var width = _device.PreferredBackBufferWidth;
            var height = (int)(width / targetAspectRatio + .5f);
            var changed = false;
            
            if (height > _device.PreferredBackBufferHeight)
            {
                height = _device.PreferredBackBufferHeight;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
                changed = true;
            }

            // set up the new viewport centered in the backbuffer
            var viewport = new Viewport
            {
                X = (_device.PreferredBackBufferWidth/2) - (width/2),
                Y = (_device.PreferredBackBufferHeight/2) - (height/2),
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };

            ViewportX = viewport.X;
            ViewportY = viewport.Y;
            ViewportWidth = viewport.Width;
            ViewportHeight = viewport.Height;

            if (changed)
            {
                _dirtyMatrix = true;
            }

            _device.GraphicsDevice.Viewport = viewport;
        }

    }
}
