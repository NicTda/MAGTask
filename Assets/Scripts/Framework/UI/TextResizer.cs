//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using TMPro;
using UnityEngine;

namespace CoreFramework
{
    /// Component that will adjust the font point sizes depending on the device's ratio
    ///
    [ExecuteInEditMode]
    public sealed class TextResizer : MonoBehaviour
    {
        [SerializeField]
        private float m_autoSizeMin = 10.0f;
        [SerializeField]
        private float m_autoSizeMax = 20.0f;
        [SerializeField]
        private float m_referenceAspectRatio = 0.5625f;
        [SerializeField]
        private TextMeshProUGUI m_textComponent = null;

#if UNITY_EDITOR
        [SerializeField]
        private bool m_updateEditor = false;
#endif

        #region Unity functions
        /// Awake function
        ///
        private void Awake()
        {
            if (m_textComponent == null)
            {
                m_textComponent = GetComponent<TextMeshProUGUI>();
                UpdateTextSize();
            }
        }

#if UNITY_EDITOR
        /// Update the UI in editor mode
        /// 
        private void OnGUI()
        {
            if (m_updateEditor == true)
            {
                UpdateTextSize();
            }
        }
#endif
        #endregion

        #region Private functions
        /// Updates the text component size
        /// 
        private void UpdateTextSize()
        {
            if (m_textComponent != null)
            {
                // Calculate the wanted sizes
                var aspectRatio = ScreenUtils.GetAspectRatio();
                m_textComponent.fontSizeMin = (m_autoSizeMin * m_referenceAspectRatio) / aspectRatio;
                m_textComponent.fontSizeMax = (m_autoSizeMax * m_referenceAspectRatio) / aspectRatio;
            }
        }
        #endregion
    }
}
