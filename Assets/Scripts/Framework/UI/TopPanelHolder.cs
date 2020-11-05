//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;
using UnityEngine.UI;

namespace MAGTask
{
    /// Holder object for each top panel
    ///
    public sealed class TopPanelHolder : MonoBehaviour
    {
        private const string k_animationShow = "Show";
        private const string k_animationHide = "Hide";

        public string CurrencyID { get { return m_currency; } }

        [SerializeField]
        private string m_currency = OverlayBankIdentifiers.k_currencyCredits;
        [SerializeField]
        private Image m_icon = null;
        [SerializeField]
        private CurrencyComponent m_currencyComponent = null;

        public Image m_flash = null;
        public Animator m_animator = null;

        [HideInInspector]
        public int m_activeParticles = 0;
        [HideInInspector]
        public bool m_hidden = false;

        private Coroutine m_coroutine = null;

        #region Unity functions
        /// Awake function
        ///
        public void Awake()
        {
            if (m_flash != null)
            {
                m_flash.color = GameUtils.k_transparent;
            }
        }

        /// OnDestroy function
        ///
        public void OnDestroy()
        {
            m_coroutine.Stop();
        }
        #endregion

        #region Public functions
        /// @param currencyID
        ///     The currency to set
        ///
        public void SetCurrency(string currencyID)
        {
            m_currency = currencyID;
            m_icon.SafeSprite(GameUtils.GetCurrencySprite(currencyID));
            if (m_currencyComponent != null)
            {
                m_currencyComponent.SetCurrency(currencyID);
            }
        }

        /// Show the panel
        ///
        public void Show()
        {
            gameObject.SetActive(true);
            if (m_animator != null && m_activeParticles == 0)
            {
                m_animator.Play(k_animationShow);
            }
        }

        /// Hide the panel
        ///
        public void Hide()
        {
            if (m_animator != null && m_activeParticles == 0)
            {
                m_coroutine = m_animator.PlayAnimation(k_animationHide, () =>
                {
                    if (m_activeParticles == 0)
                    {
                        // If still not needed, hide
                        gameObject.SetActive(!m_hidden);
                    }
                });
            }
        }
        #endregion
    }
}
