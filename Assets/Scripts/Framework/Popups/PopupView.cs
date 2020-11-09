//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreFramework
{
    /// This is the base class of all popup views
    /// 
    public class PopupView : MonoBehaviour
    {
        public Action<PopupView> OnPopupPresented;
        public Action<PopupView> OnPopupDismissed;

        [HideInInspector]
        public bool m_layerable = false;
        [HideInInspector]
        public bool m_coverable = true;
        [HideInInspector]
        public bool m_priority = false;
        [HideInInspector]
        public bool m_toast = false;

        public static string s_sfxConfirmPressed = AudioIdentifiers.k_buttonPressed;
        public static string s_sfxBackPressed = AudioIdentifiers.k_buttonPressed;
        public static string s_sfxPresent = string.Empty;
        public static string s_sfxDismiss = string.Empty;

        [SerializeField]
        private Button m_confirmButton = null;
        [SerializeField]
        private TextMeshProUGUI m_headerText = null;
        [SerializeField]
        protected TextMeshProUGUI m_bodyText = null;
        [SerializeField]
        private TextMeshProUGUI m_confirmButtonText = null;

        protected AudioService m_audioService = null;
        protected InputService m_inputService = null;
        protected LocalisationService m_localisationService = null;
        protected string m_sfxPresent = s_sfxPresent;
        protected string m_sfxDismiss = s_sfxDismiss;
        protected Coroutine m_coroutine = null;

        private bool m_backButtonEnabled = true;

        #region Unity functions
        /// OnDestoy function
        /// 
        protected virtual void OnDestoy()
        {
            m_coroutine.Stop();
        }
        #endregion

        #region Public functions
        /// Initialise function
        /// 
        public void Initialise()
        {
            m_audioService = GlobalDirector.Service<AudioService>();
            m_inputService = GlobalDirector.Service<InputService>();
            m_localisationService = GlobalDirector.Service<LocalisationService>();

            InitialiseInternal();
        }

        /// @param text
        ///     The header text to set
        ///     
        public void SetHeaderText(string text)
        {
            m_headerText.SafeText(text);
        }

        /// @param textID
        ///     The header text ID to set
        ///     
        public void SetLocalisedHeaderText(string textID)
        {
            SetHeaderText(m_localisationService.GetText(textID));
        }

        /// @param text
        ///     The body text to set
        /// 
        public virtual void SetBodyText(string text)
        {
            m_bodyText.SafeText(text);
        }

        /// @param text
        ///     The body text to set
        /// @param list
        ///     The params to use to fill the text
        /// 
        public void SetBodyText(string text, params object[] list)
        {
            var filledText = string.Format(text, list);
            m_bodyText.SafeText(filledText);
        }

        /// @param textID
        ///     The header text ID to set
        ///     
        public void SetLocalisedBodyText(string textID)
        {
            SetBodyText(m_localisationService.GetText(textID));
        }

        /// @param textID
        ///     The header text ID to set
        /// @param list
        ///     The params to use to fill the text
        ///     
        public void SetLocalisedBodyText(string textID, params object[] list)
        {
            SetBodyText(m_localisationService.GetText(textID), list);
        }

        /// @param text
        ///     The button text to set
        ///     
        public void SetConfirmButtonText(string text)
        {
            m_confirmButtonText.SafeText(text);
        }

        /// @param text
        ///     The button text to set
        /// @param list
        ///     The params to use to fill the text
        ///     
        public void SetConfirmButtonText(string text, params object[] list)
        {
            m_confirmButtonText.SafeText(text, list);
        }

        /// @param textID
        ///     The button text ID to set
        ///     
        public void SetLocalisedConfirmButtonText(string textID)
        {
            SetConfirmButtonText(m_localisationService.GetText(textID));
        }

        /// @param textID
        ///     The button text ID to set
        /// @param list
        ///     The params to use to fill the text
        ///     
        public void SetLocalisedConfirmButtonText(string textID, params object[] list)
        {
            SetConfirmButtonText(m_localisationService.GetText(textID), list);
        }

        /// @param sfx
        ///     The sfx to play when presenting
        ///     
        public void SetPresentSFX(string sfx)
        {
            m_sfxPresent = sfx;
        }

        /// @param sfx
        ///     The sfx to play when dismissing
        ///     
        public void SetDismissSFX(string sfx)
        {
            m_sfxDismiss = sfx;
        }

        /// Start the presenting process
        /// 
        public void RequestPresent()
        {
            m_coroutine = GlobalDirector.ExecuteCoroutine(Present(() =>
            {
                if (m_backButtonEnabled == true)
                {
                    m_inputService.AddBackButtonListener(OnBackPressed);
                }
            }));
        }

        /// Start the dismissing process
        /// 
        public void RequestDismiss()
        {
            DisableBackButton();
            m_coroutine = GlobalDirector.ExecuteCoroutine(Dismiss());
        }

        /// Enable the interactable parts of the popup
        /// 
        public void Enable()
        {
            if (m_confirmButton != null)
            {
                m_confirmButton.interactable = true;
            }

            EnableInternal();
        }

        /// Disable the interactable parts of the popup
        /// 
        public void Disable()
        {
            if(m_confirmButton != null)
            {
                m_confirmButton.interactable = false;
            }

            DisableInternal();
        }

        /// Instantly hides the popup
        /// 
        public void Hide()
        {
            gameObject.SafeSetActive(false);
        }

        /// Instantly shows the popup
        /// 
        public void Show()
        {
            gameObject.SafeSetActive(true);
        }

        /// Called when the confirm button is pressed
        /// 
        public void OnConfirmButtonPressed()
        {
            m_audioService.PlaySFX(s_sfxConfirmPressed);
            RequestDismiss();
        }

        /// Disables the back button for this popup
        /// 
        public void DisableBackButton()
        {
            m_backButtonEnabled = false;
            m_inputService.RemoveBackButtonListener(OnBackPressed);
        }

        /// Enables the back button for this popup
        /// 
        public void EnableBackButton()
        {
            m_backButtonEnabled = true;
        }
        #endregion

        #region Protected functions
        /// Use to create a sequence for presenting this popup
        /// After this coroutines completion the popup will be "presented"
        /// 
        protected virtual IEnumerator Presenting()
        {
            yield return null;
            m_audioService.PlaySFX(m_sfxPresent);
        }

        /// Use to create a sequence for dismissing this popup
        /// After this coroutines completion the popup will be dismissed
        /// 
        protected virtual IEnumerator Dismissing()
        {
            // Instant dismiss by default
            m_audioService.PlaySFX(m_sfxDismiss);
            yield return null;
        }

        /// Initialise function
        /// 
        protected virtual void InitialiseInternal() { }

        /// Enable the interactable parts of the popup
        /// 
        protected virtual void EnableInternal() { }

        /// Disable the interactable parts of the popup
        /// 
        protected virtual void DisableInternal() { }

        /// Called when the popup is dismissed
        /// 
        protected virtual void OnDismissedInternal() { }

        /// Called when the back button is pressed
        /// 
        protected virtual void OnBackPressed()
        {
            m_audioService.PlaySFX(s_sfxBackPressed);
            RequestDismiss();
        }
        #endregion

        #region Private functions
        /// Present this popup after the Presenting coroutine has completed
        /// 
        private IEnumerator Present(Action callback)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);

                yield return StartCoroutine(Presenting());

                callback.SafeInvoke();
                OnPopupPresented.SafeInvoke(this);
            }
        }

        /// Dismiss & destroy this popup after the Dismissing coroutine has completed
        /// 
        private IEnumerator Dismiss()
        {
            if (gameObject.activeInHierarchy == true)
            {
                Disable();

                yield return StartCoroutine(Dismissing());

                OnDismissedInternal();
                OnPopupDismissed.SafeInvoke(this);
                Hide();
            }

            GlobalDirector.Service<PopupService>().RemovePopup(this);
            Destroy(gameObject);
        }
        #endregion
    }
}
