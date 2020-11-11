//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;

namespace MAGTask
{
    /// Controller of the Main scene
    /// 
    public sealed class MainController : SceneFSMController
    {
        private const string k_actionNext = "Next";

        private const string k_stateInit = "Init";
        private const string k_stateIdle = "Idle";

        private MainView m_view = null;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view of the scene
        /// @param cameraController
        ///     The camera controller
        /// 
        public MainController(LocalDirector localDirector, MainView view)
            : base(localDirector, view)
        {
            m_view = view;

            m_audioService.PlayMusicFadeCross(AudioIdentifiers.k_musicMain);

            m_fsm.RegisterStateCallback(k_stateInit, EnterStateInit, null, null);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.ExecuteAction(k_actionNext);
        }
        #endregion

        #region FSM functions
        /// Start of the Init state
        /// 
        private void EnterStateInit()
        {
            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            m_view.OnPlayRequested += OnPlayRequested;
        }

        /// Called when the player presses the Play button
        /// 
        private void OnPlayRequested()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);
            m_sceneService.SwitchToScene(SceneIdentifiers.k_map);
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            m_view.OnPlayRequested -= OnPlayRequested;
        }
        #endregion
    }
}
