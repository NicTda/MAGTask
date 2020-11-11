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
        public static int s_levelIndex = -1;

        [SerializeField]
        private LevelView m_view = null;
        [SerializeField]
        private int m_levelIndex = 0;

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
            if(s_levelIndex < 0)
            {
                s_levelIndex = m_levelIndex;
            }

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