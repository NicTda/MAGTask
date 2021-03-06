//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using DG.Tweening;
using System;
using UnityEngine;

namespace MAGTask
{
    /// View component for a tile
    ///
    public sealed class TileView : MonoBehaviour
    {
        public Action<TileView> OnTouchEnter;
        public Action<TileView> OnTouchExit;

        public Transform TileItemHolder {  get { return m_itemHolder; } }

        [HideInInspector]
        public Vector3 m_boardPosition = Vector3.zero;
        [HideInInspector]
        public TileItemView m_tileItem = null;
        [HideInInspector]
        public TileColour m_tileColour = TileColour.None;

        [SerializeField]
        private Transform m_itemHolder = null;
        [SerializeField]
        private TouchComponent m_touchComponent = null;
        [SerializeField]
        private float m_timeToDrop = 1.0f;

        private Tweener m_tweenPosition = null;

        #region Public functions
        /// OnDestroy function
        /// 
        public void OnDestroy()
        {
            m_tweenPosition.Stop();
        }

        /// @param callback
        ///     The function to call when the animation is finished
        /// 
        public void Appear(Action callback = null)
        {
            m_tileItem.Appear();
            Reposition(callback);
        }

        /// @param callback
        ///     The function to call when the animation is finished
        /// 
        public void Reposition(Action callback = null)
        {
            m_tweenPosition.Stop();
            m_tweenPosition = transform.DOMove(m_boardPosition, m_timeToDrop).OnComplete(() =>
            {
                m_tileItem.Bounce();
                callback.SafeInvoke();
            });
        }

        /// @param callback
        ///     The function to call when the animation is finished
        /// 
        public void Pop(Action callback = null)
        {
            m_tileItem.Pop(callback);
        }

        /// @param linkPosition
        ///     The position of the linked tile
        /// 
        public void Select(Vector3 linkPosition)
        {
            m_tileItem.Select(linkPosition);
        }

        /// Called when the tile is deselected
        /// 
        public void Deselect()
        {
            m_tileItem.Deselect();
        }

        /// Called when a touch enters the tile
        /// 
        public void OnTouchEntered()
        {
            OnTouchEnter.SafeInvoke(this);
        }

        /// Called when a touch exits the tile
        /// 
        public void OnTouchExited()
        {
            OnTouchExit.SafeInvoke(this);
        }
        #endregion
    }
}
