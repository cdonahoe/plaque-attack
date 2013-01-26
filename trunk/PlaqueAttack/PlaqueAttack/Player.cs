using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PlaqueAttack
{
    class Player
    {
        private Texture2D texture;
        private Vector2 position;
        private int id;
        private Color color1;
        private Color color2;
        Vector2 motion;
        float playerSpeed = 0.8f;
        KeyboardState keyboardState;
        static int startX = 100;
        static int startY = 0;
        public bool kill;
        Rectangle playArea = new Rectangle(startX, startY, 40, 640);
        private Rectangle bar1;
        private Rectangle bar2;
        static int gridWidth;
        static int gridHeight;

        #region Constructor Region
        public Player(Texture2D texture, int id, Color color)
        {
            this.texture = texture;
            this.id = id;
            this.color1 = color;
            this.position.X = startX;
            this.position.Y = startY;
            Rectangle bar1 = new Rectangle(startX + texture.Width, startY, gridWidth, texture.Height);


        }

        public Player(Texture2D texture, int id, Color color1, Color color2)
        {
            this.texture = texture;
            this.id = id;
            this.color1 = color1;
            this.color2 = color2;
            this.position.X = startX;
            this.position.Y = startY;
            Rectangle bar1 = new Rectangle(startX + texture.Width, startY, gridWidth, (texture.Height/2));
            Rectangle bar2 = new Rectangle(startX + texture.Width, startY + (texture.Height/2), gridWidth, (texture.Height / 2));
    

        }

        #endregion

        public void Update()
        {
            motion = Vector2.Zero;
            kill = false;

            keyboardState = Keyboard.GetState();

            if(this.id == 1){

                if (keyboardState.IsKeyDown(Keys.Up))
                    motion.Y = -1;

                if (keyboardState.IsKeyDown(Keys.Down))
                    motion.Y = 1;

                if (keyboardState.IsKeyDown(Keys.Left))
                    kill = true;
                    
                motion.Y *= playerSpeed;
                position += motion;
                LockPlayer();
            }

            if(this.id == 2) {

                if (keyboardState.IsKeyDown(Keys.W))
                    motion.Y = -1;

                if (keyboardState.IsKeyDown(Keys.S))
                    motion.Y = 1;

                if (keyboardState.IsKeyDown(Keys.D))
                    kill = true;

                motion.Y *= playerSpeed;
                position += motion;
                LockPlayer();
            }


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }


        public void LockPlayer()
        {
            if (position.Y < 0)
                position.Y = 0;
            if (position.Y + texture.Height > playArea.Height)
                position.Y = playArea.Height - texture.Height;
        }

    }
}
