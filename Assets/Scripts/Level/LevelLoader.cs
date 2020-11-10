//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;

namespace MAGTask
{
    /// Data loader for LevelData
    ///
    public sealed class LevelDataLoader : MetadataLoader<LevelData>
    {
        private const string k_jsonMetadataFolderPath = "Metadata/Levels";
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
        #endregion
    }
}
