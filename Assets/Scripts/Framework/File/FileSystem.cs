//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CoreFramework
{
	/// The file system provides convenient methods for reading from / writing to files,
	/// checking if files / directories exist, deleting files and directories.
	/// 
	/// Paths are relative to the internal File System structure. 
	/// 
	public static class FileSystem
	{
		/// Location types used by the FileSystem, pointing to different
		/// directories.
		/// 
		/// Persistent - Use for saved data
		/// Cache - Use for temp data
		/// 
		public enum Location
		{
			Persistent,
			Cache
		}

		public const string k_persistentSaveDataPath = "SaveData/";
		public const string k_cacheDataPath = "CacheData/";

		private static readonly string k_temporaryCachePath = Application.temporaryCachePath;
		private static readonly string k_persistentDataPath = Application.persistentDataPath;

		/// This will give the physical path to the file given.
		/// Use for file operations that are not handled by the FileSystem.
		/// 
		/// @param relativePath
		/// 	Path relative to data storage to check
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		/// @return Absolute file path used by filesystem
		/// 
		public static string GetAbsolutePath(string relativePath, Location location = Location.Persistent)
		{
			return Path.Combine(GetAbsolutePath(location), relativePath);
		}

		/// @param location
		/// 	Directory location
		/// 
		/// @return Absolute file path used by filesystem
		/// 
		private static string GetAbsolutePath(Location location)
		{
			string path = string.Empty;

			switch(location)
			{
				case Location.Persistent:
				{
					path = Path.Combine(k_persistentDataPath, k_persistentSaveDataPath);
					break;
				}
				case Location.Cache:
				{
					path = Path.Combine(k_temporaryCachePath, k_cacheDataPath);
					break;
				}
			}

			// Ensure base directory is there
			CreateAbsoluteDirectory(path);

			return path;
		}

		/// @param directoryPath
		/// 	Path relative to data storage to check
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		/// @return Whether the directory exists
		/// 
		public static bool DoesDirectoryExist(string directoryPath, Location location = Location.Persistent)
		{
			string path = GetAbsolutePath(directoryPath, location);
			return Directory.Exists(path);
		}

		/// @param filePath
		/// 	Path relative to data storage to check
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		/// @return Whether the file exists
		/// 
		public static bool DoesFileExist(string filePath, Location location = Location.Persistent)
		{
			string path = GetAbsolutePath(filePath, location);
			return File.Exists(path);
		}

		/// Creates the directory at given relative location, if needed.
		/// Will also create all the subdirectories needed to that path.
		/// 
		/// @param relativePath
		/// 	Relative path of the directory
		/// @param [optional] location
		/// 	Directory location. Default to Persistent
		/// 
		public static void CreateDirectory(string relativePath, Location location = Location.Persistent)
		{
			string path = GetAbsolutePath(relativePath, location);
			CreateAbsoluteDirectory(path);
		}

		/// Creates the directory at given absolute location, if needed.
		/// Will also create all the subdirectories needed to that path.
		/// 
		/// @param absolutePath
		/// 	Absolute path of the directory
		/// 
		private static void CreateAbsoluteDirectory(string absolutePath)
		{
			var directoryPath = Path.GetDirectoryName(absolutePath);
			if(Directory.Exists(directoryPath) == false)
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		/// Reads a string from the text file at the given path relative
		/// to the platform's storage location. This method will log an error
		/// if the file does not exist.
		/// 
		/// @param relativePath
		/// 	Path relative to data storage at which to save, or null
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		/// @return Text as read from file
		/// 
		public static string ReadTextFile(string relativePath, Location location = Location.Persistent)
		{
			string text = null;

#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif

			string path = GetAbsolutePath(relativePath, location);
			if(File.Exists(path))
			{
				text = File.ReadAllText(path);
			}
			else
			{
				Debug.LogWarning(path + " doesn't exist.");
			}
			return text;
		}

		/// Reads data from the binary file at the given path relative
		/// to the platform's storage location. This method will log an error
		/// if the file does not exist.
		/// 
		/// @param relativePath
		/// 	Path relative to data storage at which to save, or null
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		/// @return Data as read from file
		/// 
		public static byte[] ReadBinaryFile(string relativePath, Location location = Location.Persistent)
		{
			byte[] data = null;

#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif

			string path = GetAbsolutePath(relativePath, location);
			if(File.Exists(path))
			{
				data = File.ReadAllBytes(path);
			}
			else
			{
				Debug.LogWarning(path + " doesn't exist.");
			}
			return data;
		}

		/// Reads a string from the text file at the given path relative
		/// to the platform's storage location. This method will log an error
		/// if the file does not exist, or if no action is provided.
		/// 
		/// @param relativePath
		/// 	Path relative to data storage at which to save, or null
		/// @param location
		/// 	Directory location
		/// @param action
		/// 	Action to perform when the task is completed. Will pass the read text.
		/// 
		public static void ReadTextFileAsync(string relativePath, Location location, Action<string> action)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
			if(action != null)
            {
                var taskSchedulerService = GlobalDirector.Service<TaskSchedulerService>();
                taskSchedulerService.ScheduleBackgroundTask(() =>
				{
					string text = ReadTextFile(relativePath, location);

                    taskSchedulerService.ScheduleMainThreadTask(() =>
					{
						action.Invoke(text);
					});
				});
			}
			else
			{
				Debug.LogError("No callback provided.");
			}
		}

		/// Reads data from the binary file at the given path relative
		/// to the platform's storage location. This method will log an error
		/// if the file does not exist, or if no action is provided.
		/// 
		/// @param relativePath
		/// 	Path relative to data storage at which to save, or null
		/// @param location
		/// 	Directory location
		/// @param action
		/// 	Action to perform when the task is completed. Will pass the read data.
		/// 
		public static void ReadBinaryFileAsync(string relativePath, Location location, Action<byte[]> action)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
			if(action != null)
            {
                var taskSchedulerService = GlobalDirector.Service<TaskSchedulerService>();
                taskSchedulerService.ScheduleBackgroundTask(() =>
				{
					byte[] data = ReadBinaryFile(relativePath, location);

                    taskSchedulerService.ScheduleMainThreadTask(() =>
					{
						action.Invoke(data);
					});
				});
			}
			else
			{
				Debug.LogError("No callback provided.");
			}
		}

		/// Writes the given string to a text file at the given path relative to the platform's storage 
		/// location. This method will create the file if it does not already exist and overwrite an 
		/// existing file.
		/// 
		/// @param data
		/// 	Text data to write
		/// @param relativePath
		/// 	Path relative to data storage at which to write
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		public static void WriteTextFile(string data, string relativePath, Location location = Location.Persistent)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
			string absolutePath = GetAbsolutePath(relativePath, location);
			FileSystem.CreateAbsoluteDirectory(absolutePath);
			File.WriteAllText(absolutePath, data);
		}

		/// Writes the given bytes to a binary file at the given path relative to the platform's storage 
		/// location. This method will create the file if it does not already exist and overwrite an 
		/// existing file.
		/// 
		/// @param data
		/// 	Binary data to write
		/// @param relativePath
		/// 	Path relative to data storage at which to write
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		public static void WriteBinaryFile(byte[] data, string relativePath, Location location = Location.Persistent)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
			string absolutePath = GetAbsolutePath(relativePath, location);
			FileSystem.CreateAbsoluteDirectory(absolutePath);
			File.WriteAllBytes(absolutePath, data);
		}

		/// Writes the given string to a text file at the given path relative to the platform's storage 
		/// location. This method will create the file if it does not already exist and overwrite an 
		/// existing file.
		/// 
		/// @param data
		/// 	Text data to write
		/// @param relativePath
		/// 	Path relative to data storage at which to write, or null
		/// @param location
		/// 	Directory location. Default to Persistent
		/// @param action
		/// 	Action to perform when the task is completed
		/// 
		public static void WriteTextFileAsync(string data, string relativePath, Location location = Location.Persistent, Action action = null)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
            var taskSchedulerService = GlobalDirector.Service<TaskSchedulerService>();
            taskSchedulerService.ScheduleBackgroundTask(() =>
			{
				WriteTextFile(data, relativePath, location);
				if(action != null)
				{
                    taskSchedulerService.ScheduleMainThreadTask(() =>
					{
						action.Invoke();
					});
				}
			});
		}

		/// Writes the given bytes to a binary file at the given path relative to the platform's storage 
		/// location. This method will create the file if it does not already exist and overwrite an 
		/// existing file.
		/// 
		/// @param data
		/// 	Binary data to write
		/// @param relativePath
		/// 	Path relative to data storage at which to write, or null
		/// @param location
		/// 	Directory location. Default to Persistent
		/// @param action
		/// 	Action to perform when the task is completed
		/// 
		public static void WriteBinaryFileAsync(byte[] data, string relativePath, Location location = Location.Persistent, Action action = null)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
            var taskSchedulerService = GlobalDirector.Service<TaskSchedulerService>();
            taskSchedulerService.ScheduleBackgroundTask(() =>
			{
				WriteBinaryFile(data, relativePath, location);
				if(action != null)
				{
                    taskSchedulerService.ScheduleMainThreadTask(() =>
					{
						action.Invoke();
					});
				}
			});
        }

        /// @param text
        ///     The text to check
        /// 
        /// @return The checksum of the text
        /// 
        public static string CalculateChecksum(string text)
        {
            // Calculate downloaded data checksum
            return CalculateChecksum(Encoding.UTF8.GetBytes(text));
        }

        /// @param data
        ///     The data to check
        /// 
        /// @return The checksum of the data
        /// 
        public static string CalculateChecksum(byte[] data)
        {
            // Calculate downloaded data checksum
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// @param relativePath
        /// 	Relative path of the root directory from which to get all files
        /// @param location
        /// 	Directory location. Default to Persistent
        /// 
        /// @return List of all files contained in the directory and its sub directories
        /// 
        public static List<string> GetAllFiles(string relativePath, Location location = Location.Persistent)
		{
			var path = GetAbsolutePath(relativePath, location);
			return GetAbsoluteFiles(path);
		}

		/// @param absolutePath
		/// 	Absolute path of the root directory from which to get all files
		/// 
		/// @return List of all files contained in the directory and its sub directories
		/// 
		private static List<string> GetAbsoluteFiles(string absolutePath)
		{
			List<string> files = new List<string>();

			var filesPaths = Directory.GetFiles(absolutePath);
			if(filesPaths != null)
			{
				files.AddRange(filesPaths);
			}

			string[] dirPaths = Directory.GetDirectories(absolutePath);
			foreach(var dirPath in dirPaths)
			{
				files.AddRange(GetAllFiles(dirPath));
			}

			return files;
		}

		/// @param absolutePath
		/// 	Absolute path of the root directory from which to get all files
		/// @param excludedExtensions
		/// 	Extensions to ignore. NOTE: extensions need to start with "."
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		/// @return List of all files contained in the directory and its sub directories
		/// 
		public static List<string> GetAllFiles(string relativePath, List<string> excludedExtensions, Location location = Location.Persistent)
		{
			var path = GetAbsolutePath(relativePath, location);
			return GetAbsoluteFiles(path, excludedExtensions);
		}

		/// @param absolutePath
		/// 	Absolute path of the root directory from which to get all files
		/// @param excludedExtensions
		/// 	Extensions to ignore. NOTE: extensions need to start with "."
		/// 
		/// @return List of all files contained in the directory and its sub directories
		/// 
		private static List<string> GetAbsoluteFiles(string absolutePath, List<string> excludedExtensions)
		{
			List<string> files = new List<string>();

			string[] filePaths = Directory.GetFiles(absolutePath);
			foreach(var filePath in filePaths)
			{
				if(excludedExtensions.Contains(Path.GetExtension(filePath)) == false)
				{
					files.Add(filePath);
				}
			}

			string[] dirPaths = Directory.GetDirectories(absolutePath);
			foreach(var dirPath in dirPaths)
			{
				files.AddRange(GetAllFiles(dirPath, excludedExtensions));
			}

			return files;
        }

        /// Makes  a copy of the file with the given relative path from
        /// platform specific storage.
        /// 
        /// @param pathFrom
        /// 	Path relative to data storage to move from
        /// @param pathTo
        /// 	Path relative to data storage to move to
        /// @param location
        /// 	Directory location. Default to Persistent
        /// 
        public static void CopyFile(string pathFrom, string pathTo, Location location = Location.Persistent)
        {
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
            string oldpath = GetAbsolutePath(pathFrom, location);
            string newPath = GetAbsolutePath(pathTo, location);
            if (File.Exists(oldpath) == true)
            {
                File.Copy(oldpath, newPath, true);
            }
        }

        /// Deletes the file with the given relative path from
        /// platform specific storage.
        /// 
        /// @param relativePath
        /// 	Path relative to data storage to delete
        /// @param location
        /// 	Directory location. Default to Persistent
        /// 
        public static void DeleteFile(string relativePath, Location location = Location.Persistent)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
			string path = GetAbsolutePath(relativePath, location);
			if(File.Exists(path))
			{
				File.Delete(path);
			}
		}

		/// Deletes the directory with the given relative path from
		/// platform specific storage.
		/// 
		/// NOTE: Recursively deletes sub folders
		/// 
		/// @param relativePath
		/// 	Path relative to data storage to delete
		/// @param location
		/// 	Directory location. Default to Persistent
		/// 
		public static void DeleteDirectory(string relativePath, Location location = Location.Persistent)
		{
#if UNITY_WEBGL
			Debug.Assert(false, "WebGL cannot perform file operations.");
#endif
			string path = GetAbsolutePath(relativePath, location);
            if (Directory.Exists(path) == true)
			{
				Directory.Delete(path, true);
            }
        }
	}
}
