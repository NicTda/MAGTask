//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreFramework
{
	/// MetadataLoader should be derived to load specific types of serializable data.
	/// How the data is loaded is up to the derived class, and it is not bound to file loading.
	///	
	public abstract class MetadataLoader<T> where T : class, IMetaDataSerializable, new()
    {
		private const string k_pathWithGroupFormat = "{0}{1}/";
        private const string k_keyRegistry = "Registry";

        /// Determines the behaviour of the loader - use groups, load on init, load in gameplay
        /// 
        public enum LoaderBehaviour
        {
            LoadAllAtInit,
            LoadAllDelayed,
            LoadIndividuallyWhenNeeded,
            LoadInGroups
        }

        /// Determines the file type, either single file or separate ones
        /// 
        public enum FileType
        {
            SingleFile,
            SeparateFiles
        }

        private Dictionary<string, T> m_data = new Dictionary<string, T>();
        private Dictionary<string, List<string>> m_groups = new Dictionary<string, List<string>>();
        private Dictionary<string, int> m_refCounts = new Dictionary<string, int>();

        protected string m_dataPath = string.Empty;
        protected string m_filePath = string.Empty;
        protected LoaderBehaviour m_loaderBehaviour = LoaderBehaviour.LoadAllAtInit;
        protected FileType m_fileType = FileType.SeparateFiles;
        protected List<string> m_registry = new List<string>();

		///	Constructor
		///	
		protected MetadataLoader()
		{
			GlobalDirector.Service<MetadataService>().RegisterLoader(this);

            Init();

            if(m_loaderBehaviour == LoaderBehaviour.LoadAllAtInit)
            {
                LoadGroup(string.Empty);
            }
		}

        /// Initialisation function
        /// 
		protected abstract void Init();

		///	@param key
		/// 	The unique ID of the item to check
		/// 
		/// @return Whether the item is in the loader or not
		///	
		public bool HasItem(string key)
		{
			return m_data.ContainsKey(key) || m_registry.Contains(key);
		}

		///	@param group
		/// 	The unique ID of the group to check
		/// 
		/// @return Whether the group is in the loader or not
		///	
		public bool HasGroup(string group)
		{
			return m_groups.ContainsKey(group);
		}

		///	@param key
		/// 	The unique ID of the item to retrieve
		/// 
		/// @return The item corresponding to the given key. Asserts if not found.
		///	
		public T GetItem(string key)
		{
            T item = null;
            if(m_loaderBehaviour == LoaderBehaviour.LoadIndividuallyWhenNeeded)
            {
				if(IsLoaded(key) == false)
                {
					item = LoadItem(key);
					if(item != null)
					{
						AddItem(item);
					}
                }
            }

            if(IsLoaded(key) == true)
            {
                item = m_data[key];
            }
            else
            {
			    Debug.LogError(string.Format("The item {0} is missing from loader of type {1}", key, GetType()));
            }
            return item;
        }
            
        /// @return All items in the loader as a list
        /// 
        public List<T> GetAllItems()
        {
            return m_data.Values.ToList();
        }

        /// @return All item keys in the loader as a list
        /// 
        public List<string> GetAllIDs()
        {
            List<string> IDs = null;
            if (m_registry.Count > 0)
            {
                IDs = m_registry;
            }
            else
            {
                IDs = GetAllLoadedIDs();
            }
            return IDs;
        }

        /// @return All currently loaded item keys in the loader as a list
        /// 
        public List<string> GetAllLoadedIDs()
        {
            return m_data.Keys.ToList();
        }

		/// @return Random Item from loader
		///	
		public T GetRandomItem()
		{
			List<string> keys;
			if(m_registry.Count > 0)
			{
				keys = m_registry;
			}
			else
			{
				keys = m_data.Keys.ToList();
			}
			var value = Random.Range(0, keys.Count);
			var itemName = keys[value];
			return GetItem(itemName);
        }

        /// Adds an item to a managed group
        /// 
        /// @param key
        ///     The unique ID of the item.
        /// @param group
        ///     The unique ID of the group.
        /// 
        public void AddItemToGroup(string key, string group)
        {
			Debug.Assert(IsLoaded(key), string.Format("The item {0} is not set in loader of type {1}, so it cannot be categorised", key, GetType()));
            if (m_groups.Contains(group, key) == false)
            {
                m_groups.AddValue(group, key);
                m_refCounts.AddOrUpdate(key, m_refCounts.GetValueOrDefault(key, 0) + 1);
            }
            else
            {
                Debug.LogWarning(string.Format("The item {0} is already in group {1} of loader {2}", key, group, GetType()));
            }
		}

		/// Registers an item. If the key is already used, the function asserts
		/// 
		/// @param item
		///     The item to register
		/// @param group
		///     The unique ID of the group that the item belongs to.
		/// 
		public void AddItem(T item, string group = "")
		{
            Debug.Assert(IsLoaded(item.m_id) == false, string.Format("The item {0} is already set in loader of type {1}", item.m_id, GetType()));
            m_data.Add(item.m_id, item);

			if(m_loaderBehaviour == LoaderBehaviour.LoadInGroups)
			{
				Debug.Assert(group != "", string.Format("A group needs to be specified for the item {0} in loader of type {1}", item.m_id, GetType()));
                AddItemToGroup(item.m_id, group);
			}
		}

        /// Removes a group, decrements the ref count for each item in the group, and unloads any items with ref counts of zero or less.
        /// 
        /// @param group
        ///     The unique ID of the group.
        /// 
        public void RemoveGroup(string group)
        {
            Debug.Assert((m_loaderBehaviour == LoaderBehaviour.LoadInGroups), string.Format("Groups are not in use for loader of type {0}.", GetType()));
            Debug.Assert((string.IsNullOrEmpty(group) == false), string.Format("The group name can not be null or empty in {0} loader", GetType()));
            Debug.Assert(m_groups.ContainsKey(group), string.Format("The group {0} is missing from loader of type {1}", group, GetType()));

            foreach (string key in m_groups[group])
            {
                RemoveItem(key);
            }

            m_groups.Remove(group);
        }

        /// Loads the group's data.
        /// 
        /// @param group
        ///     The unique ID of the group.
        /// 
        public void LoadGroup(string group)
        {
			if(HasGroup(group) == false)
			{
				LoadInternal(group);
			}
        }

		///	Clears the loader of all items
		///	
		public void Clear()
        {
            m_data.Clear();
            m_refCounts.Clear();
            m_groups.Clear();
			m_registry.Clear();
        }

        /// Unregisters an item. If the key is not present in the loader, the function asserts
        /// 
        /// @param key
        ///     The unique ID of the item.
        /// 
        public void RemoveItem(string key)
        {
			Debug.Assert(IsLoaded(key), string.Format("The item {0} is missing from loader of type {1}", key, GetType()));
            m_refCounts.AddOrUpdate(key, m_refCounts.GetValueOrDefault(key, 0) - 1);
            if (m_refCounts[key] <= 0)
            {
                m_data.Remove(key);
                m_refCounts.Remove(key);
            }
		}

		///	@param key
		/// 	The unique ID of the item to check
		/// 
		/// @return Whether the item has been loaded or not
		///	
		protected bool IsLoaded(string key)
		{
			return m_data.ContainsKey(key);
		}

		/// Loads all assets in a single folder and adds them to the dictionary.
		/// 
        /// @param group
        ///     The unique ID of the group to load.
		///	
        protected virtual void LoadInternal(string group)
        {
            string path = m_dataPath;
            if (m_loaderBehaviour == LoaderBehaviour.LoadInGroups)
            {
                path = string.Format(k_pathWithGroupFormat, path, group);
            }

            if(m_fileType == FileType.SeparateFiles)
            {
                TextAsset[] textAssets = Resources.LoadAll<TextAsset>(path);
                Debug.Assert(textAssets.Length > 0, string.Format("No data on path {0}", path));

                foreach (TextAsset asset in textAssets)
                {
				    // Avoid duplicate
				    if(IsLoaded(asset.name) == false)
				    {
					    T serializable = ParseItem(asset);
					    AddItem(serializable, group);
				    }
                }
            }
            else
            {
                TextAsset textAsset = Resources.Load<TextAsset>(path);
                if(textAsset != null)
                {
                    var jsonData = JsonWrapper.ParseJsonFromTextAsset(textAsset).AsDictionary();
                    foreach (var asset in jsonData)
                    {
                        // Avoid duplicate
                        if (IsLoaded(asset.Key) == false)
                        {
                            T serializable = new T();
                            serializable.m_id = asset.Key;
                            serializable.Deserialize(asset.Value);
                            AddItem(serializable, group);
                        }
                    }
                }
            }
		}

		/// Loads a single item.
		/// 
		/// @param key
		///     The unique ID of the item to load.
		/// 
		/// @return The loaded instance, or null if not found
		/// 
		protected virtual T LoadItem(string key)
		{
			T item = null;

			string path = m_dataPath + key;
			TextAsset textAsset = Resources.Load<TextAsset>(path);
			if(textAsset == null)
			{
				Debug.LogError(string.Format("Could not find file of type {0} with path '{1}'", typeof(T), path));
			}
			else
			{
				item = ParseItem(textAsset);
			}

			return item;
		}

		/// @param path
		///     The path to the registry file
		/// 
		protected void LoadRegistry(string path)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(path);
			if(textAsset == null)
			{
				Debug.LogError(string.Format("Could not find registry file with path '{0}'", path));
			}
            var dictionary = JsonWrapper.ParseJsonFromTextAsset(textAsset).AsDictionary();
			if((dictionary != null) && dictionary.ContainsKey(k_keyRegistry))
			{
				m_registry = dictionary.GetValue(k_keyRegistry).AsList<string>();
			}
		}

        /// This should be the place where the items of type T are being loaded,
        /// either on the fly, or from file, etc.
        /// 
        /// Parses a single item from textAsset
        /// 
        /// @param group
        ///     The unique ID of the group to load.
        /// 
        /// @return The parsed instance
        /// 
        protected virtual T ParseItem(TextAsset textAsset)
        {
            T serializable = new T();

			string assetName = "";
			if(textAsset != null)
			{
				assetName = textAsset.name;
                serializable.m_id = assetName;
				try
				{
                    object jsonData = JsonWrapper.ParseJsonFromTextAsset(textAsset);
					serializable.Deserialize(jsonData);
				}
				catch(System.Exception exception)
				{
                    // Something went wrong during the parsing of the file
                    Debug.LogError(string.Format("Could not parse file '{0}' of type {1}.\n{2}", textAsset.name, typeof(T), exception.Message));
				}
			}
            Debug.Assert(m_data.ContainsKey(assetName) == false, string.Format("Duplicate serializable id found: {0} already exists", assetName));
			
            return serializable;
        }
	}
}
