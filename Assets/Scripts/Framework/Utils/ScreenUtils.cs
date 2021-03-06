﻿//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
	/// A set of utility methods relating to the screen
	///
	public static class ScreenUtils
	{
#if UNITY_EDITOR
        public readonly static Vector2 k_center = new Vector2(Screen.currentResolution.width * 0.5f, Screen.currentResolution.height * 0.5f);
        public readonly static Rect k_rect = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height);
#else
        public readonly static Vector2 k_center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        public readonly static Rect k_rect = new Rect(0f, 0f, Screen.width, Screen.height);
#endif

        /// @return The current screen's aspect ratio
        /// 
        public static float GetAspectRatio()
        {
            return (float)Screen.width / (float)Screen.height;
        }

        /// @return The current screen's aspect ratio
        /// 
        public static ScreenOrientation GetOrientation()
        {
            return (Screen.width >= Screen.height) ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;
        }
    }
}
