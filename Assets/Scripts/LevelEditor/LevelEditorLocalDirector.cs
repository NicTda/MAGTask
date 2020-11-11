//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Local director for the LevelEditor scene
    /// 
    public sealed class LevelEditorLocalDirector : LocalDirector
    {
        [SerializeField]
        private LevelEditorView m_view = null;

        #region LocalDirector functions
        /// Entry point for the initialised state
        /// 
        protected override void StartInitialisedState()
        {
            // Start the level edition
            new LevelEditorController(this, m_view);
        }
        #endregion
    }
}