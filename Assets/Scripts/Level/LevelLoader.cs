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

        #region MetadataLoader functions
        /// Initialisation function
        /// 
        protected override void Init()
        {
            m_dataPath = k_jsonMetadataFolderPath;
            m_loaderBehaviour = LoaderBehaviour.LoadAllAtInit;
        }
        #endregion
    }
}
