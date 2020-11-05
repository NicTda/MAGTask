//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;

namespace CoreFramework
{
    /// FSM view for a scene
    ///
    public abstract class SceneFSMView : FSMView
    {
        public event Action OnBackRequested;

        #region Public functions
        /// Called when the back button has been tapped
        ///
        public void OnBackTapped()
        {
            OnBackRequested.SafeInvoke();
        }
        #endregion
    }
}
