//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
	/// Abstract class used to define the basic behaviour of a scene director.
	/// An implementation of a Director will create, initialise
    /// and update all entities into a scene
    /// 
	public abstract class LocalDirector : Director
	{
        protected ControllerService m_controllerService { get; private set; }

		#region Director functions
		/// A LocalDirector should not perform any action on Awake
        /// 
        /// @return Whether the Director initialised correctly
        /// 
		protected sealed override bool OnDirectorAwake()
        {
            return true;
        }

        /// This is used to delay the start of a local director
        /// 
        protected override sealed void UpdateWaitingState()
        {
            if(GlobalDirector.IsGlobalDirectorReady() == true)
            {
                m_fsm.ExecuteAction(DirectorFSM.Action.AddServices);
            }
        }

        /// Registers the ControllerService and asks to the inherited LocalDirector
        /// to register its own services
        /// 
        protected override sealed void OnStartAddServicesState()
		{
            m_controllerService = m_serviceSupplier.RegisterService<ControllerService>();
			OnRegisteringLocalServices();
		}
        #endregion

        #region Public functions
        /// Sets the update state of the controller service
        /// 
        /// @param state
        ///     Desired state
        /// 
        public void SetUpdateState(bool state)
        {
            m_controllerService.SetUpdateState(state);
        }
		#endregion

		#region Protected functions
		/// A LocalDirector should not perform any action on Dispose
		/// 
		protected override void OnDirectorDispose() { }
		
		/// Called on Awake. Used to initialise the inherited Director
		/// 
        protected virtual void OnRegisteringLocalServices() { }
		#endregion
	}
}
