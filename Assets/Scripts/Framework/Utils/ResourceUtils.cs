//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// Utility functions class for Resources
    /// 
    public static class ResourceUtils
    {
        /// @param path
        ///     File path to the resource to load
        /// 
        /// @return Instantiated GameObject
        /// 
        public static GameObject LoadAndInstantiateGameObject(string path)
        {
            GameObject loadedGameObject = Load<GameObject>(path);
            GameObject instance = null;
            if (loadedGameObject != null)
            {
                instance = InstantiateGameObject(loadedGameObject);
            }
            return instance;
        }

        /// @param path
        ///     File path to the resource to load
        /// @param name
        ///     The name of the object
        /// 
        /// @return Instantiated GameObject
        /// 
        public static GameObject LoadAndInstantiateGameObject(string path, string name)
        {
            GameObject instance = LoadAndInstantiateGameObject(path);
            if (instance != null)
            {
                instance.name = name;
            }
            return instance;
        }

        /// @param path
        ///     File path to the resource to load
        /// @param parent
        ///     Transform to parent the instantiated GameObject to
        /// 
        /// @return Instantiated GameObject
        /// 
        public static GameObject LoadAndInstantiateGameObject(string path, Transform parent)
        {
            GameObject loadedGameObject = Load<GameObject>(path);
            GameObject instance = null;
            if (loadedGameObject != null)
            {
                instance = InstantiateGameObject(loadedGameObject, parent);
            }
            return instance;
        }

        /// @param path
        ///     File path to the resource to load
        /// @param parent
        ///     Transform to parent the instantiated GameObject to
        /// @param name
        ///     The name of the object
        /// 
        /// @return Instantiated GameObject
        /// 
        public static GameObject LoadAndInstantiateGameObject(string path, Transform parent, string name)
        {
            GameObject instance = LoadAndInstantiateGameObject(path, parent);
            if (instance != null)
            {
                instance.name = name;
            }
            return instance;
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
        /// @return Instantiated GameObject
        /// 
        public static void LoadAndInstantiateGameObjectAsync(string path, Transform parent, string name, Action<GameObject> callback = null)
        {
            LoadAndInstantiateGameObjectAsync(path, parent, (instance) =>
            {
                if (instance != null && name != string.Empty)
                {
                    instance.name = name;
                }
                callback.SafeInvoke(instance);
            });
        }

        /// @param path
        ///     File path to the resource to load
        /// @param parent
        ///     Transform to parent the instantiated GameObject to
        /// @param callback
        ///     The function to call when the object is instantiated
        /// 
        /// @return Instantiated GameObject
        /// 
        public static void LoadAndInstantiateGameObjectAsync(string path, Transform parent, Action<GameObject> callback = null)
        {
            var resourceRequest = Resources.LoadAsync<GameObject>(path);
            resourceRequest.completed += (operation) =>
            {
                GameObject instance = null;
                var prefab = resourceRequest.asset as GameObject;
                if(prefab != null)
                {
                    instance = InstantiateGameObject(prefab, parent);
                }
                callback.SafeInvoke(instance);
            };
        }

        /// Loads a resource of type T
        /// 
        /// @param path
        ///     Path of the Object to load
        /// 
        /// @return Loaded resource
        /// 
        public static T Load<T>(string path) where T: UnityEngine.Object
        {
            T prefab = Resources.Load<T>(path);
#if UNITY_EDITOR
            if(prefab == null)
            {
                Debug.LogError(string.Format("Could not load prefab at '{0}' of type '{1}'", path, typeof(T).ToString()));
            }
#endif
            return prefab;
        }

        /// @param path
        ///     game object to be instatiated
        /// 
        /// @return Instantiated GameObject
        /// 
        public static GameObject InstantiateGameObject(GameObject gameObject)
        {
            GameObject instance = GameObject.Instantiate(gameObject);

#if UNITY_EDITOR
            Debug.Assert(instance != null, string.Format("Could not instantiate prefab '{0}'", gameObject.name));
#endif

            return instance;
        }

		/// @param path
		///     game object to be instatiated
		/// @param parent
		///     Transform to parent the instantiated GameObject to
		/// 
		/// @return Instantiated GameObject
		/// 
		public static GameObject InstantiateGameObject(GameObject gameObject, Transform parent)
		{
			GameObject instance = GameObject.Instantiate(gameObject, parent);

#if UNITY_EDITOR
			Debug.Assert(instance != null, string.Format("Could not instantiate prefab '{0}'", gameObject.name));
#endif

            return instance;
		}
    }
}
