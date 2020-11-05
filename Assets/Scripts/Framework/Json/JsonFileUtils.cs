//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//
  
namespace CoreFramework
{
    namespace Json
    {
        /// Utils for accessing Json file I/O
        ///
        public static class JsonFileUtils
        {
            #region Public functions
            /// Save data to Json File
            /// 
            /// @param fileName
            ///     the file name
            /// @param saveData
            ///     Data to save
            /// @param onOverwriteDelegate
            ///     Function that is called if file already exists
            /// 
            public static void SaveDataToJsonFile<T>(string fileName, T saveData, FileSystem.Location location = FileSystem.Location.Persistent, System.Func<string, bool> onOverwriteDelegate = null)
            {
#if UNITY_WEBGL
            	return;
#endif

                string data = JsonWrapper.Serialize(saveData);

                if(FileSystem.DoesDirectoryExist(fileName, location))
                {
                    if((onOverwriteDelegate == null) || (onOverwriteDelegate(fileName) == true))
                    {
                        FileSystem.WriteTextFile(data, fileName, location);
                    }
                }
                else
                {
                    FileSystem.WriteTextFile(data, fileName, location);
                }

#if UNITY_EDITOR
                //Only do refresh if were on the main thread
                if(GlobalDirector.Service<TaskSchedulerService>().IsMainThread())
                {
                    UnityEditor.AssetDatabase.Refresh();
                }
#endif
            }
            #endregion
        }
    }
}
