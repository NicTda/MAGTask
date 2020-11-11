//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Local director for the Level scene
    /// 
    public sealed class LevelLocalDirector : LocalDirector
    {
        public static string s_sceneExit = string.Empty;
        public static LevelData s_levelData = null;

        [SerializeField]
        private LevelView m_view = null;

        #region LocalDirector functions
        /// Register local services
        /// 
        protected override void OnRegisteringLocalServices()
        {
            m_factorySupplier.RegisterFactory<TileFactory>();
            m_serviceSupplier.RegisterService<ObjectiveService>();
        }

        /// Entry point for the initialised state
        /// 
        protected override void StartInitialisedState()
        {
            // Create the level controller
            var levelController = new LevelController(this, m_view);
            if(s_sceneExit != string.Empty)
            {
                levelController.SetExitScene(s_sceneExit);
                s_sceneExit = string.Empty;
            }
        }
        #endregion
    }
}