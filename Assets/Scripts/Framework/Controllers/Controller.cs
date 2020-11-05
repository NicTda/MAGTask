//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    /// Base class to inherit to implement a logic controller in the game.
    /// 
    public abstract class Controller : Icontroller, System.IDisposable
    {
        public LocalDirector m_localDirector { get; private set; }

        private bool m_disposed = false;

        #region Public functions
        /// Disposes the state of the controller
        /// 
		public void Dispose()
        {
            if(m_disposed == false)
            {
                OnDispose();

                m_localDirector.GetService<ControllerService>().UnsubscribeController(this);
                m_disposed = true;
            }
        }

        /// Implement this method if the child class needs to be disposed
        /// 
		public virtual void OnDispose() { }

        /// Update function
        /// 
        public virtual void Update() { }
        #endregion

        #region Protected functions
        /// Constructor
        /// 
        /// @param localDirector
        ///     The local director owner of the Controller
        /// 
        protected Controller(LocalDirector localDirector)
		{
			m_localDirector = localDirector;
            if (m_localDirector != null)
            {
                m_localDirector.GetService<ControllerService>().SubscribeController(this);
            }
		}
        #endregion
    }
}
