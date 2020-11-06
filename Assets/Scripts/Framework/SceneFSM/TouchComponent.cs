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
    public sealed class TouchComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action OnTouchEntered;
        public event Action OnTouchExited;
        public event Action<Vector2> OnTouchedDown;
        public event Action<Vector2> OnTouchedUp;
        public event Action<Vector2> OnTapped;

        public BoxCollider2D TouchCollider { get { return m_touchCollider; } }

        [Serializable]
        public class TapEvent : UnityEvent<Vector2> { }
        [Serializable]
        public class TouchEvent : UnityEvent { }

        [SerializeField]
        public int m_touchPriority = 0;
        [SerializeField]
        private BoxCollider2D m_touchCollider = null;
        [SerializeField]
        private TapEvent m_tapEvent = null;
        [SerializeField]
        private TouchEvent m_downEvent = null;
        [SerializeField]
        private TouchEvent m_upEvent = null;
        [SerializeField]
        private TouchEvent m_enterEvent = null;
        [SerializeField]
        private TouchEvent m_exitEvent = null;

        private bool m_touched = false;
        private bool m_entered = false;

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
            if (m_downEvent != null)
            {
                m_downEvent.Invoke();
            }
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerUp(PointerEventData eventData)
        {
            OnTouchedUp.SafeInvoke(eventData.position);
            if (m_upEvent != null)
            {
                m_upEvent.Invoke();
            }
            if (m_touched == true)
            {
                OnTapped.SafeInvoke(eventData.position);
                if (m_tapEvent != null)
                {
                    m_tapEvent.Invoke(eventData.position);
                }
            }
            m_touched = false;
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_entered = true;
            OnTouchEntered.SafeInvoke();
            if (m_enterEvent != null)
            {
                m_enterEvent.Invoke();
            }
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerExit(PointerEventData eventData)
        {
            if(m_entered == true)
            {
                m_entered = false;
                OnTouchExited.SafeInvoke();
                if (m_exitEvent != null)
                {
                    m_exitEvent.Invoke();
                }
            }
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
