//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Service used to update all controllers of a scene,
    /// This should be added as a local service and not as a global service.
    /// When it is unregistered it guarantees that all controllers not yet
    /// disposed are correctly destroyed
    /// 
    public sealed class ControllerService : Service
    {
        private List<Icontroller> m_controllers = null;
        private bool m_active = true;

        #region Public functions
        /// Constructor
        /// 
        public ControllerService()
        {
            m_controllers = new List<Icontroller>();
        }

        /// Sets the state of the update loop
        /// 
        /// @param state
        ///     Desired active state
        /// 
        public void SetUpdateState(bool state)
        {
            m_active = state;   
        }

        /// Subscribes a controller
        /// 
        /// @param controller
        ///     The controller to subscribe
        /// 
        public void SubscribeController(Icontroller controller)
		{
            Debug.Assert(controller != null, "Controller is null");
			Debug.Assert(m_controllers.Contains(controller) == false, "ControllerService already contains controller");

            if((controller != null) && (m_controllers.Contains(controller) == false))
			{
				m_controllers.Add(controller);
			}
		}

		/// Unsubscribes a controller
        /// 
		/// @param controller
        ///     The controller to unsubscribe
        /// 
        public void UnsubscribeController(Icontroller controller)
		{
			Debug.Assert(controller != null, "Controller is null");
			Debug.Assert(m_controllers.Contains(controller), "ControllerService does not contain controller to remove");

            if((controller != null) && (m_controllers.Contains(controller) == true))
			{
				m_controllers.Remove(controller);
			}
		}
        #endregion

        #region Service functions
        /// Updates the state of the service
        /// 
        public override sealed void ServiceUpdate()
        {
            if((m_ownerDirector.IsReady() == true) && (m_active == true))
            {
                for(int index = 0; index < m_controllers.Count; ++index)
                {
                    m_controllers[index].Update();
                }
            }
        }

        /// Used to perform actions before being unregistered
        /// 
        public override sealed void OnUnregister()
        {
            List<Icontroller> controllersToDispose = new List<Icontroller>(m_controllers);
            for(int controller = 0; controller < controllersToDispose.Count; ++controller)
            {
                controllersToDispose[controller].Dispose();
            }
            m_controllers.Clear();
        }
        #endregion
    }
}
