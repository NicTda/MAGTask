//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using CoreFramework.Json;

namespace MAGTask
{
    /// Container class for a level objective
    /// 
    public sealed class ObjectiveData : ISerializable
    {
        private const string k_keyType = "Type";
        private const string k_keyTarget = "Target";
        private const string k_keyAmount = "Amount";
        private const string k_keyValue = "Value";

        public ObjectiveType m_type { get; private set; } = ObjectiveType.None;
        public string m_target { get; private set; } = string.Empty;
        public int m_value { get; private set; } = 0;
        public int m_amount { get; private set; } = 0;

        #region Public functions
        /// Constructor
        /// 
        public ObjectiveData() { }

        /// @param type
        ///     The type of the objective
        /// @param amount
        ///     The amount to reach
        /// 
        public ObjectiveData(ObjectiveType type, int amount)
        {
            m_type = type;
            m_amount = amount;
        }
        #endregion

        #region ISerializable functions
        /// @return The serialized data
        ///
        public object Serialize()
        {
            var jsonData = new JsonDictionary();
            if (m_type != ObjectiveType.None)
            {
                jsonData.Add(k_keyType, (int)m_type);
            }
            if (m_target != string.Empty)
            {
                jsonData.Add(k_keyTarget, m_target);
            }
            if (m_value > 0)
            {
                jsonData.Add(k_keyValue, m_value);
            }
            if (m_amount > 0)
            {
                jsonData.Add(k_keyAmount, m_amount);
            }
            return jsonData;
        }

        /// @param data
        ///     The json data
        /// 
        public void Deserialize(object data)
        {
            var jsonData = data.AsDictionary();
            if (jsonData.ContainsKey(k_keyType))
            {
                m_type = jsonData.GetValue(k_keyType).AsEnum<ObjectiveType>();
            }
            if (jsonData.ContainsKey(k_keyTarget))
            {
                m_target = jsonData.GetString(k_keyTarget);
            }
            if (jsonData.ContainsKey(k_keyValue))
            {
                m_value = jsonData.GetInt(k_keyValue);
            }
            if (jsonData.ContainsKey(k_keyAmount))
            {
                m_amount = jsonData.GetInt(k_keyAmount);
            }
        }
        #endregion
    }
}
