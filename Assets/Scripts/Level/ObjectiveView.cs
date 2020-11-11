//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using TMPro;
using UnityEngine;

namespace MAGTask
{
    /// View component for a level Objective
    ///
    public sealed class ObjectiveView : MonoBehaviour
    {
        private const string k_animAppear = "Appear";
        private const string k_animCompleted = "Completed";

        [SerializeField]
        private Animator m_animator = null;
        [SerializeField]
        private ProgressBar m_progressBar = null;
        [SerializeField]
        private TMP_Text m_objectiveText = null;

        private ObjectiveModel m_objectiveModel = null;
        private string m_baseDisplay = string.Empty;

        #region Public functions
        /// @param model
        ///     The model to initialise with
        /// 
        public void Initialise(ObjectiveModel model)
        {
            m_objectiveModel = model;
            m_objectiveModel.OnProgressed += UpdateProgressDisplay;
            m_objectiveModel.OnCompleted += OnCompleted;

            m_baseDisplay = GameTextIdentifiers.k_objectives[m_objectiveModel.m_data.m_type];
            UpdateProgressDisplay();
            m_animator.Play(k_animAppear);
        }
        #endregion

        #region Private functions
        /// Called when the objective has progressed
        /// 
        private void UpdateProgressDisplay()
        {
            // Set the objective text
            string target = m_objectiveModel.m_data.m_target != TileColour.None ? m_objectiveModel.m_data.m_target.ToString() : m_objectiveModel.m_data.m_value.ToString();
            string amount = TextUtils.GetFormattedCurrencyString(m_objectiveModel.m_data.m_amount);
            m_objectiveText.SafeText(string.Format(m_baseDisplay, target, amount));

            // Set the objective progress bar
            float progress = m_objectiveModel.GetProgress();
            string progressString = m_objectiveModel.GetProgressDisplay();
            m_progressBar.TweenToProgress(progress, 0.5f);
            m_progressBar.Text.SafeText(progressString);
        }

        /// Called when the objective has completed
        /// 
        private void OnCompleted()
        {
            m_animator.Play(k_animCompleted);
        }
        #endregion
    }
}
