//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace MAGTask
{
    /// UI view for reward "particle"
    ///
    public sealed class ParticleReward : MonoBehaviour
    {
        public event Action OnReachedDestination;
        public event Action OnRewarded;

        [SerializeField]
        private Image m_icon = null;

        private RangeFloat m_burstDistance = new RangeFloat(Screen.width * -0.15f, Screen.width * 0.15f);
        private RangeFloat m_burstTime = new RangeFloat(0.4f, 0.6f);
        private RangeFloat m_idleTime = new RangeFloat(0.2f, 0.3f);
        private float m_timeScale = 1.0f;
        private Sequence m_rewardSequence = null;

        #region Unity functions
        /// OnDestroy function
        ///
        private void OnDestroy()
        {
            m_rewardSequence.Stop(true);
        }
        #endregion

        #region Public functions
        /// @param scale
        ///     The scale to apply to the image
        ///
        public void SetScale(float scale)
        {
            m_icon.transform.localScale = Vector3.one * scale;
        }

        /// @param timeScale
        ///     The timeScale to apply
        ///
        public void SetTimeScale(float timeScale)
        {
            m_timeScale = timeScale;
        }

        /// @param reward
        ///     The reward to display
        /// @param origin
        ///     The origin position of the particles
        /// @param destination
        ///     The destination of the particles
        ///
        public void Initialise(string currencyID, Vector3 origin, Vector3 destination)
        {
            // Show the reward at the origin position
            m_icon.SafeSprite(GameUtils.GetCurrencySprite(currencyID));
            transform.localScale = Vector3.zero;
            transform.position = origin;

            var burstTime = m_burstTime.GetRandomValue() * m_timeScale;
            var moveTime = burstTime * 0.5f;
            var randomBurstPosition = Vector3.right * m_burstDistance.GetRandomValue() + Vector3.up * m_burstDistance.GetRandomValue();

            // Burst the reward
            m_rewardSequence = DOTween.Sequence();
            m_rewardSequence.Append(transform.DOScale(1.0f, burstTime).SetEase(Ease.OutBack));
            m_rewardSequence.Insert(0.00f, transform.DOBlendableMoveBy(randomBurstPosition, burstTime));
            m_rewardSequence.Insert(0.00f, transform.DOBlendableMoveBy(Vector3.up * Screen.width * 0.05f, moveTime));
            m_rewardSequence.Insert(moveTime, transform.DOBlendableMoveBy(Vector3.down * Screen.width * 0.05f, moveTime));

            // Make the reward fly to its destination
            m_rewardSequence.AppendInterval(m_idleTime.GetRandomValue());
            m_rewardSequence.Append(transform.DOMove(destination, moveTime).SetEase(Ease.InBack));

            // Destroy itself on completion
            m_rewardSequence.AppendCallback(() =>
            {
                OnReachedDestination.SafeInvoke();
                Destroy(gameObject);
            });
            m_rewardSequence.OnComplete(() =>
            {
                OnRewarded.SafeInvoke();
            });
            m_rewardSequence.Play();
        }
        #endregion
    }
}
