//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Finite State Machine controlled by an animator.
    /// 
	public sealed class FSMAnimator
	{
        private Animator m_animator = null;
        private List<Animator> m_subAnimators = new List<Animator>();

        /// Container for state callbacks
        /// 
        private sealed class StateCallbacks
        {
            public int m_stateID = 0;
            public Action m_enterState = null;
            public Action m_updateState = null;
            public Action m_exitState = null;
        }

        private int m_nextState = 0;
        private StateCallbacks m_currentState = null;
        private StateEventsBehaviour m_stateBehaviour = null;
        private Dictionary<int, StateCallbacks> m_states = new Dictionary<int, StateCallbacks>();

        #region Public functions
        /// @param animator
        ///     The animator to initialise the FSM with
        /// 
        public void Initialise(Animator animator)
        {
            m_animator = animator;
            if (m_animator != null)
            {
                m_stateBehaviour = m_animator.GetBehaviour<StateEventsBehaviour>();
                if (m_stateBehaviour != null)
                {
                    m_stateBehaviour.OnEnterState += OnEnterStateRequested;
                    m_stateBehaviour.OnExitState += OnExitStateRequested;
                }
                else
                {
                    Debug.LogErrorFormat("There is no StateEventsBehaviour is the FSM Animator on {0}. Please add one.", animator.name);
                }
            }
            else
            {
                Debug.LogError("There is no Animator attached to the view. Please add one.");
            }
        }

        /// @param animator
        ///     The animator to add
        /// 
        public void AddSubAnimator(Animator animator)
        {
            m_subAnimators.Add(animator);
        }

        /// @param animator
        ///     The animator to remove
        /// 
        public void RemoveSubAnimator(Animator animator)
        {
            m_subAnimators.RemoveIfContained(animator);
        }

        /// @param stateName
        ///     The name of the state
        /// @param onEnter
        ///     The enter callback of the state
        /// @param onUpdate
        ///     The update callback of the state
        /// @param onExit
        ///     The exit callback of the state
        /// 
        public void RegisterStateCallback(string stateName, Action onEnter, Action onUpdate, Action onExit)
        {
            int nameHash = Animator.StringToHash(stateName);
            if (m_states.ContainsKey(nameHash) == false)
            {
                m_states.Add(nameHash, new StateCallbacks()
                {
                    m_stateID = nameHash,
                    m_enterState = onEnter,
                    m_updateState = onUpdate,
                    m_exitState = onExit
                });
            }
            else
            {
                Debug.LogErrorFormat("The state {0} already contains a callback definition.", stateName);
            }
        }

        /// @param stateName
        ///     The name of the state
        /// @param onEnter
        ///     The enter callback of the state
        /// @param onUpdate
        ///     The update callback of the state
        /// @param onExit
        ///     The exit callback of the state
        /// 
        public void OverrideStateCallback(string stateName, Action onEnter, Action onUpdate, Action onExit)
        {
            int nameHash = Animator.StringToHash(stateName);
            if (m_states.ContainsKey(nameHash) == true)
            {
                m_states[nameHash] = new StateCallbacks()
                {
                    m_stateID = nameHash,
                    m_enterState = onEnter,
                    m_updateState = onUpdate,
                    m_exitState = onExit
                };
            }
            else
            {
                RegisterStateCallback(stateName, onEnter, onUpdate, onExit);
            }
        }

        /// @param name
        ///     The name of the boolean to set
        /// @param value
        ///     The value to set the bool to
        /// 
        public void SetBool(string name, bool value)
        {
            m_animator.SetBool(name, value);
            foreach(var animator in m_subAnimators)
            {
                animator.SetBool(name, value);
            }
        }

        /// @param name
        ///     The name of the integer to set
        /// @param value
        ///     The value to set the integer to
        /// 
        public void SetInteger(string name, int value)
        {
            m_animator.SetInteger(name, value);
            foreach (var animator in m_subAnimators)
            {
                animator.SetInteger(name, value);
            }
        }

        /// @param name
        ///     The name of the float to set
        /// @param value
        ///     The value to set the float to
        /// 
        public void SetFloat(string name, float value)
        {
            m_animator.SetFloat(name, value);
            foreach (var animator in m_subAnimators)
            {
                animator.SetFloat(name, value);
            }
        }

        /// @param action
        ///     The action to execute
        /// 
        public void ExecuteAction(string action)
        {
            m_animator.SetTrigger(action);
            foreach (var animator in m_subAnimators)
            {
                animator.SetTrigger(action);
            }
        }

        /// @param action
        ///     The action to cancel
        /// 
        public void CancelAction(string action)
        {
            m_animator.ResetTrigger(action);
            foreach (var animator in m_subAnimators)
            {
                animator.ResetTrigger(action);
            }
        }

        /// Update function
        /// 
        public void Update()
        {
            if(m_animator.isActiveAndEnabled && m_currentState != null && m_nextState == 0)
            {
                m_currentState.m_updateState.SafeInvoke();
            }
        }
        #endregion

        #region Private functions
        /// @param stateNameHash
        ///     The name hash of the entered state
        ///     
        private void OnEnterStateRequested(int stateNameHash)
        {
            if(m_states.ContainsKey(stateNameHash) == true)
            {
                if (m_currentState != null)
                {
                    // Entering a state before leaving the current one
                    // So we'll wait for the state to exit
                    m_nextState = stateNameHash;
                }
                else
                {
                    EnterState(stateNameHash);
                }
            }
        }

        /// @param stateNameHash
        ///     The name hash of the exited state
        ///     
        private void OnExitStateRequested(int stateNameHash)
        {
            if (m_currentState != null && m_currentState.m_stateID == stateNameHash)
            {
                var callback = m_currentState.m_exitState;
                m_currentState = null;
                callback.SafeInvoke();

                // Check if there is a next state request
                if (m_nextState != 0)
                {
                    EnterState(m_nextState);
                    m_nextState = 0;
                }
            }
        }

        /// @param stateNameHash
        ///     The name hash to enter
        ///     
        private void EnterState(int stateNameHash)
        {
            m_currentState = m_states[stateNameHash];
            m_currentState.m_enterState.SafeInvoke();
        }
        #endregion
    }
}
