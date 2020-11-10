//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MAGTask
{
    /// View component for the Level score
    ///
    public sealed class ScoreView : MonoBehaviour
    {
        [SerializeField]
        private List<ProgressBar> m_starsProgress = new List<ProgressBar>();
        [SerializeField]
        private TMP_Text m_scoreText = null;

        private Coroutine m_coroutine = null;
        private Tweener m_tweenCount = null;
        private List<int> m_scores = null;
        private int m_score = 0;

        #region Public functions
        /// OnDestroy function
        /// 
        public void OnDestroy()
        {
            m_coroutine.Stop();
            m_tweenCount.Stop();
        }

        /// @param newScore
        ///     The score to set
        /// @param callback
        ///     The function to call when the score finished updated
        /// 
        public void SetScore(int newScore, Action callback = null)
        {
            // Update the text
            m_tweenCount = m_scoreText.DOCount(m_score, newScore, 1.0f, () =>
            {
                m_score = newScore;
            });

            // Update the stars
            m_coroutine = GlobalDirector.ExecuteCoroutine(StaggerStarProgress(newScore, callback));
        }

        /// @param scores
        ///     The scores to achieve
        /// 
        public void InitialiseScores(List<int> scores)
        {
            m_scoreText.SafeText(TextUtils.GetFormattedCurrencyString(m_score));
            m_scores = scores;
            foreach (var star in m_starsProgress)
            {
                star.SetProgress(0.0f);
            }
        }
        #endregion

        #region Private functions
        /// @param newScore
        ///     The score to set
        /// @param callback
        ///     The function to call when the score finished updated
        /// 
        private IEnumerator StaggerStarProgress(int newScore, Action callback = null)
        {
            int lastThreshold = 0;
            for (int index = 0; index < m_scores.Count; ++index)
            {
                if(m_starsProgress[index].GetProgress() < 1.0f)
                {
                    // Set the progress of that star
                    bool effectDone = false;
                    bool shouldContinue = false;
                    float progress = (float)(newScore - lastThreshold ) / (m_scores[index] - lastThreshold);
                    m_starsProgress[index].TweenToProgress(progress, 1.0f, 0.0f, () =>
                    {
                        effectDone = true;
                        if (progress >= 1.0f)
                        {
                            // The star is filled!
                            shouldContinue = true;
                            ParticleUtils.SpawnParticles(ParticleIdentifiers.k_starburstUI, transform, m_starsProgress[index].transform.position);
                        }
                    });

                    while (effectDone == false)
                    {
                        yield return null;
                    }
                    lastThreshold = m_scores[index];

                    if (shouldContinue == false)
                    {
                        // Don't progress further stars
                        break;
                    }
                }
            }

            callback.SafeInvoke();
        }
        #endregion
    }
}
