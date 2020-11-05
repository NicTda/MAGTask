//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreFramework
{
    [ExecuteInEditMode]
    /// Component that will set the given resolution, depending on the aspect ratio of the screen.
    /// Also works in editor, when the user changes the aspect ratio, for rapid iteration.
    /// 
    public sealed class ResolutionSetter : MonoBehaviour
    {
        [SerializeField]
        private Vector2 m_resolutionPortrait = new Vector2(1080.0f, 1920.0f);
        [SerializeField]
        private Vector2 m_resolutionLandscape = new Vector2(1440.0f, 1080.0f);
        [SerializeField]
        private List<CanvasScaler> m_canvasScalers = new List<CanvasScaler>();
        [SerializeField]
        private List<RectTransform> m_transforms = new List<RectTransform>();

        #region Unity functions
#if UNITY_EDITOR
        /// Update the UI in editor mode
        /// 
        private void OnGUI()
        {
            // This will change the resolution depending on the aspect ratio of the screen
            ApplyResolution();
        }
#endif

        /// Awake function
        /// 
        private void Awake()
        {
            ApplyResolution();
        }
        #endregion

        #region Private functions
        /// Apply the correct resolution
        /// 
        private void ApplyResolution()
        {
            var resolution = (ScreenUtils.GetOrientation() == ScreenOrientation.Landscape) ? m_resolutionLandscape : m_resolutionPortrait;

            foreach (var transformItem in m_transforms)
            {
                transformItem.sizeDelta = resolution;
            }
            foreach (var canvasScaler in m_canvasScalers)
            {
                canvasScaler.referenceResolution = resolution;
            }
        }
        #endregion
    }
}
