//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Component that will initialise when the Global Director is ready
    ///
    public abstract class GameBehaviour : MonoBehaviour
    {
        #region Unity functions
        /// Awake function
        ///
        private void Awake()
        {
            GlobalDirector.CallWhenReady(Initialise);
        }
        #endregion

        #region Protected functions
        /// Initialise function
        ///
        protected abstract void Initialise();
        #endregion
    }
}
