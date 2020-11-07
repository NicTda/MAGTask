//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using DG.Tweening;
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
        private const float k_distanceTiles = 2;

        private LevelView m_view = null;

        private LevelDataLoader m_levelLoader = null;
        private TileFactory m_tileFactory = null;
        private LevelData m_levelData = null;

        private List<TileView> m_tiles = new List<TileView>();
        private List<TileView> m_selectedTiles = new List<TileView>();
        private Coroutine m_coroutine = null;
        private float m_spawnHeight = 0.0f;
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
            m_tileFactory = localDirector.GetFactory<TileFactory>();

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
            m_spawnHeight = m_levelData.m_height * 0.5f + 1.0f;

            // Create the level
            m_coroutine = GlobalDirector.ExecuteCoroutine(StaggerLevelCreation(() =>
            {
                // We're done loading
                m_fsm.ExecuteAction(k_actionNext);
            }));
        }

        /// @param callback
        ///     The function to call when the level is created
        ///     
        private IEnumerator StaggerLevelCreation(Action callback)
        {
            // TODO TDA: eventually, allow irregular boards, taken from data
            // Create the board, populate tiles
            Vector3 boardPosition = new Vector3(0.0f, m_levelData.m_height * 0.5f - 0.5f);
            for (int row = 0; row < m_levelData.m_height; ++row)
            {
                boardPosition.x = -m_levelData.m_width * 0.5f + 0.5f;
                for (int col = 0; col < m_levelData.m_width; ++col)
                {
                    // Create the tile view
                    var tileView = SpawnNewTile(boardPosition);
                    m_tiles.Add(tileView);

                    boardPosition.x += 1.0f;
                }
                boardPosition.y -= 1.0f;
            }

            // Stagger the appear animations
            foreach(var tile in m_tiles)
            {
                tile.Appear();
                yield return null;
            }

            callback.SafeInvoke();
        }

        /// @param boardPosition
        ///     The position the tile occupies on the board
        ///     
        private TileView SpawnNewTile(Vector3 boardPosition)
        {
            // Create the tile view at the top of the board
            var spawnPosition = boardPosition;
            spawnPosition.y = m_spawnHeight;
            var tileView = m_tileFactory.CreateTile(m_view.TilesHolder, spawnPosition);
            tileView.m_boardPosition = boardPosition;
            return tileView;
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
            m_coroutine = GlobalDirector.ExecuteCoroutine(StaggerTilesPop(() =>
            {
                // Add new tiles
                foreach (var tile in m_selectedTiles)
                {
                    // TODO TDA: move the tiles above it
                    // Spawn the new tile to replace this one
                    var tileView = SpawnNewTile(tile.m_boardPosition);
                    m_tiles.Add(tileView);
                    tileView.Appear();
                }
                m_selectedTiles.Clear();

                // TODO TDA: Check objectives
                m_fsm.ExecuteAction(k_actionIdle);
            }));
        }

        /// @param callback
        ///     The function to call when the level is created
        ///     
        private IEnumerator StaggerTilesPop(Action callback)
        {
            // Pop the tiles
            int popped = 0;
            foreach (var tile in m_selectedTiles)
            {
                // TODO TDA: Add SFX
                // TODO TDA: Add score
                // TODO TDA: Particles
                tile.Pop(() =>
                {
                    // Tile popped, remove it
                    m_tiles.Remove(tile);

                    // TODO TDA: Pool the objects
                    // For now, destroy / recreate game objects
                    MonoBehaviour.Destroy(tile.gameObject);
                    ++popped;
                });
                yield return null;
            }

            while(popped < m_selectedTiles.Count)
            {
                yield return null;
            }
            callback.SafeInvoke();
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
                if (m_selectedTiles.Count == 0)
                {
                    // Add the first tile to the selected list
                    SelectTile(tile);
                }
                else
                {
                    var lastIndex = m_selectedTiles.Count - 1;
                    var lastTile = m_selectedTiles[lastIndex];
                    if (m_selectedTiles.Contains(tile) == true)
                    {
                        /*/ Cut the trailing tiles (hard cancel)
                        for(int index = m_selectedTiles.Count - 1; index >=0; --index)
                        {
                            if(m_selectedTiles[index] == tile)
                            {
                                break;
                            }

                            m_selectedTiles[index].Deselect();
                            m_selectedTiles.RemoveAt(index);
                        }*/

                        // Check if we should cancel a tile
                        if (m_selectedTiles.Count > 1)
                        {
                            // Cut the last tile if the entered tile is n-1 (soft cancel)
                            if (tile == m_selectedTiles[lastIndex - 1])
                            {
                                m_selectedTiles[lastIndex].Deselect();
                                m_selectedTiles.RemoveAt(lastIndex);
                            }
                        }
                    }
                    else if(tile.m_tileColour == lastTile.m_tileColour)
                    {
                        // The tile type is valid, check if they're neighbours
                        var distance = (lastTile.transform.position - tile.transform.position).sqrMagnitude;
                        if(distance <= k_distanceTiles)
                        {
                            SelectTile(tile);
                        }
                    }
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
        }

        /// @param tile
        ///     The tile exited
        /// 
        private void DeselectTile(TileView tile)
        {
            tile.Deselect();
            m_selectedTiles.Remove(tile);
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
