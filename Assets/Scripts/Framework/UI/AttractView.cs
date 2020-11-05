//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// View component that attracts attention to the object it's attached to
    ///
    public sealed class AttractView : MonoBehaviour
    {
        [SerializeField]
        private RangeFloat m_pulseRange = new RangeFloat(3.0f, 6.0f);

        private readonly KeyValuePair<float, Vector3>[] k_scaleSequence = new KeyValuePair<float, Vector3>[]
        {
            new KeyValuePair<float, Vector3>(0.20f, new Vector3(1.0f, 1.1f, 1.0f)),
            new KeyValuePair<float, Vector3>(0.10f, new Vector3(1.1f, 0.9f, 1.0f)),
            new KeyValuePair<float, Vector3>(0.10f, new Vector3(0.95f, 1.05f, 1.0f)),
            new KeyValuePair<float, Vector3>(0.10f, new Vector3(1.05f, 0.95f, 1.0f)),
            new KeyValuePair<float, Vector3>(0.10f, new Vector3(1.0f, 1.0f, 1.0f)),
        };

        private Sequence m_pulseSequence = null;
        private float m_baseScale = 1.0f;
        private float m_pulseTimer = 5.0f;

        #region Unity functions
        /// Awake function
        ///
        private void Awake()
        {
            if(transform.localScale.x != 0.0f)
            {
                m_baseScale = transform.localScale.x;
            }
            m_pulseSequence = DOTween.Sequence();
            foreach (var pair in k_scaleSequence)
            {
                m_pulseSequence.Append(transform.DOScale(pair.Value * m_baseScale, pair.Key));
            }
            m_pulseSequence.onComplete += () =>
            {
                PausePulse();
            };
            SchedulePulse();
        }

        /// Update function
        ///
        private void Update()
        {
            m_pulseTimer -= Time.deltaTime;
            if(m_pulseTimer <= 0.0f && m_pulseSequence.IsPlaying() == false)
            {
                m_pulseSequence.Play();
                SchedulePulse();
            }
        }

        /// OnDestroy function
        ///
        private void OnDestroy()
        {
            StopPulse();
        }
        #endregion

        #region Public functions
        /// Schedule the next pulse
        ///
        public void SchedulePulse()
        {
            m_pulseTimer = m_pulseRange.GetRandomValue();
        }

        /// Pauses the pulsing
        ///
        public void PausePulse()
        {
            m_pulseSequence.Restart();
            m_pulseSequence.Pause();
        }

        /// Stops the pulsing
        ///
        public void StopPulse()
        {
            m_pulseSequence.Stop();
        }
        #endregion
    }
}
