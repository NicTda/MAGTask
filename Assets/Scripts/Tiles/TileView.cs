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

        private const string k_animAppear = "Appear";
        private const string k_animBounce = "Bounce";
        private const string k_animPop = "Pop";
        private const string k_animSelected = "Selected";

        public int m_index = 0;

        [SerializeField]
        private Animator m_animator = null;
        [SerializeField]
        private TouchComponent m_touchComponent = null;
        [SerializeField]
        private GameObject m_selected = null;

        Tweener m_tweenPosition = null;

        #region Public functions
        /// Called when the tile should appear
        /// 
        public void Appear()
        {
            m_animator.Play(k_animAppear);
        }

        /// Called when the tile should pop
        /// 
        public void Pop()
        {
            // TODO TDA: Particles
            m_animator.Play(k_animPop);
            m_selected.SafeSetActive(false);
        }

        /// Called when the tile is selected
        /// 
        public void Select()
        {
            m_animator.Play(k_animBounce);
            m_selected.SafeSetActive(true);
        }

        /// Called when the tile is deselected
        /// 
        public void Deselect()
        {
            m_animator.Play(k_animBounce);
            m_selected.SafeSetActive(false);
        }

        /// Called when the player taps the tile
        /// 
        public void OnTapped()
        {
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
