//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using TMPro;
using UnityEngine;

namespace CoreFramework
{
    /// Assign localised text to a TMP component at run time.
    ///
    public class LocalisedTextUI : MonoBehaviour
    {
        [SerializeField]
        protected string m_textKey = string.Empty;
        [SerializeField]
        protected string m_category = LocalisedTextIdentifiers.k_categoryGame;
        [SerializeField]
        private TMP_Text m_textComponent = null;

        private LocalisationService m_localisationService = null;

        #region Unity functions        	 
        /// Awake function
        ///
        private void Awake()
        {
            if (string.IsNullOrEmpty(m_category) == true)
            {
                m_category = LocalisedTextIdentifiers.k_categoryGame;
            }

            if (m_textComponent == null)
            {
                m_textComponent = GetComponent<TMP_Text>();
            }
            Debug.Assert(m_textComponent != null, string.Format("Could not find TextMeshPro component on '{0}", transform.name));

            GlobalDirector.CallWhenReady(OnDirectorReady);
        }

        /// OnDestroy function
        ///
        protected void OnDestroy()
        {
            if(m_localisationService != null)
            {
                m_localisationService.OnLanguageChanged -= OnLanguageChanged;
            }
        }
        #endregion

        #region Public functions
        ///	@param key
        ///		The key of the localised text to set
        ///
        public void SetTextKey(string key)
        {
            m_textKey = key;

            if(string.IsNullOrEmpty(m_textKey) == false)
            {
                // Set localised text
                var text = m_localisationService.GetText(m_category, m_textKey);
                SetText(text);
            }
        }

        ///	@param key
        ///		The key of the localised text to set
        /// @oaram args
        ///     The format arguments
        ///
        public void SetTextKey(string key, params object[] args)
        {
            m_textKey = key;

            if (string.IsNullOrEmpty(m_textKey) == false)
            {
                // Set localised text
                var text = string.Format(m_localisationService.GetText(m_category, m_textKey), args);
                SetText(text);
            }
        }

        /// @param text
        ///     Text to set
        ///
        public void SetText(string text)
        {
            m_textComponent.SafeText(text);
        }

        /// @param category
        ///		the localisation category of this text
        /// @param key
        /// 	text key to localise
        ///
        public void Initialise(string category, string key)
        {
            m_category = category;
            SetTextKey(key);
        }

        /// @param category
        ///		the localisation category of this text
        /// @param key
        /// 	text key to localise
        /// @oaram args
        ///     The format arguments
        ///
        public void Initialise(string category, string key, params object[] args)
        {
            m_category = category;
            SetTextKey(key, args);
        }
        #endregion

        #region Protected functions
        /// Called when the global director is ready
        /// 
        protected virtual void OnDirectorReady()
        {
            m_localisationService = GlobalDirector.Service<LocalisationService>();
            m_localisationService.OnLanguageChanged += OnLanguageChanged;
            if (m_localisationService.m_loaded == true)
            {
                OnLanguageChanged();
            }
        }

        /// Called when language has changed
        /// 
        protected virtual void OnLanguageChanged()
        {
            Initialise(m_category, m_textKey);
        }
        #endregion
    }
}
