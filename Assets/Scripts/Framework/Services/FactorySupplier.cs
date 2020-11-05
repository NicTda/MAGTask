//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreFramework
{
	/// Supplier of factories
    /// 
	public class FactorySupplier
	{
        private Dictionary<System.Type, Factory> m_factories = new Dictionary<System.Type, Factory>();

	    #region Public functions
        /// Registers a new factory
        /// 
        /// @return The newly created factory, or the existing factory if it already exists
        /// 
        public T RegisterFactory<T>() where T : Factory, new()
        {
            return RegisterFactory<T>(typeof(T));
        }

        /// Registers a new factory
        /// 
        /// @param baseFactory
        ///     Type of the base factory to override
        /// 
        /// @return The newly created factory, or the existing factory if it already exists
        /// 
        public T RegisterFactory<T>(System.Type baseFactory) where T : Factory, new()
        {
            T factory = null;
            if(m_factories.ContainsKey(baseFactory) == false)
            {
                factory = new T();
                m_factories.Add(baseFactory, factory);
            }
            else
            {
                Debug.LogError(string.Format("FactorySupplier already contains a factory for type {0}", baseFactory));

                factory = GetFactory<T>();
            }

            return factory;
        }

        /// @return Factory of the desired base type, or null of not found
        /// 
        public T GetFactory<T>() where T : Factory
        {
            T output = null;
            if(m_factories.ContainsKey(typeof(T)) == true)
            {
                output = (T)m_factories[typeof(T)];
            }
            else
            {
                Debug.LogError(string.Format("No factory exists for type {0}", typeof(T)));
            }

            return output;
        }

        /// Completes the initialisation for all registered factories
        /// 
        /// @param director
        ///     The Director owner of the instance
        /// 
        public void InitialiseAllFactories(Director director)
        {
            List<Factory> factories = m_factories.Values.ToList();
            foreach(Factory factory in factories)
            {
                factory.CompleteInitialisation(director);
            }
        }
        #endregion
	}
}
