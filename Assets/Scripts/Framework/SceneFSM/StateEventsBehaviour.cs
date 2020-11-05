//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// State machine behaviour for states
    ///
    public sealed class StateEventsBehaviour : StateMachineBehaviour
    {
        public event Action<int> OnEnterState;
        public event Action<int> OnUpdateState;
        public event Action<int> OnExitState;

        #region StateMachineBehaviour functions
        /// @param animator
        ///     The animator
        /// @param stateInfo
        ///     The info of the state
        /// @param layerIndex
        ///     The index of the layer
        ///
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnEnterState.SafeInvoke(stateInfo.shortNameHash);
        }

        /// @param animator
        ///     The animator
        /// @param stateInfo
        ///     The info of the state
        /// @param layerIndex
        ///     The index of the layer
        ///
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnUpdateState.SafeInvoke(stateInfo.shortNameHash);
        }

        /// @param animator
        ///     The animator
        /// @param stateInfo
        ///     The info of the state
        /// @param layerIndex
        ///     The index of the layer
        ///
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnExitState.SafeInvoke(stateInfo.shortNameHash);
        }
        #endregion
    }
}
