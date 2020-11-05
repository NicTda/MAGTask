//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;

namespace CoreFramework
{
    /// Extension class for currency item
    ///
	public static class CurrencyExtensions
    {
        /// @param list
        ///     The list to append to
        /// @param item
        ///     The item to append
        ///
	    public static void Append(this List<CurrencyItem> list, CurrencyItem newItem)
        {
            if(list == null)
            {
                list = new List<CurrencyItem>();
            }

            foreach(var item in list)
            {
                if(item.m_currencyID == newItem.m_currencyID)
                {
                    item.m_value += newItem.m_value;
                    return;
                }
            }

            list.Add(new CurrencyItem(newItem));
        }

        /// @param item
        ///     The item to clone
        ///     
        /// @return The cloned currency
        ///
        public static CurrencyItem Clone(this CurrencyItem item)
        {
            var currency = new CurrencyItem();
            if (item != null)
            {
                currency.m_currencyID = item.m_currencyID;
                currency.m_value = item.m_value;
            }
            return currency;
        }

        /// @param item
        ///     The item to multiply
        /// @param quantity
        ///     The quantity to multiply by
        ///     
        /// @return The multiplied currency
        ///
        public static CurrencyItem Multiply(this CurrencyItem item, int quantity)
        {
            var currency = item.Clone();
            if (currency != null)
            {
                currency.m_value *= quantity;
            }
            return currency;
        }

        /// @param list
        ///     The list to clone
        ///     
        /// @return The cloned currency list
        ///
        public static List<CurrencyItem> Clone(this List<CurrencyItem> list)
        {
            var currencies = new List<CurrencyItem>(list.Count);
            foreach (var item in list)
            {
                currencies.Add(item.Clone());
            }
            return currencies;
        }

        /// @param list
        ///     The list to append to
        /// @param quantity
        ///     The quantity to apply
        ///     
        /// @return The multiplied currencies
        ///
        public static List<CurrencyItem> Multiply(this List<CurrencyItem> list, int quantity)
        {
            var currencies = new List<CurrencyItem>(list.Count);
            foreach (var item in list)
            {
                currencies.Add(new CurrencyItem(item.m_currencyID, item.m_value * quantity));
            }
            return currencies;
        }
    }
}
