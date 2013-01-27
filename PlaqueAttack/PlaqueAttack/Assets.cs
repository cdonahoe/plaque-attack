using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace PlaqueAttack
{
    /// <summary>
    /// Asset helper class, which supports loading game content in small
    /// chunks, then retrieving it from a cache later.
    /// </summary>
    static class Assets
    {
        /// <summary>
        /// List of assets, the first asset in the list is guaranteed to
        /// load before the body of any Update or Draw call is made.
        /// </summary>
        public static readonly List<AssetModel> AssetList = new List<AssetModel>
        {
           new AssetModel(FirstLoad),
           new AssetModel("Title", "Music/Title - Heartbeat", typeof(Song)),
           new AssetModel("Swing", "Music/Stage 1 - Heartfelt Swing", typeof(Song)),
           new AssetModel("Swing Fast", "Music/Stage 2 - Heartfelt Swing (Faster)", typeof(Song)),
           new AssetModel("Chest", "Music/Stage 3 - Pounding in My Chest", typeof(Song)),
           new AssetModel("Chest Fast", "Music/Stage 4 - Pounding in My Chest (Faster)", typeof(Song)),
           new AssetModel("Disheartening", "Music/Stage 5 - Disheartening", typeof(Song)),
           // new AssetModel("Small font", "Fonts/Small", typeof(SpriteFont)),
           new AssetModel("Artery", "Images/arteryBackground", typeof(Texture2D)),
           new AssetModel("Banana", "Images/bananaFadeIn", typeof(Texture2D)),
           new AssetModel("Salad", "Images/saladFadeIn", typeof(Texture2D)),
           new AssetModel("Hamburger", "Images/burgerFadeIn", typeof(Texture2D)),
           new AssetModel("Pizza", "Images/pizzaFadeIn", typeof(Texture2D)),
           new AssetModel("Ice Cream", "Images/icecreamFadeIn", typeof(Texture2D)),
           new AssetModel("Block", "Images/squareCircle", typeof(Texture2D)),
           new AssetModel("Heart Concerned", "Images/heart/heartpumpConcerned", typeof(Texture2D)),
           new AssetModel("Heart Dead", "Images/heart/heartpumpDead", typeof(Texture2D)),
           new AssetModel("Heart Hard", "Images/heart/heartpumpHard", typeof(Texture2D)),
           new AssetModel("Heart Smile", "Images/heart/heartpumpSmile", typeof(Texture2D)),
           new AssetModel("Start", "Images/title", typeof(Texture2D)),
           new AssetModel("Lose", "Images/gameOver", typeof(Texture2D)),
           new AssetModel("Win", "Images/victorySpritesheet", typeof(Texture2D)),

           new AssetModel("Attach", "SFX/Brick Attach", typeof(SoundEffect)),
           new AssetModel("Destroy", "SFX/Brick Destroy", typeof(SoundEffect)),
           new AssetModel("Danger", "SFX/Danger Sound", typeof(SoundEffect)),
           new AssetModel("Game Over", "SFX/Game Over", typeof(SoundEffect)),
           new AssetModel("Victory", "SFX/Game Victory", typeof(SoundEffect)),
           new AssetModel("Stage Complete", "SFX/Stage Victory", typeof(SoundEffect)),
           new AssetModel("Warning", "SFX/Warning Sound", typeof(SoundEffect)),
        };

        /// <summary>
        /// Index storing which asset we are currently loading.
        /// </summary>
        private static int _index;

        /// <summary>
        /// Map between asset keys and their loaded data.
        /// </summary>
        private static readonly Dictionary<string, object> Data 
            = new Dictionary<string, object>();

        /// <summary>
        /// Return the percent of assets which have been loaded as an integer 0 to 100.
        /// </summary>
        public static int PercentLoaded
        {
            get { return (AssetList.Count == 0) ? 100 : (100 * _index) / AssetList.Count; }
        }

        /// <summary>
        /// Return true if all loading is complete.
        /// </summary>
        public static bool Loaded
        {
            get { return _index >= AssetList.Count; }
        }

        /// <summary>
        /// Load the next asset in the list then returns, advancing the index by one step.
        ///
        /// Currently supports:
        ///     Texture2D
        ///     Model
        ///     SpriteFont
        ///     Song
        ///     SoundEffect
        ///     Effect
        ///     Video
        ///     Delegate (calls a custom worker method)
        /// </summary>
        /// <returns>True if all assets have been loaded and false otherwise.</returns>
        public static bool LoadOne()
        {
            if (Loaded) return true;

            var nextAsset = AssetList[_index];
            if (nextAsset.Type == typeof(Texture2D))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<Texture2D>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(Model))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<Model>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(SpriteFont))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<SpriteFont>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(SoundEffect))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<SoundEffect>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(Song))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<Song>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(Effect))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<Effect>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(Video))
            {
                Data[nextAsset.Key] = Game.GetGame().Content.Load<Video>(nextAsset.Loc);
            }
            else if (nextAsset.Type == typeof(Delegate))
            {
                nextAsset.Call();
            }

            _index++;
            return false;
        }

        /// <summary>
        /// Return an asset by its key.
        /// </summary>
        /// <typeparam name="T">The type of this asset.</typeparam>
        /// <param name="key">The string-based key of the asset.</param>
        /// <returns>The asset object representing this key.</returns>
        public static T Get<T>(string key)
        {
            Debug.Assert(Contains(key), "Asset with key " + key + " not found.");

            return (T) Data[key];
        }

        /// <summary>
        /// Return whether an asset is loaded with the given key.
        /// </summary>
        /// <param name="key">The string-based key of the asset.</param>
        /// <returns>True if an asset of this key has been loaded.</returns>
        public static bool Contains(string key)
        {
            return Data.ContainsKey(key);
        }

        /// <summary>
        /// This method does any additional loading required before the first screen is
        /// presented to the user. It loads the assets needed to display a splash screen.
        /// </summary>
        private static void FirstLoad()
        {
        }

        /*
        /// <summary>
        /// Loads XACT, which holds our sound effects.
        /// </summary>
        private static void LoadSoundEffects()
        {
            Game.GetGame().LoadXact();
        }
        */
    }
}
