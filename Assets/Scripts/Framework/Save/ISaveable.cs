//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System.Collections.Generic;

namespace CoreFramework
{
    /// Describes a class that can be saved using the SaveService
    /// 
	public interface ISavable : ISerializable
    {
        /// Clears the local data
        ///     
        void Clear();
    }

    /// Extension for ISavable class
    /// 
    public static partial class SavableExtension
    {
        private const string k_filePathFormat = "{0}.json";
        private const string k_filePathBackupFormat = "{0}_Backup.json";

        /// @param savable
        ///     The savable to register for local caching only
        ///     
        public static void RegisterCaching(this ISavable savable)
        {
            GlobalDirector.Service<SaveService>().RegisterCaching(savable);
        }

        /// @param savable
        ///     The savable to register for local caching only
        ///     
        public static void UnregisterCaching(this ISavable savable)
        {
            GlobalDirector.Service<SaveService>().UnregisterCaching(savable);
        }

        /// @param savable
        ///     The savable to serialize to file
        /// 
        public static void Save(this ISavable savable)
        {
            string filePath = string.Format(k_filePathFormat, savable.GetType().ToString());
            if (FileSystem.DoesFileExist(filePath, FileSystem.Location.Persistent))
            {
                // Create a backup automatically
                string backup = string.Format(k_filePathBackupFormat, savable.GetType().ToString());
                FileSystem.CopyFile(filePath, backup, FileSystem.Location.Persistent);
            }

            // Queue the save
            var saveData = (Dictionary<string, object>)savable.Serialize();
            GlobalDirector.Service<SaveService>().AddSaveJob(filePath, saveData);
        }

        /// @param savable
        ///     The savable to deserialize from file
        ///     
        /// @return Whether the data was successfully loaded
        /// 
        public static void Load(this ISavable savable)
        {
            var jsonData = new Dictionary<string, object>();
            string filePath = string.Format(k_filePathFormat, savable.GetType().ToString());
            if (FileSystem.DoesFileExist(filePath, FileSystem.Location.Persistent))
            {
                jsonData = JsonWrapper.ParseJsonFile(filePath, FileSystem.Location.Persistent);
                if(jsonData == null)
                {
                    // Delete the corrupted file
                    FileSystem.DeleteFile(filePath, FileSystem.Location.Persistent);

                    // Load a backup
                    string backup = string.Format(k_filePathBackupFormat, savable.GetType().ToString());
                    jsonData = JsonWrapper.ParseJsonFile(backup, FileSystem.Location.Persistent);
                }
            }
            else if (FileSystem.DoesFileExist(filePath, FileSystem.Location.Cache))
            {
                // Legacy file in cache
                jsonData = JsonWrapper.ParseJsonFile(filePath, FileSystem.Location.Cache);
            }
            savable.Deserialize(jsonData);
        }
    }
}