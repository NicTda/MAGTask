//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using CoreFramework.Json;
using System;

namespace MAGTask
{
    /// Service that handles the loading and saving of the game's progress
    /// 
	public sealed class GameProgressService : Service, ISavable
    {
        private const string k_keyLevel = "Lvl";
        private const string k_keyFirstDate = "FD";

        public DateTime m_firstDate = DateTime.MinValue;
        public int m_level = 0;

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            base.OnCompleteInitialisation();
        }

        /// @return The serialized data
        /// 
        public object Serialize()
        {
            var jsonData = new JsonDictionary();
            if (m_level > 0)
            {
                jsonData.Add(k_keyLevel, m_level);
            }
            if (m_firstDate != DateTime.MinValue)
            {
                jsonData.Add(k_keyFirstDate, m_firstDate.DateToString());
            }
            return jsonData;
        }

        /// @param jsonData
        /// 	The json data
        /// 
        public void Deserialize(object data)
        {
            if (data != null)
            {
                var jsonData = data.AsDictionary();
                if (jsonData.ContainsKey(k_keyLevel))
                {
                    m_level = jsonData.GetInt(k_keyLevel);
                }
                if (jsonData.ContainsKey(k_keyFirstDate))
                {
                    m_firstDate = jsonData.GetDateTime(k_keyFirstDate);
                }
            }
        }

        /// Clears the progress
        /// 
        public void Clear()
        {
            m_level = 0;
            m_firstDate = DateTime.MinValue;
        }
        #endregion
    }
}
