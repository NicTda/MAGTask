//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
	/// Handles the de/activation of objects that depend on this component's game object state
	/// 
	[RequireComponent(typeof(StateComponent))]
	public sealed class StateObjectActivatorComponent : MonoBehaviour
	{
		[SerializeField]
		private StateComponent m_stateComponent = null;
		[SerializeField]
		private List<StateObjectsContainer> m_stateContainers = new List<StateObjectsContainer>();

		#region Unity functions
		/// Awake function
		///
		private void Awake()
		{
			if(m_stateComponent == null)
			{
				m_stateComponent = gameObject.GetComponent<StateComponent>();
				Debug.Assert(m_stateComponent != null, "StateComponent could not be found on object " + name);
			}

			m_stateComponent.OnStateChangedEvent += OnStateChanged;
		}
		
		/// Start function
		///
		private void Start()
		{
			ActivateObjectsForState(m_stateComponent.GetCurrentState());
		}
		#endregion

		#region Private functions
		///	Callback for when the state of the game object has changed
		///
		///	@param stateGameObject
		///		Game object for which the state has changed
		///	@param oldState
		///		The old state 
		///	@param newState
		///		The new state
		///
		private void OnStateChanged(GameObject stateGameObject, string oldState, string newState)
		{
			ActivateObjectsForState(newState);
		}

		///	@param state
		///		State for which to activate objects
		///
		private void ActivateObjectsForState(string state)
		{
			foreach(var stateContainer in m_stateContainers)
			{
				if(stateContainer.m_state != state)
				{
					// Deactivate irrelevant states
					SetObjectsActive(stateContainer, !stateContainer.m_show);
				}
			}
			
			foreach(var stateContainer in m_stateContainers)
            {
                if (stateContainer.m_state == state)
                {
                    // Activate relevant states
                    SetObjectsActive(stateContainer, stateContainer.m_show);
                }
            }
		}

		///	@param objectContainer
		///		Container of objects we want to activate
		///	@param active
		///		Whether to activate or deactivate
		///
		private void SetObjectsActive(StateObjectsContainer objectContainer, bool active)
		{
            foreach(GameObject obj in objectContainer.m_objects)
			{
				obj.SafeSetActive(active);
			}
		}
		#endregion
	}
}
