//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;
using System.Collections.Generic;

namespace CoreFramework
{
    /// Extension class for adding additional functionality to Dictionary
    /// 
	public static class DictionaryUtils
	{
		/// If the key already exists it is updated, otherwise a new key
        /// is added and the value set
        /// 
		/// @param key
        ///     Key to add or update
		/// @param val
        ///     Value to set
        /// 
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue val)
		{
			if(dict.ContainsKey(key) == true)
			{
				dict[key] = val;
			}
			else
			{
				dict.Add(key, val);
			}
		}

        /// If the key already exists it is updated, otherwise a new key
        /// is added and the value set
        /// 
        /// @param newDict
        ///     dictionary to add to the old dictionary
        /// 
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> newDict)
        {
            foreach (var pair in newDict)
            {
                dict.AddOrUpdate(pair.Key, pair.Value);
            }
        }

        /// Tries to get the value associated to passed key otherwise returns
        /// the default value
        /// 
        /// @param key
        ///     Key to retrieve
        /// @param defaultValue
        ///     The default value in case the get fails
        /// 
        /// @return Either the associated value or the default value if not found
        /// 
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
		{
			TValue value;
			if(dict.TryGetValue(key, out value) == true)
			{
				return value;
			}

			return defaultValue;
		}

		/// Tries to get the value associated to passed key
		/// 
		/// @param key
        ///     Key to retrieve
        /// 
        /// @return The value of the passed key
        /// 
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			TValue value;
			dict.TryGetValue(key, out value);
			return value;
		}

		/// @param data
		///     The json data
		/// @param key
		///     The json key for the value to deserialize
		/// @param [optional] assert
		/// 	Whether to assert if the value isn't found or not
		/// 
		/// @return Deserialized int from the json dictionary
		/// 
		public static int GetInt(this Dictionary<string, object> data, string key, bool assert = false)
		{
			var value = data.GetValue(key).AsInt();
			return value;
		}

		/// @param data
		///     The json data
		/// @param key
		///     The json key for the value to deserialize
		/// @param [optional] assert
		/// 	Whether to assert if the value isn't found or not
		/// 
		/// @return Deserialized float from the json dictionary
		/// 
		public static float GetFloat(this Dictionary<string, object> data, string key, bool assert = false)
		{
			var value = data.GetValue(key).AsFloat();
			return value;
		}

		/// @param data
		///     The json data
		/// @param key
		///     The json key for the value to deserialize
		/// @param [optional] assert
		/// 	Whether to assert if the value isn't found or not
		/// 
		/// @return Deserialized bool from the json dictionary
		/// 
		public static bool GetBool(this Dictionary<string, object> data, string key, bool assert = false)
		{
			var value = data.GetValue(key).AsBool();
			return value;
        }

        /// @param data
        ///     The json data
        /// @param key
        ///     The json key for the value to deserialize
        /// @param [optional] assert
        /// 	Whether to assert if the value isn't found or not
        /// 
        /// @return Deserialized string from the json dictionary
        /// 
        public static string GetString(this Dictionary<string, object> data, string key, bool assert = false)
        {
            var value = data.GetValue(key).AsString();
            return value;
        }

        /// @param data
        ///     The json data
        /// @param key
        ///     The json key for the value to deserialize
        /// 
        /// @return Deserialized DateTime from the json dictionary
        /// 
        public static DateTime GetDateTime(this Dictionary<string, object> data, string key)
        {
            var value = data.GetValue(key).AsDateTime();
            return value;
        }
    }
}
