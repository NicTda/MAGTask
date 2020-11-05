//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Component that guarantees to be on the Safe Area of the device.
    /// This is full screen for all devices, except for devices with Notch.
    /// Attach this component to a full screen UI holder under your Canvas.
    ///
    public sealed class SafeAreaView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_rectTransform = null;

        private Rect m_lastSafeArea;

        #region Unity functions
        /// Awake function
        ///
        private void Awake()
        {
            if(m_rectTransform == null)
            {
                m_rectTransform = GetComponent<RectTransform>();
            }

            // Default to full screen Rect so device without Notch aren't affected
            m_lastSafeArea = ScreenUtils.k_rect;

            Refresh();
        }
        #endregion

        #region Public functions
        /// Refreshes the UI
        ///
        public void Refresh()
        {
            ApplySafeArea(Screen.safeArea);
        }
        #endregion

        #region Private functions
        /// @param safeAreaRect
        ///     The safe area rect to apply
        ///
        private void ApplySafeArea(Rect safeAreaRect)
        {
            if (safeAreaRect != m_lastSafeArea)
            {
                m_lastSafeArea = safeAreaRect;

                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 anchorMin = safeAreaRect.position;
                Vector2 anchorMax = safeAreaRect.position + safeAreaRect.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;
                m_rectTransform.anchorMin = anchorMin;
                m_rectTransform.anchorMax = anchorMax;
            }
        }
        #endregion
    }
}
