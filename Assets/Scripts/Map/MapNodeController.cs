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
        private const string k_actionOpen = "Open";
        private const string k_actionUnlock = "Unlock";
        private const string k_actionComplete = "Complete";

        private const string k_stateInit = "Init";
        private const string k_stateIdle = "Idle";
        private const string k_stateLocked = "Locked";
        private const string k_stateUnlock = "Unlock";
        private const string k_stateDone = "Done";

        public MapNodeView MapNodeView { get; private set; } = null;
        public LevelModel LevelModel { get; private set; } = null;

        private PopupService m_popupService = null;
        private LevelService m_levelService = null;

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
            m_popupService = GlobalDirector.Service<PopupService>();
            m_levelService = GlobalDirector.Service<LevelService>();

            MapNodeView = view;
            LevelModel = m_levelService.GetLevelModel(MapNodeView.LevelIndex);

            m_fsm.RegisterStateCallback(k_stateInit, EnterStateInit, null, null);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.RegisterStateCallback(k_stateLocked, EnterStateLocked, null, ExitStateLocked);
            m_fsm.RegisterStateCallback(k_stateUnlock, null, null, ExitStateUnlock);
            m_fsm.RegisterStateCallback(k_stateDone, EnterStateDone, null, ExitStateDone);
            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Trigger the unlock of the node
        /// 
        public void TriggerUnlock()
        {
            if(m_levelService.HasLevel(LevelModel.m_index) == true)
            {
                m_fsm.ExecuteAction(k_actionUnlock);
            }
        }
        #endregion

        #region FSM functions
        /// Start of the Init state
        /// 
        private void EnterStateInit()
        {
            // Check the level's progress
            var nodeState = m_levelService.GetLevelState(MapNodeView.LevelIndex);
            if (nodeState == NodeState.Completed)
            {
                m_fsm.ExecuteAction(k_actionComplete);
            }
            else if (nodeState == NodeState.Open)
            {
                m_fsm.ExecuteAction(k_actionOpen);
            }
            else
            {
                m_fsm.ExecuteAction(k_actionLock);
            }
        }

        /// Start of the Locked state
        /// 
        private void EnterStateLocked()
        {
            MapNodeView.OnRequested += OnLockedTapped;
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
            MapNodeView.OnRequested -= OnLockedTapped;
        }

        /// End of the Unlock state
        /// 
        private void ExitStateUnlock()
        {
            // Save the level state
            m_levelService.OpenLevel(LevelModel.m_index);

            // Particles
            ParticleUtils.SpawnParticles(ParticleIdentifiers.k_starburst, MapNodeView.transform.position);
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            MapNodeView.OnRequested += OnPlayLevelRequested;
        }

        private void OnPlayLevelRequested()
        {
            // Start the level
            m_levelService.PlayLevel(MapNodeView.LevelIndex);
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            MapNodeView.OnRequested -= OnPlayLevelRequested;
        }

        /// Start of the Done state
        /// 
        private void EnterStateDone()
        {
            MapNodeView.OnRequested += OnPlayLevelRequested;
        }

        /// End of the Done state
        /// 
        private void ExitStateDone()
        {
            MapNodeView.OnRequested -= OnPlayLevelRequested;
        }
        #endregion
    }
}