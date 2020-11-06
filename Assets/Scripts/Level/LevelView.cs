//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using UnityEngine;

namespace MAGTask
{
    /// View component for the Level scene
    ///
    public sealed class LevelView : SceneFSMView
    {
        public event Action OnInteractStarted;
        public event Action OnInteractEnded;

        public Transform TilesHolder { get { return m_tilesHolder; } }

        [SerializeField]
        private Transform m_tilesHolder = null;

        #region Public functions
        /// Called when the player starts interacting with the board
        /// 
        public void OnInteractStart()
        {
            OnInteractStarted.SafeInvoke();
        }

        /// Called when the player ends interacting with the board
        /// 
        public void OnInteractEnd()
        {
            OnInteractEnded.SafeInvoke();
        }
        #endregion
    }
}
