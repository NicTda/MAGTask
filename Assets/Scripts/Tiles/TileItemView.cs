//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using UnityEngine;

namespace MAGTask
{
    /// View component for a tile
    ///
    public sealed class TileItemView : MonoBehaviour
    {
        private const string k_animAppear = "Appear";
        private const string k_animBounce = "Bounce";
        private const string k_animPop = "Pop";
        private const string k_animSelected = "Selected";

        [SerializeField]
        private Animator m_animator = null;
        [SerializeField]
        private GameObject m_selected = null;

        #region Public functions
        /// Called when the tile should appear
        /// 
        public void Appear()
        {
            m_animator.Play(k_animAppear);
        }

        /// @param callback
        ///     The function to call when the animation is finished
        /// 
        public void Pop(Action callback = null)
        {
            m_selected.SafeSetActive(false);
            m_animator.PlayAnimation(k_animPop, callback);
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
        #endregion
    }
}
