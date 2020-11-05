//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreFramework
{
	/// Extension class for adding additional functionality to Dictionary
	/// 
	public static class Dictionary
	{
		public delegate bool DictionaryRandomSelectorDelegate<TValue>(TValue value);

        /// If the key already exists it is updated, otherwise a new key
        /// is added and the value set.
        /// 
        /// @param dictionary
        ///     dictionary to add to, or update
        /// @param dictionaryToAdd
        ///     dictionary to add to the given dictionary
        /// 
        public static void AddDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> dictionaryToAdd)
        {
            foreach(var item in dictionaryToAdd)
            {
                dictionary.AddOrUpdate(item.Key, item.Value); 
            }
		}

		/// If the key already exists it is updated, otherwise a new key
		/// is added and the value set.
		/// 
		/// @param dictionary
		/// 	dictionary to add to, or update
		/// @param key
		/// 	Key to add or update
		/// @param value
		/// 	Value to set
		/// 
		public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if(dictionary.ContainsKey(key) == true)
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		/// @param dictionary
		/// 	dictionary to check
		/// @param key
		/// 	Key to check
		/// 
		/// @return value from dictionary or the given default value
		/// 
		public static TValue GetValueOrAssert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
            return dictionary.GetValueOrAssert(key, "Failed, {0} does not correspond to a value", key);
        }

        /// @param dictionary
        ///     dictionary to check
        /// @param key
        ///     Key to check
        /// @param format
        ///     the format of text that will be logged if the method fails
        /// @param args
        ///     the arguments that will be inserted into the format statement
        /// 
        /// @return value from dictionary or the given default value
        /// 
        public static TValue GetValueOrAssert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, string format, params object[] args)
        {
            Debug.Assert(dictionary.ContainsKey(key), string.Format(format, args));
            return dictionary[key];
        }

		/// @param dictionary
		/// 	dictionary to check
		/// @param key
		/// 	Key to check
		/// @param defaultValue
		/// 	Value to return if key does not exist
		/// 
		/// @return value from dictionary or the given default value
		/// 
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			if(dictionary.TryGetValue(key, out value) == true)
			{
				return value;
			}

			return defaultValue;
        }

        /// @param dictionary
        /// 	dictionary to check
        /// 
        /// @return random pair from dictionary
        /// 
        public static KeyValuePair<TKey, TValue> GetRandom<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            Debug.Assert(dictionary != null, "Dictionary should not be null");
            Debug.Assert(dictionary.Count > 0, "Dictionary should not be empty");

            System.Random rand = new System.Random();
            return dictionary.ElementAt(rand.Next(0, dictionary.Count));
        }

        /// @param dictionary
        /// 	dictionary to check
        /// 
        /// @return random value from dictionary
        /// 
        public static TValue GetRandomValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			Debug.Assert(dictionary != null, "Dictionary should not be null");
			Debug.Assert(dictionary.Count > 0, "Dictionary should not be empty");

			System.Random rand = new System.Random();
			return dictionary.ElementAt(rand.Next(0, dictionary.Count)).Value;
		}

		/// Get Random Value from Dictionary, with a delegate to decide
		/// whether the random value meets the callers requirements
		/// 
		/// @param dictionary
		/// 	dictionary to check
		/// @param selectionDelegate
		/// 	The delegate that will filter the values
		/// 
		/// @return Random value from the dictionary with custom criteria
		/// 
		public static TValue GetRandomValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, DictionaryRandomSelectorDelegate<TValue> selectionDelegate)
		{
			Debug.Assert(dictionary != null, "Dictionary should not be null or empty");
			Debug.Assert(selectionDelegate != null, "Delegate cannot be null!");

			System.Random rand = new System.Random();

			List<int> indexesToCheck = new List<int>(dictionary.Count);
			for(int i = 0; i < dictionary.Count; ++i)
			{
				indexesToCheck.Add(i);
			}

			TValue selectedElement = default(TValue);
			while(indexesToCheck.Count > 0)
			{
				int nextIndexToCheck = rand.Next(indexesToCheck.First(), indexesToCheck.Last());
				TValue tempSelection = dictionary.ElementAt(nextIndexToCheck).Value;

				// Check if the value satisfies the delegate check
				if(selectionDelegate.Invoke(tempSelection))
				{
					selectedElement = tempSelection;
					break;
				}
				else 
				{
					indexesToCheck.Remove(nextIndexToCheck);
				}
			}

			return selectedElement;
		}

		/// Removes the given item from the dictionary, provided that the item is found.
		/// 
		/// @param dictionary
		/// 	dictionary to check
		/// @param item
		/// 	The item to remove
		/// 
		public static void RemoveIfContained<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			Debug.Assert(dictionary != null, "Dictionary can't be null");
			if(dictionary.ContainsKey(key))
			{
				dictionary.Remove(key);
			}
		}

		/// Convert object to dictionary
		/// 
		/// @param dictionary
		/// 	dictionary to convert 
		/// 
		/// @return Dictionary<string, T>
		/// 
		public static IDictionary<string, T> ToDictionary<T>(this IDictionary<string, object> dictionary)
		{
			Debug.Assert(dictionary != null, "Dictionary should not be null or empty");
			
			var newDictionary = new Dictionary<string, T>();

			foreach(KeyValuePair<string, object> newObject in dictionary)
			{
				AddPropertyToDictionary<T>(newObject, newDictionary);
			}
			return newDictionary;
		}

		/// Add property to dictionary
		/// 
		/// @param property
		/// 	the property to be added to dictionary (the key)
		/// @param obj
		/// 	object to add to dictionary (the value) 
		/// @param dictionary
		/// 	the dictioanry to add value too (Dictionary<string, T>)
		/// 
		private static void AddPropertyToDictionary<T>(KeyValuePair<string, object> obj, Dictionary<string, T> dictionary)
		{
			object value = obj.Value;
			string key = obj.Key;

			if(dictionary is Dictionary<string, string>)
			{
				(dictionary as Dictionary<string, string>).Add(key, System.Convert.ToString(value));
			}
			else if(dictionary is Dictionary<string, int>)
			{
				(dictionary as Dictionary<string, int>).Add(key, System.Convert.ToInt32(value));
			}
			else if(dictionary is Dictionary<string, float>)
			{
				(dictionary as Dictionary<string, float>).Add(key, System.Convert.ToSingle(value));
			}
			else if(dictionary is Dictionary<string, bool>)
			{
				(dictionary as Dictionary<string, bool>).Add(key, System.Convert.ToBoolean(value));
			}
			else
			{
                Debug.LogError(string.Format("Type not handled {0}", dictionary.GetType()));
			}
        }

		/// Gets the index of a key within a dictionary. Note that whenever an
		/// item is added to a dictionary, a pre-existing index may change. So,
		/// this should only be used with static dictionaries.
		/// 
		/// @param dictionary
		/// 	Dictionary to get index of key
		/// @param key
		/// 	Key to check against dictionary
		/// 
		/// @return Index of key within dict, or -1 if dict does not contain key
		/// 
		public static int IndexOf<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			Debug.Assert(dictionary != null, "Cannot have null dictionary!");
			Debug.Assert(key != null, "Cannot have null key!");

			TKey[] keys = dictionary.Keys.ToArray();
			for (int i = 0; i < dictionary.Keys.Count; ++i)
			{
				if(key.Equals(keys[i]))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
