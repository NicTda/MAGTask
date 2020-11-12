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
        private const string k_actionNext = "Next";
        private const string k_actionIdle = "Idle";
        private const string k_actionReload = "Reload";
        private const string k_actionShuffle = "Shuffle";
        private const string k_actionResolve = "Resolve";
        private const string k_actionWin = "Win";
        private const string k_actionLose = "Lose";

        private const string k_stateInit = "Init";
        private const string k_stateLoad = "Load";
        private const string k_stateIdle = "Idle";
        private const string k_stateShuffle = "Shuffle";
        private const string k_stateResolve = "Resolve";
        private const string k_stateWin = "Win";
        private const string k_stateLose = "Lose";

        private const int k_tileScore = 100;
        private const int k_bonusScore = 50;
        private const int k_minActiveTiles = 3;
        private const int k_extraMoves = 5;
        private const float k_boardPadding = 1.25f;
        private const float k_distanceTiles = 2;

        private readonly Vector3 k_boardPosBack = Vector3.forward * 5;
        private readonly Vector3 k_boardPosFront = Vector3.back * 5;
        private readonly List<Vector3> k_validNeighbours = new List<Vector3>()
        {
            Vector3.left + Vector3.up,
            Vector3.left,
            Vector3.left + Vector3.down,
            Vector3.up,
            Vector3.down,
            Vector3.right + Vector3.up,
            Vector3.right,
            Vector3.right + Vector3.down,
        };

        private LevelView m_view = null;
        private LevelData m_levelData = null;

        private TileFactory m_tileFactory = null;
        private LevelService m_levelService = null;
        private PopupService m_popupService = null;
        private ObjectiveService m_objectiveService = null;

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
            : base(localDirector, view, SceneIdentifiers.k_map)
        {
            m_view = view;

            m_tileFactory = localDirector.GetFactory<TileFactory>();
            m_levelService = GlobalDirector.Service<LevelService>();
            m_popupService = GlobalDirector.Service<PopupService>();
            m_objectiveService = localDirector.GetService<ObjectiveService>();

            m_audioService.PlayMusicFadeCross(AudioIdentifiers.k_musicLevel);

            m_fsm.RegisterStateCallback(k_stateInit, EnterStateInit, null, null);
            m_fsm.RegisterStateCallback(k_stateLoad, EnterStateLoad, null, null);
            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.RegisterStateCallback(k_stateShuffle, EnterStateShuffle, null, null);
            m_fsm.RegisterStateCallback(k_stateResolve, EnterStateResolve, null, ExitStateResolve);
            m_fsm.RegisterStateCallback(k_stateWin, EnterStateWin, null, null);
            m_fsm.RegisterStateCallback(k_stateLose, EnterStateLose, null, null);
            m_fsm.ExecuteAction(k_actionNext);
        }

        /// Called when the player wants to go back to the main menu
        ///     
        protected override void OnBackButtonRequest()
        {
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonBack);
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameQuestionProminent) as PopupYesNoView;
            popupView.SetBodyText(GameTextIdentifiers.k_levelExit);
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
        /// Start of the Init state
        /// 
        private void EnterStateInit()
        {
            m_levelData = LevelLocalDirector.s_levelData;
            m_view.ScoreView.InitialiseScores(m_levelData.m_scores);
            m_view.BoardBacking.size = new Vector2(m_levelData.m_width + k_boardPadding, m_levelData.m_height + k_boardPadding);
            m_view.SetLevelName(string.Format(GameTextIdentifiers.k_levelDisplay, m_levelData.m_index));

            // Load level data
            m_tiles.Capacity = m_levelData.m_height * m_levelData.m_width;
            m_selectedTiles.Capacity = m_levelData.m_height * m_levelData.m_width;
            m_spawnHeight = m_levelData.m_height * 0.5f + 1.0f;
            SetMovesLeft(m_levelData.m_moves);

            // Initialise the objectives
            bool hasScore = false;
            for (int index = 0; index < m_levelData.m_objectives.Count; ++index)
            {
                var objectiveData = m_levelData.m_objectives[index];
                RegisterObjective(objectiveData, index);
                hasScore |= objectiveData.m_type == ObjectiveType.Score;
            }

            if(hasScore == false)
            {
                // Add a score objective as a default
                RegisterObjective(new ObjectiveData()
                {
                    m_type = ObjectiveType.Score,
                    m_amount = m_levelData.m_scores.GetFirst()

                }, m_levelData.m_objectives.Count);
            }

            m_fsm.ExecuteAction(k_actionNext);
        }
        
        /// Start of the Load state
        /// 
        private void EnterStateLoad()
        {
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
            TileColour randomColour = m_levelData.m_tiles.GetRandom();
            var tileView = m_tileFactory.CreateTile(randomColour, m_view.TilesHolder, spawnPosition);
            tileView.m_boardPosition = boardPosition;
            return tileView;
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

            // Check that the board has at least one valid move
            var boardState = ValidateBoard();
            if (boardState == BoardState.Reshuffle)
            {
                // The board needs reshuffled
                m_fsm.ExecuteAction(k_actionShuffle);
            }
            else if(boardState == BoardState.Recreate)
            {
                // We can't reshuffle the board as it is, we need to recreate it
                foreach (var tile in m_tiles)
                {
                    GameObject.Destroy(tile.gameObject);
                }
                m_tiles.Clear();
                m_fsm.ExecuteAction(k_actionReload);
            }
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
                m_audioService.PlaySFX(AudioIdentifiers.k_sfxTileDeselect);
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

        /// Start of the Shuffle state
        /// 
        private void EnterStateShuffle()
        {
            // Get all board positions
            var tilePositions = new List<Vector3>(m_tiles.Count);
            foreach (var tile in m_tiles)
            {
                tilePositions.Add(tile.m_boardPosition);
            }

            // Assign new positions to the tiles
            int repositioned = 0;
            foreach (var tile in m_tiles)
            {
                tile.m_boardPosition = tilePositions.ExtractRandom();
                tile.Reposition(() =>
                {
                    ++repositioned;
                    if(repositioned == m_tiles.Count)
                    {
                        m_fsm.ExecuteAction(k_actionIdle);
                    }
                });
            }
        }

        /// Start of the Resolve state
        /// 
        private void EnterStateResolve()
        {
            // Update moves
            SetMovesLeft(m_movesLeft - 1);

            // Pop the tiles
            m_coroutine = GlobalDirector.ExecuteCoroutine(StaggerTilesPop(() =>
            {
                m_coroutine = GlobalDirector.ExecuteCoroutine(StaggerReplaceTiles(() =>
                {
                    if(IsLevelCompleted() == true)
                    {
                        // Level success
                        m_fsm.ExecuteAction(k_actionWin);
                    }
                    else if (m_movesLeft > 0)
                    {
                        m_fsm.ExecuteAction(k_actionIdle);
                    }
                    else if(IsLevelCompleted() == true)
                    {
                        // Level success
                        m_fsm.ExecuteAction(k_actionWin);
                    }
                    else
                    {
                        // Level fail
                        m_fsm.ExecuteAction(k_actionLose);
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
            int tilePopped = 0;
            int tilesChained = 0;
            int tileScore = k_tileScore;
            foreach (var tile in m_selectedTiles)
            {
                string audioSFX = AudioIdentifiers.k_sfxPopPositive;
                string particleID = ParticleIdentifiers.k_tilePop;
                string textFormat = GameTextIdentifiers.k_rewardFormat;
                Color textColour = Color.white;

                // Special tiles
                if (tile.m_tileColour == TileColour.Grey)
                {
                    audioSFX = AudioIdentifiers.k_sfxPopNegative;
                    particleID = ParticleIdentifiers.k_tilePopNegative;
                    textFormat = GameTextIdentifiers.k_penaltyFormat;
                    textColour = Color.red;
                    tileScore = -k_tileScore;
                }

                // Audio SFX
                m_audioService.PlaySFX(audioSFX);

                // Particle effect and score
                m_currentScore += tileScore;
                var textParticle = ParticleUtils.SpawnTextParticles(string.Format(textFormat, tileScore), m_view.TilesHolder, tile.m_boardPosition);
                textParticle.SetColour(textColour);
                ParticleUtils.SpawnParticles(particleID, tile.m_boardPosition);

                // Log the score event
                m_objectiveService.LogEvent(ObjectiveType.Score, tileScore);

                tile.Pop(() =>
                {
                    // Tile popped, remove it
                    m_tiles.Remove(tile);
                    tile.gameObject.SetActive(false);
                    ++tilePopped;

                    // Log the popped tile event
                    m_objectiveService.LogEvent(ObjectiveType.Colour, tile.m_tileColour);
                });

                yield return new WaitForSeconds(0.1f);

                // Add bonus score if any
                ++tilesChained;
                if (tileScore > 0 && tilesChained % k_minActiveTiles == 0)
                {
                    tileScore += k_bonusScore;
                }
            }

            while (tilePopped < m_selectedTiles.Count)
            {
                yield return null;
            }

            // Log the chain event
            m_objectiveService.LogEventWithValue(ObjectiveType.Chain, tilesChained);

            callback.SafeInvoke();
        }

        /// @param callback
        ///     The function to call when the tiles are replaced
        ///     
        private IEnumerator StaggerReplaceTiles(Action callback)
        {
            // Update score
            bool scoreUpdated = false;
            m_view.ScoreView.SetScore(m_currentScore, () =>
            {
                scoreUpdated = true;
            });

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

            while (scoreUpdated == false || appeared < m_selectedTiles.Count)
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
            // Register the level as completed
            m_levelService.CompleteLevel(m_levelData.m_index, m_currentScore);

            // Unlock next level
            m_levelService.UnlockLevel(m_levelData.m_index + 1);

            // Next level or back to map
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameInfo);
            popupView.SetPresentSFX(AudioIdentifiers.k_sfxLevelComplete);
            popupView.SetBodyText(GameTextIdentifiers.k_levelWinBody, m_levelData.m_index);
            popupView.OnPopupDismissed += (popup) =>
            {
                // TODO TDA: popup with home, retry, next
                m_sceneService.SwitchToScene(m_exitSceneID, SceneIdentifiers.k_transition);
            };
        }

        /// Start of the Lose state
        /// 
        private void EnterStateLose()
        {
            // Continue?
            var popupView = m_popupService.QueuePopup(PopupIdentifiers.k_gameQuestion) as PopupYesNoView;
            popupView.SetPresentSFX(AudioIdentifiers.k_sfxLevelFail);
            popupView.SetBodyText(GameTextIdentifiers.k_levelLostRetry, k_extraMoves);
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
        #endregion

        #region Private functions
        /// Listens to the tiles input
        /// 
        private void RegisterTilesInput()
        {
            foreach(var tile in m_tiles)
            {
                tile.OnTouchEnter += OnTileEntered;
            }
        }

        /// Listens to the tiles input
        /// 
        private void UnregisterTilesInput()
        {
            foreach (var tile in m_tiles)
            {
                tile.OnTouchEnter -= OnTileEntered;
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
                SelectTile(tile, tile.TileItemHolder.position);
            }
            else
            {
                var lastIndex = m_selectedTiles.Count - 1;
                var lastTile = m_selectedTiles[lastIndex];

                // Check if the player is swiping back on the tiles
                if (m_selectedTiles.Contains(tile) == true)
                {
                    // Cut the last tile if the entered tile is n-1 (soft cancel)
                    if (m_selectedTiles.Count > 1 && tile == m_selectedTiles[lastIndex - 1])
                    {
                        DeselectTile(lastTile);
                    }
                }
                else if(tile.m_tileColour == lastTile.m_tileColour)
                {
                    // The tile type is valid, check if they're neighbours
                    var distance = (lastTile.m_boardPosition - tile.m_boardPosition).sqrMagnitude;
                    if(distance <= k_distanceTiles)
                    {
                        SelectTile(tile, lastTile.TileItemHolder.position);
                    }
                }
            }
        }

        /// @param tile
        ///     The tile exited
        /// @param linkPosition
        ///     The position of the linked tile
        /// 
        private void SelectTile(TileView tile, Vector3 linkPosition)
        {
            // Audio SFX
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxTileSelect);

            tile.Select(linkPosition);
            m_selectedTiles.Add(tile);
        }

        /// @param tile
        ///     The tile exited
        /// 
        private void DeselectTile(TileView tile)
        {
            // Audio SFX
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxTileDeselect);

            tile.Deselect();
            m_selectedTiles.Remove(tile);
        }

        /// @param moves
        ///     The moves to set
        /// 
        private void SetMovesLeft(int moves)
        {
            m_movesLeft = moves;
            m_view.SetMovesLeft(m_movesLeft.ToString());
        }

        /// @param objectiveData
        ///     The data of the objective
        /// @param index
        ///     The index of the objective
        /// 
        private void RegisterObjective(ObjectiveData objectiveData, int index)
        {
            if(objectiveData.m_type != ObjectiveType.None)
            {
                var model = m_objectiveService.AddObjective(objectiveData);
                model.OnCompleted += OnObjectiveCompleted;
                m_view.ShowObjective(index, model);
            }
        }

        /// Called when an objective completed
        /// 
        private void OnObjectiveCompleted()
        {
            // Audio SFX
            m_audioService.PlaySFX(AudioIdentifiers.k_sfxObjectiveComplete);
        }

        /// @return Whether the current board has at least a valid move
        /// 
        private BoardState ValidateBoard()
        {
            var boardState = BoardState.Recreate;
            List<TileView> pendingTiles = new List<TileView>(m_tiles.Count);
            List<TileView> currentChain = new List<TileView>(9);
            TileView activeTile = null;

            // Check each colour on the board
            foreach (var tileColour in m_levelData.m_tiles)
            {
                pendingTiles = m_tiles.FindAll((view) => view.m_tileColour == tileColour);
                if (pendingTiles.Count < k_minActiveTiles)
                {
                    // No need to check the colours that can't have valid matches
                    continue;
                }

                // At least one colour is valid
                boardState = BoardState.Reshuffle;

                // Check chains on the valid colours
                currentChain.Clear();
                while (pendingTiles.Count > 0)
                {
                    if (activeTile == null)
                    {
                        // Get a random tile to check
                        activeTile = pendingTiles.ExtractRandom();
                    }

                    // Get its valid neighbours
                    var neighbours = pendingTiles.FindAll((view) => k_validNeighbours.Contains(activeTile.m_boardPosition - view.m_boardPosition));
                    if (neighbours.Count == 0)
                    {
                        // No valid neighbour, we stop that chain
                        currentChain.Clear();
                        activeTile = null;
                    }
                    else if (currentChain.Count + neighbours.Count >= k_minActiveTiles - 1)
                    {
                        // If there's a valid chain, the board is valid so we can stop
                        return BoardState.Valid;
                    }
                    else
                    {
                        // There is a chain, but not long enough
                        activeTile = neighbours.GetRandom();
                        currentChain.AddRange(neighbours);
                        foreach (var neighbour in neighbours)
                        {
                            pendingTiles.Remove(neighbour);
                        }
                    }
                }
            }

            return boardState;
        }

        /// @return Whether the level's objectives are done
        /// 
        private bool IsLevelCompleted()
        {
            // Check all objectives
            return m_objectiveService.AreObjectivesComplete();
        }
        #endregion
    }
}
