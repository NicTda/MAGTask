//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Local director for the game
    /// 
    public sealed class StartupLocalDirector : LocalDirector
    {
        [SerializeField]
        private StartupView m_view = null;

        #region StartupLocalDirector functions
        /// Entry point for the initialised state
        /// 
        protected override void StartInitialisedState()
        {
            // Make the game run as fast as possible
            Application.targetFrameRate = 60;

            // Kickstart the game's logic
            new StartupController(this, m_view);
        }
        #endregion
    }
}