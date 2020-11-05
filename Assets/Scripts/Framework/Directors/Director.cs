//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
	/// A Director (scene director) is a MonoBehaviour that has
	/// the responsibility to prepare the scene execution. It uses a FSM
	/// to create and initialise all services that are needed to run 
    /// the logic
    /// 
	public abstract class Director : MonoBehaviour, System.IDisposable
	{
		/// FSM for the Director class.
        /// 
		protected class DirectorFSM : FSMData
		{
			public enum State
			{
                Waiting,
                AddingServices,
                Initialised
			}

			public enum Action
			{
                AddServices,
                Ready
			}

            /// Constructor
            /// 
			public DirectorFSM()
			{
				// transitions table
                AddTransitionInfo(Action.AddServices, State.Waiting, State.AddingServices);
                AddTransitionInfo(Action.Ready, State.AddingServices, State.Initialised);
            }
		}

		protected bool m_disposed = false;

		protected ServiceSupplier m_serviceSupplier = null;
        protected FactorySupplier m_factorySupplier = null;
		protected FSM<DirectorFSM> m_fsm = null;

        #region Unity functions
        /// Awake
        /// 
        private void Awake()
		{
            // Initialises the instance creating the ServiceSupplier and the FSM
            m_disposed = false;

            if(OnDirectorAwake())
            {
				m_serviceSupplier = new ServiceSupplier();
	            m_factorySupplier = new FactorySupplier();
				m_fsm = new FSM<DirectorFSM>();
				
				m_fsm.AddStateBinding(DirectorFSM.State.Waiting, null, UpdateWaitingState, null);
				m_fsm.AddStateBinding(DirectorFSM.State.AddingServices, StartAddServicesState, UpdateAddServicesState, null);
				m_fsm.AddStateBinding(DirectorFSM.State.Initialised, StartInitialisedState, null, null);
                m_fsm.Start(DirectorFSM.State.Waiting);
            }
		}

        /// OnDestroy
        /// 
		private void OnDestroy()
		{
            Dispose();
		}

		/// Update
        /// 
		private void Update()
		{
            if(m_disposed == false)
			{
				m_serviceSupplier.Update();
				m_fsm.Update();
			}
		}

		/// @param paused
        ///     If this application has been paused or unpaused
        /// 
        private void OnApplicationPause(bool paused)
		{
			// This method is called on the first frame of life of the app
			// (with paused == false) so it shouldn't be called before the director is ready
			if(IsReady() == true)
			{
				if(paused == true)
				{
					m_serviceSupplier.OnApplicationPause();
					OnDirectorPause();
				}
				else
				{
					m_serviceSupplier.OnApplicationResume();
					OnDirectorResume();
				}
			}
        }
        #endregion

        #region IDisposable functions
        /// Destroys the instance disposing the ServiceSupplier
        /// 
        public void Dispose()
		{
            if(m_disposed == false)
			{
				OnDirectorDispose();

				m_serviceSupplier.Dispose();
				m_fsm.Stop();

				m_disposed = true;
			}
		}
        #endregion

        #region Public functions
        /// @return the instance of the requested service if any, null otherwise
        /// 
        public T GetService<T>() where T : Service
		{
			return m_serviceSupplier.GetService<T>();
		}

        /// @return the instance of the requested factory if any, null otherwise
        /// 
        public T GetFactory<T>() where T : Factory
        {
            return m_factorySupplier.GetFactory<T>();
        }

        /// @return whether the Director is ready to be used or not
        /// 
		public bool IsReady()
		{
            bool result = false;

            if(m_fsm != null)
            {
                result = ((m_disposed == false) && m_fsm.IsInState(DirectorFSM.State.Initialised));
            }
			return result;
		}
        #endregion

        #region Protected functions
        /// Called on Awake. Used to initialise the inherited Director
        /// 
        /// @return Whether the Director initialised correctly and should continue
        /// 
        protected abstract bool OnDirectorAwake();

        /// Called on OnDestroy. Used to disposed the inherited Director
        /// 
		protected abstract void OnDirectorDispose();

        /// Called when the application is paused
        /// 
		protected virtual void OnDirectorPause() {}

        /// Called when the application is resumed
        /// 
		protected virtual void OnDirectorResume() { }

        /// This is used to delay the start of a local director
        /// 
        protected virtual void UpdateWaitingState()
        {
            m_fsm.ExecuteAction(DirectorFSM.Action.AddServices);
        }

        /// Once all services are ready it switches to the next state
        /// 
        protected virtual void UpdateAddServicesState()
        {
            if(m_serviceSupplier.AllServicesReady())
            {
                m_fsm.ExecuteAction(DirectorFSM.Action.Ready);
            }
        }

        /// Requests to the inherited Director to register its own services
        /// 
		protected abstract void OnStartAddServicesState();

		/// Starts the initialised State
		/// 
		protected virtual void StartInitialisedState() { }
		#endregion

		#region Private functions
		/// Asks the inherited Director to register its own services & factories
		/// A two step initialisation is needed because some services could
        /// depend from other ones
        /// 
		private void StartAddServicesState()
		{
			OnStartAddServicesState();
			m_serviceSupplier.InitialiseAllServices(this);
            m_factorySupplier.InitialiseAllFactories(this);
		}
		#endregion
	}
}
