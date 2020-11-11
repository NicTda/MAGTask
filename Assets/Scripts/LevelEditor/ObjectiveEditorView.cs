//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using TMPro;
using UnityEngine;

namespace MAGTask
{
    /// View component for an objective in LevelEditor scene
    ///
    public sealed class ObjectiveEditorView : MonoBehaviour
    {
        public event Action<int, ObjectiveData> OnObjectiveChanged;

        [SerializeField]
        private int m_index = 0;
        [SerializeField]
        private TMP_Text m_typeText = null;
        [SerializeField]
        private TMP_Text m_targetText = null;
        [SerializeField]
        private TMP_InputField m_valueText = null;
        [SerializeField]
        private TMP_InputField m_amountText = null;
        [SerializeField]
        private StateComponent m_stateComponent = null;

        private ObjectiveData m_objectiveData = null;

        #region Public functions
        /// @param objectiveData
        ///     The data to initialise with
        /// 
        public void Initialise(ObjectiveData objectiveData)
        {
            m_objectiveData = objectiveData;
            RefreshUI();
        }

        /// Called when the player changes the type of the objective
        /// 
        public void OnTypeNext()
        {
            var typeCount = Enum.GetValues(typeof(ObjectiveType)).Length;
            int typeIndex = (int)m_objectiveData.m_type + 1;
            if(typeIndex >= typeCount)
            {
                typeIndex = 0;
            }
            m_objectiveData.m_type = (ObjectiveType)typeIndex;
            RefreshUI();

            OnObjectiveChanged.SafeInvoke(m_index, m_objectiveData);
        }

        /// Called when the player changes the target of the objective
        /// 
        public void OnTargetNext()
        {
            var targetCount = Enum.GetValues(typeof(TileColour)).Length;
            var targetIndex = (int)m_objectiveData.m_target + 1;
            if (targetIndex >= targetCount)
            {
                targetIndex = 1;
            }
            m_objectiveData.m_target = (TileColour)targetIndex;
            RefreshUI();

            OnObjectiveChanged.SafeInvoke(m_index, m_objectiveData);
        }

        /// @param value
        ///     The new value
        /// 
        public void OnValueChanged(string value)
        {
            if (int.TryParse(value, out int integer) && m_objectiveData.m_value.ToString() != value)
            {
                m_objectiveData.m_value = integer;
                RefreshUI();

                OnObjectiveChanged.SafeInvoke(m_index, m_objectiveData);
            }
        }

        /// @param amount
        ///     The new amount
        /// 
        public void OnAmountChanged(string amount)
        {
            if (int.TryParse(amount, out int integer) && m_objectiveData.m_amount.ToString() != amount)
            {
                m_objectiveData.m_amount = integer;
                RefreshUI();

                OnObjectiveChanged.SafeInvoke(m_index, m_objectiveData);
            }
        }
        #endregion

        #region Private functions
        /// Refresh the UI
        /// 
        private void RefreshUI()
        {
            var typeString = m_objectiveData.m_type.ToString();
            m_typeText.SafeText(typeString);
            m_stateComponent.SafeState(typeString);
            m_targetText.SafeText(m_objectiveData.m_target.ToString());
            m_valueText.SafeText(m_objectiveData.m_value.ToString());
            m_amountText.SafeText(m_objectiveData.m_amount.ToString());
        }
        #endregion
    }
}
