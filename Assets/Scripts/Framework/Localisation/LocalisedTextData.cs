//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Data container for a localised text group
    ///
    public sealed class LocalisedTextData : IMetaDataSerializable
    {
#if UNITY_EDITOR
        public const string k_missingKeyFormat = "#{0}";
#else
        public const string k_missingKeyFormat = "{0}";
#endif

        private const string k_textKey = "Text";

        public string m_id { get; set; }
        public Dictionary<string, string> m_text { get; private set; }

#region Public functions
        /// @param key
        ///		The key to search for
        ///
        /// @return Whether the data has the given key
        ///
        public bool HasText(string key)
        {
            return m_text.ContainsKey(key);
        }

        /// @param key
        ///		The key to search for
        ///
        /// @return the localised text value for given key, or key if no value exists
        ///
        public string GetText(string key)
        {
            if (m_text.ContainsKey(key) == false)
            {
                Debug.LogWarning(string.Format("Missing master text key: '{0}'", key));
            }
            string missingKey = string.Format(k_missingKeyFormat, key);
            return m_text.GetValueOrDefault(key, missingKey);
        }
#endregion

#region IMetaDataSerializable functions
        /// Serializes the data
        ///
        /// @return The serialized data
        /// 
        public object Serialize()
        {
            return null; // not used
        }

        /// Deserializes the data
        ///
        /// @param data
        /// 	The json data
        /// 
        public void Deserialize(object data)
        {
            Dictionary<string, object> jsonData = data.AsDictionary();
            m_text = jsonData.GetValueOrAssert(k_textKey).AsDictionary<string, string>();
        }
#endregion
    }
}
