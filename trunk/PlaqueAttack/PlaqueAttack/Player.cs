using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PlaqueAttack
{
    public class Player
    {   
        KeyboardState keyboardState;
        KeyboardState oldState;

        private int id;

        public Texture2D texture;
        public Vector2 position;
        public int barNumber;
        public  Rectangle bar1;
        public Rectangle bar2;
        public Color color1;
        public Color color2;
        public bool kill;

        static int gridWidth = 480;
        static int gridHeight = 640;
        static int startX = 220;
        static int startY = 0;
 
        Rectangle playArea = new Rectangle(startX, startY, 40, 640);
        

        #region Constructor Region
        public Player(Texture2D texture, int id, Color color)
        {
            this.texture = texture;
            this.id = id;
            this.color1 = color;
            this.position.X = startX;
            this.position.Y = startY;
            this.bar1 = new Rectangle(startX + texture.Width, (int)position.Y, gridWidth, texture.Height);
            this.barNumber = 1;

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
            this.barNumber = 2;
        }

        #endregion

        public void Update()
        {
            kill = false;

            keyboardState = Keyboard.GetState();

            if(this.id == 1){

                if (keyboardState.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
                    position.Y -= 40;
                    //Console.Write("player" + position.Y + "   " );
                    //Console.Write("bar" + bar1.Y);

                if (keyboardState.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
                    position.Y += 40;
                    

                if (keyboardState.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
                    kill = true;

                oldState = keyboardState;
                LockPlayer();
            }

            if(this.id == 2) {

                if (keyboardState.IsKeyDown(Keys.W) && !oldState.IsKeyDown(Keys.W))
                    position.Y -= 40;

                if (keyboardState.IsKeyDown(Keys.S) && !oldState.IsKeyDown(Keys.S))
                    position.Y += 40;

                if (keyboardState.IsKeyDown(Keys.D) && !oldState.IsKeyDown(Keys.D))
                    kill = true;

                oldState = keyboardState;
                LockPlayer();
            }


        }

        public Vector2 GetPosition()
        {
            return position;
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
