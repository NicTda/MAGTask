//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Info button to give the player some guidance.
    /// It can display multiple popups in a row.
    /// 
    public sealed class InfoButtonComponent : MonoBehaviour
    {
        public string m_popupType = PopupIdentifiers.k_popupInfo;
        public string m_headerCategory = LocalisedTextIdentifiers.k_categoryGame;
        public string m_headerID = string.Empty;
        public string m_bodyCategory = LocalisedTextIdentifiers.k_categoryGame;
        public List<string> m_infoBodies = new List<string>();

        private int m_currentIndex = 0;

        #region Public functions
        /// Called when the player presses the info button
        /// 
        public void OnPressed()
        {
            GlobalDirector.Service<AudioService>().PlaySFX(AudioIdentifiers.k_buttonPressed);
            Trigger();
        }

        /// @param [optional] callback
        ///     The function to call when all popups are dismissed
        /// 
        public void Trigger(Action callback = null)
        {
            DisplayNextPopup(callback);
        }
        #endregion

        #region Private functions
        /// @param callback
        ///     The function to call when all popups are dismissed
        /// 
        private void DisplayNextPopup(Action callback)
        {
            if ((m_currentIndex == 0) || (m_currentIndex < m_infoBodies.Count))
            {
                var popupView = GlobalDirector.Service<PopupService>().QueuePopup(m_popupType);
                popupView.SetHeaderText(m_headerID.Localise(m_headerCategory));

                if(m_currentIndex < m_infoBodies.Count)
                {
                    popupView.SetBodyText(m_infoBodies[m_currentIndex].Localise(m_bodyCategory));
                }

                popupView.OnPopupDismissed += (popup) =>
                {
                    DisplayNextPopup(callback);
                };

                ++m_currentIndex;
            }
            else
            {
                m_currentIndex = 0;
                callback.SafeInvoke();
            }
        }
        #endregion
    }
}
