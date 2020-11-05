//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// This service handles currencies
    /// 
	public sealed class BankService : Service
    {
        private const int k_zeroBalance = 0;

        /// Container for currency balance
        /// 
        public struct Balance
        {
            public int m_previous;
            public int m_current;

            /// Constructor
            /// 
            /// @param previous
            /// 	the previous balance of the currency
            /// @param current
            /// 	the current balance of the currency
            /// 
            public Balance(int previous, int current)
            {
                m_previous = previous;
                m_current = current;
            }
        };

        public delegate void CurrencyAmountChangedDelegate(string currencyID, Balance balance);

        private Dictionary<string, int> m_currencies = new Dictionary<string, int>();
        private Dictionary<string, CurrencyAmountChangedDelegate> m_currencyChangedDelegates = new Dictionary<string, CurrencyAmountChangedDelegate>();

        private Dictionary<string, int> m_pending = new Dictionary<string, int>();

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
        }
        #endregion

        #region Public functions
        /// @param currencyID
        /// 	The currency ID to retrieve the balance for
        /// 
        /// @return The current balance of the queried currency
        /// 
        public int GetBalance(string currencyID)
        {
            return m_currencies.GetValueOrDefault(currencyID, k_zeroBalance);
        }

        /// @param currencyID
        /// 	The currency ID to retrieve the balance for
        /// 
        /// @return The currently pending balance of the queried currency
        /// 
        public int GetPendingBalance(string currencyID)
        {
            return m_pending.GetValueOrDefault(currencyID, k_zeroBalance);
        }

        /// @param currencyID
        /// 	The currency ID to retrieve the balance for
        /// 
        /// @return The current balance of the queried currency minus the pensing value
        /// 
        public int GetVirtualBalance(string currencyID)
        {
            return GetBalance(currencyID) - GetPendingBalance(currencyID);
        }

        /// Sets the balance of a given currency
        /// 
        /// @param currencyID
        /// 	The currency ID to set
        /// @param currentAmount
        /// 	The amount to set
        /// 
        public void SetBalance(string currencyID, int currentAmount)
        {
            int oldAmount = GetBalance(currencyID);
            m_currencies.AddOrUpdate(currencyID, currentAmount);
            NotifyCurrencyChange(currencyID, oldAmount, currentAmount);
        }

        /// @param currencyID
        /// 	The currency ID to check
        /// @param amount
        /// 	The amount to check
        /// 
        /// @return Whether the user can afford the currency
        /// 
        public bool CanAfford(string currencyID, int amount)
        {
            int value = m_currencies.GetValueOrDefault(currencyID, k_zeroBalance);
            return value >= amount;
        }

        /// @param currencyItem
        /// 	The currency Item we are checking
        /// 
        /// @return Whether the user can afford the currency
        /// 
        public bool CanAfford(CurrencyItem currencyItem)
        {
            return CanAfford(currencyItem.m_currencyID, currencyItem.m_value);
        }

        /// @param currencyItems
        /// 	The currency item array to check
        /// 
        /// @return Whether the user can afford the currencies
        /// 
        public bool CanAfford(IList<CurrencyItem> currencyItems)
        {
            bool canAfford = true;
            foreach (CurrencyItem item in currencyItems)
            {
                if (CanAfford(item) == false)
                {
                    canAfford = false;
                    break;
                }
            }
            return canAfford;
        }

        /// @param currencyID
        /// 	Currency ID to deposit
        /// @param amount
        /// 	Amount to deposit
        /// 
        public void Deposit(string currencyID, int amount)
        {
            // Check for negative deposit
            if (amount < k_zeroBalance)
            {
                Debug.LogWarning(string.Format("Can't deposit the negative value {0}. Use withdraw instead", amount));
                return;
            }

            SetBalance(currencyID, GetBalance(currencyID) + amount);
        }

        /// @param currency
        /// 	The currency to deposit
        /// 
        public void Deposit(CurrencyItem currency)
        {
            Deposit(currency.m_currencyID, currency.m_value);
        }

        /// Adds a delayed currency. Pending currencies need to be added after a deposit / withdraw.
        /// Remember to Flush the pending currencies when needed.
        ///
        /// @param currency
        /// 	The currency to add as pending
        /// 
        public void AddPending(CurrencyItem currency)
        {
            AddPending(currency.m_currencyID, currency.m_value);
        }

        /// @param currencyID
        /// 	Currency ID to add as pending
        /// @param amount
        /// 	Amount to add as pending
        /// 
        public void AddPending(string currencyID, int amount)
        {
            int oldAmount = GetPendingBalance(currencyID);
            SetPending(currencyID, oldAmount + amount);
        }

        /// @param currencyID
        /// 	Currency ID to set as pending
        /// @param amount
        /// 	Amount to set as pending
        /// 
        public void SetPending(string currencyID, int amount)
        {
            int oldAmount = GetVirtualBalance(currencyID);
            m_pending.AddOrUpdate(currencyID, amount);

            var currentAmount = GetVirtualBalance(currencyID);
            NotifyCurrencyChange(currencyID, oldAmount, currentAmount);
        }

        /// @param currency
        /// 	The currency to set as pending
        /// 
        public void SetPending(CurrencyItem currency)
        {
            SetPending(currency.m_currencyID, currency.m_value);
        }

        /// Triggers the pending currencies transfer
        /// 
        public void FlushPendingCurrencies()
        {
            var pendingCurrencies = new Dictionary<string, int>(m_pending);
            foreach (var pair in pendingCurrencies)
            {
                if (pair.Value > k_zeroBalance)
                {
                    SetPending(pair.Key, k_zeroBalance);
                }
            }
        }

        /// @param currencyID
        /// 	Currency ID to withdraw
        /// @param amount
        /// 	Amount to withdraw
        /// 
        public void Withdraw(string currencyID, int amount)
        {
            // Check for negative withdraw
            if (amount < k_zeroBalance)
            {
                Debug.LogWarning(string.Format("Can't withdraw the negative value {0}. Use Deposit instead.", amount));
                return;
            }

            int newAmount = GetBalance(currencyID) - amount;
            if (newAmount < k_zeroBalance)
            {
                Debug.Log(string.Format("Trying to withdraw {0} {1} but only has {2}", amount, currencyID, GetBalance(currencyID)));
                newAmount = k_zeroBalance;
            }

            SetBalance(currencyID, newAmount);
        }

        /// @param item
        /// 	The currency item to withdraw
        /// 
        public void Withdraw(CurrencyItem item)
        {
            Withdraw(item.m_currencyID, item.m_value);
        }

        /// Adds a listener for a currency ID
        /// 
        /// @param currencyID
        /// 	CurrencyID to add a listener to
        /// @param newDelegate
        /// 	Delegate to call
        /// 
        public void AddCurrencyListener(string currencyID, CurrencyAmountChangedDelegate newDelegate)
        {
            if((m_currencyChangedDelegates.ContainsKey(currencyID)) && (m_currencyChangedDelegates[currencyID] != null))
            {
                m_currencyChangedDelegates[currencyID] += newDelegate;
            }
            else
            {
                m_currencyChangedDelegates[currencyID] = newDelegate;
            }
        }

        /// Removes a listener for a currency ID
        /// 
        /// @param currencyID
        /// 	CurrencyID to remove a listener from
        /// @param newDelegate
        /// 	Delegate to remove
        /// 
        public void RemoveCurrencyListener(string currencyID, CurrencyAmountChangedDelegate newDelegate)
        {
            if ((m_currencyChangedDelegates.ContainsKey(currencyID)) && (m_currencyChangedDelegates[currencyID] != null))
            {
                m_currencyChangedDelegates[currencyID] -= newDelegate;
            }
        }
        #endregion

        #region Private functions
        /// @param currencyID
        /// 	Currency ID to notify as changed
        /// @param oldAmount
        /// 	The old amount of the currency
        /// @param currentAmount
        /// 	The current amount of the currency
        /// 
        private void NotifyCurrencyChange(string currencyID, int oldAmount, int currentAmount)
        {
            if (m_currencyChangedDelegates.ContainsKey(currencyID))
            {
                Balance newBalanceValue = new Balance
                {
                    m_previous = oldAmount,
                    m_current = currentAmount
                };

                if (m_currencyChangedDelegates[currencyID] != null)
                {
                    m_currencyChangedDelegates[currencyID].Invoke(currencyID, newBalanceValue);
                }
            }
        }
        #endregion
    }
}
