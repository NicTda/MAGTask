//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using DG.Tweening;
using UnityEngine;

namespace CoreFramework
{
    /// Component that displays a specific currency amount
    ///
    public class CurrencyComponent : LocalisedTextUI
    {
        [SerializeField]
        protected string m_currencyID = string.Empty;
        [SerializeField] [Tooltip("The time to mount up to the new amount when it changes.")]
        protected float m_timeToTally = 0.0f;

        protected BankService m_bankService = null;
        protected int m_amount = 0;
        protected Tweener m_tallyTweener = null;

        #region Unity functions
        /// Destroy function
        ///
        private new void OnDestroy()
        {
            base.OnDestroy();
            if (m_bankService != null)
            {
                m_bankService.RemoveCurrencyListener(m_currencyID, OnCurrencyAmountChanged);
            }
            m_tallyTweener.Stop();
        }
        #endregion

        #region Public functions
        /// @param currencyID
        ///     The currency to set
        /// 
        public void SetCurrency(string currencyID)
        {
            if (currencyID != string.Empty)
            {
                m_bankService.RemoveCurrencyListener(m_currencyID, OnCurrencyAmountChanged);
                m_bankService.AddCurrencyListener(currencyID, OnCurrencyAmountChanged);
                m_amount = m_bankService.GetVirtualBalance(currencyID);
                m_currencyID = currencyID;
                UpdateAmountInstantly(m_amount);
            }
        }
        #endregion

        #region Protected functions
        /// Called when the global director is ready
        /// 
        protected override void OnDirectorReady()
        {
            base.OnDirectorReady();
            m_bankService = GlobalDirector.Service<BankService>();
            if (m_currencyID != string.Empty)
            {
                SetCurrency(m_currencyID);
            }
        }

        /// Called when language has changed
        /// 
        protected override void OnLanguageChanged()
        {
            UpdateAmountInstantly(m_amount);
        }
        #endregion

        #region Private functions
        /// @param currencyID
        ///     The ID of the currency that changed
        /// @param balance
        ///     The balance of the currency 
        /// 
        protected virtual void OnCurrencyAmountChanged(string currencyID, BankService.Balance balance)
        {
            if(m_timeToTally > 0.0f)
            {
                UpdateAmount(balance.m_current);
            }
            else
            {
                UpdateAmountInstantly(balance.m_current);
            }
        }

        /// @param amount
        ///     The new amount to display
        /// 
        protected virtual void UpdateAmount(int amount)
        {
            m_tallyTweener.Stop();
            m_tallyTweener = DOTween.To(() => m_amount, (value) =>
            {
                UpdateAmountInstantly(value);

            }, amount, m_timeToTally).OnComplete(() =>
            {
                UpdateAmountInstantly(amount);
            });
        }

        /// @param amount
        ///     The new amount to display
        /// 
        protected virtual void UpdateAmountInstantly(int amount)
        {
            m_amount = amount;
            string formattedAmount = TextUtils.GetFormattedCurrencyString(m_amount);
            if (m_textKey != string.Empty)
            {
                Initialise(m_category, m_textKey, formattedAmount);
            }
            else
            {
                SetText(formattedAmount);
            }
        }
        #endregion
    }
}
