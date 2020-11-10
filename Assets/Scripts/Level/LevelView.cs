//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MAGTask
{
    /// View component for the Level scene
    ///
    public sealed class LevelView : SceneFSMView
    {
        public event Action OnInteractStarted;
        public event Action OnInteractEnded;

        public ScoreView ScoreView { get { return m_scoreView; } }
        public Transform TilesHolder { get { return m_tilesHolder; } }
        public Transform BoardTouchArea = null;
        public SpriteRenderer BoardBacking = null;

        [SerializeField]
        private Transform m_tilesHolder = null;
        [SerializeField]
        private ScoreView m_scoreView = null;
        [SerializeField]
        private TMP_Text m_levelName = null;
        [SerializeField]
        private TMP_Text m_moveText = null;
        [SerializeField]
        private List<ObjectiveView> m_objectiveViews = new List<ObjectiveView>();

        #region Public functions
        /// @param name
        ///     The name to set
        /// 
        public void SetLevelName(string name)
        {
            m_levelName.SafeText(name);
        }

        /// @param moves
        ///     The moves to set
        /// 
        public void SetMovesLeft(string moves)
        {
            m_moveText.SafeText(moves);
        }

        /// @param objectiveText
        ///     The text to set
        /// 
        public void ShowObjective(int index, ObjectiveModel model)
        {
            if(index < m_objectiveViews.Count)
            {
                m_objectiveViews[index].Initialise(model);
            }
        }

        /// Called when the player starts interacting with the board
        /// 
        public void OnInteractStart()
        {
            OnInteractStarted.SafeInvoke();
        }

        /// Called when the player ends interacting with the board
        /// 
        public void OnInteractEnd()
        {
            OnInteractEnded.SafeInvoke();
        }
        #endregion
    }
}
