//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
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

        public Transform TilesHolder { get { return m_tilesHolder; } }

        [SerializeField]
        private Transform m_tilesHolder = null;
        [SerializeField]
        private TMP_Text m_scoreText = null;

        #region Public functions
        /// @param score
        ///     The score to set
        /// 
        public void SetScore(int score)
        {
            m_scoreText.SafeText(TextUtils.GetFormattedCurrencyString(score));
        }

        /// @param score
        ///     The score to set
        /// 
        public void SetScore(int previouScore, int newScore)
        {
            m_scoreText.DOCount(previouScore, newScore, 1.0f);
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
