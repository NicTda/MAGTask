//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
	/// Base class for a factory
    /// 
	public abstract class Factory
	{
        protected Director m_ownerDirector = null;

        #region Public functions
        /// Used to complete the initialisation
        /// 
        /// @param director
        ///     The Director owner of the instance
        /// 
        public void CompleteInitialisation(Director director)
        {
            m_ownerDirector = director;

            OnCompleteInitialisation();
        }
        #endregion

        #region Protected functions
        /// Callback for when the factory is being initialised
        /// 
        protected virtual void OnCompleteInitialisation() { }
        #endregion
	}
}
