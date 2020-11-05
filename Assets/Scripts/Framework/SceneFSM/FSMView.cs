//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// FSM view using an animator
    ///
    public abstract class FSMView : MonoBehaviour
    {
        public Animator FSMAnimator { get { return m_fsmAnimator; } }

        [SerializeField]
        protected Animator m_fsmAnimator = null;

        #region Public functions
        /// Initialises the view
        ///
        public virtual void Initialise()
        {
            InitialiseInternal();
        }
        #endregion

        #region Protected functions
        /// Initialises the view
        ///
        protected virtual void InitialiseInternal()
        {
        }
        #endregion
    }
}
