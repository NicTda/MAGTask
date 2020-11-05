//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System.Collections.Generic;
using UnityEngine;

namespace MAGTask
{
    /// Collection of handy functions, mainly to retrieve sprites
    /// 
    public static class GameUtils
    {
        public static readonly Color k_transparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        private const string k_currencyPath = "Images/Currencies/{0}";

        #region Public functions
        /// @param currencyID
        ///     The ID of the currency
        ///     
        /// @return The currency sprite
        /// 
        public static Sprite GetCurrencySprite(string currencyID)
        {
            return GetSprite(string.Format(k_currencyPath, currencyID));
        }

        /// @param costs
        ///     The costs to check
        ///     
        /// @return Whether all costs are free
        /// 
        public static bool IsFree(this List<CurrencyItem> costs)
        {
            bool isFree = true;
            foreach(var cost in costs)
            {
                isFree &= cost.IsFree();
            }
            return isFree;
        }

        /// @param spritePath
        ///     The sprite path
        /// 
        /// @return The loaded sprite
        /// 
        public static Sprite GetSprite(string spritePath)
        {
            return Resources.Load<Sprite>(spritePath);
        }
        #endregion

        #region Private functions
        /// @param pathFormat
        ///     The sprite path
        /// @param spriteID
        ///     The sprite ID
        /// 
        /// @return The loaded sprite
        /// 
        private static Sprite GetSprite(string pathFormat, string spriteID)
        {
            return GetSprite(string.Format(pathFormat, spriteID));
        }
        #endregion
    }
}
