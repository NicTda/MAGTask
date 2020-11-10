//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System.Collections.Generic;

namespace MAGTask
{
    /// Controller of the Map scene
    /// 
    public sealed class MapController : SceneFSMController
    {
        private const string k_actionNext = "Next";
        private const string k_actionIdle = "Idle";

        private const string k_stateInit = "Init";
        private const string k_stateIdle = "Idle";

        private MapView m_view = null;
        private CameraController m_cameraController = null;

        private PopupService m_popupService = null;

        private List<MapNodeController> m_nodes = new List<MapNodeController>();
        private MapNodeController m_nodeToUnlock = null;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view of the scene
        /// @param cameraController
        ///     The camera controller
        /// 
        public MapController(LocalDirector localDirector, MapView view, CameraController cameraContoller)
            : base(localDirector, view, SceneIdentifiers.k_main)
        {
            m_view = view;
            m_cameraController = cameraContoller;

            m_popupService = GlobalDirector.Service<PopupService>();

            m_audioService.PlayMusicFadeCross(AudioIdentifiers.k_musicMain);

            m_fsm.RegisterStateCallback(k_stateInit, EnterStateInit, null, null);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Called when the player wants to go back to the main menu
        ///     
        protected override void OnBackButtonRequest()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonBack);
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameQuestionProminent) as PopupYesNoView;
            popupView.SetBodyText("Do you want to go back to the main menu?");
            popupView.OnPopupConfirmed += () =>
            {
                base.OnBackButtonRequest();
            };
        }

        /// Implement this method if the child class needs to be disposed
        /// 
        public override void OnDispose()
        {
            foreach(var node in m_nodes)
            {
                node.Dispose();
            }
            base.OnDispose();
        }
        #endregion

        #region FSM functions
        /// Start of the Init state
        /// 
        private void EnterStateInit()
        {
            // Initialise locations
            MapNodeView focusNode = m_view.Nodes.GetFirst();
            foreach (var nodeView in m_view.Nodes)
            {
                // Create the node controller
                var nodeController = new MapNodeController(m_localDirector, nodeView);
                m_nodes.Add(nodeController);

                if(nodeController.LevelModel != null)
                {
                    if(nodeController.LevelModel.m_nodeState == NodeState.Unlocked)
                    {
                        m_nodeToUnlock = nodeController;
                    }
                    else if (nodeController.LevelModel?.m_nodeState == NodeState.Open)
                    {
                        focusNode = nodeView;
                    }
                }
            }

            // Focus on current level
            m_cameraController.FocusOnTargetWithinBounds(focusNode.transform.position);

            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            RegisterBackButton();

            // TODO TDA: new state for ceremony: complete, then unlock
            if(m_nodeToUnlock != null)
            {
                m_cameraController.FocusOnTargetWithinBounds(m_nodeToUnlock.MapNodeView.transform.position);
                m_nodeToUnlock.TriggerUnlock();
            }
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            UnregisterBackButton();
        }
        #endregion
    }
}