//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    /// Extension class for State components
    ///
    public static class StateExtension
	{
        #region Public functions
        /// @param stateComponent
        ///     The state component to modify
        /// @param state
        ///     The state to safely set
        ///
        public static void SafeState(this StateComponent stateComponent, string state)
        {
            if (stateComponent != null)
            {
                stateComponent.SetState(state);
            }
        }
        #endregion
	}
}
