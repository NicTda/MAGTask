//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Extension class for adding additional functionality to Dictionary containing List
    /// 
    public static class DictionaryListExtension
    {
        /// Check if the keyed list within the dictionary contains this value
        /// 
        /// @param dictionary
        ///     The dictionary to check
        /// @param key
        ///     The key of the list within the dictionary to check
        /// @param value
        ///     The value to look for
        /// 
        /// @return true if the entry exists
        /// 
        public static bool Contains<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            Debug.Assert(dictionary != null, "Cannot have null dictionary!");
            Debug.Assert(key != null, "Cannot have null key!");

            bool exists = false;

            // Does the dictionary hold this list?
            if (dictionary.ContainsKey(key))
            {
                // Is the list initialised?
                List<TValue> list = dictionary.GetValueOrDefault(key, null);
                if (list != null)
                {
                    // Does the list hold this value?
                    if (list.Contains(value))
                    {
                        exists = true;
                    }
                }
            }

            return exists;
        }

        /// Add to a list in a Dictionary, where the value param of the dictionary is the list to add to.
        /// Creates the list if first entry
        /// 
        /// @param dictionary
        ///     The dictionary to add to
        /// @param key
        ///     The key of the dictionary to add to
        /// @param value
        ///     The value to add
        /// 
        public static void AddValue<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            Debug.Assert(dictionary != null, "Cannot have null dictionary!");
            Debug.Assert(key != null, "Cannot have null key!");

            // Check if we need to new a list
            if((dictionary.ContainsKey(key) == false) || (dictionary.GetValueOrDefault(key, null) == null))
            {
                dictionary[key] = new List<TValue>();
            }

            // Then add to it
            dictionary[key].Add(value);
        }

        /// Add to a list in a Dictionary, where the value param of the dictionary is the list to add to.
        /// Creates the list if first entry
        /// 
        /// @param dictionary
        ///     The dictionary to add to
        /// @param key
        ///     The key of the dictionary to add to
        /// @param value
        ///     The value to add
        /// 
        public static void AddValues<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, List<TValue> listValues)
        {
            Debug.Assert(dictionary != null, "Cannot have null dictionary!");
            Debug.Assert(key != null, "Cannot have null key!");

            // Check if we need to new a list
            if((dictionary.ContainsKey(key) == false) || (dictionary.GetValueOrDefault(key, null) == null))
            {
                dictionary[key] = new List<TValue>();
            }

            // Then add to it
            dictionary[key].AddRange(listValues);
        }

        /// Chooses a random key in the dictionary based the values' weighted odds
        /// 
        /// @param dictionary
        ///     The dictionary to check
        /// 
        /// @return The randomly chosen key
        /// 
        public static TKey GetWeightedKey<TKey>(this Dictionary<TKey, float> dictionary)
        {
            Debug.Assert(dictionary != null, "Cannot have null dictionary!");

            // Random key to be safe
            TKey chosenKey = dictionary.GetRandom().Key;

            // Calculate total odds
            float totalChances = 0.0f;
            foreach (var pair in dictionary)
            {
                totalChances += pair.Value;
            }

            // Get the weighted key
            float chosenChance = UnityEngine.Random.Range(0.0f, totalChances);
            foreach (var pair in dictionary)
            {
                if (chosenChance <= pair.Value)
                {
                    chosenKey = pair.Key;
                    break;
                }
                chosenChance -= pair.Value;
            }

            return chosenKey;
        }
    }
}
