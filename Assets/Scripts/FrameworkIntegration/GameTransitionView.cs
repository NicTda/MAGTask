//
// Copyright (c) 2020 Nicolas Tanda Ltd. All rights reserved
//

using CoreFramework;
using System.Collections;
using UnityEngine;

namespace MAGTask
{
    /// Transition between scenes
    ///
    public sealed class GameTransitionView : SceneTransitionView
    {
        private const string k_triggerShow = "Show";
        private const string k_triggerHide = "Hide";

		[SerializeField]
		private Animator m_animator = null;

        private bool m_continue = true;

        #region SceneTransitionView functions
        /// Coroutine to handle the 'showing' sequence of this transition UI
        /// 
        public override IEnumerator Showing()
        {
            m_continue = false;
            m_animator.SetTrigger(k_triggerShow);

            while(m_continue == false)
            {
                yield return null;
            }
        }

        /// Coroutine to handle the 'hiding' sequence of this transition UI
        /// 
        public override IEnumerator Hiding()
        {
            m_continue = false;
            m_animator.SetTrigger(k_triggerHide);

            while (m_continue == false)
            {
                yield return null;
            }
        }

        /// Called when the transition animation is done
        /// 
        public void OnContinue()
        {
            m_continue = true;
        }
        #endregion
	}
}
