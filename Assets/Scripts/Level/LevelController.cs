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

        private const int k_tileScore = 100;
        private const int k_minActiveTiles = 3;
        private const int k_extraMoves = 5;
        private const float k_distanceTiles = 2;

        private readonly Vector3 k_boardPosBack = Vector3.forward * 5;
        private readonly Vector3 k_boardPosFront = Vector3.back * 5;

        private LevelView m_view = null;
        private LevelData m_levelData = null;

        private LevelDataLoader m_levelLoader = null;
        private PopupService m_popupService = null;
        private TileFactory m_tileFactory = null;

        private List<TileView> m_tiles = new List<TileView>();
        private List<TileView> m_selectedTiles = new List<TileView>();
        private Coroutine m_coroutine = null;
        private float m_spawnHeight = 0.0f;
        private int m_previousScore = 0;
        private int m_currentScore = 0;
        private int m_movesLeft = 0;

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

            m_popupService = GlobalDirector.Service<PopupService>();
            m_tileFactory = localDirector.GetFactory<TileFactory>();
            m_levelLoader = GlobalDirector.Service<MetadataService>().GetLoader<LevelData>() as LevelDataLoader;
            m_levelData = m_levelLoader.GetLevel(LevelLocalDirector.s_levelIndex);
            m_view.ScoreView.InitialiseScores(m_levelData.m_scores);

            m_audioService.PlayMusicFadeCross(AudioIdentifiers.k_musicLevel);

            m_fsm.RegisterStateCallback(k_stateLoad, EnterStateLoad, null, ExitStateLoad);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.RegisterStateCallback(k_stateResolve, EnterStateResolve, null, ExitStateResolve);
            m_fsm.RegisterStateCallback(k_stateWin, EnterStateWin, null, ExitStateWin);
            m_fsm.RegisterStateCallback(k_stateLose, EnterStateLose, null, ExitStateLose);
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
            // Load level data
            m_tiles.Capacity = m_levelData.m_height * m_levelData.m_width;
            m_selectedTiles.Capacity = m_levelData.m_height * m_levelData.m_width;
            m_spawnHeight = m_levelData.m_height * 0.5f + 1.0f;
            SetMovesLeft(m_levelData.m_moves);
            
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

            // Choose a random tile colour
            TileColour randomColour = (TileColour)m_levelData.m_tiles.GetRandom();
            var tileView = m_tileFactory.CreateTile(randomColour, m_view.TilesHolder, spawnPosition);
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

            // TODO TDA: Check that the board has at least one valid move
            // TODO TDA: If not, reshuffle
        }

        /// Called when the player starts interacting with the board
        /// 
        public void OnInteractStarted()
        {
            // Pushes the board backwards to allow touch on the tiles
            m_view.BoardTouchArea.position = k_boardPosBack;
        }

        /// Called when the player ends interacting with the board
        /// 
        public void OnInteractEnded()
        {
            // Pushes the board forward to allow touch on the board
            m_view.BoardTouchArea.position = k_boardPosFront;
            if (m_selectedTiles.Count >= k_minActiveTiles)
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
                // Update score
                m_view.ScoreView.SetScore(m_currentScore);

                // Update moves
                SetMovesLeft(m_movesLeft - 1);

                m_coroutine = GlobalDirector.ExecuteCoroutine(StaggerReplaceTiles(() =>
                {
                    if (m_movesLeft <= 0)
                    {
                        // Player is out of move
                        if (IsLevelCompleted() == true)
                        {
                            // Level success
                            m_fsm.ExecuteAction(k_actionWin);
                        }
                        else
                        {
                            // Level fail
                            m_fsm.ExecuteAction(k_actionLose);
                        }
                    }
                    else
                    {
                        m_fsm.ExecuteAction(k_actionIdle);
                    }
                }));
            }));
        }

        /// @param callback
        ///     The function to call when the tiles are popped
        ///     
        private IEnumerator StaggerTilesPop(Action callback)
        {
            // Pop the tiles
            int popped = 0;
            foreach (var tile in m_selectedTiles)
            {
                // TODO TDA: Add Audio SFX
                // TODO TDA: Add defined score, and combo score?
                m_currentScore += k_tileScore;

                // Particle effect and score
                ParticleUtils.SpawnTextParticles(string.Format(GameTextIdentifiers.k_rewardFormat, k_tileScore), m_view.TilesHolder, tile.m_boardPosition);
                ParticleUtils.SpawnParticles(ParticleIdentifiers.k_tilePop, tile.m_boardPosition);

                tile.Pop(() =>
                {
                    // Tile popped, remove it
                    m_tiles.Remove(tile);
                    tile.gameObject.SetActive(false);
                    ++popped;
                });
                yield return new WaitForSeconds(0.1f);
            }

            while(popped < m_selectedTiles.Count)
            {
                yield return null;
            }
            callback.SafeInvoke();
        }

        /// @param callback
        ///     The function to call when the tiles are replaced
        ///     
        private IEnumerator StaggerReplaceTiles(Action callback)
        {
            // Add new tiles
            var tilesToAdd = new List<TileView>(m_selectedTiles.Count);
            int appeared = 0;
            foreach (var poppedTile in m_selectedTiles)
            {
                // Get all tiles above the popped one
                var newSpawnPosition = poppedTile.m_boardPosition;
                var tilesAbove = m_tiles.FindAll((tile) => tile.m_boardPosition.x == newSpawnPosition.x && tile.transform.position.y > newSpawnPosition.y);
                foreach (var tileAbove in tilesAbove)
                {
                    // Move them down one unit
                    tileAbove.m_boardPosition.y -= 1.0f;
                    tileAbove.Reposition();

                    // The new spawn should spawn higher than the popped tile
                    newSpawnPosition.y += 1.0f;
                }

                // Spawn the new tile to replace this one
                var tileView = SpawnNewTile(newSpawnPosition);
                tilesToAdd.Add(tileView);
                tileView.Appear(() =>
                {
                    ++appeared;
                });

                // For now, destroy game objects
                MonoBehaviour.Destroy(poppedTile.gameObject);

                yield return null;
            }

            while (appeared < m_selectedTiles.Count)
            {
                yield return null;
            }
            m_selectedTiles.Clear();
            m_tiles.AddRange(tilesToAdd);
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
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameInfo);
            popupView.SetBodyText("Well done! Level {0} completed!", m_levelData.m_index);
            popupView.OnPopupDismissed += (popup) =>
            {
                // TODO TDA: home, retry, next
                m_sceneService.SwitchToScene(SceneIdentifiers.k_map);
            };
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
            // Continue?
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameQuestion) as PopupYesNoView;
            popupView.SetBodyText("Out of moves! Do you want to continue playing by adding {0} moves?", k_extraMoves);
            popupView.OnPopupConfirmed += () =>
            {
                SetMovesLeft(k_extraMoves);
                m_fsm.ExecuteAction(k_actionIdle);
            };

            popupView.OnPopupCancelled += () =>
            {
                base.OnBackButtonRequest();
            };
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
                    // Cut the last tile if the entered tile is n-1 (soft cancel)
                    if (m_selectedTiles.Count > 1 && tile == m_selectedTiles[lastIndex - 1])
                    {
                        lastTile.Deselect();
                        m_selectedTiles.RemoveAt(lastIndex);
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
            // TODO TDA: do a bounce
        }

        /// @param moves
        ///     The moves to set
        /// 
        private void SetMovesLeft(int moves)
        {
            m_movesLeft = moves;
            m_view.SetMovesLeft(m_movesLeft.ToString());
        }

        /// @return Whether the level's objectives are done
        /// 
        private bool IsLevelCompleted()
        {
            // Check minimum score
            return m_currentScore >= m_levelData.m_scores.GetFirst();
        }
        #endregion
    }
}
