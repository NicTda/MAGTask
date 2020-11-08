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
        [SerializeField]
        private LevelView m_view = null;

        #region LocalDirector functions
        /// Register local services
        /// 
        protected override void OnRegisteringLocalServices()
        {
            m_factorySupplier.RegisterFactory<TileFactory>();
        }

        /// Entry point for the initialised state
        /// 
        protected override void StartInitialisedState()
        {
            // Create the level controller
            new LevelController(this, m_view);
        }
        #endregion
    }
}