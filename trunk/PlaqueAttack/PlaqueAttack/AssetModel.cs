using System;

namespace PlaqueAttack
{
    /// <summary>
    /// Delegate describing a function that will perform a small bit of 
    /// loading for the game.
    /// </summary>
    delegate void AssetLoad();

    /// <summary>
    /// Describes a single asset during the load process. Either contains 
    /// a key, a content location and type to be loaded or contains a
    /// delegate method to call.
    /// </summary>
    class AssetModel
    {
        /// <summary>
        /// Create an asset which is described by a content location, a key, and the 
        /// type of content stored.
        /// </summary>
        /// <param name="key">String based key for later retrieval.</param>
        /// <param name="loc">Location of the content relative to the 
        ///     content manager's root directory.</param>
        /// <param name="type">Type of the content, ex. Texture2D, SoundEffect, Model.</param>
        public AssetModel(string key, string loc, Type type)
        {
            Key = key;
            Loc = loc;
            Type = type;
        }

        /// <summary>
        /// Create an asset which is described by one method to call during the loading process.
        /// </summary>
        /// <param name="loader">AssetLoad delegate method.</param>
        public AssetModel(AssetLoad loader)
        {
            Call = loader;
            Type = typeof (Delegate);    
        }

        /// <summary>
        /// String based key for later retrieval.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Location of the content relative to the content manager's root directory.
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// Type of the content, ex. Texture2D, SoundEffect, Model.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// <c>AssetLoad</c> delegate method.
        /// </summary>
        public AssetLoad Call { get; set; }
    }
}
