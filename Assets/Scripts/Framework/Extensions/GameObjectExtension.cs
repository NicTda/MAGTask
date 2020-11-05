//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Extension class for the GameObject class
    /// 
	public static class GameObjectExtension
    {
        #region Public functions
        /// @param active
        ///     Whether to set the game object active or not
        ///
        public static void SafeSetActive(this GameObject gameObject, bool active)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(active);
            }
        }

        /// Toggles the object on or off
        ///
        public static void SafeToggleActive(this GameObject gameObject)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
        }

        ///	@param gameObjects
        ///		The list of game object to activate / deactivate
        ///	@param active
        ///		Whether to activate or deactivate
        ///
        public static void SetActive(this List<GameObject> gameObjects, bool active)
        {
            if (gameObjects != null)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.SafeSetActive(active);
                }
            }
        }
        #endregion
    }
}
