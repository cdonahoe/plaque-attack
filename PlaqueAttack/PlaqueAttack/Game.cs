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
        private Player player2;
        private List<TransformAnimation> animationUpdateArray;
        private Food currentFood;
        private Queue<Food> foodLevels;
        private int spawnTimer = 0;
        private int endTimer;
        public int score;

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
            SoundEffect.MasterVolume = 0.1f;


            _state = GameState.Loading;
            playingState = PlayingState.Title;

            Window.Title = "Plaque Attack";

            // Initialize things
            board = new Board(12, 14);
            animationUpdateArray = new List<TransformAnimation>();

            //player1 = new Player(1, Block.BlockColor.Yellow, Block.BlockColor.Blue);
            player1 = new Player(1, Block.BlockColor.Yellow, Block.BlockColor.Purple);
            player2 = new Player(2, Block.BlockColor.Blue, Block.BlockColor.Red);

            foodLevels = new Queue<Food>();
            foodLevels.Enqueue(new Food(Food.FoodTypes.Banana));
            foodLevels.Enqueue(new Food(Food.FoodTypes.Salad));
            foodLevels.Enqueue(new Food(Food.FoodTypes.Hamburger));
            foodLevels.Enqueue(new Food(Food.FoodTypes.Pizza));
            foodLevels.Enqueue(new Food(Food.FoodTypes.IceCream));

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

                    // Update heart
                    UpdateHeart(gameTime);
                    // Update title
                    UpdateTitle(gameTime);

                    switch (playingState)
                    {
                        case PlayingState.Title:
                            if (MediaPlayer.State != MediaState.Playing)
                            {
                                heartstate = 0;
                                MediaPlayer.IsRepeating = true;
                                MediaPlayer.Volume = 1f;
                                MediaPlayer.Play(Assets.Get<Song>("Title"));
                            }
                            // Start when any key is pressed
                            if (gameTime.TotalGameTime.Seconds >= endTimer + 1 &&
                                keyboardState.GetPressedKeys().Length > 0)
                            {
                                playingState = PlayingState.Transition;
                            }
                            break;
                        case PlayingState.Transition:
                            if (currentFood == null)
                            {
                                MediaPlayer.Stop();
                                var vict = Assets.Get<SoundEffect>("Stage Complete");
                                vict.Play();
                                currentFood = foodLevels.Dequeue();
                            }
                            currentFood.anim.Update(gameTime);
                            currentFood.clip = currentFood.anim.CurrentFrame;
                            if (currentFood.anim.IsFinished())
                            {
                                switch (currentFood.foodType)
                                {
                                    case Food.FoodTypes.Banana:
                                        MediaPlayer.Play(Assets.Get<Song>("Swing"));
                                        break;
                                    case Food.FoodTypes.Salad:
                                        MediaPlayer.Play(Assets.Get<Song>("Swing Fast"));
                                        break;
                                    case Food.FoodTypes.Hamburger:
                                        heartstate = 1;
                                        MediaPlayer.Play(Assets.Get<Song>("Chest"));
                                        break;
                                    case Food.FoodTypes.Pizza:
                                        heartstate = 2;
                                        MediaPlayer.Play(Assets.Get<Song>("Chest Fast"));
                                        break;
                                    case Food.FoodTypes.IceCream:
                                        MediaPlayer.Play(Assets.Get<Song>("Disheartening"));
                                        break;
                                }
                                playingState = PlayingState.Level;
                            }
                            break;
                        case PlayingState.Level:
                            Rectangle temp = currentFood.anim.CurrentFrame;
                            float percent = (float) currentFood.numBlocks / (float) currentFood.startBlocks;
                            temp.Width = (int) (currentFood.anim.CurrentFrame.Width * percent);
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
                                            

                                          /*  if (currentFood.foodType == Food.FoodTypes.Salad)
                                            {
                                                player1.setBarNumber(2);
                                                player2.setBarNumber(2);
                                            }*/
                                            currentFood = null;

                                            if (foodLevels.Count == 0)
                                            {
                                                // No more foods in queue... YOU WIN!
                                                currentFood = null;
                                                MediaPlayer.Stop();
                                                heartstate = 0;
                                                var victory = Assets.Get<SoundEffect>("Victory");
                                                victory.Play();
                                                playingState = PlayingState.Victory;
                                                endTimer = gameTime.TotalGameTime.Seconds;
                                                break;
                                            }

                                            playingState = PlayingState.Transition;
                                        }
                                    }
                                }
                            }
                            break;
                        case PlayingState.GameOver:
                            if (gameTime.TotalGameTime.Seconds >= endTimer + 5 &&
                                keyboardState.GetPressedKeys().Length > 0)
                            {
                                board.ClearBoard();
                                score = 0;
                                board.gameLost = false;
                                foodLevels = new Queue<Food>();
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Banana));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Salad));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Hamburger));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Pizza));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.IceCream));
                                endTimer = gameTime.TotalGameTime.Seconds;
                                playingState = PlayingState.Title;
                            }
                            break;
                        case PlayingState.Victory:
                            UpdateVictory(gameTime);
                            if (gameTime.TotalGameTime.Seconds >= endTimer + 5 &&
                                keyboardState.GetPressedKeys().Length > 0)
                            {
                                board.ClearBoard();
                                score = 0;
                                board.gameLost = false;
                                foodLevels = new Queue<Food>();
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Banana));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Salad));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Hamburger));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.Pizza));
                                foodLevels.Enqueue(new Food(Food.FoodTypes.IceCream));
                                playingState = PlayingState.Title;
                                endTimer = gameTime.TotalGameTime.Seconds;
                            }
                            break;
                    }

                    // Update any transform animations; delete them if they're done
                    for (int i = 0; i < board.getAnimationArray().Count; i++)
                    {
                        bool done = board.blockFallUpdate[i].Update(gameTime);
                        if (done)
                        {
                            Block b = ((Block)board.blockFallUpdate[i].GetObject());
                            board.MakeFall((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                            board.blockFallUpdate.Remove(board.blockFallUpdate[i]);
                        }
                    }
                    for (int i = 0; i < animationUpdateArray.Count; i++)
                    {
                        bool done = animationUpdateArray[i].Update(gameTime);
                        if (done)
                        {
                            // If a block has finished animating, make sure it is set to active
                            if (animationUpdateArray[i].GetObject() is Block)
                            {
                                ((Block)animationUpdateArray[i].GetObject()).SetActive(true);
                                var attach = Assets.Get<SoundEffect>("Destroy");
                                attach.Play();
                                
                                // GAME OVER!!!
                                if (board.gameLost)
                                {
                                    endTimer = gameTime.TotalGameTime.Seconds;
                                    currentFood = null;
                                    MediaPlayer.Stop();
                                    heartstate = 3;
                                    var gameover = Assets.Get<SoundEffect>("Game Over");
                                    gameover.Play();
                                    playingState = PlayingState.GameOver;
                                }  
                            }
                            animationUpdateArray.Remove(animationUpdateArray[i]);
                            
                        }
                    }
                    player1.Update();
                    player2.Update();
                    if (player1.position == player2.position)
                    {
                        twoPlayerBlockCollision(player1, player2, board);
                    }
                    else
                    {
                        playerBlockCollision(player1, board);
                        playerBlockCollision(player2, board);
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

                case GameState.Playing:

                    /// DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 40, 40, Color.Black, Color.White);



                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    // Draw heart
                    DrawHeart();
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
                                        color = new Color(255, 255, 0);
                                        break;
                                    }
                                    case Block.BlockColor.Blue:
                                    {
                                        color = new Color(0, 255, 255);
                                        break;
                                    }
                                    case Block.BlockColor.Purple:
                                    {
                                        color = new Color(128, 0, 128);
                                        break;
                                    }
                                    case Block.BlockColor.Red:
                                    {
                                        color = new Color(255, 0, 0);
                                        break;
                                    }
                                    case Block.BlockColor.Brown:
                                    {
                                        color = new Color(180, 85, 0);
                                        break;
                                    }
                                    case Block.BlockColor.Green:
                                    {
                                        color = new Color(12, 255, 0);
                                        break;
                                    }
                                    case Block.BlockColor.Magenta:
                                    {
                                        color = new Color(255, 0, 255);
                                        break;
                                    }
                                    case Block.BlockColor.Orange:
                                    {
                                        color = new Color(255, 180, 0);
                                        break;
                                    }
                                }
                                _spriteBatch.Draw(blockTexture, b[c, r].GetLoc(), color);
                            }
                        }
                    }
                    
                    //players
                    Texture2D barTexture = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 480, 40, Color.White, Color.White);
                    Texture2D playerTexture = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 40, 40, Color.White, Color.White);
                    Texture2D p1Sprite = Assets.Get<Texture2D>("P1");
                    Texture2D p2Sprite = Assets.Get<Texture2D>("P2");
                    
                    Vector2 barPosition = new Vector2(player1.position.X + 40, player1.position.Y);
                    Vector2 endPosition = new Vector2(player1.position.X + 520, player1.position.Y);
                    Vector2 p2barPosition = new Vector2(player2.position.X + 40, player2.position.Y);
                    Vector2 p2endPosition = new Vector2(player2.position.X + 520, player2.position.Y);

                    _spriteBatch.Draw(p1Sprite, player1.position, GetVisualColor(player1.color1));
                    //_spriteBatch.Draw(playerTexture, endPosition, GetVisualColor(player1.color1));
                    _spriteBatch.Draw(barTexture, barPosition, GetVisualColor(player1.color1) * 0.3f);

                    //_spriteBatch.Draw(playerTexture, player2.position, GetVisualColor(player2.color1));
                    _spriteBatch.Draw(p2Sprite, p2endPosition, GetVisualColor(player2.color1));
                    _spriteBatch.Draw(barTexture, p2barPosition, GetVisualColor(player2.color1) * 0.3f);

                    if (player1.barNumber == 2)
                    {
                        Vector2 startPosition = new Vector2(player1.position.X, player1.position.Y + 40);
                        Vector2 barPosition2 = new Vector2(player1.position.X + 40, player1.position.Y + 40);
                        Vector2 endPosition2 = new Vector2(player1.position.X + 520, player1.position.Y + 40);
                        _spriteBatch.Draw(playerTexture, startPosition, GetVisualColor(player1.color2));
                        _spriteBatch.Draw(playerTexture, endPosition2, GetVisualColor(player1.color2));
                        _spriteBatch.Draw(barTexture, barPosition2, GetVisualColor(player1.color2) * 0.5f);

                    }

                    if (player2.barNumber == 2)
                    {
                        Vector2 p2startPosition = new Vector2(player2.position.X, player2.position.Y + 40);
                        Vector2 p2barPosition2 = new Vector2(player2.position.X + 40, player2.position.Y + 40);
                        Vector2 p2endPosition2 = new Vector2(player2.position.X + 520, player2.position.Y + 40);
                        _spriteBatch.Draw(playerTexture, p2startPosition, GetVisualColor(player2.color2));
                        _spriteBatch.Draw(playerTexture, p2endPosition2, GetVisualColor(player2.color2));
                        _spriteBatch.Draw(barTexture, p2barPosition2, GetVisualColor(player2.color2) * 0.5f);
                    }

                    

                    Texture2D greyBack = DrawUtils.CreateFilledRectangle(_graphics.GraphicsDevice, 200, 200, Color.LightGray, Color.LightGray);
                    _spriteBatch.Draw(greyBack, new Vector2(9, 118), Color.White);

                    // Draw food if set
                    if (currentFood != null)
                    {
                        // Draw caution too
                        Texture2D caution = Assets.Get<Texture2D>("CautionG");
                        


                        Texture2D banana = Assets.Get<Texture2D>("Banana");
                        Texture2D salad = Assets.Get<Texture2D>("Salad");
                        Texture2D hamburger = Assets.Get<Texture2D>("Hamburger");
                        Texture2D pizza = Assets.Get<Texture2D>("Pizza");
                        Texture2D icecream = Assets.Get<Texture2D>("Ice Cream");

                        if (currentFood.foodType == Food.FoodTypes.Banana)
                        {
                            _spriteBatch.Draw(banana, new Vector2(9, 118), currentFood.clip, Color.White);
                            caution = Assets.Get<Texture2D>("CautionG");
                        }
                        else if (currentFood.foodType == Food.FoodTypes.Salad)
                        {
                            _spriteBatch.Draw(salad, new Vector2(9, 118), currentFood.clip, Color.White);
                            caution = Assets.Get<Texture2D>("CautionG");
                        }
                        else if (currentFood.foodType == Food.FoodTypes.Hamburger)
                        {
                            caution = Assets.Get<Texture2D>("CautionY");
                            _spriteBatch.Draw(hamburger, new Vector2(9, 118), currentFood.clip, Color.White);
                        }
                        else if (currentFood.foodType == Food.FoodTypes.Pizza)
                        {
                            caution = Assets.Get<Texture2D>("CautionO");
                            _spriteBatch.Draw(pizza, new Vector2(9, 118), currentFood.clip, Color.White);
                        }
                        else if (currentFood.foodType == Food.FoodTypes.IceCream)
                        {
                            caution = Assets.Get<Texture2D>("CautionR");
                            _spriteBatch.Draw(icecream, new Vector2(9, 118), currentFood.clip, Color.White);
                        }

                        _spriteBatch.Draw(caution, new Vector2(9, 55), cautionClip, Color.White);
                        
                    }


                    if (playingState == PlayingState.Title)
                    {
                        DrawTitle();
                    }
                    if (playingState == PlayingState.GameOver)
                    {
                        Texture2D gameover = Assets.Get<Texture2D>("Lose");
                        _spriteBatch.Draw(gameover, new Vector2(276, 176), Color.White);
                    }
                    // Draw victory
                    if (playingState == PlayingState.Victory)
                        DrawVictory();


                    _spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }

        private double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }

        #region Hacky Sprite drawing

        private Animation cautionAnimation = new Animation(2, 1, 200, 64, 2, 1, 0, 0);
        private Rectangle? cautionClip;

        private int heartstate = 0;
        private Animation heartAnimation = new Animation(2, 1, 300, 240, 2, 1, 0, 0);
        private Rectangle? heartClip;
        public void UpdateHeart(GameTime gameTime)
        {
            heartAnimation.Update(gameTime);
            heartClip = heartAnimation.CurrentFrame;
            // Just do the cuation update too
            cautionAnimation.Update(gameTime);
            cautionClip = cautionAnimation.CurrentFrame;
        }
        public void DrawHeart()
        {
            // Draw score too
            var font = Assets.Get<SpriteFont>("Font");
            _spriteBatch.DrawString(font, "SCORE: " + score, new Vector2(9, 350), Color.White);

            switch (heartstate)
            {
                case 0:
                    Texture2D heart = Assets.Get<Texture2D>("Heart Smile");
                    _spriteBatch.Draw(heart, new Vector2(0, 420), heartClip, Color.White);
                    break;
                case 1:
                    Texture2D heartc = Assets.Get<Texture2D>("Heart Concerned");
                    _spriteBatch.Draw(heartc, new Vector2(0, 420), heartClip, Color.White);
                    break;
                case 2:
                    Texture2D hearth = Assets.Get<Texture2D>("Heart Hard");
                    _spriteBatch.Draw(hearth, new Vector2(0, 420), heartClip, Color.White);
                    break;
                case 3:
                    Texture2D heartd = Assets.Get<Texture2D>("Heart Dead");
                    _spriteBatch.Draw(heartd, new Vector2(0, 420), Color.White);
                    break;
            }
            
        }

        private Animation titleAnimation = new Animation(2, 1, 476, 580, 2, 1, 0, 0);
        private Rectangle? titleClip;
        public void UpdateTitle(GameTime gameTime)
        {
            titleAnimation.Update(gameTime);
            titleClip = titleAnimation.CurrentFrame;
        }
        public void DrawTitle()
        {
           
            Texture2D title = Assets.Get<Texture2D>("Start");
            _spriteBatch.Draw(title, new Vector2(262, 44), titleClip, Color.White);
        }

        // Victory animation
        private Animation victoryAnimation = new Animation(5, 0.5, 682, 624, 3, 2, 0, 0);
        private Rectangle? victoryClip;
        public void UpdateVictory(GameTime gameTime)
        {
            victoryAnimation.Update(gameTime);
            victoryClip = victoryAnimation.CurrentFrame;
        }
        public void DrawVictory()
        {
            Texture2D victory = Assets.Get<Texture2D>("Win");
            _spriteBatch.Draw(victory, new Vector2(110, 23), victoryClip, Color.White);
        }


        #endregion


        #region Game Methods


        public void playerBlockCollision(Player player, Board board)
        {
            bool blockDeleted;
            Block[,] blocks = board.GetBoard();
            

            if (player.kill == true)
            {
                List<Block> intersected = new List<Block>();
                blockDeleted = false;

                foreach (Block b in blocks)
                {
                    //for (int cols = 0; cols < blocks.GetLength(0); cols++) {
                    //   for (int rows = 0; rows < blocks.GetLength(1); rows++) {
                    //   Block b = blocks[cols, rows];
                    if (b != null)
                    {
                        Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                        Rectangle bar = new Rectangle((int)player.GetPosition().X + 40, (int)player.GetPosition().Y, 480, 40);
                        
                        if (bar.Intersects(r))
                        {
                            intersected.Add(b);
                            
                            if (player.color1 == (b.GetColor()))
                            {

                                bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                score += 10;
                                var destroy = Assets.Get<SoundEffect>("Attach");
                                destroy.Play();
                                blockDeleted = true;

                            }

                        }
                    }
                }
                if (blockDeleted == false)
                {
                    var error = Assets.Get<SoundEffect>("Error");
                    error.Play();
                    error.Play();
                    foreach (Block b in intersected)
                        b.SetColor(Block.BlockColor.Brown);
                }
            }
        }
        /*
        public void playerBlockCollision(Player player, Board board)
        
            Block[,] blocks = board.GetBoard();

            if (player.kill == true)
            {
                foreach (Block b in blocks)
                {
                //for (int cols = 0; cols < blocks.GetLength(0); cols++) {
                 //   for (int rows = 0; rows < blocks.GetLength(1); rows++) {
                 //   Block b = blocks[cols, rows];
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
                                    var destroy = Assets.Get<SoundEffect>("Attach");
                                    destroy.Play();
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
                                    var destroy = Assets.Get<SoundEffect>("Attach");
                                    destroy.Play();
                                }
                            }
                            if (bar2.Intersects(r))
                            {
                                if (player.color2 == (b.GetColor()))
                                {
                                    bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                    var destroy = Assets.Get<SoundEffect>("Attach");
                                    destroy.Play();
                                }

                            }
                        }
                    }

                }
            } 

        }

        */
        public void twoPlayerBlockCollision(Player player1, Player player2, Board board)
        {
            Block[,] blocks = board.GetBoard();
            bool blockDeleted;

            Rectangle p1bar1 = new Rectangle((int)player1.GetPosition().X + 40, (int)player1.GetPosition().Y, 480, 40);
            //Rectangle p1bar2 = new Rectangle((int)player1.GetPosition().X + 40, (int)player1.GetPosition().Y + 40, 480, 40);
            Rectangle p2bar1 = new Rectangle((int)player2.GetPosition().X + 40, (int)player2.GetPosition().Y, 480, 40);
            //Rectangle p2bar2 = new Rectangle((int)player2.GetPosition().X + 40, (int)player2.GetPosition().Y + 40, 480, 40);

            if (p1bar1.Intersects(p2bar1) && (player1.kill == true || player2.kill == true))
            {
                List<Block> intersected = new List<Block>();
                blockDeleted = false;
                foreach (Block b in blocks)
                {
                    if (b != null)
                    {

                        Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                        if (p1bar1.Intersects(r))
                        {
                            intersected.Add(b);
                            //checks if bar and block colors match
                            if (colorCombo(player1.color1, player2.color1) == (b.GetColor()))
                            {

                                bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                score += 20;
                                var destroy = Assets.Get<SoundEffect>("Attach");
                                destroy.Play();
                                blockDeleted = true;
                                //Console.WriteLine(cleared);
                            }

                        }

                    }
                }
                if (blockDeleted == false)
                {
                    var error = Assets.Get<SoundEffect>("Error");
                    error.Play();
                    foreach (Block b in intersected)
                        b.SetColor(Block.BlockColor.Brown);
                }
            }
        }
                /*
            else if (p1bar1.Intersects(p2bar2) && (player1.kill == true || player2.kill == true))
                {
                    foreach (Block b in blocks)
                    {
                        if (b != null)
                        {
                            Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                            if (p1bar1.Intersects(r))
                            {
                                //checks if bar and block colors match
                                if (colorCombo(player1.color1, player2.color2) == (b.GetColor()))
                                {

                                    bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                    //Console.WriteLine(cleared);
                                }

                            }

                        }
                    }
                }
            else if (p1bar2.Intersects(p2bar2) && (player1.kill == true || player2.kill == true))
            {
                foreach (Block b in blocks)
                {
                    if (b != null)
                    {
                        Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                        if (p1bar2.Intersects(r))
                        {
                            //checks if bar and block colors match
                            if (colorCombo(player1.color2, player2.color2) == (b.GetColor()))
                            {

                                bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                //Console.WriteLine(cleared);
                            }

                        }

                    }
                }
            }
            else if (p1bar2.Intersects(p2bar1) && (player1.kill == true || player2.kill == true))
            {
                foreach (Block b in blocks)
                {
                    if (b != null)
                    {
                        Rectangle r = new Rectangle((int)b.GetLoc().X, (int)b.GetLoc().Y, 40, 40);
                        if (p1bar2.Intersects(r))
                        {
                            //checks if bar and block colors match
                            if (colorCombo(player1.color2, player2.color1) == (b.GetColor()))
                            {

                                bool cleared = board.ClearTile((int)b.getGridLoc().X, (int)b.getGridLoc().Y);
                                //Console.WriteLine(cleared);
                            }

                        }

                    }
                }
            }
        }
            */

        private Block.BlockColor colorCombo(Block.BlockColor color1, Block.BlockColor color2)
        {
            if ((color1 == Block.BlockColor.Yellow && color2 == Block.BlockColor.Red) ||
                (color1 == Block.BlockColor.Red && color2 == Block.BlockColor.Yellow))
            {
                return Block.BlockColor.Orange;
            } 
            else if ((color1 == Block.BlockColor.Yellow && color2 == Block.BlockColor.Blue) ||
                (color1 == Block.BlockColor.Blue && color2 == Block.BlockColor.Yellow))
            {
                return Block.BlockColor.Green;
            }
            else if ((color1 == Block.BlockColor.Purple && color2 == Block.BlockColor.Red) ||
                (color1 == Block.BlockColor.Red && color2 == Block.BlockColor.Purple))
            {
                return Block.BlockColor.Magenta;
            }

            else if ((color1 == Block.BlockColor.Purple && color2 == Block.BlockColor.Blue) ||
                (color1 == Block.BlockColor.Blue && color2 == Block.BlockColor.Purple))
            {
                return Block.BlockColor.Brown;
            }

            else return Block.BlockColor.Black;

        }


        public Color GetVisualColor(Block.BlockColor b)
        {
            if (b.Equals(Block.BlockColor.Yellow))
            {
                return new Color(255, 255, 0);
            }
            else if (b.Equals(Block.BlockColor.Blue))
            {
                return new Color(0, 255, 255);
            }
            else if (b.Equals(Block.BlockColor.Purple))
            {
                return new Color(128, 0, 128);
            }
            else if (b.Equals(Block.BlockColor.Red))
            {
                return new Color(255, 0, 0);
            }
            else if (b.Equals(Block.BlockColor.Green))
            {
                return new Color(12, 255, 0);
            }
            else if (b.Equals(Block.BlockColor.Magenta))
            {
                return new Color(255, 0, 255);
            }
            else if (b.Equals(Block.BlockColor.Orange))
            {
                return new Color(255, 180, 0);
            }
            else return new Color(180, 85, 0);
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
