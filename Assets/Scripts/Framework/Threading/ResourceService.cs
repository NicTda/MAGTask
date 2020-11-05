//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Service that handle particular resource loading cases
    /// 
	public sealed class ResourceService : Service
    {
        public event Action OnResourcesLoaded;

        private const int k_simultaneousLoading = 10;

        /// Container class for a loading request
        /// 
        private class LoadRequest
        {
            public string m_path = string.Empty;
            public Transform m_parent = null;
            public string m_name = string.Empty;
            public Action<GameObject> m_callback = null;
            public bool m_cancelled = false;
        }

        private TaskSchedulerService m_taskScheduler = null;
        private Queue<LoadRequest> m_loadingQueue = new Queue<LoadRequest>();
        private int m_amountLoading = 0;

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            m_taskScheduler = GlobalDirector.Service<TaskSchedulerService>();
        }
        #endregion

        #region Public functions
        /// @param path
        ///     File path to the resource to load
        /// @param parent
        ///     Transform to parent the instantiated GameObject to
        /// @param callback
        ///     The function to call when the object is instantiated
        /// 
        public void RequestLoad(string path, Transform parent, Action<GameObject> callback = null)
        {
            RequestLoad(path, parent, string.Empty, callback);
        }

        /// @param path
        ///     File path to the resource to load
        /// @param parent
        ///     Transform to parent the instantiated GameObject to
        /// @param name
        ///     The name of the object
        /// @param callback
        ///     The function to call when the object is instantiated
        /// 
        public void RequestLoad(string path, Transform parent, string name, Action<GameObject> callback = null)
        {
            m_loadingQueue.Enqueue(new LoadRequest()
            {
                m_path = path,
                m_name = name,
                m_parent = parent,
                m_callback = callback
            });

            CheckRequests();
        }

        /// Cancels all requests
        /// 
        public void CancelAllRequests()
        {
            m_loadingQueue.Clear();
        }

        /// @param parent
        ///     The parent object of the requests to cancel
        /// 
        public void CancelRequestWithParent(Transform parent)
        {
            foreach (var request in m_loadingQueue)
            {
                if (request.m_parent == parent)
                {
                    request.m_cancelled = true;
                    break;
                }
            }
        }

        /// @param parent
        ///     The parent object of the requests to cancel
        /// 
        public void CancelAllRequestsWithParent(Transform parent)
        {
            foreach (var request in m_loadingQueue)
            {
                if (request.m_parent == parent)
                {
                    request.m_cancelled = true;
                }
            }
        }

        /// @param path
        ///     File path to the resource to cancel
        /// 
        public void CancelRequestWithPath(string path)
        {
            foreach (var request in m_loadingQueue)
            {
                if (request.m_path == path)
                {
                    request.m_cancelled = true;
                    break;
                }
            }
        }

        /// @param path
        ///     File path to the resource to cancel
        /// 
        public void CancelAllRequestsWithPath(string path)
        {
            foreach (var request in m_loadingQueue)
            {
                if (request.m_path == path)
                {
                    request.m_cancelled = true;
                }
            }
        }
        #endregion

        #region Private functions
        /// Checks if there is any request to be executed
        /// 
        private void CheckRequests()
        {
            if (m_amountLoading < k_simultaneousLoading)
            {
                if (m_loadingQueue.Count > 0)
                {
                    ++m_amountLoading;
                    var request = m_loadingQueue.Dequeue();
                    if (request.m_cancelled == true)
                    {
                        OnRequestExecuted();
                    }
                    else
                    {
                        m_taskScheduler.ScheduleMainThreadTask(() =>
                        {
                            ExecuteRequest(request, OnRequestExecuted);
                        });
                    }
                }
                else if (m_amountLoading == 0)
                {
                    OnResourcesLoaded.SafeInvoke();
                }
            }
        }

        /// Called when a request has been executed
        /// 
        private void OnRequestExecuted()
        {
            --m_amountLoading;
            CheckRequests();
        }

        /// @param request
        ///     The load request to execute
        /// @param callback
        ///     The function to call when the resource is loaded
        /// 
        private void ExecuteRequest(LoadRequest request, Action callback)
        {
            ResourceUtils.LoadAndInstantiateGameObjectAsync(request.m_path, request.m_parent, request.m_name, (gameObject) =>
            {
                request.m_callback.SafeInvoke(gameObject);
                callback.SafeInvoke();
            });
        }
        #endregion
    }
}
