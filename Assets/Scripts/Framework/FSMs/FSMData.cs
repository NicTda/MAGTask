//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
	/// Describes a state table and a transition table of a FSM
    /// 
	public abstract class FSMData
	{
		public static readonly int k_undefinedState = -1;
		public static readonly int k_undefinedAction = -1;

        /// Contains the data that defines a transition
        /// 
		public class TransitionInfo
		{
			public int m_fromState = k_undefinedState;
			public int m_toState = k_undefinedState;
			public int m_actionID = k_undefinedAction;

			/// Constructor
            /// 
			/// @param actionID
			///     the ID of the action
			/// @param fromStateID
			///     the ID of the state where to apply the action
			/// @param toStateID
            ///     the ID of the destination state
            /// 
			public TransitionInfo(int actionID, int fromStateID, int toStateID)
			{
				m_actionID = actionID;
				Debug.Assert(m_actionID != k_undefinedAction, string.Format("Undefined action ID for action {0}", m_actionID));

				m_fromState = fromStateID;
				m_toState = toStateID;
			}
		}

		private Dictionary<int, string> m_states = new Dictionary<int, string>();
		private List<TransitionInfo> m_transitions = new List<TransitionInfo>();

		#region Public functions
		/// @return The newly created instance of the data
        /// 
		public static T CreateInstance<T>() where T : FSMData, new()
		{
			return new T();
		}

		/// Adds a new transition to the FSM data
		/// 
		/// @param actionEnum
		/// 	The enum of the action
		/// @param fromStateEnum
		///     the enum of the state where to apply the action
		/// @param toStateEnum
		///     the enum of the destination state
		/// 
		public void AddTransitionInfo<ActionType, StateType>(ActionType actionEnum, StateType fromStateEnum, StateType toStateEnum) where StateType : struct, System.IConvertible 
																																	where ActionType : struct, System.IConvertible
		{
			int fromStateID = System.Convert.ToInt32(fromStateEnum);
			AddState(fromStateID, fromStateEnum.ToString());

			int toStateID = System.Convert.ToInt32(toStateEnum);
			AddState(toStateID, toStateEnum.ToString());

			AddTransitionInfo(System.Convert.ToInt32(actionEnum), fromStateID, toStateID);
		}

        /// @return the number of states
        /// 
		public int GetStatesCount()
		{
			return m_states.Count;
		}

		/// @param actionID
        ///     the ID of the action to execute
		/// @param fromStateID
        ///     the ID of the state where to apply the action
		/// 
        /// @return the reference to the transition if existing, null otherwise
        /// 
		public TransitionInfo GetTransition(int actionID, int fromStateID)
		{
			Debug.Assert(m_states.ContainsKey(fromStateID), string.Format("The FSM data doesn't contain a state '{0}'", fromStateID));

            return m_transitions.Find(item => (item.m_actionID == actionID) && (item.m_fromState == fromStateID));
		}

		/// @param stateID
		///     ID of the state to find
		/// 
		/// @return Whether the state exists or not
		/// 
		public bool HasState(int stateID)
		{
			return m_states.ContainsKey(stateID);
		}

		/// @param stateName
		///     Name of the state
		/// 
		/// @return The ID of the state
		/// 
		public int GetStateID(string stateName)
		{
			int stateID = k_undefinedState;
			foreach(var pair in m_states)
			{
				if(pair.Value == stateName)
				{
					stateID = pair.Key;
					break;
				}
			}
			return stateID;
		}

		/// @param stateID
		///     ID of the state
		/// 
		/// @return The name of the state
		/// 
		public string GetStateName(int stateID)
		{
			return m_states.GetValueOrDefault(stateID, string.Empty);
		}
		#endregion

		#region Private functions
		/// Adds a new state to the FSM data
		/// 
		/// @param stateID
		///     The ID of the state
		/// @param stateName
		/// 	The name of the state
		/// 
		private void AddState(int stateID, string stateName)
		{
			if(m_states.ContainsKey(stateID) == false)
			{
				m_states.Add(stateID, stateName);
			}
		}

        /// Adds a new transition to the FSM data
        /// 
        /// @param actionID
        ///     the ID of the action
        /// @param fromStateID
        ///     the ID of the state where to apply the action
        /// @param toStateID
        ///     the ID of the destination state
        /// 
        private void AddTransitionInfo(int actionID, int fromStateID, int toStateID)
		{
			Debug.Assert(m_states.ContainsKey(fromStateID), string.Format("The FSM data doesn't contain a state with id {0}", fromStateID));
			Debug.Assert(m_states.ContainsKey(toStateID), string.Format("The FSM data doesn't contain a state with id {0}", toStateID));

			TransitionInfo transition = GetTransition(actionID, fromStateID);
			Debug.Assert(transition == null, string.Format("The FSM already contains a transition from state {0} with action {1}", fromStateID, actionID));

			if(transition == null)
			{
				m_transitions.Add(new TransitionInfo(actionID, fromStateID, toStateID));
			}
		}
		#endregion
	}
}
