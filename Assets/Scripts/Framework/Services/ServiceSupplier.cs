//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Object responsible to handle a set of services.
    /// It lets the user to register services and to access them. Being a
    /// Disposable object means that the owner will must take care of
    /// disposing it when it won't be needed
    /// 
    public class ServiceSupplier : IDisposable
    {
        private Dictionary<Type, Service> m_services = new Dictionary<Type, Service>();
        private List<Service> m_servicesList = new List<Service>();

        #region Public functions
        /// Disposes all services
        /// 
        public void Dispose()
        {
            // services are unregistered in the opposite order used to register them
            for(int service = m_servicesList.Count - 1; service >= 0; --service)
            {
                m_servicesList[service].OnUnregister();
            }

            m_servicesList.Clear();
            m_services.Clear();
        }

        /// Updates all registered services
        /// 
		public void Update()
        {
            for(int service = 0; service < m_servicesList.Count; ++service)
            {
                m_servicesList[service].ServiceUpdate();
            }
        }

        /// Informs all services that the application has been paused
        /// 
		public void OnApplicationPause()
        {
            for(int service = 0; service < m_servicesList.Count; ++service)
            {
                m_servicesList[service].OnApplicationPause();
            }
        }

        /// Informs all services that the application has been resumed
        /// 
		public void OnApplicationResume()
        {
            for(int service = 0; service < m_servicesList.Count; ++service)
            {
                m_servicesList[service].OnApplicationResume();
            }
        }

        /// Registers a new service
        /// 
        /// @return The newly created service, or the existing service if it already exists
        /// 
        public T RegisterService<T>() where T : Service, new()
        {
            T service = null;
            Type serviceType = typeof(T);
            if (m_services.ContainsKey(serviceType) == false)
            {
                service = new T();

                var baseType = service.GetBaseType();
                if(baseType != typeof(T))
                {
                    if(TryRegisterBaseType(service, baseType) == false)
                    {
                        Debug.LogAssertionFormat("ServiceSupplier already contains a base service {0} for derived type {1}", baseType, serviceType);
                    }
                }

                m_services.Add(serviceType, service);
                m_servicesList.Add(service);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("ServiceSupplier already contains a service of type {0}", serviceType);
#endif
                // There's already a service of that type, return it
                service = GetService<T>();
            }

            return service;
        }

        /// Completes the initialisation for all registered services
        /// 
        /// @param localDirector
        ///     The Director owner of the instance
        /// 
        public void InitialiseAllServices(Director localDirector)
        {
            for(int service = 0; service < m_servicesList.Count; ++service)
            {
                m_servicesList[service].CompleteInitialisation(localDirector);
            }
        }

        /// @return whether all services currently registered are ready to run
        /// 
		public bool AllServicesReady()
        {
            bool returnValue = true;
            for(int service = 0; service < m_servicesList.Count; ++service)
            {
                if(m_servicesList[service].IsServiceReady() == true)
                {
                    m_servicesList[service].TriggerReady();
                }
                else
                {
                    returnValue = false;
                }
            }
            return returnValue;
        }

        /// @return the instance of the requested service if any, null otherwise
        /// 
        public T GetService<T>() where T : Service
        {
            T returnValue = m_services.TryGetValue(typeof(T)) as T;
            if (returnValue == null)
            {
                Debug.LogWarningFormat("Service {0} not registered - returns null", typeof(T));
            }
            return returnValue;
        }

        /// @return Whether the service is already registered
        /// 
        public bool HasService<T>() where T : Service
        {
            return m_services.ContainsKey(typeof(T));
        }
        #endregion

        #region Private functions
        /// @param service
        ///     The service to register
        /// @param baseType
        ///     The base type to regiter
        /// 
        /// @return Whether the registration was completed or not
        /// 
        private bool TryRegisterBaseType(Service service, Type baseType)
        {
            bool registered = true;
            if (baseType != null)
            {
                if (m_services.ContainsKey(baseType) == false)
                {
                    m_services.Add(baseType, service);
                }
                else
                {
                    registered = false;
                }
            }
            return registered;
        }
        #endregion
    }
}
