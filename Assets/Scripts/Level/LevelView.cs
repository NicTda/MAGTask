//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;

namespace MAGTask
{
    /// View component for the Level scene
    ///
    public sealed class LevelView : SceneFSMView
    {
        public event Action OnPlayRequested;

        #region Public functions
        /// Called when the player presses the Play button
        /// 
        public void OnPlayPressed()
        {
            OnPlayRequested.SafeInvoke();
        }
        #endregion
    }
}
