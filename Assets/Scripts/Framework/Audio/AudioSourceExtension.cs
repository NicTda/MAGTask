//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Extension class for AudioSource
    ///
    public static class AudioSourceExtension
    {
        #region Public functions
        /// @param audioSource
        /// 	The audioSource to stop
        ///
        public static void SafeStop(this AudioSource audioSource)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        #endregion
    }
}
