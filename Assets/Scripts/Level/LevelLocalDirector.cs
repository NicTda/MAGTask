//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Local director for the Level scene
    /// 
    public sealed class LevelLocalDirector : LocalDirector
    {
        [SerializeField]
        private LevelView m_view = null;

        #region LocalDirector functions
        /// Entry point for the initialised state
        /// 
        protected override void StartInitialisedState()
        {
            // Make the game run as fast as possible
            Application.targetFrameRate = 60;

            // Kickstart the game's logic
            new LevelController(this, m_view);
        }
        #endregion
    }
}