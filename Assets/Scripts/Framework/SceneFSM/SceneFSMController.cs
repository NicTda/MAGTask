//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;

namespace CoreFramework
{
    /// FSM controller for a scene
    ///
	public abstract class SceneFSMController : FSMController
    {
        protected SceneFSMView m_sceneView = null;

        protected AudioService m_audioService = null;
        protected InputService m_inputService = null;
        protected SceneService m_sceneService = null;

        protected string m_exitSceneID = string.Empty;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param sceneView
        ///     The view component of the scene
        /// @param exitSceneID
        ///     The ID of the scene to exit to
        /// 
        public SceneFSMController(LocalDirector localDirector, SceneFSMView sceneView, string exitSceneID = "")
            : base(localDirector, sceneView)
        {
            m_sceneView = sceneView;
            m_exitSceneID = exitSceneID;

            m_audioService = GlobalDirector.Service<AudioService>();
            m_inputService = GlobalDirector.Service<InputService>();
            m_sceneService = GlobalDirector.Service<SceneService>();
        }
        #endregion

        #region Controller functions
        /// OnDispose method
        /// 
        public override void OnDispose()
        {
            UnregisterBackButton();
        }
        #endregion

        #region Public functions
        /// @param exitSceneID
        ///     The ID of the scene to exit to
        /// 
        public void SetExitScene(string exitSceneID)
        {
            m_exitSceneID = exitSceneID;
        }
        #endregion

        #region Protected functions
        /// Call from derived class to register the back button calls
        /// 
        protected void RegisterBackButton()
        {
            RegisterBackButton(OnBackButtonRequest);
        }

        /// @param callback
        ///     The function to register
        /// 
        protected void RegisterBackButton(Action callback)
        {
            m_sceneView.OnBackRequested += callback;
            m_inputService.AddBackButtonListener(callback);
        }

        /// Call from derived class to unregister the back button calls
        /// 
        protected void UnregisterBackButton()
        {
            UnregisterBackButton(OnBackButtonRequest);
        }

        /// @param callback
        ///     The function to unregister
        /// 
        protected void UnregisterBackButton(Action callback)
        {
            m_sceneView.OnBackRequested -= callback;
            m_inputService.RemoveBackButtonListener(callback);
        }

        /// Called when the player wants to go back
        /// 
        protected virtual void OnBackButtonRequest()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonBack);
            if (m_exitSceneID == string.Empty)
            {
                m_sceneService.UnloadScene();
            }
            else
            {
                m_sceneService.SwitchToScene(m_exitSceneID);
            }
        }
        #endregion
    }
}
