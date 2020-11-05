//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;

namespace CoreFramework
{
    /// Component that listens to events on animators
    ///
    public class AnimationListener : GameBehaviour
    {
        public event Action<string> OnEventReceived;

        #region GameBehaviour functions
        /// The Initialise function
        ///
        protected override void Initialise()
        {
        }
        #endregion

        #region Public functions
        /// @param param
        ///     The param received from the event
        ///
        public virtual void TriggerEvent(string param)
        {
            OnEventReceived.SafeInvoke(param);
        }
        #endregion
    }
}
