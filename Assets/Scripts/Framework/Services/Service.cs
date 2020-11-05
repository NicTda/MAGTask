//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;

namespace CoreFramework
{
    /// Abstract class used to implement the behaviour of a Service
    /// 
	public abstract class Service
	{
		protected Director m_ownerDirector = null;

        private event Action OnServiceReady;
        private bool m_ready = false;

        #region Public functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        /// @param director
        ///     The Director owner of the instance
        /// 
        public void CompleteInitialisation(Director director)
		{
			m_ownerDirector = director;
			OnCompleteInitialisation();
		}

		/// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
		public virtual void OnCompleteInitialisation() { }

        /// Used to perform actions before being unregistered
        /// 
		public virtual void OnUnregister() {}

        /// Updates the state of the service
        /// 
		public virtual void ServiceUpdate() {}

        /// Called when the application is paused
        /// 
		public virtual void OnApplicationPause() {}

        /// Called when the application is resumed
        /// 
        public virtual void OnApplicationResume() { }

        /// @return The base type to register for this service. Null otherwise.
        /// 
        public virtual Type GetBaseType() { return null; }

        /// @return whether the service is ready to be used or not
        /// 
		public virtual bool IsServiceReady() { return true; }

        /// @param callback
        ///     The function to call when the service is ready. 
        ///     Will be called immediately if it already is
        /// 
        public void CallWhenReady(Action callback)
        {
            if(m_ready == true)
            {
                callback.SafeInvoke();
            }
            else
            {
                OnServiceReady += callback;
            }
        }

        /// Mark the service as ready
        /// 
		public void TriggerReady()
        {
            if(m_ready == false)
            {
                m_ready = true;
                OnServiceReady.SafeInvoke();
            }
        }
        #endregion
    }
}
