//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using TMPro;
using UnityEngine;

namespace MAGTask
{
    /// View component for a Map node
    ///
    public sealed class MapNodeView : FSMView
    {
        public event Action OnRequested;

        public NodeType NodeType { get { return m_nodeType; } }
        public int LevelIndex { get { return m_levelIndex; } }

        [SerializeField]
        private TextMeshPro m_name = null;
        [SerializeField]
        private NodeType m_nodeType = NodeType.None;
        [SerializeField]
        private int m_levelIndex = 0;

        private bool m_unlocking = false;

        #region Public functions
        /// @param name
        ///     The name to set
        /// 
        public void SetName(string name)
        {
            m_name.SafeText(name);
        }

        /// Called when the player taps on the item
        ///
        public void OnTapped()
        {
            if (m_unlocking == false)
            {
                OnRequested.SafeInvoke();
            }
        }
        #endregion
    }
}
