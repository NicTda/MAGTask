//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;

namespace MAGTask
{
    /// Container class for a level objective
    /// 
    public sealed class ObjectiveModel
    {
        public event Action OnProgressed;
        public event Action OnCompleted;

        public ObjectiveData m_data { get; private set; } = null;

        private int m_progress = 0;

        #region Public functions
        /// @param data
        ///     The objective data
        ///
        public ObjectiveModel(ObjectiveData data)
        {
            m_data = data;
        }

        /// @param amount
        ///     The amount to progress
        ///
        public void AddProgress(int amount)
        {
            if(IsComplete() == false)
            {
                m_progress += amount;
                OnProgressed.SafeInvoke();

                if(IsComplete() == true)
                {
                    OnCompleted.SafeInvoke();
                }
            }
        }

        /// @return The progress in percentage
        ///
        public float GetProgress()
        {
            return (float)m_progress / m_data.m_amount;
        }

        /// @return The progress as a string
        ///
        public string GetProgressDisplay()
        {
            return string.Format(GameTextIdentifiers.k_progressFormat, m_progress, m_data.m_amount);
        }

        /// @return Whether the objective is completed
        ///
        public bool IsComplete()
        {
            return m_progress >= m_data.m_amount;
        }
        #endregion
    }
}
