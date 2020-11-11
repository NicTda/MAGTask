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

        private const string k_stateNone = "none";
        private const string k_stateSelected = "selected";

        [SerializeField]
        private Animator m_animator = null;
        [SerializeField]
        private StateComponent m_stateComponent = null;
        [SerializeField]
        private GameObject m_linkHolder = null;
        [SerializeField]
        private SpriteRenderer m_linkSprite = null;

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
            m_stateComponent.SafeState(k_stateNone);
            m_animator.PlayAnimation(k_animPop, callback);
        }

        /// Called when the tile is selected
        /// 
        public void Bounce()
        {
            m_animator.Play(k_animBounce);
        }

        /// @param linkPosition
        ///     The position of the linked tile
        /// 
        public void Select(Vector3 linkPosition)
        {
            Bounce();
            m_stateComponent.SafeState(k_stateSelected);
            m_linkHolder.SafeSetActive(false);

            var distance = Vector3.Distance(transform.position, linkPosition);
            if (distance != 0.0f)
            {
                // Update the link
                m_linkHolder.SafeSetActive(true);
                var angle = Vector3.SignedAngle(Vector3.right, linkPosition - transform.position, Vector3.back);
                m_linkSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
                m_linkSprite.size = new Vector2(distance, m_linkSprite.size.y);
            }
        }

        /// Called when the tile is deselected
        /// 
        public void Deselect()
        {
            Bounce();
            m_stateComponent.SafeState(k_stateNone);
            m_linkHolder.SafeSetActive(false);
        }
        #endregion
    }
}
