//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;

namespace CoreFramework
{
    /// Metadata loader for localised text data
    ///
    public sealed class LocalisedTextLoader : MetadataLoader<LocalisedTextData>
    {
        private const string k_jsonMetadataFolderPath = "Localisation/";
        private const string k_fileNameFormat = "{0}{1}/{2}.json";

        #region MetadataLoader functions
        /// Initialisation function
        /// 
        protected override void Init()
        {
            m_dataPath = k_jsonMetadataFolderPath;
            m_loaderBehaviour = LoaderBehaviour.LoadIndividuallyWhenNeeded;
        }

        /// Loads all assets in a single folder and adds them to the dictionary.
        /// 
        /// @param group
        ///     The unique ID of the group to load.
        ///	
        protected override void LoadInternal(string group)
        {
            // Do nothing - we load items only
        }
        #endregion

        #region Public functions
        /// @param category
        ///     The category to check
        /// @param countryID
        ///     The ID of the requested country
        ///     
        /// @return The resource ID of the requested category
        /// 
        public string GetResourceID(string category, string countryID)
        {
            var resourceID = string.Format(k_fileNameFormat, k_jsonMetadataFolderPath, countryID, category);
            return resourceID;
        }

        /// @param resourceID
        ///     The unique ID of the item to load.
        /// @param dataID
        ///     The ID of the data to store
        /// @param callback
        ///     The function to call when the data is loaded
        ///	
        public void LoadResource(string resourceID, string dataID, Action callback = null)
        {
            // Query server for json file
            {
                //if (dictionary != null)
                //{
                //    // Add the data
                //    var data = new LocalisedTextData();
                //    data.Deserialize(dictionary);
                //    data.m_id = dataID;

                //    if (IsLoaded(data.m_id))
                //    {
                //        // The data was already in the loader, remove
                //        RemoveItem(data.m_id);
                //    }

                //    AddItem(data);
                //}

                //callback.SafeInvoke();
            }
        }
        #endregion
    }
}
