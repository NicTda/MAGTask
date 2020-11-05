//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Extension class for the Coroutine class
    /// 
	public static class CoroutineExtension
    {
        #region Public functions
        /// Stops the coroutine
        /// 
        public static void Stop(this Coroutine coroutine)
        {
            if (GlobalDirector.HasInstance() == true)
            {
                GlobalDirector.CancelCoroutine(coroutine);
            }
        }
        #endregion
    }
}
