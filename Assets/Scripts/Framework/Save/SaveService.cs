//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CoreFramework
{
    /// This service handles caching data on disk and syncing with server.
    /// 
	public sealed class SaveService : Service
    {
        private const string k_saveThreadName = "SaveThread";
        private const int k_noJobSleepTimeMS = 2000;

        private bool m_saveEnabled = true;
        private bool m_saveRequested = false;
        private List<ISavable> m_savables = new List<ISavable>();

        private List<SaveData> m_saveQueue = new List<SaveData>();
        private Thread m_saveThread = null;
        private volatile bool m_saveThreadActive = true;
        private Mutex m_jobQueueMutex = new Mutex();
        private Coroutine m_coroutine = null;

        /// Container for save data
        /// 
        private sealed class SaveData
        {
            public string m_fileName = string.Empty;
            public Dictionary<string, object> m_data = null;

            /// Execute the save action
            ///
            public void ExecuteSave()
            {
                JsonFileUtils.SaveDataToJsonFile(m_fileName, m_data, FileSystem.Location.Persistent);
            }
        }

        #region Service functions
        /// Used to perform actions before being unregistered
        /// 
        public override void OnUnregister()
        {
            m_coroutine.Stop();
        }

        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            m_saveThread = new Thread(new ThreadStart(DoSaveJob));
            m_saveThread.Name = k_saveThreadName;
            m_saveThread.Start();
            InitialiseDirectories();
        }

        /// Updates the service
        /// 
        public override void ServiceUpdate()
        {
            if (m_saveRequested == true)
            {
                SaveRegisteredData();
                m_saveRequested = false;
            }
        }

        /// If Application suspends should call all systems to save
        /// 
        public override void OnApplicationPause()
        {
            RequestSaveImmediate();
        }
        #endregion

        #region Public functions
        /// @param savable
        ///     The savable to register for local save only
        /// 
        public void RegisterCaching(ISavable savable)
        {
            m_savables.Add(savable);
        }

        /// @param savable
        ///     The savable to unregister from local saving
        /// 
        public void UnregisterCaching(ISavable savable)
        {
            m_savables.RemoveIfContained(savable);
        }

        /// @param fileName
        ///     filename in which to save data
        /// @param data
        ///     the data to be saved
        /// 
        public void AddSaveJob(string fileName, Dictionary<string, object> data)
        {
            if(m_saveEnabled == false)
            {
                return;
            }

            var saveJob = new SaveData
            {
                m_fileName = fileName,
                m_data = data
            };

            m_jobQueueMutex.WaitOne();

            // Delete old data
            for(int i = m_saveQueue.Count - 1; i >= 0; --i)
            {
                if(m_saveQueue[i].m_fileName == fileName)
                {
                    m_saveQueue.RemoveAt(i);
                }
            }
            m_saveQueue.Add(saveJob);
            m_jobQueueMutex.ReleaseMutex();
        }

        /// @param callback
        ///     The function to call when the data is loaded
        /// 
        public void LoadCachedData(Action<bool> callback)
        {
            m_coroutine = GlobalDirector.ExecuteCoroutine(LoadDataAsync(callback));
        }

        /// Request Save Data
        ///
        public void RequestSaveData()
        {
            m_saveRequested = true;
        }

        /// @param callback
        ///     The function to call when everything is saved
        ///
        public void RequestSaveImmediate(Action callback = null)
        {
            StopSaving(() =>
            {
                SaveRegisteredData();
                callback.SafeInvoke();
            });
        }

        /// Delete all saved files
        /// 
        public void Clear(Action callback = null)
        {
            m_saveEnabled = false;
            m_jobQueueMutex.WaitOne();
            if (AreSaveJobsWaiting())
            {
                m_saveQueue.Clear();
            }
            m_jobQueueMutex.ReleaseMutex();

            m_savables.Clear();
            StopSaving(() =>
            {
                m_saveThread.Abort();
                m_saveThread = null;

                FileSystem.DeleteDirectory(string.Empty, FileSystem.Location.Persistent);
                FileSystem.DeleteDirectory(string.Empty, FileSystem.Location.Cache);
                callback.SafeInvoke();
            });
        }
        #endregion

        #region Private functions
        /// @param callback
        ///     The function to call when saving is stopped
        ///
        private void StopSaving(Action callback = null)
        {
            if (m_saveThread != null)
            {
                // Set the thread to inactive
                m_saveThreadActive = false;

                // Block this thread until the save thread finishes whatever it was doing
                m_saveThread.Join();

                // May still be jobs left to do, we need to make sure everything was saved, if not save now
                while (m_saveQueue.Count > 0)
                {
                    var saveData = m_saveQueue.GetFirst();
                    m_saveQueue.Remove(saveData);
                    saveData.ExecuteSave();
                }
            }

            callback.SafeInvoke();
        }

        /// Create the directories if needed
        /// 
        private void InitialiseDirectories()
        {
            // Need to call this to evaluate the file path on Main thread
            FileSystem.GetAbsolutePath(string.Empty, FileSystem.Location.Cache);
            string persistentPath = FileSystem.GetAbsolutePath(string.Empty, FileSystem.Location.Persistent);

#if UNITY_IPHONE || UNITY_IOS
            // Prevents the Persistent data to be backed up automatically
            UnityEngine.iOS.Device.SetNoBackupFlag(persistentPath);
#endif
        }

        /// @param callback
        ///     The function to call when the data is loaded
        ///     
        private IEnumerator LoadDataAsync(Action<bool> callback)
        {
            bool loaded = true;
            foreach (var savable in m_savables)
            {
                savable.Load();
                yield return null;
            }
            callback.SafeInvoke(loaded);
        }

        /// Saves registered objects data
        /// 
        private void SaveRegisteredData()
        {
            foreach (var savable in m_savables)
            {
                savable.Save();
            }
        }

        /// @return true if there are save jobs in the queue
        /// 
        private bool AreSaveJobsWaiting()
        {
            return (m_saveQueue.Count > 0);
        }

        /// Save any jobs that need to be processed
        ///
        private void DoSaveJob()
        {
            while (m_saveThreadActive == true)
            {
                SaveData saveData = null;

                // Lock the job queue to check for next task
                m_jobQueueMutex.WaitOne();
                if (AreSaveJobsWaiting())
                {
                    saveData = m_saveQueue.GetFirst();
                    m_saveQueue.Remove(saveData);
                }

                // Release straight away so we don't block the main thread
                m_jobQueueMutex.ReleaseMutex();

                if (saveData != null)
                {
                    try
                    {
                        // Save to disk
                        saveData.ExecuteSave();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Exception in save thread - " + e.ToString());
                    }
                }
                else
                {
                    // Sleep for a bit
                    Thread.Sleep(k_noJobSleepTimeMS);
                }
            }
        }
        #endregion
    }
}