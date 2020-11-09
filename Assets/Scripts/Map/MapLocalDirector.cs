//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// LocalDirector for the Map scene
    /// 
	public sealed class MapLocalDirector : LocalDirector
    {
        [SerializeField]
        private MapView m_view = null;
        [SerializeField]
        private CameraView m_cameraView = null;
        [SerializeField]
        private BoxCollider2D m_cameraBounds = null;

        #region Director functions
        /// Entry point for the initialised state
        /// 
        protected override void StartInitialisedState()
        {
            // Create the map controller
            var cameraController = new CameraController(this, m_cameraView, m_cameraBounds);
            new MapController(this, m_view, cameraController);
        }
        #endregion
    }
}
