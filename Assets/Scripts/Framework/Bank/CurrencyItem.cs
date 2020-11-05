//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;

namespace CoreFramework
{
    /// Container for a currency item
    ///
    [System.Serializable]
    public class CurrencyItem : ISerializable
    {
        public const string k_keyCurrencyID = "CurrencyID";
        public const string k_keyValue = "Value";

        public string m_currencyID = string.Empty;
        public int m_value = 0;

        #region Public functions
        /// Constructor
        /// 
        public CurrencyItem()
        {
        }

        /// @param currencyItem
        /// 	the currency item to copy
        /// 
        public CurrencyItem(CurrencyItem currencyItem)
        {
            m_currencyID = currencyItem.m_currencyID;
            m_value = currencyItem.m_value;
        }

        /// @param currencyID
        /// 	the currency ID
        /// @param amount
        /// 	amount of currency
        /// 
        public CurrencyItem(string currencyID, int amount)
        {
            m_currencyID = currencyID;
            m_value = amount;
        }

        /// @return Whether the item is free
        /// 
        public bool IsFree()
        {
            return (m_currencyID == string.Empty) || (m_value == 0);
        }
        #endregion

        #region ISerializable functions
        /// @return The serialized data
        ///
        public object Serialize()
        {
            var jsonData = new JsonDictionary()
            {
                { k_keyCurrencyID, m_currencyID },
                { k_keyValue, m_value }
            };
            return jsonData;
        }

        /// @param data
        ///     The json data
        /// 
        public void Deserialize(object data)
        {
            var jsonData = data.AsDictionary();
            m_currencyID = jsonData.GetValueOrAssert(k_keyCurrencyID).AsString();
            m_value = jsonData.GetValueOrAssert(k_keyValue).AsInt();
        }
        #endregion
    }
}
