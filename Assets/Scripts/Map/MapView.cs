//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MAGTask
{
    /// View component for the Map scene
    ///
    public sealed class MapView : SceneFSMView
    {
        public List<MapNodeView> Nodes { get { return m_nodes; } }

        [SerializeField]
        private List<MapNodeView> m_nodes = new List<MapNodeView>();

        #region Protected functions
        /// Initialises the view
        ///
        protected override void InitialiseInternal()
        {
        }
        #endregion
    }
}