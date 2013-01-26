using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace PlaqueAttack.Utilities
{
    /// <summary>
    /// Represents an animation as a sequence of rectangle clips of some sprite sheet elsewhere.
    /// </summary>
    class Animation : SpriteSheet
    {
        /// <summary>
        /// The length of time for which each frame is displayed.
        /// </summary>
        private readonly TimeSpan _frameLength;

        /// <summary>
        /// An internal timer used to decide how long a frame has been shown.
        /// </summary>
        private TimeSpan _frameTimer;

        /// <summary>
        /// The current animation frame.
        /// </summary>
        private int _currentFrame;

        /// <summary>
        /// The number of times to run the animation. 0 if infinitely.
        /// </summary>
        private readonly int _numTimes;

        /// <summary>
        /// The number of times currently run.
        /// </summary>
        private int _numTimesRun;

        /// <summary>
        /// The direction the animation is clipped in.
        /// </summary>
        private SheetDirection _direction;

        

        #region SpriteSheet animation constructors

        /// <summary>
        /// Create an animation.
        /// </summary>
        /// <param name="frameCount">The number of frames in the animation.</param>
        /// <param name="secondsPerFrame">The number of seconds for which each frame is shown.</param>
        /// <param name="frameWidth">The width in pixels of a frame.</param>
        /// <param name="frameHeight">The height in pixels of a frame.</param>
        /// <param name="xOffset">The beginning x-coord of the animation in the sprite sheet.</param>
        /// <param name="yOffset">The beginning y-coord of the animation in the sprite sheet.</param>
        /// <param name="numTimes">Number of times to run the animation. 0 if run infinitely.</param>
        /// <param name="reversed">Whether the animation runs in reverse or not.</param>
        /// <param name="direction">The direction the animation is clipped in.</param>
        public Animation(int frameCount, double secondsPerFrame, int frameWidth, int frameHeight, 
            int numFrameWidth, int numFrameHeight,
            int xOffset, int yOffset, int numTimes = 0, bool reversed = false,
            SheetDirection direction = SheetDirection.LeftToRight) 
            : base(frameCount, frameWidth, frameHeight, numFrameWidth, numFrameHeight, xOffset, yOffset, direction)
        {
            Debug.Assert(secondsPerFrame > 0, "Each animation frame must display for a " +
                                              "positive number of seconds.");

            _frameLength = TimeSpan.FromSeconds(secondsPerFrame);
            _frameTimer = TimeSpan.Zero;
            _numTimes = numTimes;
            _direction = direction;

            Reversed = reversed;
            if (reversed)
            {
                _currentFrame = frameCount - 1;
            }
        }

        /// <summary>
        /// Create a spritesheet animation.
        /// </summary>
        /// <param name="frameLength">The length for which each frame is shown.</param>
        /// <param name="sheet">The underlying sprite sheet for the animation.</param>
        public Animation(TimeSpan frameLength, SpriteSheet sheet)
            : base(sheet)
        {
            _frameLength = frameLength;
            _frameTimer = TimeSpan.Zero;
        }

        #endregion

        /// <summary>
        /// Whether the animation is reversed.
        /// </summary>
        public bool Reversed { get; set; }

        /// <summary>
        /// The rectangle clip for the current frame.
        /// </summary>
        public Rectangle CurrentFrame
        {
            get { return Frames[_currentFrame]; }
        }

        /// <summary>
        /// Update the animation if necessary, depending on the time.
        /// </summary>
        /// <param name="time">The current game time.</param>
        public void Update(GameTime time)
        {
            if (_numTimes > 0 && _numTimesRun >= _numTimes)
                return;

            _frameTimer += time.ElapsedGameTime;

            if (_frameTimer < _frameLength)
                return;

            _frameTimer = TimeSpan.Zero;

            if (Reversed)
            {
                _currentFrame--;

                if (_currentFrame < 0)
                {
                    if (_numTimes <= 0)
                        _currentFrame = Frames.Length - 1;
                    else if (++_numTimesRun >= _numTimes)
                        _currentFrame = 0;
                }

                return;
            }

            _currentFrame = (_currentFrame + 1) % Frames.Length;

            if (_numTimes <= 0 || _currentFrame != 0)
                return;

            if (++_numTimesRun >= _numTimes)
                _currentFrame = Frames.Length - 1;
        }

        /// <summary>
        /// Reset the animation instantly to its beginning.
        /// </summary>
        public void Reset()
        {
            _currentFrame = Reversed ? Frames.Length - 1 : 0;
            _frameTimer = TimeSpan.Zero;
            _numTimesRun = 0;
        }

        /// <summary>
        /// Returns if an animation is finished.
        /// </summary>
        public bool IsFinished()
        {
            if (_numTimesRun < _numTimes || _numTimes == 0)
                return false;

            return (Reversed && _currentFrame <= 0) || _currentFrame >= Frames.Length - 1;
        }

        /// <summary>
        /// Force the animation into the finished state.
        /// </summary>
        public void Finish()
        {
            _numTimesRun = _numTimes;
            _currentFrame = Frames.Length - 1;
        }
    }
}
