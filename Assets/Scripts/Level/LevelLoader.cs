//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using CoreFramework.Json;
using System.IO;
using UnityEngine;

namespace MAGTask
{
    /// Data loader for LevelData
    ///
    public sealed class LevelDataLoader : MetadataLoader<LevelData>
    {
        private const string k_jsonMetadataFolderPath = "Metadata/Levels";
        private const string k_savePath = "{0}/Resources/{1}/{2}.json";
        private const string k_levelFormat = "Level{0}";

        #region MetadataLoader functions
        /// Initialisation function
        /// 
        protected override void Init()
        {
            m_dataPath = k_jsonMetadataFolderPath;
            m_loaderBehaviour = LoaderBehaviour.LoadAllAtInit;
        }
        #endregion

        #region Public functions
        /// @param index
        ///     The index of the level to get
        ///     
        /// @return The level data for that index, or null
        /// 
        public LevelData GetLevel(int index)
        {
            return GetItem(string.Format(k_levelFormat, index));
        }

        /// @param index
        ///     The index of the level to get
        ///     
        /// @return The level data for that index, or null
        /// 
        public void SaveLevelData(LevelData levelData)
        {
            // Save the level data on disk
            string levelID = string.Format(k_levelFormat, levelData.m_index);
            var filePath = string.Format(k_savePath, Application.dataPath, k_jsonMetadataFolderPath, levelID);
            var dataToWrite = levelData.SerializeToString();
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataToWrite);
                }
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            // Add the level to the loader if needed
            if(HasItem(levelID) == false)
            {
                AddItem(levelData);
            }
        }
        #endregion
    }
}
