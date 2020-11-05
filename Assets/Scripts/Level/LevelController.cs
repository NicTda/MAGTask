//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;

namespace MAGTask
{
    /// Controller of the Level scene
    /// 
    public sealed class LevelController : SceneFSMController
    {
        private const string k_actionNext = "Next";

        private const string k_stateIdle = "Idle";

        private LevelView m_view = null;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view of the scene
        /// @param cameraController
        ///     The camera controller
        /// 
        public LevelController(LocalDirector localDirector, LevelView view)
            : base(localDirector, view, SceneIdentifiers.k_main)
        {
            m_view = view;

            m_audioService.PlayMusicFadeCross(AudioIdentifiers.k_musicLevel);

            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.ExecuteAction(k_actionNext);
        }
        #endregion

        #region FSM functions
        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            RegisterBackButton();
            m_view.OnPlayRequested += OnPlayRequested;
        }

        /// Called when the player presses the Play button
        /// 
        private void OnPlayRequested()
        {
            m_sceneService.SwitchToScene(SceneIdentifiers.k_level);
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            UnregisterBackButton();
            m_view.OnPlayRequested -= OnPlayRequested;
        }
        #endregion
    }
}
