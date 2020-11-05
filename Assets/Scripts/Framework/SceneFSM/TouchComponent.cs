//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CoreFramework
{
    /// Component that handles touch input
    ///
    public sealed class TouchComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<Vector2> OnTouchedDown;
        public event Action<Vector2> OnTouchedUp;
        public event Action<Vector2> OnTapped;

        public BoxCollider2D TouchCollider { get { return m_touchCollider; } }

        [Serializable]
        public class TapEvent : UnityEvent<Vector2> { }

        [SerializeField]
        public int m_touchPriority = 0;
        [SerializeField]
        private BoxCollider2D m_touchCollider = null;
        [SerializeField]
        private TapEvent m_tapEvent = null;

        private bool m_touched = false;

        #region Unity functions
        /// Awake function
        ///
        public void Awake()
        {
            if(m_touchCollider == null)
            {
                m_touchCollider = GetComponent<BoxCollider2D>();
            }

            if(m_touchCollider != null && m_touchCollider.gameObject != gameObject)
            {
                Debug.LogError("TouchComponent should be on same level as BoxCollider - " + name);
            }
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerDown(PointerEventData eventData)
        {
            OnTouchedDown.SafeInvoke(eventData.position);
            m_touched = true;
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerUp(PointerEventData eventData)
        {
            OnTouchedUp.SafeInvoke(eventData.position);
            if(m_touched == true)
            {
                OnTapped.SafeInvoke(eventData.position);
                if (m_tapEvent != null)
                {
                    m_tapEvent.Invoke(eventData.position);
                }
            }
            m_touched = false;
        }
        #endregion

        #region Public functions
        /// @param enable
        ///     Whether the touch is enabled
        /// 
        public void EnableTouch(bool enable)
        {
            m_touchCollider.enabled = enable;
        }

        /// @param size
        ///     The collider's size
        /// @param offset
        ///     The collider's offset
        /// 
        public void UpdateTouchSize(Vector2 size, Vector2 offset)
        {
            m_touchCollider.size = size;
            m_touchCollider.offset = offset;
        }
        #endregion
    }
}
