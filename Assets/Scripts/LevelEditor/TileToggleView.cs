//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MAGTask
{
    /// View component for the colour toggles in LevelEditor scene
    ///
    public sealed class TileToggleView : MonoBehaviour
    {
        public event Action<TileColour, bool> OnToggled;

        [SerializeField]
        private Toggle m_toggle = null;
        [SerializeField]
        private TMP_Text m_tileText = null;

        private TileColour m_tileColour = TileColour.None;

        #region Public functions
        /// @param tileColour
        ///     The tile colour
        /// @param text
        ///     The text to set
        /// 
        public void SetTileColour(TileColour tileColour, string text)
        {
            m_tileColour = tileColour;
            m_tileText.SafeText(text);
        }

        /// @param isOn
        ///     Whether the toggle should be on
        /// 
        public void SetOn(bool isOn)
        {
            m_toggle.isOn = isOn;
        }

        /// @param isOn
        ///     Whether the toggle is on
        /// 
        public void OnChanged(bool isOn)
        {
            OnToggled.SafeInvoke(m_tileColour, isOn);
        }
        #endregion
    }
}
