using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlaqueAttack.Utilities
{
    class TransformAnimation
    {
        // For translation animations
        private Vector2 currentPosition;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private TimeSpan duration;
        private TimeSpan currentStep;
        private AnimationCurve curve;
        private Object obj;

                /// <summary>
        /// Type of animation curve for translation animation
        /// </summary>
        public enum AnimationCurve
        {
            Lerp,
            Smooth,
        }

        /// <summary>
        /// Create a translate animation that will lerp (todo: other curves) between
        /// start and end points.
        /// </summary>
        /// <param name="obj">The object to animate</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        public TransformAnimation(Object obj, TimeSpan duration, Vector2 start, Vector2 end, AnimationCurve curve) 
        {
            this.obj = obj;
            startPosition = start;
            endPosition = end;
            currentPosition = start;
            this.duration = duration;
            this.curve = curve;
            currentStep = TimeSpan.FromSeconds(0);
        }

        /// <summary>
        /// Next step in animation
        /// </summary>
        /// <returns>Returns true if animation has completed</returns>
        public bool Update(GameTime gameTime)
        {
            if (currentStep != duration)
            {
                currentStep += gameTime.ElapsedGameTime;
                // If it's the last step
                if (currentStep >= duration)
                {
                    currentStep = duration;
                    if (curve == TransformAnimation.AnimationCurve.Lerp)
                    {
                        currentPosition = Vector2.Lerp(startPosition, endPosition, 1);
                    }
                    else if (curve == TransformAnimation.AnimationCurve.Smooth)
                    {
                        currentPosition = Vector2.SmoothStep(startPosition, endPosition, 1);
                    }
                    else throw new Exception("AnimationCurve type not found!");

                }
                // If it's not the last step
                else
                {
                    float amount = (float)currentStep.Ticks / duration.Ticks;
                    if (curve == TransformAnimation.AnimationCurve.Lerp)
                    {
                        currentPosition = Vector2.Lerp(startPosition, endPosition, amount);
                    }
                    else if (curve == TransformAnimation.AnimationCurve.Smooth)
                    {
                        currentPosition = Vector2.SmoothStep(startPosition, endPosition, amount);
                    }
                    else throw new Exception("AnimationCurve type not found!");
                }
                if (obj is Block) ((Block)obj).SetLoc(currentPosition);

            }
            return true;
        }

        public Vector2 getCurrentPosition()
        {
            return currentPosition;
        }
    }
}
