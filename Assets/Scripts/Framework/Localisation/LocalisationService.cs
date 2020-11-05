//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Manages the loading and retrieval of localised text resources
    ///
    public sealed class LocalisationService : Service
    {
        public event Action OnLanguageChanged;

        public static readonly SystemLanguage k_defaultLanguage = SystemLanguage.English;

        public SystemLanguage m_language { get; private set; }
        public bool m_loaded { get; private set; }

        private LocalisedTextLoader m_loader = null;
        private List<string> m_categoriesToLoad = new List<string>();

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            m_loaded = false;
            m_language = SystemLanguage.Unknown;
            m_loader = GlobalDirector.Service<MetadataService>().GetLoader<LocalisedTextData>() as LocalisedTextLoader;

            // Retrieve the starting language
            var startingLanguage = Application.systemLanguage;
            if (PlayerPrefs.HasKey(LocalisedTextIdentifiers.k_languageSettings) == true)
            {
                startingLanguage = PlayerPrefs.GetString(LocalisedTextIdentifiers.k_languageSettings).AsEnum<SystemLanguage>();
            }
            SetLanguage(startingLanguage);

            LoadCategories();
        }
        #endregion

        #region Public functions
        /// @param language
        ///     The language to set
        ///
        public void SetLanguage(SystemLanguage language, Action<bool> callback = null)
        {
            if (language == SystemLanguage.Chinese)
            {
                language = SystemLanguage.ChineseSimplified;
            }

            if (m_language != language)
            {
                m_language = language;

                // Reload the categories
                if (m_loaded == true)
                {
                    LoadCategories();
                }

                callback.SafeInvoke(true);
            }
            else
            {
                callback.SafeInvoke(false);
            }
        }

        /// @param category
        ///     The category to add
        /// @param callback
        ///     The function to call when the category is loaded
        /// 
        public void AddCategoryToLoad(string category, Action callback = null)
        {
            m_categoriesToLoad.AddUnique(category);

            if (m_loaded == true)
            {
                // Should load the category
                LoadCategory(category, callback);
            }
            else
            {
                callback.SafeInvoke();
            }
        }

        /// @param category
        ///		The localisation category to search in
        ///	@param key
        ///		The key to search for
        ///
        /// @return Whether the text exists for the given category
        ///
        public bool HasText(string category, string key)
        {
            bool hasText = false;
            if (m_loader.HasItem(category) == true)
            {
                var textData = m_loader.GetItem(category);
                hasText = textData.HasText(key);
            }
            return hasText;
        }

        /// @param category
        ///		The localisation category to search in
        ///	@param key
        ///		The key to search for
        ///
        /// @return The localised text string for the current language
        ///
        public string GetText(string category, string key)
        {
            var text = string.Empty;
            if (m_loader.HasItem(category) == true)
            {
                var textData = m_loader.GetItem(category);
                text = textData.GetText(key);
            }
            else
            {
                text = string.Format(LocalisedTextData.k_missingKeyFormat, key);
            }
            return text;
        }

        /// @param category
        ///		The localisation category to search in
        ///	@param key
        ///		The key to search for
        ///
        /// @return The localised text string for the current language
        ///
        public string GetText(string key)
        {
            var text = string.Format(LocalisedTextData.k_missingKeyFormat, key);

            var knownCategories = m_loader.GetAllItems();
            foreach(var category in knownCategories)
            {
                if (category.HasText(key))
                {
                    text = category.GetText(key);
                    break;
                }
            }
            return text;
        }

        ///	@param baseKey
        ///		The key to search for
        ///
        /// @return The random localised text string for the current language
        ///
        public string GetRandomText(string category, string baseKey)
        {
            int index = 0;
            string key = baseKey;
            while (true)
            {
                if ((HasText(category, baseKey + index) == true))
                {
                    ++index;
                }
                else
                {
                    if(index > 0)
                    {
                        key = baseKey + UnityEngine.Random.Range(0, index);
                    }
                    break;
                }
            }

            return GetText(category, key);
        }

        ///	@param key
        ///		The key to search for
        ///
        /// @return The localised text string for the current language
        ///
        public string GetGameText(string key)
        {
            return GetText(LocalisedTextIdentifiers.k_categoryGame, key);
        }

        ///	@param baseKey
        ///		The key to search for
        ///
        /// @return The random localised text string for the current language
        ///
        public string GetRandomGameText(string baseKey)
        {
            return GetRandomText(LocalisedTextIdentifiers.k_categoryGame, baseKey);
        }

        ///	@param language
        ///		The language to check
        ///
        /// @return Whether languages files are available locally
        ///
        public bool HasLocalLanguage(SystemLanguage language)
        {
            bool available = true;
            return available;
        }
        #endregion

        #region Private functions
        /// Registers the listeners for the localisation manifest changes
        /// 
        private void RegisterChanges()
        {
            LoadCategories();
        }

        /// Loads the categories
        ///
        private void LoadCategories()
        {
            m_loader.Clear();

            int loaded = 0;
            var resources = GetResourcesForLanguage(m_language);
            foreach (var resource in resources)
            {
                m_loader.LoadResource(resource.Key, resource.Value, () =>
                {
                    ++loaded;
                    if (loaded == resources.Count)
                    {
                        m_loaded = true;
                        PlayerPrefs.SetString(LocalisedTextIdentifiers.k_languageSettings, m_language.ToString());
                        PlayerPrefs.Save();
                        OnLanguageChanged.SafeInvoke();
                    }
                });
            }
        }

        ///	@param language
        ///		The language to check
        ///
        /// @return The lsit of resources to load for this language
        ///
        public Dictionary<string, string> GetResourcesForLanguage(SystemLanguage language)
        {
            var resources = new Dictionary<string, string>();
            var categories = new List<string>(m_categoriesToLoad);
            foreach (var category in categories)
            {
                var resourceID = GetResourceIDForCategory(category, language);
                resources.Add(resourceID, category);
            }
            return resources;
        }

        /// @param category
        ///     The category to load
        /// @param callback
        ///     The function to call when the category is loaded
        ///
        private void LoadCategory(string category, Action callback)
        {
            var resourceID = GetResourceIDForCategory(category, m_language);
            m_loader.LoadResource(resourceID, category, callback);
        }

        /// @param category
        ///     The category to load
        /// @param language
        ///     The language to load
        ///     
        /// @return The resourceID for this category
        ///
        private string GetResourceIDForCategory(string category, SystemLanguage language)
        {
            var resourceID = m_loader.GetResourceID(category, language.ToString());
            return resourceID;
        }
        #endregion
    }
}
