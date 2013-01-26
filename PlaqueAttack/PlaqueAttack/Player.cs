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
        public Color color1;
        public Color color2;
        KeyboardState keyboardState;
        static int startX = 100;
        static int startY = 0;
        public bool kill;
        Rectangle playArea = new Rectangle(startX, startY, 40, 640);
        public  Rectangle bar1;
        public Rectangle bar2;
        static int gridWidth = 480;
        static int gridHeight = 640;

        #region Constructor Region
        public Player(Texture2D texture, int id, Color color)
        {
            this.texture = texture;
            this.id = id;
            this.color1 = color;
            this.position.X = startX;
            this.position.Y = startY;
            this.bar1 = new Rectangle(startX + texture.Width, startY, gridWidth, texture.Height);


        }

        public Player(Texture2D texture, int id, Color color1, Color color2)
        {
            this.texture = texture;
            this.id = id;
            this.color1 = color1;
            this.color2 = color2;
            this.position.X = startX;
            this.position.Y = startY;
            this.bar1 = new Rectangle(startX + texture.Width, startY, gridWidth, (texture.Height/2));
            this.bar2 = new Rectangle(startX + texture.Width, startY + (texture.Height/2), gridWidth, (texture.Height / 2));

        }

        #endregion

        public void Update()
        {
            kill = false;
            keyboardState = Keyboard.GetState();

            if(this.id == 1){

                if (keyboardState.IsKeyDown(Keys.Up))
                    position.Y -= 40;

                if (keyboardState.IsKeyDown(Keys.Down))
                    position.Y += 40;

                if (keyboardState.IsKeyDown(Keys.Left))
                    kill = true;
                   
                LockPlayer();
            }

            if(this.id == 2) {

                if (keyboardState.IsKeyDown(Keys.W))
                    position.Y -= 40;

                if (keyboardState.IsKeyDown(Keys.S))
                    position.Y += 40;

                if (keyboardState.IsKeyDown(Keys.D))
                    kill = true;
                
                LockPlayer();
            }


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
