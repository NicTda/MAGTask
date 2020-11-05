//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
	/// The Metadata service handles metadata loaders.
	/// 
	public sealed class MetadataService : Service
	{
        // The list of default data / data loaders association to initialise
		private static Dictionary<System.Type, System.Type> m_defaultMetadataLoaders = new Dictionary<System.Type, System.Type>();

		private Dictionary<string, object> m_loaders = new Dictionary<string, object>();

		#region Public functions
		/// Sets a default loader for that type of data.
		/// Will only create the loader when data is requested.
		/// 
		/// @param dataType
		///     Type of the metadata
		/// @param loaderType
		///     Type of the metadata loader
		/// 
		public void SetDefaultLoader(System.Type dataType, System.Type loaderType)
		{
			m_defaultMetadataLoaders.AddOrUpdate(dataType, loaderType);
		}

        /// Manually creates a loader
        /// 
        /// @param dataType
        ///     Type of the metadata
        /// @param loaderType
        ///     Type of the metadata loader
        /// 
        public void CreateLoader(System.Type dataType, System.Type loaderType)
        {
            if(HasLoader(dataType) == false)
            {
                try
                {
					System.Activator.CreateInstance(loaderType);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(string.Format("Failed to load metadata due to exception '{0}'", e));
                }
            }
        }

		/// @param type
		/// 	The type of the serialisable data
		/// 
		/// @return Whether there is a registered loader for the given type of data
		/// 
		public bool HasLoader(System.Type type)
		{
			return HasLoader(type.ToString());
		}

		/// Generic function to retrieve the metadata loader associated to
		/// a particular type of serializable data.
		/// If no loader was found, it will try to create one from the default list of loaders.
		/// 
		/// @return The metadata loader associated to the serializable data
		/// 
		public MetadataLoader<T> GetLoader<T>() where T : class, IMetaDataSerializable, new()
		{
			var dataType = typeof(T);
			string typeName = dataType.ToString();
			if(HasLoader(typeName) == false)
			{
				Debug.Assert(m_defaultMetadataLoaders.ContainsKey(dataType), "There is no metadata loader registered for data type: " + typeName);
				CreateLoader(dataType, m_defaultMetadataLoaders[dataType]);
			}
			return m_loaders[typeName] as MetadataLoader<T>;
		}

		/// Registers a metadata loader for a specific data type.
		/// There can be only one loader for a given type, so this function will
		/// assert if a loader is already registered for the serializable data type.
		/// 
		/// @param loader
		/// 	The metadata loader object for data of type T
		/// 
		public void RegisterLoader<T>(MetadataLoader<T> loader) where T : class, IMetaDataSerializable, new()
		{
			string typeName = GetTypeName<T>();
			Debug.Assert(HasLoader(typeName) == false, "There is already a metadata loader registered for data type: " + typeName);
			m_loaders[typeName] = loader;
		}

		/// Unregisters a metadata loader for a specific data type.
		/// This function will assert if there is no loader registered for the serializable 
		/// data type.
		/// 
        public void UnregisterLoader<T>() where T : IMetaDataSerializable
		{
			string typeName = GetTypeName<T>();
			Debug.Assert(HasLoader(typeName), "There is no metadata loader registered for data type: " + typeName);
			m_loaders.Remove(typeName);
		}
        #endregion

        #region Private functions
		/// @param loaderType
		/// 	The type of the serialisable data, in string form
		/// 
		/// @return Whether there is a registered loader for the given type of data
		/// 
		private bool HasLoader(string type)
		{
			return m_loaders.ContainsKey(type);
		}

		/// @return The name of the given type
		/// 
		private string GetTypeName<T>()
		{
			return typeof(T).ToString();
		}
        #endregion
	}
}
