//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;

namespace MAGTask
{
    /// View component for the Main scene
    ///
    public sealed class MainView : SceneFSMView
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
