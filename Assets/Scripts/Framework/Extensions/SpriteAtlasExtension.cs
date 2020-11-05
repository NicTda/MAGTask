//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace CoreFramework
{
    /// Extension class for text
    ///
    public static class SpriteAtlasExtension
    {
        private const string k_cloneString = "(Clone)";

        #region Public functions
        /// @return The sprites list, without (Clone) in their name
        /// 
        public static List<Sprite> GetSpritesList(this SpriteAtlas spriteAtlas)
        {
            List<Sprite> sprites = new List<Sprite>(spriteAtlas.spriteCount);

            Sprite[] spritesArray = new Sprite[spriteAtlas.spriteCount];
            spriteAtlas.GetSprites(spritesArray);

            foreach (var sprite in spritesArray)
            {
                // (Clone) is added by Unity when retrieving the sprites list, which is annoying
                sprite.name = sprite.name.Replace(k_cloneString, string.Empty);
                sprites.Add(sprite);
            }

            return sprites;
        }
        #endregion
    }
}
