//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// Utils for Device
    /// 
	public static class DeviceUtils
    {
        public static readonly string k_keyStoreAmazon = "Amazon";
        public static readonly string k_keyStoreGoogle = "Google";
        public static readonly string k_keyStoreiOS = "iOS";
        public const string k_emptyIDFA = "00000000-0000-0000-0000-000000000000";

        #region Public functions
        /// @return Whether the device is a kindle
        /// 
        public static bool IsKindle()
        {
            return SystemInfo.deviceModel.Contains(k_keyStoreAmazon);
        }

        /// @param callback
        ///     The function to call when the advertising ID is retrieved
        ///     
        public static void RetrieveAdvertisingID(Action<string> callback)
        {
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            bool success = Application.RequestAdvertisingIdentifierAsync((string advertisingId, bool trackingEnabled, string error) =>
            {
                if (string.IsNullOrEmpty(advertisingId) == false && advertisingId != k_emptyIDFA)
                {
                    deviceID = advertisingId;
                }
                callback.SafeInvoke(deviceID);
            });

            if (success == false)
            {
                callback.SafeInvoke(deviceID);
            }
        }
        #endregion
    }
}
