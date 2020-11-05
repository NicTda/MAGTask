//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// Service for time control
    /// 
    public sealed class TimeService : Service
    {
        public event Action<float> OnTimeScaleChanged;

        private const float k_fastScale = 2.0f;
        private const float k_regularScale = 1.0f;
        private const float k_slowScale = 0.1f;
        private const float k_pausedScale = 0.0f;

        private float m_scale = 1.0f;
        private bool m_paused = false;

        #region Public functions
        /// Pauses the game
        /// 
        public void Pause()
        {
            m_paused = true;
            Time.timeScale = k_pausedScale;
        }

        /// Resumes the game
        /// 
        public void Resume()
        {
            m_paused = false;
            Time.timeScale = m_scale;
        }

        /// @return The current time scale of the game
        /// 
        public float GetCurrentTimeScale()
        {
            return m_scale;
        }

        /// Activates Slow Motion
        /// 
        public void SlowMotion()
        {
            UpdateScale(k_slowScale);
        }

        /// Activates Regular Motion
        /// 
        public void RegularMotion()
        {
            UpdateScale(k_regularScale);
        }

        /// Activates fast Motion
        /// 
        public void FastMotion()
        {
            UpdateScale(k_fastScale);
        }
        #endregion

        #region Private functions
        ///
        private void UpdateScale(float scale)
        {
            if(m_scale != scale)
            {
                m_scale = scale;
                if(m_paused == false)
                {
                    Resume();
                }
                OnTimeScaleChanged(scale);
            }
        }
        #endregion
    }
}
