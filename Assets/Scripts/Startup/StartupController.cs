//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;

namespace MAGTask
{
    /// Controller of the Startup scene
    /// 
    public sealed class StartupController : SceneFSMController
    {
        private const string k_actionNext = "Next";

        private const string k_stateLoad = "Load";
        private const string k_stateOutro = "Outro";

        private StartupView m_view = null;

        private SaveService m_saveService = null;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view of the scene
        /// @param cameraController
        ///     The camera controller
        /// 
        public StartupController(LocalDirector localDirector, StartupView view)
            : base(localDirector, view)
        {
            m_view = view;

            m_saveService = GlobalDirector.Service<SaveService>();

            m_audioService.PlayMusic(AudioIdentifiers.k_musicMain);

            m_fsm.RegisterStateCallback(k_stateLoad, EnterStateLoad, null, null);
            m_fsm.RegisterStateCallback(k_stateOutro, EnterStateOutro, null, null);
            m_fsm.ExecuteAction(k_actionNext);
        }
        #endregion

        #region FSM functions
        /// Start of the Load state
        /// 
        private void EnterStateLoad()
        {
            m_saveService.LoadCachedData(() =>
            {
                m_fsm.ExecuteAction(k_actionNext);
            });
        }

        /// Start of the Outro state
        /// 
        private void EnterStateOutro()
        {
            m_view.OnOutroFinished += OnOutroFinished;
        }

        /// Called when the outro animation is done
        /// 
        private void OnOutroFinished()
        {
            m_sceneService.SwitchToScene(SceneIdentifiers.k_main);
        }
        #endregion
    }
}
