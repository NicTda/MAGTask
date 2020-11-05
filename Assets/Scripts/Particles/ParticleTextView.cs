//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;
using TMPro;

namespace MAGTask
{
    /// View component for a short lived text display
    ///
    public sealed class ParticleTextView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_text = null;

        #region Public functions
        /// @param text
        ///     The text to set
        ///
        public void SafeText(string text)
        {
            m_text.SafeText(text);
        }

        /// @param colour
        ///     The colour to set
        ///
        public void SetColour(Color colour)
        {
            if(m_text != null)
            {
                m_text.color = colour;
            }
        }

        /// Self destruct
        ///
        public void OnAnimationDone()
        {
            GameObject.Destroy(gameObject);
        }
        #endregion
    }
}
