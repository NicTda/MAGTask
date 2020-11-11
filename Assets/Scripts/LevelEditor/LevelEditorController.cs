//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System.Collections.Generic;
using UnityEngine;

namespace MAGTask
{
    /// Controller of the LevelEditor scene
    /// 
    public sealed class LevelEditorController : SceneFSMController
    {
        private const string k_togglePrefab = "Prefabs/TileToggle";

        private const string k_actionNext = "Next";
        private const string k_actionLoad = "Load";

        private const string k_stateInit = "Init";
        private const string k_stateLoad = "Load";
        private const string k_stateIdle = "Idle";

        private const string k_boardSizeText = "Width: {0}\nHeight: {1}";
        private const string k_movesText = "Max moves: {0}";
        private const string k_tileText = "<sprite name=\"{0}\">";

        private const int k_boardMin = 3;
        private const int k_boardMax = 9;

        private static LevelData s_cachedLevel = null;
        private static LevelData s_levelData = null;
        private static bool s_unsavedChanged = false;

        private LevelEditorView m_view = null;

        private PopupService m_popupService = null;
        private LevelService m_levelService = null;
        private LevelDataLoader m_levelLoader = null;

        private Dictionary<TileColour, TileToggleView> m_tileToggles = new Dictionary<TileColour, TileToggleView>();

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view of the scene
        /// @param cameraController
        ///     The camera controller
        /// 
        public LevelEditorController(LocalDirector localDirector, LevelEditorView view)
            : base(localDirector, view)
        {
            m_view = view;

            m_popupService = GlobalDirector.Service<PopupService>();
            m_levelService = GlobalDirector.Service<LevelService>();
            m_levelLoader = GlobalDirector.Service<MetadataService>().GetLoader<LevelData>() as LevelDataLoader;

            m_fsm.RegisterStateCallback(k_stateInit, EnterStateInit, null, null);
            m_fsm.RegisterStateCallback(k_stateLoad, EnterStateLoad, null, null);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.ExecuteAction(k_actionNext);
        }
        #endregion

        #region FSM functions
        /// Start of the Init state
        /// 
        private void EnterStateInit()
        {
            // Load the level
            if(s_levelData == null)
            {
                s_levelData = m_levelLoader.GetLevel(m_view.LevelIndex).Clone();
            }

            // Create the possible tiles
            foreach(var tile in TileIdentifiers.k_availableTiles)
            {
                // Create the tile toggle
                var toggleObject = ResourceUtils.LoadAndInstantiateGameObject(k_togglePrefab, m_view.TilesHolder, tile.ToString());
                var toggleView = toggleObject.GetComponent<TileToggleView>();
                toggleView.SetTileColour(tile, string.Format(k_tileText, tile.ToString()));
                m_tileToggles.Add(tile, toggleView);
            }

            m_fsm.ExecuteAction(k_actionLoad);
        }

        /// Start of the Load state
        /// 
        private void EnterStateLoad()
        {
            // Refresh display
            RefreshDisplay(s_levelData);

            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            m_view.OnPlayRequested += OnPlayRequested;
            m_view.OnLoadRequested += OnLoadRequested;
            m_view.OnSaveRequested += OnSaveRequested;
            m_view.OnSizeRequested += OnSizeRequested;
            m_view.OnPreviousRequested += OnPreviousRequested;
            m_view.OnNextRequested += OnNextRequested;
            m_view.OnMovesRequested += OnMovesRequested;
            m_view.OnScoreRequested += OnScoreRequested;

            foreach (var objective in m_view.Objectives)
            {
                objective.OnObjectiveChanged += OnObjectiveRequested;
            }
            foreach (var pair in m_tileToggles)
            {
                pair.Value.OnToggled += OnTileToggled;
            }
        }

        /// @param sizeDelta
        ///     The change in board size
        /// 
        private void OnSizeRequested(int width, int height)
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            // Update the size of the board
            var newWidth = Mathf.Clamp(s_levelData.m_width + width, k_boardMin, k_boardMax);
            var newHeight = Mathf.Clamp(s_levelData.m_height + height, k_boardMin, k_boardMax);
            if (newWidth != s_levelData.m_width || newHeight != s_levelData.m_height)
            {
                s_unsavedChanged = true;
                s_levelData.m_width = newWidth;
                s_levelData.m_height = newHeight;
                m_view.SetBoardSize(string.Format(k_boardSizeText, s_levelData.m_width, s_levelData.m_height));
            }
        }

        /// @param amount
        ///     The change in maximum moves allowed
        /// 
        private void OnMovesRequested(int amount)
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            // Update the maximum moves
            var newMoves = Mathf.Max(s_levelData.m_moves + amount, 1);
            if(newMoves != s_levelData.m_moves)
            {
                s_unsavedChanged = true;
                s_levelData.m_moves = newMoves;
                m_view.SetMoves(string.Format(k_movesText, s_levelData.m_moves));
            }
        }

        /// @param amount
        ///     The change in maximum moves allowed
        /// 
        private void OnScoreRequested(int index, int amount)
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            // Update the maximum moves
            if(s_levelData.m_scores[index] != amount)
            {
                s_unsavedChanged = true;
                s_levelData.m_scores[index] = amount;
                m_view.SetScore(index, amount);
            }
        }

        /// @param index
        ///     The index of the objective
        /// @param data
        ///     The data of the objective
        /// 
        private void OnObjectiveRequested(int index, ObjectiveData data)
        {
            // Update the objective
            s_unsavedChanged = true;
            if(s_levelData.m_objectives.Count <= index)
            {
                s_levelData.m_objectives.Add(new ObjectiveData());
            }
            s_levelData.m_objectives[index] = data;
        }

        /// @param tileColour
        ///     The tile colour toggled
        /// @param enabled
        ///     Whether the tile is enabled
        /// 
        private void OnTileToggled(TileColour tileColour, bool enabled)
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            // Update the tiles
            if(enabled == false && s_levelData.m_tiles.Contains(tileColour) == true)
            {
                s_unsavedChanged = true;
                s_levelData.m_tiles.Remove(tileColour);
            }
            else if(enabled == true && s_levelData.m_tiles.Contains(tileColour) == false)
            {
                s_unsavedChanged = true;
                s_levelData.m_tiles.Add(tileColour);
            }
        }

        /// Called when the player presses the Play button
        /// 
        private void OnPlayRequested()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            // Choose level to test from the data
            LevelLocalDirector.s_levelData = s_levelData;
            LevelLocalDirector.s_sceneExit = SceneIdentifiers.k_levelEditor;
            m_sceneService.SwitchToScene(SceneIdentifiers.k_level);
        }

        /// Called when the player presses the Previous button
        /// 
        private void OnPreviousRequested()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            if(s_levelData.m_index > 0)
            {
                OnLoadRequested(s_levelData.m_index - 1);
            }
        }

        /// Called when the player presses the Next button
        /// 
        private void OnNextRequested()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            if (m_levelService.HasLevel(s_levelData.m_index) == true)
            {
                OnLoadRequested(s_levelData.m_index + 1);
            }
        }

        /// @param levelIndex
        ///     The level to load
        /// 
        private void OnLoadRequested(int levelIndex)
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            if(s_unsavedChanged == true)
            {
                // Popup to confirm loading
                var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameQuestion) as PopupYesNoView;
                popupView.SetBodyText("You have unsaved changed for level {0}. Do you want to discard them and load Level {1}?", s_levelData.m_index, levelIndex);
                popupView.OnPopupConfirmed += () =>
                {
                    // Retrieve cached level
                    s_levelData = s_cachedLevel;
                    s_cachedLevel = null;

                    // Load the level data
                    LoadLevel(levelIndex);
                };
            }
            else
            {
                // Load the level data
                LoadLevel(levelIndex);
            }
        }

        /// Called when the player presses the Save button
        /// 
        private void OnSaveRequested()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);

            if(s_unsavedChanged == true)
            {
                // Popup to confirm save
                var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameQuestion) as PopupYesNoView;
                popupView.SetBodyText("Override level {0}?", m_view.LevelIndex);
                popupView.OnPopupConfirmed += () =>
                {
                    // Save the level data on disk
                    s_unsavedChanged = false;
                    m_levelLoader.SaveLevelData(s_levelData);
                };
            }
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            m_view.OnPlayRequested -= OnPlayRequested;
            m_view.OnLoadRequested -= OnLoadRequested;
            m_view.OnSaveRequested -= OnSaveRequested;
            m_view.OnSizeRequested -= OnSizeRequested;
            m_view.OnPreviousRequested -= OnPreviousRequested;
            m_view.OnNextRequested -= OnNextRequested;
            m_view.OnMovesRequested -= OnMovesRequested;
            m_view.OnScoreRequested -= OnScoreRequested;

            foreach (var objective in m_view.Objectives)
            {
                objective.OnObjectiveChanged -= OnObjectiveRequested;
            }

            foreach (var pair in m_tileToggles)
            {
                pair.Value.OnToggled -= OnTileToggled;
            }
        }
        #endregion

        #region Private functions
        /// @param levelData
        ///     The data of the level
        /// 
        private void RefreshDisplay(LevelData levelData)
        {
            m_view.LevelIndex = levelData.m_index;
            m_view.SetLevelName(string.Format(GameTextIdentifiers.k_levelDisplay, levelData.m_index));
            m_view.SetBoardSize(string.Format(k_boardSizeText, levelData.m_width, levelData.m_height));
            m_view.SetMoves(string.Format(k_movesText, levelData.m_moves));
            for (int index = 0; index < levelData.m_scores.Count; ++index)
            {
                m_view.SetScore(index, levelData.m_scores[index]);
            }
            foreach (var tile in TileIdentifiers.k_availableTiles)
            {
                if(m_tileToggles.ContainsKey(tile) == true)
                {
                    bool enabled = levelData.m_tiles.Contains(tile);
                    m_tileToggles[tile].SetOn(enabled);
                }
            }
            for (int index = 0; index < levelData.m_objectives.Count; ++index)
            {
                m_view.SetObjective(index, levelData.m_objectives[index]);
            }
        }

        /// @param levelIndex
        ///     The level to load
        /// 
        private void LoadLevel(int levelIndex)
        {
            // Load the level data
            s_unsavedChanged = false;
            var levelData = m_levelLoader.GetLevel(levelIndex);

            // If there is no such level, create it
            if (levelData == null)
            {
                levelData = new LevelData()
                {
                    m_id = LevelData.k_levelPrefix + levelIndex,
                    m_index = levelIndex
                };
            }
            s_levelData = levelData.Clone();

            // Reload the screen
            m_fsm.ExecuteAction(k_actionLoad);
        }
        #endregion
    }
}
