//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreFramework
{
    /// This view implements a simple yes/no popup with a confirm and cancel button
    /// 
    public class PopupYesNoView : PopupView
    {
        public event Action<bool> OnPopupDecided;
        public event Action OnPopupConfirmed;
        public event Action OnPopupCancelled;

        [SerializeField]
        private Button m_cancelButton = null;
        [SerializeField]
        private TextMeshProUGUI m_cancelButtonText = null;

        private bool m_confirmed = true;

        #region PopupView functions
        /// Initialise function
        /// 
        protected override void InitialiseInternal()
        {
            if(m_cancelButtonText != null)
            {
                m_cancelButtonText.text = m_localisationService.GetGameText(LocalisedTextIdentifiers.k_cancelButton);
            }
        }

        /// Disable the interactable parts of the popup
        /// 
        protected override void DisableInternal()
        {
            if (m_cancelButton != null)
            {
                m_cancelButton.interactable = false;
            }
        }

        /// Called when the popup is dismissed
        /// 
        protected override void OnDismissedInternal()
        {
            OnPopupDecided.SafeInvoke(m_confirmed);
            if (m_confirmed == true)
            {
                OnPopupConfirmed.SafeInvoke();
            }
            else
            {
                OnPopupCancelled.SafeInvoke();
            }
        }

        /// Called when the back button is pressed
        /// 
        protected override void OnBackPressed()
        {
            OnCancelButtonPressed();
        }
        #endregion

        #region Public functions
        /// @param text
        ///     The button text to set
        ///     
        public void SetCancelButtonText(string text)
        {
            m_cancelButtonText.SafeText(text);
        }

        /// @param textID
        ///     The button text ID to set
        ///     
        public void SetLocalisedCancelButtonText(string textID)
        {
            SetCancelButtonText(m_localisationService.GetText(textID));
        }

        /// Called when the cancel button is pressed
        /// 
        public void OnCancelButtonPressed()
        {
            m_confirmed = false;
            m_audioService.PlaySFX(s_sfxBackPressed);
            RequestDismiss();
        }
        #endregion
    }
}
