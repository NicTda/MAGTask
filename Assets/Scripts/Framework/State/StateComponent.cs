//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
	/// Component that handles states
	/// 
	public class StateComponent : MonoBehaviour
	{
		public static readonly string k_defaultState = "none";

		public delegate void StateChangedDelegate(GameObject obj, string oldState, string newState);
		public event StateChangedDelegate OnStateChangedEvent;

		[SerializeField]
		private string m_currentState = k_defaultState;

		/// Awake function
		/// 
		private void Awake()
		{
			// Making sure states are lower case
			m_currentState = m_currentState.ToLower();
		}

		/// @return The current state of the object
		/// 
		public string GetCurrentState()
		{
			return m_currentState;
		}

		/// Setter for the current state
		/// 
		/// @param state
		/// 	The new state to apply to the object
		/// 
		public void SetState(string state)
		{
			string newState = state.ToLower();
			if(newState != m_currentState)
			{
				var oldState = m_currentState;
				m_currentState = newState;

				if(OnStateChangedEvent != null)
				{
					OnStateChangedEvent(gameObject, oldState, newState);
				}
			}
		}
	}
}
