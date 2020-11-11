//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEditor;

namespace MAGTask
{
    /// Editor layout for ProgressBar
    /// 
    [CustomEditor(typeof(ProgressBar), false)]
    [CanEditMultipleObjects]
    public sealed class ProgressBarEditor : Editor
    {
        SerializedProperty m_progressStyle;
        SerializedProperty m_fillStyle;
        SerializedProperty m_fillRect;
        SerializedProperty m_fillImage;
        SerializedProperty m_overrideColour;
        SerializedProperty m_fillerColour;
        SerializedProperty m_completeColour;
        SerializedProperty m_text;
        SerializedProperty m_progress;
        SerializedProperty m_update;

        /// Called when the component is enabled
        /// 
        private void OnEnable()
        {
            m_progressStyle = serializedObject.FindProperty("m_progressStyle");
            m_fillStyle = serializedObject.FindProperty("m_fillStyle");
            m_fillRect = serializedObject.FindProperty("m_fillRect");
            m_fillImage = serializedObject.FindProperty("m_fillImage");
            m_overrideColour = serializedObject.FindProperty("m_overrideColour");
            m_fillerColour = serializedObject.FindProperty("m_fillerColour");
            m_completeColour = serializedObject.FindProperty("m_completeColour");
            m_text = serializedObject.FindProperty("m_text");
            m_progress = serializedObject.FindProperty("m_progress");
            m_update = serializedObject.FindProperty("m_update");
        }

        /// Called when the editor draws the UI for the component in the inspector
        /// 
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_progressStyle, true);
            switch((ProgressBar.ProgressStyle)m_progressStyle.enumValueIndex)
            {
                case ProgressBar.ProgressStyle.Rect:
                {
                    ShowHeader("Rect properties");
                    EditorGUILayout.PropertyField(m_fillStyle, true);
                    EditorGUILayout.PropertyField(m_fillRect, true);
                    break;
                }
                case ProgressBar.ProgressStyle.FillImage:
                {
                    ShowHeader("Image properties");
                    EditorGUILayout.PropertyField(m_fillImage, true);
                    break;
                }
            }

            ShowHeader("Common properties");
            EditorGUILayout.PropertyField(m_progress, true);
            EditorGUILayout.PropertyField(m_text, true);
            EditorGUILayout.PropertyField(m_overrideColour, true);
            if (m_overrideColour.boolValue == true)
            {
                EditorGUILayout.PropertyField(m_fillerColour, true);
                EditorGUILayout.PropertyField(m_completeColour, true);
            }

            ShowHeader("Editor properties");
            EditorGUILayout.PropertyField(m_update, true);

            serializedObject.ApplyModifiedProperties();
        }

        /// @param headerText
        ///     The header to display
        ///     
        private void ShowHeader(string headerText)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
        }
    }
}
