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
    /// View component for the LevelEditor scene
    ///
    public sealed class LevelEditorView : SceneFSMView
    {
        public event Action OnPlayRequested;
        public event Action<int> OnLoadRequested;
        public event Action OnSaveRequested;
        public event Action OnPreviousRequested;
        public event Action OnNextRequested;
        public event Action<int> OnMovesRequested;
        public event Action<int, int> OnSizeRequested;
        public event Action<int, int> OnScoreRequested;

        public int LevelIndex = 0;
        public RectTransform TilesHolder { get { return m_tilesHolder; } }
        public List<ObjectiveEditorView> Objectives { get { return m_objectives; } }

        [SerializeField]
        private RectTransform m_tilesHolder = null;
        [SerializeField]
        private TMP_InputField m_levelString = null;
        [SerializeField]
        private TMP_Text m_levelName = null;
        [SerializeField]
        private TMP_Text m_boardSize = null;
        [SerializeField]
        private TMP_Text m_moves = null;
        [SerializeField]
        private List<TMP_InputField> m_scores = null;
        [SerializeField]
        private List<ObjectiveEditorView> m_objectives = new List<ObjectiveEditorView>(2);

        #region Public functions
        /// @param text
        ///     The text to set
        /// 
        public void SetLevelName(string text)
        {
            m_levelName.SafeText(text);
        }
        
        /// @param text
        ///     The text to set
        /// 
        public void SetBoardSize(string text)
        {
            m_boardSize.SafeText(text);
        }

        /// @param text
        ///     The text to set
        /// 
        public void SetMoves(string text)
        {
            m_moves.SafeText(text);
        }

        /// @param index
        ///     The index of the score
        /// @param text
        ///     The text to set
        /// 
        public void SetScore(int index, int text)
        {
            m_scores[index].SafeText(text.ToString());
        }

        /// @param index
        ///     The index of the objective
        /// @param objective
        ///     The objective to set
        /// 
        public void SetObjective(int index, ObjectiveData objective)
        {
            m_objectives[index].Initialise(objective);
        }

        /// Called when the player presses the Play button
        /// 
        public void OnPlayPressed()
        {
            OnPlayRequested.SafeInvoke();
        }

        /// Called when the player presses the Load button
        /// 
        public void OnLoadPressed()
        {
            OnLoadRequested.SafeInvoke(LevelIndex);
        }

        /// Called when the player presses the Save button
        /// 
        public void OnSavePressed()
        {
            OnSaveRequested.SafeInvoke();
        }

        /// Called when the player presses the Previous button
        /// 
        public void OnPreviousPressed()
        {
            OnPreviousRequested.SafeInvoke();
        }

        /// Called when the player presses the Next button
        /// 
        public void OnNextPressed()
        {
            OnNextRequested.SafeInvoke();
        }

        /// @param amount
        ///     The value of width change
        /// 
        public void OnWidthChanged(int amount)
        {
            OnSizeRequested.SafeInvoke(amount, 0);
        }

        /// @param amount
        ///     The value of height change
        /// 
        public void OnHeightChanged(int amount)
        {
            OnSizeRequested.SafeInvoke(0, amount);
        }

        /// @param amount
        ///     The value of moves change
        /// 
        public void OnMovesChanged(int amount)
        {
            OnMovesRequested.SafeInvoke(amount);
        }

        /// @param value
        ///     The new level value
        /// 
        public void OnLevelIndexChanged(string value)
        {
            if(int.TryParse(value, out int integer) && LevelIndex.ToString() != value)
            {
                LevelIndex = integer;
                m_levelString.SafeText(LevelIndex.ToString());
            }
        }

        /// @param value
        ///     The new score value
        /// 
        public void OnMinScoreChanged(string value)
        {
            if (int.TryParse(value, out int integer))
            {
                OnScoreRequested.SafeInvoke(0, integer);
            }
        }

        /// @param value
        ///     The new score value
        /// 
        public void OnMidScoreChanged(string value)
        {
            if (int.TryParse(value, out int integer))
            {
                OnScoreRequested.SafeInvoke(1, integer);
            }
        }

        /// @param value
        ///     The new score value
        /// 
        public void OnMaxScoreChanged(string value)
        {
            if (int.TryParse(value, out int integer))
            {
                OnScoreRequested.SafeInvoke(2, integer);
            }
        }
        #endregion
    }
}
