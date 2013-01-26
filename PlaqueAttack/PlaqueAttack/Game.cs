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
            TitleScreen,
            Playing,
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
        private Board board;
        private Player player1;
        private List<TransformAnimation> animationUpdateArray;
        private Food currentFood;
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

        // TESTING
        private Block block;
        private Texture2D playerTexture;
        private TransformAnimation rectAnimation;
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            IsMouseVisible = true;

            _state = GameState.Loading;

            Window.Title = "Plaque Attack";

            // Initialize things
            board = new Board(12, 14);
            animationUpdateArray = new List<TransformAnimation>();
            //TESTING
            block = new Block(Block.BlockColor.Orange, new Vector2(100, 100));
            playerTexture = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 40, 40, Color.Green, Color.Green);
            player1 = new Player(playerTexture, 1, Color.Green);

            currentFood = new Food(Food.FoodTypes.Banana);

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
            var mouseState = Mouse.GetState();

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

                        

                        // TESTING Add a bunch of blocks to the board
                        Food banana = new Food(Food.FoodTypes.Banana);
                        for (int i = 0; i < 50; i++)
                        {
                            //Block b = new Block(Block.BlockColor.Orange, new Vector2(0, 0));
                            //Vector2 endPos = TransformGridToScreen(board.PlaceBlock(b));
                            //Vector2 startPos = new Vector2(endPos.X, -40);
                            //TransformAnimation tran = new TransformAnimation(b, TimeSpan.FromSeconds(0.5), startPos, endPos, TransformAnimation.AnimationCurve.Smooth);
                            //animationUpdateArray.Add(tran);
                            //banana.spawnToBoard(board, animationUpdateArray);
                        }
                    }
                    else
                    {
                        Assets.LoadOne();
                    }

                    break;

                case GameState.TitleScreen:

                    var state = Mouse.GetState();

                    if (state.LeftButton == ButtonState.Pressed)
                    {
                        _state = GameState.Playing;
                    }

                    break;

                case GameState.Playing:
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
                                if (board.gameLost) board.ClearBoard();
                                currentFood = null;
                            }
                            animationUpdateArray.Remove(animationUpdateArray[i]);
                            
                        }
                    }

                    // Update food if set
                    spawnTimer++;
                    if (currentFood != null)
                    { 
                        if (spawnTimer >= currentFood.GetSpeed())
                        {
                            currentFood.spawnToBoard(board, animationUpdateArray);
                            spawnTimer = 0;
                        }
                    }

                    player1.Update();
                    Block[,] blockArray = board.GetBoard();
                    foreach (Block b in blockArray){
                        if (b != null)
                        {
                            b.playerBlockCollision(player1);
                        }
                    }
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

                case GameState.TitleScreen:
                    DrawTitleScreen();
                    break;

                case GameState.Playing:

                    Texture2D blockTexture = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 40, 40, Color.Black, Color.White);



                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    // Draw background
                    Texture2D artery = Assets.Get<Texture2D>("Artery");
                    _spriteBatch.Draw(artery, new Vector2(236, 0), Color.White);

                    // Draw blocks from the board
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
                    _spriteBatch.Draw(playerTexture, player1.position, Color.White);

                    // Draw 
                    Animation a = new Animation(5, 0.2, 50, 50, 5, 1, 0, 0, 1); 
                    Texture2D banana = Assets.Get<Texture2D>("Banana");
                    _spriteBatch.Draw(banana, new Vector2(4, 68), Color.White);
                    _spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }

        private double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }



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
