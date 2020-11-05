//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Handles back button input for now.
    ///
    public sealed class InputService : Service
    {
        private SceneService m_sceneService = null;
        private Dictionary<string, List<Action>> m_delegates = new Dictionary<string, List<Action>>();

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            m_sceneService = GlobalDirector.Service<SceneService>();
        }

        /// Update
        /// 
        public override void ServiceUpdate()
        {
            // Back button support
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var currentScene = m_sceneService.GetActiveScene();
                if ((m_delegates.ContainsKey(currentScene)) && (m_delegates[currentScene].IsNullOrEmpty() == false))
                {
                    m_delegates[currentScene].GetLast().Invoke();
                }
            }
        }
        #endregion

        #region Public functions
        /// @param callback
        ///     The function to call when the back button is pressed in the currently active scene
        /// 
        public void AddBackButtonListener(Action callback)
        {
            var currentScene = m_sceneService.GetActiveScene();
            if ((m_delegates.ContainsKey(currentScene) == false) || (m_delegates[currentScene] == null))
            {
                m_delegates[currentScene] = new List<Action>();
            }
            m_delegates[currentScene].Add(callback);
        }

        /// @param callback
        /// 	The callback to remove
        /// 
        public void RemoveBackButtonListener(Action callback)
        {
            if (m_delegates.Count > 0)
            {
                var keys = new List<string>(m_delegates.Keys);
                foreach (var key in keys)
                {
                    if (m_delegates[key] != null)
                    {
                        m_delegates[key].RemoveIfContained(callback);
                    }
                }
            }
        }
        #endregion
    }
}
