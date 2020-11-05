//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Implementation of a Finite State Machine
    /// The generic type T is the data that describes the FSM.
    /// 
    /// Before starting using it the user must register, initialise and
    /// update delegates for each state defined in the descriptor
    /// 
    public class FSM<T> where T : FSMData, new()
	{
        public event Action OnStateChanged;

        /// Stores references to initialise and update delegates for a state
        /// 
		protected class StateBinding
		{
			public int m_stateID = FSMData.k_undefinedState;
			public Action m_enterStateFunc = null;
			public Action m_updateStateFunc = null;
            public Action m_exitStateFunc = null;

			/// Constructor
			/// 
			/// @param stateID
			///     The ID of the state
			/// @param enterStateFunc
			///     The enter delegate of the state
			/// @param updateStateFunc
			///     The update delegate of the state
            /// @param exitStateFunc
            ///     The exit delegate of the state
			/// 
            public StateBinding(int stateID, Action enterStateFunc, Action updateStateFunc, Action exitStateFunc)
			{
				Debug.Assert(stateID != FSMData.k_undefinedState, string.Format("Undefined state ID"));

				m_stateID = stateID;
				m_enterStateFunc = enterStateFunc;
				m_updateStateFunc = updateStateFunc;
                m_exitStateFunc = exitStateFunc;
			}
		}

		protected FSMData m_fsmData = null;
		protected StateBinding m_currentState = null;

		private Dictionary<int, StateBinding> m_stateBindings = new Dictionary<int, StateBinding>();

		#region Public functions
		/// Constructor
        /// 
        public FSM()
		{
			m_fsmData = FSMData.CreateInstance<T>();
		}

		/// Adds a new state to the FSM
		/// 
		/// @param stateEnum
		///     The enum of the state
		/// @param enterStateFunc
		///     The enter delegate of the state
		/// @param updateStateFunc
		///     The update delegate of the state
		/// @param exitStateFunc
		///     The exit delegate of the state
		/// 
		public void AddStateBinding<StateType>(StateType stateEnum, Action enterStateFunc, Action updateStateFunc, Action exitStateFunc) where StateType : struct, IConvertible
		{
			AddStateBinding(Convert.ToInt32(stateEnum), stateEnum.ToString(), enterStateFunc, updateStateFunc, exitStateFunc);
		}

		/// Asks to the FSM to execute an action
		/// 
		/// @param actionEnum
		///     The enum of the action to execute
		/// 
		public void ExecuteAction<ActionType>(ActionType actionEnum) where ActionType : struct, IConvertible
		{
			ExecuteAction(Convert.ToInt32(actionEnum), actionEnum.ToString());
		}

		/// Starts the FSM
		/// NOTE: this method will immediately execute the initialiser of the starting state
		/// 
		/// @param stateEnum
		///     The enum of the starting state
		/// 
		public void Start<StateType>(StateType stateEnum) where StateType : struct, IConvertible
		{
			Start(Convert.ToInt32(stateEnum));
		}

		/// Starts the FSM
		/// NOTE: this method will immediately execute the initialiser of the starting state
		/// 
		/// @param [optional] startingStateID
		/// 	The state to starting in. Defaults to 0.
		/// 
		public void Start(int startingStateID = 0)
		{
            Debug.Assert(m_fsmData.GetStatesCount() == m_stateBindings.Count, string.Format("The FSM is not completely initialised, some bindings are missing"));
            Debug.Assert(HasStarted() == false, "The FSM has been already initialised, you cannnot call the Start method more than once!");
			Debug.Assert(startingStateID != FSMData.k_undefinedState, "Undefined starting state for this FSM");

			ChangeState(startingStateID);
		}

		/// Stops the FSM
		/// 
		public void Stop()
		{
			m_currentState = null;
		}

        /// Updates the FSM
        /// 
		public void Update()
		{
			if(HasStarted())
			{
				m_currentState.m_updateStateFunc.SafeInvoke();
			}
		}

		/// @return Whether the FSM has started or not
		/// 
		public bool HasStarted()
		{
			return (m_currentState != null);
		}

		/// @return The ID of the current state
		/// 
		public int GetCurrentState()
		{
			return HasStarted() ? m_currentState.m_stateID : FSMData.k_undefinedState;
		}

		/// @return The name of the current state
		/// 
		public string GetCurrentStateName()
		{
			return m_fsmData.GetStateName(GetCurrentState());
		}

		/// @param stateEnum
		/// 	The enum of the state to check
		/// 
		/// @return Whether the FSM is in the given state
		/// 
		public bool IsInState<StateType>(StateType stateEnum) where StateType : struct, IConvertible
		{
			return IsInState(Convert.ToInt32(stateEnum));
		}
		#endregion

		#region Protected functions
		/// Performs the state transition
		/// 
		/// @param nextStateID
		///     The ID of the next state
		/// 
		protected virtual void ChangeState(int nextStateID)
		{
			StateBinding nextState = m_stateBindings.TryGetValue(nextStateID);
			Debug.Assert(nextState != null, string.Format("Cannot find the desired state with id {0}", nextStateID));

			if(m_currentState != null)
			{
				m_currentState.m_exitStateFunc.SafeInvoke();
			}

			m_currentState = nextState;
            OnStateChanged.SafeInvoke();
            m_currentState.m_enterStateFunc.SafeInvoke();
		}
		#endregion

		#region Private functions
		/// Adds a new state to the FSM
		/// 
		/// @param stateID
		///     The ID of the state
		/// @param stateName
		/// 	The name of the state
		/// @param enterStateFunc
		///     The initialise delegate of the state
		/// @param updateStateFunc
		///     The update delegate of the state
		/// @param exitStateFunc
		///     The exit delegate of the state
		/// 
		private void AddStateBinding(int stateID, string stateName, Action enterStateFunc, Action updateStateFunc, Action exitStateFunc)
		{
			Debug.Assert(m_fsmData.HasState(stateID), string.Format("FSM data doesn't contain a state with name {0}", stateName));

			Debug.Assert(m_stateBindings.ContainsKey(stateID) == false, string.Format("The FSM already contains a binding to the state {0}", stateName));
			if(m_stateBindings.ContainsKey(stateID) == false)
			{
				m_stateBindings.Add(stateID, new StateBinding(stateID, enterStateFunc, updateStateFunc, exitStateFunc));
			}
		}

		/// Asks to the FSM to execute an action
		/// 
		/// @param actionID
		///     The ID of the action to execute
		/// @param actionName
		///     The name of the action to execute
		/// 
		private void ExecuteAction(int actionID, string actionName)
		{
			Debug.Assert(HasStarted(), string.Format("The FSM is not initialised"));

			if(HasStarted() == true)
			{
				FSMData.TransitionInfo transition = m_fsmData.GetTransition(actionID, m_currentState.m_stateID);
				Debug.Assert(transition != null, string.Format("There are no transitions from state {0} for the action '{1}'", m_fsmData.GetStateName(m_currentState.m_stateID), actionName));

				if(transition != null)
				{
					ChangeState(transition.m_toState);
				}
			}
		}

		/// @param stateID
		/// 	The ID of the state to check
		/// 
		/// @return whether the FSM is in the given state
		/// 
		private bool IsInState(int stateID)
		{
			return (m_currentState.m_stateID == stateID);
		}
		#endregion
	}
}
