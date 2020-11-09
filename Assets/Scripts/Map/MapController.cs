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
        private LevelDataLoader m_levelLoader = null;

        private List<MapNodeController> m_nodes = new List<MapNodeController>();
        private bool m_nodeRequested = false;

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
            m_levelLoader = GlobalDirector.Service<MetadataService>().GetLoader<LevelData>() as LevelDataLoader;
            m_nodes.Capacity = m_levelLoader.GetAllIDs().Count;

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
            // Set the camera bounds
            m_cameraController.SetScreenBounds(m_view.CameraBounds.bounds.min, m_view.CameraBounds.bounds.max);

            // Initialise locations
            foreach (var nodeView in m_view.Nodes)
            {
                // Create the node controller
                var nodeController = new MapNodeController(m_localDirector, nodeView);
                m_nodes.Add(nodeController);
            }

            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            RegisterBackButton();
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            UnregisterBackButton();
        }
        #endregion

        #region Private functions
        /// @param itemView
        ///     The requested location
        ///     
        private void OnLocationRequested(MapNodeView itemView)
        {
            if (m_nodeRequested == false)
            {
                // Focus on the location
                m_cameraController.FocusOnTargetWithinBounds(itemView.transform.position);

                // Check if the location is enabled
                if (itemView.NodeType == NodeType.Level)
                {
                    var levelData = m_levelLoader.GetLevel(itemView.LevelIndex);
                    if (levelData != null)
                    {
                        // Play that level
                        m_nodeRequested = true;
                        LevelLocalDirector.s_levelIndex = itemView.LevelIndex;
                        m_sceneService.SwitchToScene(SceneIdentifiers.k_level);
                    }
                    else
                    {
                        // Open a teaser popup
                        var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameInfo);
                        popupView.SetBodyText("This level is not available just yet!");
                    }
                }
            }
        }
        #endregion
    }
}