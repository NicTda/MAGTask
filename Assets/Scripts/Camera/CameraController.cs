//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MAGTask
{
    /// Controller of the Camera
    /// 
	public sealed class CameraController : FSMController
    {
        public event Action<Vector3> OnCameraMoved;
        public event Action<Vector3> OnCameraTapped;

        private const string k_actionIdle = "Idle";
        private const string k_actionPan = "Pan";
        private const string k_actionDrag = "Drag";
        private const string k_actionDragEnd = "DragEnd";

        private const string k_stateIdle = "Idle";
        private const string k_statePanning = "Panning";
        private const string k_stateDragging = "Dragging";
        private const string k_stateDragInertia = "DragInertia";

        private const float k_inertiaThreshold = 1.0f;
        private const float k_focusTime = 1.0f;

        private CameraView m_view = null;
        private Vector2 m_lastMovement = Vector2.zero;
        private float m_inertiaDecrease = 0.85f;

        private Vector3 m_origin = Vector3.zero;
        private Vector3 m_offset = Vector3.zero;
        private Vector3 m_minPos = Vector3.zero;
        private Vector3 m_maxPos = Vector3.zero;

        private Ease m_focusEase = Ease.OutBack;
        private Vector3 m_focusTarget = Vector3.zero;
        private Tweener m_panTween = null;
        private float m_panTime = 0.0f;
        private bool m_dragEnabled = true;

        private DateTime m_clickDownTime = DateTime.MinValue;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param view
        ///     The view
        /// 
        public CameraController(LocalDirector localDirector, CameraView view, BoxCollider2D cameraBounds = null)
        	:base(localDirector, view)
        {
            m_view = view;
            m_origin = m_view.CameraHolder.localPosition;

            if(cameraBounds != null)
            {
                SetScreenBounds(cameraBounds.bounds.min, cameraBounds.bounds.max);
            }

            m_fsm.RegisterStateCallback(k_stateIdle, EnterStateIdle, null, ExitStateIdle);
            m_fsm.RegisterStateCallback(k_statePanning, EnterStatePanning, null, ExitStatePanning);
            m_fsm.RegisterStateCallback(k_stateDragging, EnterStateDragging, null, ExitStateDragging);
            m_fsm.RegisterStateCallback(k_stateDragInertia, EnterStateDragInertia, UpdateStateDragInertia, ExitStateDragInertia);
            m_fsm.ExecuteAction(k_actionIdle);
        }

        /// Implement this method if the child class needs to be disposed
        /// 
        public override void OnDispose()
        {
            m_panTween.Stop();
            base.OnDispose();
        }
        #endregion

        #region Public functions
        /// @param minPos
        ///     The min position the camera can be
        /// @param maxPos
        ///     The max position the camera can be
        ///     
        public void SetScreenBounds(Vector3 minPos, Vector3 maxPos)
        {
            var cameraSize = m_view.CameraSize;

            m_minPos.x = Mathf.Min(minPos.x + (cameraSize.x * 0.5f), m_origin.x);
            m_minPos.y = Mathf.Min(minPos.y + (cameraSize.y * 0.5f), m_origin.y);

            m_maxPos.x = Mathf.Max(maxPos.x - (cameraSize.x * 0.5f), m_origin.x);
            m_maxPos.y = Mathf.Max(maxPos.y - (cameraSize.y * 0.5f), m_origin.y);
        }

        /// @param minPos
        ///     The min position the camera can be
        /// @param maxPos
        ///     The max position the camera can be
        ///     
        public void SetCameraBounds(Vector3 minPos, Vector3 maxPos)
        {
            m_minPos = minPos;
            m_maxPos = maxPos;
        }

        /// @return The current camera
        ///     
        public Camera GetCamera()
        {
            return m_view.Camera;
        }

        /// @return The current camera's position
        ///     
        public Vector3 GetCameraPosition()
        {
            return m_view.CameraHolder.localPosition;
        }

        /// @return The camera's size
        ///     
        public Vector2 GetCameraSize()
        {
            return m_view.CameraSize;
        }

        /// @param target
        ///     The location of the target to focus on
        /// @param time
        ///     The time to take during the panning
        ///     
        public void FocusOnTarget(Vector3 target, float time = k_focusTime)
        {
            m_focusTarget = target;
            m_panTime = time;
            m_fsm.ExecuteAction(k_actionPan);
        }

        /// @param target
        ///     The location of the target to focus on
        /// @param time
        ///     The time to take during the panning
        /// @param ease
        ///     The easing function to use for panning
        ///     
        public void FocusOnTargetWithinBounds(Vector3 target, float time = k_focusTime, Ease ease = Ease.OutBack)
        {
            m_focusEase = ease;
            m_focusTarget = target;
            if (m_view.m_boundlessDrag == false)
            {
                m_focusTarget.x = Mathf.Clamp(m_focusTarget.x, m_minPos.x, m_maxPos.x);
                m_focusTarget.y = Mathf.Clamp(m_focusTarget.y, m_minPos.y, m_maxPos.y);
            }
            m_panTime = time;
            m_fsm.ExecuteAction(k_actionPan);
        }

        /// @param target
        ///     The location of the target to focus on
        ///     
        public void FocusOnTargetImmediate(Vector3 target)
        {
            FocusOnTarget(target, 0.0f);
        }

        /// @param enabled
        ///     Whether the drag should be enabled or not
        ///     
        public void SetDragEnabled(bool enabled)
        {
            m_dragEnabled = enabled;
        }
        #endregion

        #region FSM functions
        /// Start of the Idle state
        /// 
        private void EnterStateIdle()
        {
            m_view.OnClickDownRequested += OnClickDownRequested;
            m_view.OnClickUpRequested += OnClickUpRequested;
            m_view.OnDragBeginRequested += OnDragBeginRequested;
        }

        /// @param eventData
        ///     The tap event
        ///     
        private void OnClickDownRequested(PointerEventData eventData)
        {
            m_clickDownTime = DateTime.Now;
            CheckPropagateTouch(eventData, (touchComponent) =>
            {
                touchComponent.OnPointerDown(eventData);
            });
        }

        /// @param eventData
        ///     The tap event
        ///     
        private void OnClickUpRequested(PointerEventData eventData)
        {
            if((DateTime.Now - m_clickDownTime) <= TimeSpan.FromSeconds(1))
            {
                var propagated = CheckPropagateTouch(eventData, (touchComponent) =>
                {
                    touchComponent.OnPointerUp(eventData);
                });

                if(propagated == false)
                {
                    OnCameraTapped.SafeInvoke(eventData.pointerCurrentRaycast.worldPosition);
                }
            }
        }

        /// Called when a drag is requested
        /// 
        private void OnDragBeginRequested()
        {
            if(m_dragEnabled == true)
            {
                m_fsm.ExecuteAction(k_actionDrag);
            }
        }

        /// End of the Idle state
        /// 
        private void ExitStateIdle()
        {
            m_view.OnClickDownRequested -= OnClickDownRequested;
            m_view.OnClickUpRequested -= OnClickUpRequested;
            m_view.OnDragBeginRequested -= OnDragBeginRequested;
        }

        /// Start of the Panning state
        /// 
        private void EnterStatePanning()
        {
            var cameraPosition = m_focusTarget;
            cameraPosition.z = m_view.CameraHolder.localPosition.z;

            // Start panning to the target
            m_panTween = m_view.CameraHolder.DOMove(cameraPosition, m_panTime)
                .SetEase(m_focusEase)
                .OnComplete(OnPanningComplete);
        }

        /// Called when the panning is done
        /// 
        private void OnPanningComplete()
        {
            m_offset = m_view.CameraHolder.position - m_origin;
            m_fsm.ExecuteAction(k_actionIdle);
        }

        /// End of the Panning state
        /// 
        private void ExitStatePanning()
        {
        }

        /// Start of the Dragging state
        /// 
        private void EnterStateDragging()
        {
            m_view.OnDragRequested += OnDragRequested;
            m_view.OnDragEndRequested += OnDragEndRequested;
        }

        /// Called when a drag ends
        /// 
        private void OnDragEndRequested()
        {
            m_fsm.ExecuteAction(k_actionDragEnd);
        }

        /// @param eventData
        ///     The drag event
        /// 
        private void OnDragRequested(PointerEventData eventData)
        {
            m_lastMovement = eventData.delta;
            MoveCamera(m_lastMovement);
        }

        /// End of the Dragging state
        /// 
        private void ExitStateDragging()
        {
            m_view.OnDragRequested -= OnDragRequested;
            m_view.OnDragEndRequested -= OnDragEndRequested;
        }

        /// Start of the DragInertia state
        /// 
        private void EnterStateDragInertia()
        {
            m_view.OnClickDownRequested += OnClickDownRequested;
            m_view.OnClickUpRequested += OnClickUpRequested;
            m_view.OnDragBeginRequested += OnDragBeginRequested;
        }

        /// Update of the DragInertia state
        /// 
        private void UpdateStateDragInertia()
        {
            m_lastMovement *= m_inertiaDecrease;
            if(!MoveCamera(m_lastMovement) || m_lastMovement.sqrMagnitude <= k_inertiaThreshold)
            {
                m_fsm.ExecuteAction(k_actionIdle);
            }
        }

        /// End of the DragInertia state
        /// 
        private void ExitStateDragInertia()
        {
            m_view.OnClickDownRequested -= OnClickDownRequested;
            m_view.OnClickUpRequested -= OnClickUpRequested;
            m_view.OnDragBeginRequested -= OnDragBeginRequested;
        }
        #endregion

        #region Private functions
        /// @param deltaMovement
        ///     The movement to apply
        ///     
        /// @return Whether the camera was moved - false if reaching its bounds
        /// 
        private bool MoveCamera(Vector2 deltaMovement)
        {
            bool moved = false;
            var dragDelta = deltaMovement;

            // Inverse axis
            if (m_view.m_inverseX == true)
            {
                dragDelta.x *= -1;
            }
            if (m_view.m_inverseY == true)
            {
                dragDelta.y *= -1;
            }
            m_offset += m_view.CameraHolder.TransformDirection((Vector3)(dragDelta * m_view.Camera.orthographicSize / m_view.Camera.pixelHeight * 2.0f));

            var position = m_origin + m_offset;
            if (m_view.m_boundlessDrag == false)
            {
                position.x = Mathf.Clamp(position.x, m_minPos.x, m_maxPos.x);
                position.y = Mathf.Clamp(position.y, m_minPos.y, m_maxPos.y);
                m_offset = position - m_origin;
            }

            if(m_view.CameraHolder.position != position)
            {
                moved = true;
                m_view.CameraHolder.position = position;

                OnCameraMoved.SafeInvoke(position);
            }
            return moved;
        }

        /// @param eventData
        ///     The data of the touch event to check
        /// @param action
        ///     The action to perform if we need to propagate the touch
        ///     
        /// @return Whether the touch was propagated
        ///
        private bool CheckPropagateTouch(PointerEventData eventData, Action<TouchComponent> action)
        {
            bool propagated = false;
            if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId) == true)
            {
                // Cast a ray to see who's hit
                List<RaycastResult> results = new List<RaycastResult>(10);
                EventSystem.current.RaycastAll(eventData, results);

                // Get a list of hit TouchComponent
                List<TouchComponent> touchComponents = new List<TouchComponent>(results.Count);
                foreach (var result in results)
                {
                    if (result.gameObject != m_view.gameObject)
                    {
                        var touchComponent = result.gameObject.GetComponent<TouchComponent>();
                        if (touchComponent != null)
                        {
                            touchComponents.Add(touchComponent);
                        }
                    }
                }

                if (touchComponents.Count > 1)
                {
                    // Sort the list by priority
                    touchComponents.Sort((t1, t2) => t2.m_touchPriority - t1.m_touchPriority);
                }

                if (touchComponents.Count > 0)
                {
                    // Call the first component
                    action.SafeInvoke(touchComponents.GetFirst());
                    propagated = true;
                }
            }
            return propagated;
        }
        #endregion
    }
}
