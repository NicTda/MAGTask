//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Helps setting the orthographic camera size so content fits on all aspect ratio
    /// 
    [ExecuteInEditMode]
	public sealed class CameraSizeSetter : MonoBehaviour
    {
        [SerializeField]
        private Camera m_camera = null;
        [SerializeField]
        [Tooltip("The width in Units to fit")]
        private float m_unitWidth = 7.0f;

        #region Unity functions
        /// Awake function
        /// 
        public void Awake()
        {
            if (m_camera == null)
            {
                m_camera = GetComponent<Camera>();
            }
            UpdateCameraSize();
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool m_updateEditor = false;

        /// OnGUI function
        /// 
        public void OnGUI()
        {
            if (m_updateEditor == true)
            {
                m_updateEditor = false;
                UpdateCameraSize();
            }
        }
#endif
        #endregion

        #region Public functions
        /// Updates the camera size
        /// 
        public void UpdateCameraSize()
        {
            float screenRatio = ScreenUtils.GetAspectRatio();
            float unitHeight = m_unitWidth / screenRatio;
            m_camera.orthographicSize = unitHeight * 0.5f;
        }
        #endregion
    }
}
