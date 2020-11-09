//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;

namespace MAGTask
{
    /// Controller of a Map Node
    /// 
    public sealed class MapNodeController : FSMController
    {
        private const string k_actionNext = "Next";
        private const string k_actionLock = "Lock";
        private const string k_actionUnlock = "Unlock";
        private const string k_actionComplete = "Complete";

        private const string k_stateInit = "Init";
        private const string k_stateIdle = "Idle";
        private const string k_stateLocked = "Locked";
        private const string k_stateDone = "Done";

        private MapNodeView m_view = null;

        private PopupService m_popupService = null;
        private LevelDataLoader m_levelLoader = null;
        private GameProgressService m_progressService = null;

        private LevelData m_levelData = null;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view of the scene
        /// @param cameraController
        ///     The camera controller
        /// 
        public MapNodeController(LocalDirector localDirector, MapNodeView view)
            : base(localDirector, view)
        {
            m_view = view;

            m_popupService = GlobalDirector.Service<PopupService>();
            m_progressService = GlobalDirector.Service<GameProgressService>();
            m_levelLoader = GlobalDirector.Service<MetadataService>().GetLoader<LevelData>() as LevelDataLoader;

            m_fsm.RegisterStateCallback(k_stateInit, EnterStateInit, null, null);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.RegisterStateCallback(k_stateLocked, EnterStateLocked, null, ExitStateLocked);
            m_fsm.RegisterStateCallback(k_stateDone, EnterStateDone, null, ExitStateDone);
            m_fsm.ExecuteAction(k_actionNext);
        }
        #endregion

        #region FSM functions
        /// Start of the Init state
        /// 
        private void EnterStateInit()
        {
            m_levelData = m_levelLoader.GetLevel(m_view.LevelIndex);

            // Check the level's progress
            if(m_levelData == null || m_levelData.m_index > m_progressService.m_level)
            {
                m_fsm.ExecuteAction(k_actionLock);
            }
            else if (m_levelData.m_index == m_progressService.m_level)
            {
                m_fsm.ExecuteAction(k_actionUnlock);
            }
            else
            {
                m_fsm.ExecuteAction(k_actionComplete);
            }
        }

        /// Start of the Locked state
        /// 
        private void EnterStateLocked()
        {
            m_view.OnRequested += OnLockedTapped;
        }

        private void OnLockedTapped()
        {
            // Open a teaser popup
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameInfo);
            popupView.SetBodyText("This level is not available just yet!");
        }

        /// End of the Locked state
        /// 
        private void ExitStateLocked()
        {
            m_view.OnRequested -= OnLockedTapped;
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            m_view.OnRequested += OnPlayLevelRequested;
        }

        private void OnPlayLevelRequested()
        {
            // TODO TDA: Query the LevelService for this instead
            //LevelLocalDirector.s_levelIndex = itemView.LevelIndex;
            GlobalDirector.Service<SceneService>().SwitchToScene(SceneIdentifiers.k_level);
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            m_view.OnRequested -= OnPlayLevelRequested;
        }

        /// Start of the Done state
        /// 
        private void EnterStateDone()
        {
            m_view.OnRequested += OnPlayLevelRequested;
        }

        /// End of the Done state
        /// 
        private void ExitStateDone()
        {
            m_view.OnRequested -= OnPlayLevelRequested;
        }
        #endregion
    }
}