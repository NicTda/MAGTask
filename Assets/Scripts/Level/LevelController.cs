//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGTask
{
    /// Controller of the Level scene
    /// 
    public sealed class LevelController : SceneFSMController
    {
        private const string k_slotPrefab = "Prefabs/Tiles/TileSlot";
        private const string k_slotName = "Slot{0}";

        private const string k_actionNext = "Next";
        private const string k_actionIdle = "Idle";
        private const string k_actionResolve = "Resolve";
        private const string k_actionWin = "Win";
        private const string k_actionLose = "Lose";

        private const string k_stateLoad = "Load";
        private const string k_stateIdle = "Idle";
        private const string k_stateResolve = "Resolve";
        private const string k_stateWin = "Win";
        private const string k_stateLose = "Lose";

        private const int k_minActiveTiles = 3;

        private LevelView m_view = null;

        private LevelDataLoader m_levelLoader = null;
        private LevelData m_levelData = null;

        private List<TileView> m_tiles = new List<TileView>();
        private List<TileView> m_selectedTiles = new List<TileView>();
        private Coroutine m_coroutine = null;
        private bool m_interacting = false;

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

            m_levelLoader = GlobalDirector.Service<MetadataService>().GetLoader<LevelData>() as LevelDataLoader;

            m_audioService.PlayMusicFadeCross(AudioIdentifiers.k_musicLevel);

            m_fsm.RegisterStateCallback(k_stateLoad, EnterStateLoad, null, ExitStateLoad);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.RegisterStateCallback(k_stateResolve, EnterStateResolve, null, ExitStateResolve);
            m_fsm.RegisterStateCallback(k_stateWin, EnterStateWin, null, ExitStateWin);
            m_fsm.RegisterStateCallback(k_stateLose, EnterStateLose, null, ExitStateLose);
            m_fsm.ExecuteAction(k_actionNext);
        }

        /// OnDispose method
        /// 
        public override void OnDispose()
        {
            m_coroutine.Stop();
            base.OnDispose();
        }
        #endregion

        #region FSM functions
        /// Start of the Load state
        /// 
        private void EnterStateLoad()
        {
            // TODO TDA: level ID from current progression
            // Load level data
            m_levelData = m_levelLoader.GetItem("Level0");
            m_tiles.Capacity = m_levelData.m_height * m_levelData.m_width;
            m_selectedTiles.Capacity = m_levelData.m_height * m_levelData.m_width;

            // Create the level
            m_coroutine = GlobalDirector.ExecuteCoroutine(CreateLevel( () =>
            {
                // We're done loading
                m_fsm.ExecuteAction(k_actionNext);
            }));
        }

        /// @param callback
        ///     The function to call when the level is created
        ///     
        private IEnumerator CreateLevel(Action callback)
        {
            // TODO TDA: eventually, allow irregular boards, taken from data
            // Create the board, populate tiles
            int index = 0;
            Vector3 position = new Vector3(0.0f, m_levelData.m_height * 0.5f - 0.5f);
            for (int row = 0; row < m_levelData.m_height; ++row)
            {
                position.x = -m_levelData.m_width * 0.5f + 0.5f;
                for (int col = 0; col < m_levelData.m_width; ++col)
                {
                    // Create the tile view
                    var tileObject = ResourceUtils.LoadAndInstantiateGameObject(k_slotPrefab, m_view.TilesHolder, string.Format(k_slotName, index));
                    tileObject.transform.position = position;
                    var tileView = tileObject.GetComponent<TileView>();
                    tileView.m_index = index;
                    m_tiles.Add(tileView);

                    position.x += 1.0f;
                    ++index;
                }
                position.y -= 1.0f;
            }

            // Stagger the appear animations
            foreach(var tile in m_tiles)
            {
                tile.Appear();
                yield return null;
            }

            callback.SafeInvoke();
        }

        /// End of the Load state
        /// 
        private void ExitStateLoad()
        {
        }

        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            // Wait for player input
            RegisterBackButton();
            RegisterTilesInput();
            m_view.OnInteractStarted += OnInteractStarted;
            m_view.OnInteractEnded += OnInteractEnded;
        }

        /// Called when the player starts interacting with the board
        /// 
        public void OnInteractStarted()
        {
            m_interacting = true;
        }

        /// Called when the player ends interacting with the board
        /// 
        public void OnInteractEnded()
        {
            m_interacting = false;
            if(m_selectedTiles.Count >= k_minActiveTiles)
            {
                // Resolve the linking!
                m_fsm.ExecuteAction(k_actionResolve);
            }
            else
            {
                // Deselect tiles
                foreach (var tile in m_selectedTiles)
                {
                    tile.Deselect();
                }
                m_selectedTiles.Clear();
            }
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            UnregisterBackButton();
            UnregisterTilesInput();
            m_view.OnInteractStarted -= OnInteractStarted;
            m_view.OnInteractEnded -= OnInteractEnded;
        }

        /// Start of the Resolve state
        /// 
        private void EnterStateResolve()
        {
            // Pop the tiles
            foreach(var tile in m_selectedTiles)
            {
                // TODO TDA: Add score
                tile.Pop();
            }

            // Add new tiles
            m_selectedTiles.Clear();

            // Check objectives
            {
                m_fsm.ExecuteAction(k_actionIdle);
            }
        }

        /// End of the Resolve state
        /// 
        private void ExitStateResolve()
        {
        }

        /// Start of the Win state
        /// 
        private void EnterStateWin()
        {
            // Ceremony

            // Reward

            // Next level or back to map
        }

        /// End of the Win state
        /// 
        private void ExitStateWin()
        {
        }

        /// Start of the Lose state
        /// 
        private void EnterStateLose()
        {
            // Ceremony

            // Retry level or back to map
        }

        /// End of the Lose state
        /// 
        private void ExitStateLose()
        {
        }
        #endregion

        #region Private functions
        /// Listens to the tiles input
        /// 
        private void RegisterTilesInput()
        {
            foreach(var tile in m_tiles)
            {
                tile.OnTouchEnter += OnTileEntered;
                tile.OnTouchExit += OnTileExited;
            }
        }

        /// Listens to the tiles input
        /// 
        private void UnregisterTilesInput()
        {
            foreach (var tile in m_tiles)
            {
                tile.OnTouchEnter -= OnTileEntered;
                tile.OnTouchExit -= OnTileExited;
            }
        }

        /// @param tile
        ///     The tile entered
        /// 
        private void OnTileEntered(TileView tile)
        {
            if(m_interacting == true)
            {
                if(m_selectedTiles.Contains(tile) == true)
                {
                    // Check if we should cancel a tile
                    // TODO TDA: either cut the trailing tiles (hard cancel)
                    for(int index = m_selectedTiles.Count - 1; index >=0; --index)
                    {
                        if(m_selectedTiles[index] == tile)
                        {
                            break;
                        }

                        m_selectedTiles[index].Deselect();
                        m_selectedTiles.RemoveAt(index);
                    }

                    // TODO TDA: or only cut the last tile if the entered tile is n-1 (soft cancel)
                }
                else if (m_selectedTiles.Count == 0)
                {
                    // Add the first tile to the selected list
                    SelectTile(tile);
                }
                else
                {
                    // TODO TDA: Check if the tile type is valid (for now, the same)
                    // TODO TDA: Check if they are neighbours
                    SelectTile(tile);
                }
            }
        }

        /// @param tile
        ///     The tile exited
        /// 
        private void SelectTile(TileView tile)
        {
            tile.Select();
            m_selectedTiles.Add(tile);
            Debug.LogError("Selected " + tile.m_index);
        }

        /// @param tile
        ///     The tile exited
        /// 
        private void DeselectTile(TileView tile)
        {
            tile.Deselect();
            m_selectedTiles.Remove(tile);
            Debug.LogError("Deselected " + tile.m_index);
        }

        /// @param tile
        ///     The tile exited
        /// 
        private void OnTileExited(TileView tile)
        {
            if (m_interacting == true)
            {
                // TODO TDA: do a bounce
            }
        }
        #endregion
    }
}
