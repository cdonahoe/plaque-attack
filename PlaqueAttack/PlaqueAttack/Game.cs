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
        public const int WINDOW_HEIGHT = 600;

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
        private Texture2D rect;
        private TransformAnimation rectAnimation;
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            IsMouseVisible = true;

            _state = GameState.Playing;

            Window.Title = "Plaque Attack";

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
                        _state = GameState.TitleScreen;

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

                case GameState.TitleScreen:

                    var state = Mouse.GetState();

                    if (state.LeftButton == ButtonState.Pressed)
                    {
                        _state = GameState.Playing;
                    }

                    break;

                case GameState.Playing:
                               
                    break;
            }

            base.Update(gameTime);
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

                    
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

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
