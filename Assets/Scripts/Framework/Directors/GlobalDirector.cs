//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;
using System.Collections;

namespace CoreFramework
{
    /// The GlobalDirector is a singleton Director that will
    /// instantiate, manage and access all global services of the game.
    /// This class is abstract so each game can specify its own
    /// global services
    /// 
    public abstract class GlobalDirector : Director
    {
        private static event System.Action OnReady;

        private const string k_directorName = "GlobalDirector";
        private const string k_defaultDirectorPath = "Prefabs/GlobalDirector";

        // When closing the application we can't control to destroy the GlobalDirector
        // after everything else, so we need this boolean in case it is destroyed before
        // other Directors
        private static bool s_isDisposed = false;

        private static GlobalDirector s_instance = null;

        #region Director functions
        /// Initialises the singleton reference. If this is already set it
        /// proceeds to destroy the current instance. It also create and
        /// initialises the FSM
        /// 
        /// @return Whether the Director initialised correctly
        /// 
        protected override bool OnDirectorAwake()
        {
            bool result = true;

            if(s_instance == null)
            {
                s_instance = this;

#if UNITY_EDITOR || DEBUG
                LocalDirector[] localDirectors = GetComponents<LocalDirector>();
                Debug.Assert(localDirectors.Length == 0, "GlobalDirector GameObject should not have a LocalDirector on the same object");
#endif
                DontDestroyOnLoad(gameObject);
            }
            else if(s_instance != this)
            {
                m_disposed = true;
                Destroy(this);
                result = false;
            }

            return result;
        }

        /// Disposes the static instance of the singleton
        /// 
		protected override void OnDirectorDispose()
        {
            if(s_instance == this)
            {
                s_instance = null;
                s_isDisposed = true;
            }
        }

        /// If the application is not closed explicitly (for example closing
        /// it in the Unity Editor) this event will be called when
        /// m_disposed is false
        /// 
        private void OnApplicationQuit()
        {
            if(m_disposed == false)
            {
                LocalDirector[] localDirectors = FindObjectsOfType<LocalDirector>();
                for(int director = 0; director < localDirectors.Length; ++director)
                {
                    localDirectors[director].Dispose();
                }

                Dispose();
            }
        }
        /// Starts the initialised State
        /// 
        protected override void StartInitialisedState()
        {
            OnReady.SafeInvoke();
            OnReady = null;
        }
        #endregion

        #region Public functions
        /// @return Whether the director can be called
        /// 
        public static bool HasInstance()
        {
            return (s_isDisposed == false) && (s_instance != null);
        }

        /// @return the reference to the requested Service if the GlobalDirector
        /// 		has completed the initialisation and if any; null otherwise
        /// 
        public static T Service<T>() where T : Service
        {
            T returnValue = null;
            if (GetInstance())
            {
                returnValue = GetInstance().GetService<T>();
            }
            else
            {
                Debug.LogError(string.Format("Cannot access '{0}' until the GlobalDirector is fully initialised", typeof(T)));
            }

            return returnValue;
        }

        /// @return the reference to the requested Factory if the GlobalDirector
        ///     has completed the initialisation and if any; otherwise null
        /// 
        public static T Factory<T>() where T : Factory
        {
            T returnValue = null;
            if(GetInstance())
            {
                returnValue = GetInstance().GetFactory<T>();
            }
            else
            {
                Debug.LogError(string.Format("Cannot access '{0}' until the GlobalDirector is fully initialised", typeof(T)));
            }

            return returnValue;
        }

        /// @return whether the GlobalDirector is ready to be used or not
        /// 
        public static bool IsGlobalDirectorReady()
        {
            return GetInstance().IsReady();
        }

        /// @param callback
        ///     The function to call when the Global Director is ready
        /// 
        public static void CallWhenReady(System.Action callback)
        {
            if (HasInstance() && GetInstance().IsReady())
            {
                callback.SafeInvoke();
            }
            else
            {
                OnReady += callback;
            }
        }

        /// @param function
        ///     Function to start as a coroutine
        /// 
        /// @return The started coroutine
        /// 
        public static Coroutine ExecuteCoroutine(IEnumerator function)
        {
            return GetInstance().StartCoroutine(function);
        }
        
        /// @param coroutine
        ///     Coroutine to stop
        /// 
        public static void CancelCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                GetInstance().StopCoroutine(coroutine);
            }
        }

        /// @param time
        ///		the time to wait for in seconds
        /// @param callback
        ///		the action to perform once the time is up
        /// 
        /// @return The started coroutine
        ///
        public static Coroutine WaitForRealTime(float time, System.Action callback)
        {
            return ExecuteCoroutine(WaitForRealSeconds(time, callback));
        }

        /// @param time
        ///		the time to wait for in seconds
        /// @param callback
        ///		the action to perform once the time is up
        /// 
        /// @return The started coroutine
        ///
        public static Coroutine WaitFor(float time, System.Action callback)
        {
            return ExecuteCoroutine(WaitForSeconds(time, callback));
        }

        /// @param callback
        ///		the action to perform once the frame ended
        /// 
        /// @return The started coroutine
        ///
        public static Coroutine WaitForFrame(System.Action callback)
        {
            return ExecuteCoroutine(WaitForEndOfFrame(callback));
        }
        #endregion

        #region Private functions
        /// Returns the reference to the instance. If it's null it tries to 
        /// retrieve it into the scene.
        /// 
        /// @return the singleton instance
        /// 
        private static GlobalDirector GetInstance()
		{
			if(s_instance == null)
			{
                if(s_isDisposed == false)
				{
					s_instance = FindObjectOfType<GlobalDirector>();
					if(s_instance == null)
                    {
                        // Try to load default director
                        var directorPrefab = Resources.Load<GameObject>(k_defaultDirectorPath);
                        if(directorPrefab == null)
                        {
                            Debug.LogError("Failed to find a GlobalDirector - and to create a default one");
                        }
                        else
                        {
                            // Create the default director
                            var directorObject = Instantiate(directorPrefab);
                            directorObject.name = k_directorName;
                        }
					}
				}
				else
				{
					Debug.LogError("The GlobalDirector has been already disposed");
				}
			}

			return s_instance;
        }

        /// @param time
        ///		the time to wait for in seconds
        /// @param callback
        ///		the action to perform once the time is up
        ///
        private static IEnumerator WaitForRealSeconds(float time, System.Action callback)
        {
            yield return new WaitForSecondsRealtime(time);

            callback.SafeInvoke();
        }

        /// @param time
        ///		the time to wait for in seconds
        /// @param callback
        ///		the action to perform once the time is up
        ///
        private static IEnumerator WaitForSeconds(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);

            callback.SafeInvoke();
        }

        /// @param callback
        ///		the action to perform once the frame ended
        ///
        private static IEnumerator WaitForEndOfFrame(System.Action callback)
        {
            yield return null;

            callback.SafeInvoke();
        }
        #endregion
    }
}
