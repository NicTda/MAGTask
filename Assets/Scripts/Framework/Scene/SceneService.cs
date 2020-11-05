//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreFramework
{
    /// Handles the transition between scenes.
    /// 
    public sealed class SceneService : Service
    {
        public event Action<string> OnSceneActive;

        private const string k_pathTransitionUI = "Prefabs/Transitions/";
        private const int k_defaultCanvasSortOrder = 1000;

        private Dictionary<string, Action> m_unloadedDelegates = new Dictionary<string, Action>();
        private List<string> m_scenes = new List<string>(5);

        #region Public functions
        /// @param sceneName
        ///     Name of the scene to switch to
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        public void SwitchToScene(string sceneName, Action callback = null)
        {
            m_scenes.Clear();
            GlobalDirector.ExecuteCoroutine(StartLoadScene(sceneName, LoadSceneMode.Single, callback));
        }

        /// @param sceneName
        ///     Name of the scene to switch to
        /// @param transitionPrefabName
        ///     Name of the prefab inside the default transition folder to use for the transition screen
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        public void SwitchToScene(string sceneName, string transitionPrefabName, Action callback = null)
        {
            m_scenes.Clear();
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            SceneTransitionView transitionView = CreateTransitionView(transitionPrefabName);
            GlobalDirector.ExecuteCoroutine(StartTransition(loadSceneOperation, transitionView, callback));
        }

        ///@param sceneName
        ///     Name of the scene to switch to
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        public void LoadSceneAdditively(string sceneName, Action callback = null)
        {
            GlobalDirector.ExecuteCoroutine(StartLoadScene(sceneName, LoadSceneMode.Additive, () =>
            {
                callback.SafeInvoke();
            }));
        }

        /// @return The name of the active scene
        /// 
        public string GetActiveScene()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// @return The amount of loaded scenes
        /// 
        public int GetSceneCount()
        {
            return SceneManager.sceneCount;
        }

        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        public void UnloadScene(Action callback = null)
        {
            var sceneName = GetActiveScene();
            UnloadScene(sceneName, callback);
        }

        ///@param sceneName
        ///     Name of the scene to unload
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        public void UnloadScene(string sceneName, Action callback = null)
        {
            if (IsSceneLoaded(sceneName) == true)
            {
                GlobalDirector.ExecuteCoroutine(StartUnloadScene(SceneManager.GetSceneByName(sceneName), callback));
            }
            else
            {
                callback.SafeInvoke();
            }
        }

        ///@param sceneName
        ///     Name of the scene to wait for unload
        /// @param [optional] callback
        ///     Function to call when the scene is unloaded
        /// 
        public void CallWhenUnloaded(string sceneName, Action callback)
        {
            if(m_unloadedDelegates.ContainsKey(sceneName) == false)
            {
                m_unloadedDelegates.Add(sceneName, null);
            }
            m_unloadedDelegates[sceneName] += callback;
        }
        #endregion

        #region Private functions
        /// @param transitionPrefabName
        ///     Name of the transition prefab to load and instantiate
        /// 
        /// @return The SceneTransitionView of the transitionUI object
        /// 
        private SceneTransitionView CreateTransitionView(string transitionPrefabName)
        {
            SceneTransitionView transitionView = null;
            string fullPath = k_pathTransitionUI + transitionPrefabName;
            var transitionUIPrefab = Resources.Load(fullPath) as GameObject;

            if(transitionUIPrefab == null)
            {
                Debug.LogError(string.Format("Failed to load transition UI at '{0}'", fullPath));
            }
            else
            {
                var transitionObject = GameObject.Instantiate(transitionUIPrefab);
                GameObject.DontDestroyOnLoad(transitionObject);
                transitionObject.SetActive(false);
                transitionView = transitionObject.GetComponent<SceneTransitionView>();

                var canvas = transitionObject.GetComponent<Canvas>();
                canvas.sortingOrder = k_defaultCanvasSortOrder;
            }

            return transitionView;
        }

        /// @param sceneName
        ///     Name of the scene to check
        /// 
        /// @return Whether the scene is loaded or not
        /// 
        private bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }

        /// @param sceneName
        ///     Name of the scene to set as active
        /// 
        private void SetActiveScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if(scene.IsValid() == true)
            {
                SceneManager.SetActiveScene(scene);
            }
        }

        /// Coroutine to handle transitioning to a scene with a transition screen
        /// 
        /// @param sceneLoadOperation
        ///     AsyncOperation for the scene loading
        /// @param transitionView
        ///     SceneTransitionView to use as the transition screen
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        private IEnumerator StartTransition(AsyncOperation sceneLoadOperation, SceneTransitionView transitionView, Action callback = null)
        {
            sceneLoadOperation.allowSceneActivation = false;

            if(transitionView != null)
            {
                yield return GlobalDirector.ExecuteCoroutine(transitionView.Show());
            }

            sceneLoadOperation.allowSceneActivation = true;
            yield return new WaitUntil(() => sceneLoadOperation.isDone);

            LocalDirector[] localDirectors = GameObject.FindObjectsOfType<LocalDirector>();
            foreach(LocalDirector localDirector in localDirectors)
            {
                yield return new WaitUntil(() => localDirector.IsReady());
            }

            callback.SafeInvoke();
            OnSceneActive.SafeInvoke(GetActiveScene());

            if (transitionView != null)
            {
                yield return GlobalDirector.ExecuteCoroutine(transitionView.Hide());
            }
        }

        /// @param sceneName
        ///     Name of the scene to load
        /// @param loadSceneMode
        ///     Way to load the scene (additively to the scene or as a scene switch)
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        private IEnumerator StartLoadScene(string sceneName, LoadSceneMode loadSceneMode, Action callback = null)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            yield return loadOperation;

            m_scenes.Add(sceneName);
            SetActiveScene(sceneName);
            callback.SafeInvoke();
            OnSceneActive.SafeInvoke(GetActiveScene());
        }

        /// Coroutine to handle unloading a scene
        /// 
        /// @param scene
        ///     Scene to unload
        /// @param [optional] callback
        ///     Function to call when the transiton is done
        /// 
        private IEnumerator StartUnloadScene(Scene scene, Action callback = null)
        {
            var sceneName = scene.name;
            AsyncOperation loadOperation = SceneManager.UnloadSceneAsync(scene);
            yield return loadOperation;

            callback.SafeInvoke();

            // Scene unloaded is a one-off callback
            if (m_unloadedDelegates.ContainsKey(sceneName) == true)
            {
                m_unloadedDelegates[sceneName].SafeInvoke();
                m_unloadedDelegates.Remove(sceneName);
            }

            // Make sure the correct scene is set active
            for(int index = m_scenes.Count - 1; index >= 0; --index)
            {
                if (m_scenes[index] == sceneName)
                {
                    m_scenes.RemoveAt(index);

                    if(m_scenes.Count > 0)
                    {
                        SetActiveScene(m_scenes.GetLast());
                    }
                    break;
                }
            }

            OnSceneActive.SafeInvoke(GetActiveScene());
        }
        #endregion
    }
}
