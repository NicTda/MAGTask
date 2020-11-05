//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System.Collections.Generic;

namespace CoreFramework
{
    /// Service that manages the game local saves
    /// 
	public sealed class LocalSaveService : Service, ISavable
    {
        public bool m_loaded { get; set; }

        private Dictionary<string, object> m_saves = new Dictionary<string, object>(10);
        private bool m_requestSave = false;

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            this.RegisterCaching();
        }

        /// Updates the state of the service
        /// 
		public override void ServiceUpdate()
        {
            if (m_requestSave == true)
            {
                this.Save();
                m_requestSave = false;
            }
        }

        /// @return The serialized data
        /// 
        public object Serialize()
        {
            var jsonData = new JsonDictionary();
            foreach(var pair in m_saves)
            {
                jsonData.Add(pair.Key, pair.Value);
            }
            return jsonData;
        }

        /// @param data
        /// 	The json data
        /// 
        public void Deserialize(object data)
        {
            var jsonData = data.AsDictionary();
            foreach (var pair in jsonData)
            {
                m_saves.Add(pair.Key, pair.Value);
            }
            m_loaded = true;
        }

        /// Clears the local data
        ///     
        public void Clear()
        {
            m_saves.Clear();
            m_requestSave = false;
            m_loaded = false;
        }
        #endregion

        #region Public functions
        /// @param key
        ///     The key to retrieve
        ///     
        /// @return The saved data
        /// 
        public object LoadData(string key)
        {
            return m_saves.ContainsKey(key) ? m_saves[key] : null;
        }

        /// @param key
        ///     The key to save
        /// @param data
        ///     The data to save
        /// 
        public void SaveData(string key, object value)
        {
            m_saves.AddOrUpdate(key, value);
            m_requestSave = true;
        }

        /// @param key
        ///     The key to delete
        /// 
        public void DeleteData(string key)
        {
            m_saves.RemoveIfContained(key);
            m_requestSave = true;
        }
        #endregion
    }
}
