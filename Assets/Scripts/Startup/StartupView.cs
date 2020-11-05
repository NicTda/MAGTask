//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;

namespace MAGTask
{
    /// View component for the Startup scene
    ///
    public sealed class StartupView : SceneFSMView
    {
        public event Action OnOutroFinished;

        #region Public functions
        /// Flag the outro as done
        /// 
        public void TriggerOutroDone()
        {
            OnOutroFinished.SafeInvoke();
        }
        #endregion
    }
}
