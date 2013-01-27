using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PlaqueAttack.Utilities;
using Microsoft.Xna.Framework.Audio;

namespace PlaqueAttack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Constants

        /// <summary>
        /// Designed window width.
        /// </summary>
        public const int WINDOW_WIDTH = 800;

        /// <summary>
        /// Designed window height.
        /// </summary>
        public const int WINDOW_HEIGHT = 736;

        #endregion

        #region Factory

        /// <summary>
        /// Singleton game instance.
        /// </summary>
        private static Game _game;

        /// <summary>
        /// Retrieve the singleton game instance.
        /// </summary>
        /// <returns>The game instance.</returns>
        public static Game GetGame()
        {
            return _game ?? (_game = new Game());
        }

        #endregion

        #region Constructors

        private Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #endregion

        #region Type Definitions

        private enum GameState
        {
            Loading,
            Playing,
        }

        private enum PlayingState
        {
            Title,
            Transition,
            Level,
            GameOver,
            Victory,
        }


        #endregion

        #region Static Elements

        private static Random _random = new Random();

        #endregion

        #region Private Elements

        #region XNA Graphics Elements

        /// <summary>
        /// Keeps track of the XNA graphics manager.
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// Keeps track of the <see cref="SpriteBatch"/> to which we draw.
        /// </summary>
        private SpriteBatch _spriteBatch;

        #endregion

        #region Game Elements

        /// <summary>
        /// Keeps track of the state of the <see cref="Game"/> finite state 
        /// machine.
        /// </summary>
        private GameState _state;
        private PlayingState playingState;
        private Board board;
        private Player player1;
        private List<TransformAnimation> animationUpdateArray;
        private Food currentFood;
        private Queue<Food> foodLevels;
        private int spawnTimer = 0;

        #endregion

        #endregion

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        ///
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            IsMouseVisible = true;

            _state = GameState.Loading;
            playingState = PlayingState.Title;

            Window.Title = "Plaque Attack";

            // Initialize things
            board = new Board(12, 14);
            animationUpdateArray = new List<TransformAnimation>();

            player1 = new Player(1, Block.BlockColor.Yellow);

            //currentFood = new Food(Food.FoodTypes.Banana);
            foodLevels = new Queue<Food>();
            foodLevels.Enqueue(new Food(Food.FoodTypes.Banana));
            foodLevels.Enqueue(new Food(Food.FoodTypes.Salad));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            if (_spriteBatch != null)
                _spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>      
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            switch (_state)
            {
                case GameState.Loading:

                    if (Assets.Loaded)
                    {
                        _state = GameState.Playing;

                        if (MediaPlayer.State != MediaState.Playing)
                        {
                            MediaPlayer.IsRepeating = true;
                            MediaPlayer.Volume = 1f;
                            //MediaPlayer.Play(Assets.Get<Song>("Background Music"));
                        }
                    }
                    else
                    {
                        Assets.LoadOne();
                    }

                    break;

                case GameState.Playing:
                    // Switch for playing states
                    switch (playingState)
                    {
                        case PlayingState.Title:
                            // Start when any key is pressed
                            if (keyboardState.GetPressedKeys().Length > 0)
                            {
                                playingState = PlayingState.Transition;
                            }
                            break;
                        case PlayingState.Transition:
                            if (currentFood == null) currentFood = foodLevels.Dequeue();
                            currentFood.anim.Update(gameTime);
                            currentFood.clip = currentFood.anim.CurrentFrame;
                            if (currentFood.anim.IsFinished())
                                playingState = PlayingState.Level;
                            break;
                        case PlayingState.Level:
                            Rectangle temp = currentFood.anim.CurrentFrame;
                            float percent = (float) currentFood.numBlocks / (float) currentFood.startBlocks;
                            temp.Width = (int) (currentFood.anim.CurrentFrame.Width * percent);
                            //Console.WriteLine(currentFood.numBlocks / currentFood.startBlocks);
                            currentFood.clip = temp;

                            spawnTimer++;
                            if (currentFood != null)
                            {
                                if (spawnTimer >= currentFood.GetSpeed())
                                {
                                    if (currentFood.numBlocks > 0)
                                    {
                                        currentFood.spawnToBoard(board, animationUpdateArray);
                                        spawnTimer = 0;
                                    }
                                    else
                                    {
                                        //if (board.GetNumBlocks() == 0)
                                        {
                                            currentFood = null;
                                            if (foodLevels.Count == 0)
                                            {
                                                // No more foods... YOU WIN!
                                                playingState = PlayingState.Victory;
                                                break;
                                            }
                                            playingState = PlayingState.Transition;
                                        }
                                    }
                                }
                            }
                            break;
                        case PlayingState.GameOver:
                            Console.WriteLine("GameOver state");
                            break;
                        case PlayingState.Victory:
                            break;
                    }

                    // Update any transform animations; delete them if they're done
                    for (int i = 0; i < animationUpdateArray.Count; i++)
                    {
                        bool done = animationUpdateArray[i].Update(gameTime);
                        if (done)
                        {
                            // If a block has finished animating, make sure it is set to active
                            if (animationUpdateArray[i].GetObject() is Block)
                            {
                                ((Block)animationUpdateArray[i].GetObject()).SetActive(true);
                                if (board.gameLost)
                                {
                                    board.ClearBoard();
                                    currentFood = null;
                                    playingState = PlayingState.GameOver;
                                }  
                            }
                            animationUpdateArray.Remove(animationUpdateArray[i]);
                            
                        }
                    }
                    player1.Update();
                    playerBlockCollision(player1, board);
                    break;
            }

            base.Update(gameTime);
        }

        public static Vector2 TransformGridToScreen(Vector2 gridLocation)
        {
            return new Vector2(260 + gridLocation.X * 40, 40 + gridLocation.Y * 40);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (_state)
            {
                case GameState.Loading:
                    DrawLoadingScreen();
                    break;

                case GameState.Playing:

                    /// DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 40, 40, Color.Black, Color.White);



                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    // Draw background
                    Texture2D artery = Assets.Get<Texture2D>("Artery");
                    _spriteBatch.Draw(artery, new Vector2(236, 0), Color.White);

                    // Draw blocks from the board
                    Texture2D blockTexture = Assets.Get<Texture2D>("Block");
                    Block[,] b = board.GetBoard();
                    for (int c = 0; c < b.GetLength(0); c++)
                    {
                        for (int r = 0; r < b.GetLength(1); r++)
                        {
                            if (b[c, r] != null)
                            {
                                Color color = new Color();
                                Block.BlockColor bc = b[c, r].GetColor();
                                switch (bc)
                                {
                                    case Block.BlockColor.Yellow:
                                    {
                                        color = Color.Yellow;
                                        break;
                                    }
                                    case Block.BlockColor.Blue:
                                    {
                                        color = Color.Blue;
                                        break;
                                    }
                                    case Block.BlockColor.Purple:
                                    {
                                        color = Color.Purple;
                                        break;
                                    }
                                    case Block.BlockColor.Red:
                                    {
                                        color = Color.Red;
                                        break;
                                    }
                                    case Block.BlockColor.Brown:
                                    {
                                        color = Color.Brown;
                                        break;
                                    }
                                    case Block.BlockColor.Green:
                                    {
                                        color = Color.Green;
                                        break;
                                    }
                                    case Block.BlockColor.Magenta:
                                    {
                                        color = Color.Magenta;
                                        break;
                                    }
                                    case Block.BlockColor.Orange:
                                    {
                                        color = Color.Orange;
                                        break;
                                    }
                                }
                                _spriteBatch.Draw(blockTexture, b[c, r].GetLoc(), color);
                            }
                        }
                    }
                    
                    //players
                    Texture2D barTexture = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 480, 40, Color.Yellow, Color.Yellow);
                    Texture2D playerTexture = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 40, 40, Color.Yellow, Color.Yellow);
                    Vector2 barPosition = new Vector2(player1.position.X + 40, player1.position.Y);
                    Vector2 endPosition = new Vector2(player1.position.X + 520, player1.position.Y);

                    _spriteBatch.Draw(playerTexture, player1.position, Color.White);
                    _spriteBatch.Draw(playerTexture, endPosition, Color.White);
                    _spriteBatch.Draw(barTexture, barPosition, Color.White);

                    // Draw 

                    // Draw food if set
                    if (currentFood != null)
                    {
                        Texture2D banana = Assets.Get<Texture2D>("Banana");
                        Texture2D salad = Assets.Get<Texture2D>("Salad");
                        if (currentFood.foodType == Food.FoodTypes.Banana)
                            _spriteBatch.Draw(banana, new Vector2(4, 68), currentFood.clip, Color.White);
                        else if (currentFood.foodType == Food.FoodTypes.Salad)
                            _spriteBatch.Draw(salad, new Vector2(4, 68), currentFood.clip, Color.White);
                        
                    }
                    _spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }

        private double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }

        #region Game Methods
        public void playerBlockCollision(Player player, Board board)
        {
            Block[,] blocks = board.GetBoard();

            if (player.kill == true)
            {
                foreach (Block b in blocks)
                {
                    if (b != null)
                    {
                        //Console.WriteLine("checked space");
                        if (player.barNumber == 1)
                        {

                            //checks if player bar intersects block
                            Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                            Rectangle bar = new Rectangle((int)player.GetPosition().X + 40, (int)player.GetPosition().Y, 480, 40);

                            if (bar.Intersects(r))
                            {
                                //Console.WriteLine("collision");

                                //checks if bar and block colors match
                                if (player.color1 == (b.GetColor()))
                                {

                                    bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                    //Console.WriteLine(cleared);
                                }

                            }
                        }
                        if (player.barNumber == 2)
                        {
                            Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                            Rectangle bar1 = new Rectangle((int)player.GetPosition().X + 40, (int)player.GetPosition().Y, 480, 40);
                            Rectangle bar2 = new Rectangle((int)player.GetPosition().X + 40, (int)player.GetPosition().Y + 40, 480, 40);
                            if (bar1.Intersects(r))
                            {
                                //checks if bar and block colors match
                                if (player.color1 == (b.GetColor()))
                                {
                                    bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                }
                            }
                            if (bar2.Intersects(r))
                            {
                                if (player.color2 == (b.GetColor()))
                                {
                                    bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                }

                            }
                        }
                    }

                }
            } 

        }

        #endregion

        #region Drawing Methods
        /// <summary>
        /// Draw the loading resources screen.
        /// </summary>
        private void DrawLoadingScreen()
        {
            Debug.Assert(_spriteBatch != null, "The sprite batch we use should not be null.");

        }

        private void DrawTitleScreen()
        {
            Debug.Assert(_spriteBatch != null, "The sprite batch we use should not be null.");

        }


        #endregion
    }
}
