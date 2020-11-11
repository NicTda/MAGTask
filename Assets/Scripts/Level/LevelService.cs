//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using CoreFramework.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MAGTask
{
    /// Service that handles the level progression
    /// 
	public sealed class LevelService : Service, ISavable
    {
        private const string k_keyLevels = "Levels";

        public bool m_loaded { get; set; }

        private SceneService m_sceneService = null;
        private LevelDataLoader m_levelLoader = null;

        private List<LevelModel> m_levels = new List<LevelModel>();

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            m_sceneService = GlobalDirector.Service<SceneService>();
            m_levelLoader = GlobalDirector.Service<MetadataService>().GetLoader<LevelData>() as LevelDataLoader;
            this.RegisterCaching();
        }

        /// @return The serialized data
        /// 
        public object Serialize()
        {
            var jsonData = new JsonDictionary();
            if (m_levels.Count > 0)
            {
                jsonData.Add(k_keyLevels, JsonWrapper.SerializeListOfSerializables(m_levels));
            }
            return jsonData;
        }

        /// @param data
        /// 	The json data
        /// 
        public void Deserialize(object data)
        {
            var jsonData = data.AsDictionary();
            if(jsonData.ContainsKey(k_keyLevels))
            {
                int index = 0;
                var levelData = jsonData[k_keyLevels] as IEnumerable;
                foreach (var value in levelData)
                {
                    LevelModel level = value.AsSerializable<LevelModel>();
                    level.m_index = index++;
                    m_levels.Add(level);
                }
            }

            if(m_levels.Count == 0)
            {
                // Ensure the first level is accessible
                m_levels.Add(new LevelModel()
                {
                    m_nodeState = NodeState.Open
                });
            }
            m_loaded = true;
        }

        /// Clears the local data
        ///     
        public void Clear()
        {
            m_levels.Clear();
            m_loaded = false;
        }
        #endregion

        #region Public functions
        /// @param levelIndex
        ///     The index of the level to check
        ///     
        /// @return Whether the level data exists
        /// 
        public bool HasLevel(int levelIndex)
        {
            return m_levelLoader.GetLevel(levelIndex) != null;
        }

        /// @param levelIndex
        ///     The index of the level to retrieve
        ///     
        /// @return The data of the level
        /// 
        public LevelData GetLevelData(int levelIndex)
        {
            return m_levelLoader.GetLevel(levelIndex);
        }

        /// @param levelIndex
        ///     The index of the level to retrieve
        ///     
        /// @return The model of the level
        /// 
        public LevelModel GetLevelModel(int levelIndex)
        {
            while (levelIndex >= m_levels.Count)
            {
                m_levels.Add(new LevelModel()
                {
                    m_index = m_levels.Count
                });
            }
            return m_levels[levelIndex];
        }

        /// @param levelIndex
        ///     The level to unlock
        ///     
        public void UnlockLevel(int levelIndex)
        {
            var level = GetLevelModel(levelIndex);
            if(level.m_nodeState == NodeState.Locked)
            {
                level.m_nodeState = NodeState.Unlocked;
                this.Save();
            }
        }

        /// @param levelIndex
        ///     The level to open
        ///     
        public void OpenLevel(int levelIndex)
        {
            var level = GetLevelModel(levelIndex);
            if (level.m_nodeState != NodeState.Open)
            {
                m_levels[levelIndex].m_nodeState = NodeState.Open;
                this.Save();
            }
        }

        /// @param levelIndex
        ///     The level to update
        /// @param score
        ///     The score to set
        /// 
        public void CompleteLevel(int levelIndex, int score)
        {
            var level = GetLevelModel(levelIndex);
            if (level.m_highscore < score)
            {
                level.m_highscore = score;
                level.m_nodeState = NodeState.Completed;
                this.Save();
            }
        }

        /// @param levelIndex
        ///     The level to check
        ///     
        /// @return The current map state of the level
        /// 
        public NodeState GetLevelState(int levelIndex)
        {
            var level = GetLevelModel(levelIndex);
            return level.m_nodeState;
        }

        /// @param levelIndex
        ///     The index of the level to play
        /// @param callback
        ///     The function to call if the level is starting
        /// 
        public void RequestLevel(int levelIndex, Action callback = null)
        {
            LevelLocalDirector.s_levelData = m_levelLoader.GetLevel(levelIndex);

            // TODO TDA: show the level Popup
            callback.SafeInvoke();

            m_sceneService.SwitchToScene(SceneIdentifiers.k_level);
        }
        #endregion
    }
}
