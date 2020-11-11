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

        public ObjectiveType m_type = ObjectiveType.None;
        public TileColour m_target = TileColour.None;
        public int m_value = 0;
        public int m_amount = 0;

        #region ISerializable functions
        /// @return The serialized data
        ///
        public object Serialize()
        {
            var jsonData = new JsonDictionary()
            {
                { k_keyType, m_type.ToString() }
            };
            if (m_type == ObjectiveType.Chain && m_value > 0)
            {
                jsonData.Add(k_keyValue, m_value);
            }
            if (m_type == ObjectiveType.Colour && m_target != TileColour.None)
            {
                jsonData.Add(k_keyTarget, m_target.ToString());
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
                m_target = jsonData.GetValue(k_keyTarget).AsEnum<TileColour>();
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
