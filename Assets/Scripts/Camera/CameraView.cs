//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MAGTask
{
    /// View component for CameraView
    ///
    public sealed class CameraView : FSMView, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<PointerEventData> OnClickDownRequested;
        public event Action<PointerEventData> OnClickUpRequested;
        public event Action OnDragBeginRequested;
        public event Action<PointerEventData> OnDragRequested;
        public event Action OnDragEndRequested;

        public Camera Camera { get { return m_camera; } }
        public Transform CameraHolder { get { return m_cameraHolder; } }
        public Vector2 CameraSize { get { return m_size; } }

        [SerializeField]
        private Camera m_camera = null;
        [SerializeField]
        private Transform m_cameraHolder = null;
        [SerializeField]
        private BoxCollider2D m_collider = null;
        [SerializeField]
        private bool m_changeColliderSize = true;

        public bool m_inverseX = true;
        public bool m_inverseY = true;
        public bool m_boundlessDrag = false;

        private Vector2 m_size = Vector2.zero;

        #region FSMView functions
        /// Initialises the view
        ///
        protected override void InitialiseInternal()
        {
            m_size.y = m_camera.orthographicSize * 2;
            m_size.x = m_size.y * ScreenUtils.GetAspectRatio();
            if(m_changeColliderSize == true)
            {
                m_collider.size = m_size;
            }
        }
        #endregion

        #region Public functions
        /// @param eventData
        ///     The begin drag event
        ///     
        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragBeginRequested.SafeInvoke();
        }

        /// @param eventData
        ///     The drag event
        /// 
        public void OnDrag(PointerEventData eventData)
        {
            OnDragRequested.SafeInvoke(eventData);
        }

        /// @param eventData
        ///     The end drag event
        /// 
        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEndRequested.SafeInvoke();
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickDownRequested.SafeInvoke(eventData);
        }

        /// @param eventData
        ///     The data of the touch event
        ///
        public void OnPointerUp(PointerEventData eventData)
        {
            OnClickUpRequested.SafeInvoke(eventData);
        }
        #endregion
    }
}
