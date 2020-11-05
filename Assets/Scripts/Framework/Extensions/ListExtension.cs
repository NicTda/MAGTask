//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
	/// Extension class for lists
	/// 
	public static class List
    {
        /// @param min
        /// 	The min number, included
        /// @param max
        ///     The max number, excluded
        /// 
        /// @return The generated serie of numbers
        /// 
        public static List<int> GenerateSerie(int min, int max)
        {
            List<int> serie = new List<int>();
            for (int i = min; i < max; ++i)
            {
                serie.Add(i);
            }
            return serie;
        }

        /// @param list
        /// 	The list to choose from
        /// @param item
        /// 	The item to add
        /// @param amount
        ///     The amount of time to add the item
        /// 
        public static void AddSeveral<T>(this List<T> list, T item, int amount)
        {
            if(list == null)
            {
                list = new List<T>(amount);
            }

            for (int i = 0; i < amount; ++i)
            {
                list.Add(item);
            }
        }

        /// @param list
        /// 	The list to choose from
        /// 
        /// @return The random item selected
        /// 
        public static T GetRandom<T>(this List<T> list)
		{
			Debug.Assert(list.IsNullOrEmpty() == false, "List can't be null or empty");
			return list[Random.Range(0, list.Count)];
        }

        /// @param list
        /// 	The list to extract from
        /// 
        /// @return The random item extracted from the list
        /// 
        public static T ExtractRandom<T>(this List<T> list)
        {
            Debug.Assert(list.IsNullOrEmpty() == false, "List can't be null or empty");
            int index = Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);

            return item;
        }

        /// @param list
        /// 	The list to extract from
        /// @param amount
        /// 	The number of elements requested
        /// 
        /// @return The random items extracted from the list
        /// 
        public static List<T> ExtractRandoms<T>(this List<T> list, int amount)
        {
            List<T> newList = new List<T>();
			while(newList.Count < amount && list.IsNullOrEmpty() == false)
            {
                newList.Add(list.ExtractRandom());
            }
            return newList;
        }

        /// @param amount
        /// 	The number of elements requested
        /// 
        /// @return A list of distinct elements from the given list. It will contain
        /// 	the lower of amount and list.Count elements
        /// 
        public static List<T> GetDistinctRandoms<T>(this List<T> list, int amount)
		{
			Debug.Assert(list.IsNullOrEmpty() == false, "List can't be null or empty");
			Debug.Assert(amount >= 0, "Can't request a negative amount: " + amount);

			List<T> newList = new List<T>(amount);
			List<T> tempList = new List<T>(list);

			// Try and get random numbers
			for(int i = 0; i < amount; ++i)
			{
				if(tempList.IsNullOrEmpty())
				{
					break;
				}
				// Extract the random item
				var element = ExtractRandom(tempList);
				newList.Add(element);
			}
			return newList;
		}

	    /// @param list
		/// 	The list to check
		/// 
	    /// @return Whether the list is empty or not
		/// 
	    public static bool IsNullOrEmpty<T>(this List<T> list)
	    {
			return (list == null) || (list.Count == 0);
	    }

	    /// @param list
		/// 	The list to check from
		/// 
	    /// @return The first item in the list
		/// 
	    public static T GetFirst<T>(this List<T> list)
	    {
	        Debug.Assert(list.IsNullOrEmpty() == false, "List can't be null or empty");
	        return list[0];
	    }

	    /// @param list
		/// 	The list to check from
		/// 
	    /// @return The last item in the list
		/// 
	    public static T GetLast<T>(this List<T> list)
	    {
	        Debug.Assert(list.IsNullOrEmpty() == false, "List can't be null or empty");
	        return list[list.Count - 1];
		}

		/// Adds the given item to the list, but only if it isn't already
		/// in the list
		/// 
		/// @param list
		/// 	The list to try to add to
		/// @param item
		/// 	The item to add
		/// 
		public static void AddUnique<T>(this List<T> list, T item)
		{
			Debug.Assert(list != null, "List can't be null");
			if(list.Contains(item) == false)
			{
				list.Add(item);
			}
        }

        /// Adds the given items to the list, but only if they aren't already
        /// in the list
        /// 
        /// @param list
        /// 	The list to try to add to
        /// @param items
        /// 	The list of items to add
        /// 
        public static void AddUnique<T>(this List<T> list, List<T> items)
        {
            Debug.Assert(items != null, "List can't be null");
            foreach(var item in items)
            {
                list.AddUnique(item);
            }
        }

        /// Adds the given item to the list, if not null
        /// 
        /// @param list
        /// 	The list to try to add to
        /// @param item
        /// 	The item to add
        /// 
        public static void AddIfNotNull<T>(this List<T> list, T item)
        {
            Debug.Assert(list != null, "List can't be null");
            if (item != null)
            {
                list.Add(item);
            }
        }

        /// Removes the given item from the list, but only if it is already
        /// in the list
        /// 
        /// @param list
        /// 	The list to try to remove from
        /// @param item
        /// 	The item to remove
        /// 
        public static void RemoveIfContained<T>(this List<T> list, T item)
		{
			Debug.Assert(list != null, "List can't be null");
			if(list.Contains(item))
			{
				list.Remove(item);
			}
        }

        /// Removes the given items from the list, but only if it they already
        /// are in the list
        /// 
        /// @param list
        /// 	The list to try to remove from
        /// @param items
        /// 	The list of items to remove
        /// 
        public static void RemoveIfContained<T>(this List<T> list, List<T> items)
        {
            Debug.Assert(list != null, "List can't be null");
            foreach(var item in items)
            {
                list.RemoveIfContained(item);
            }
        }

        /// @param list
        /// 	The list to remove duplicates from
        /// 
        /// @return The list without duplicates
        /// 
        public static List<T> RemoveDuplicates<T>(this List<T> list)
		{
			Debug.Assert(list.IsNullOrEmpty() == false, "List can't be null or empty");
			List<T> newList = new List<T>(list.Count);

			foreach(T item in list)
			{
				if(newList.Contains(item) == false)
				{
					newList.Add(item);
				}
			}
			return newList;
		}

		/// Fisher-Yates Shuffle algorithm adapted from:
		/// http://www.dotnetperls.com/fisher-yates-shuffle
		/// 
		/// @param list
		/// 	The generic list to be shuffled. Empty lists are allowed.
		/// 
		public static void Shuffle<T>(this List<T> list)
		{
			Debug.Assert(list != null, "List can't be null.");

			int n = list.Count;
			if (n > 0)
			{
				for (int i = 0; i < n; ++i)
				{
					// value returns a random number between 0 and 1.
					// ... It is equivalent to Math.random() in Java.
                    int r = i + (int)(UnityEngine.Random.value * (n - i));
					T t = list[r];
					list[r] = list[i];
					list[i] = t;
				}
			}
		}

        /// @param list 
        ///     Source list
        /// @param amount
        ///     Target list size
        /// @param weightingFunc
        ///     Function that takes a list element, index and retrieves the weight as a float.
        /// 
        /// @return A list of a specified size, populated using a weighted random exclusive choice (roulette wheel).
        /// 
        public static List<T> GetWeightedDistinctRandomItemsFromList<T>(this List<T> list, int amount, System.Func<T, int, float> weightingFunc)
        {
            List<int> chosenIndices = GetWeightedDistinctRandomIndicesFromList(list, amount, weightingFunc);

            List<T> returnList = new List<T>();
            for (int i = 0; i < amount; ++i)
            {
                returnList.Add(list[chosenIndices[i]]);
            }

            return returnList;
        }

        /// @param list 
        ///     Source list
        /// @param amount
        ///     Target list size
        /// @param weightingFunc
        ///     Function that takes a list element, index and retrieves the weight as a float.
        /// 
        /// @return A list of the specified amount of indices into the source list
        ///     populated using a weighted random exclusive choice (roulette wheel).
        /// 
        /// NOTE: in the case where all weights are zero, the first elements will be chosen
        /// 
        public static List<int> GetWeightedDistinctRandomIndicesFromList<T>(this List<T> list, int amount, System.Func<T, int, float> weightingFunc)
        {
            Debug.Assert(list.Count >= amount, "Weighted Random: There's not enough " + typeof(T).ToString() + " items in the list!");

            float totalWeight = 0.0f;
            float[] weights = new float[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                weights[i] = weightingFunc(list[i], i);
                totalWeight += weights[i];
            }

            List<int> chosenIndices = new List<int>();
            for (int i = 0; i < amount; ++i)
            {
                float rand = UnityEngine.Random.Range(0.0f, totalWeight);

                for (int j = 0; j < list.Count; ++j)
                {
                    if (chosenIndices.Contains(j))
                    {
                        continue;
                    }

                    rand -= weights[j];
                    if (rand <= 0.0f)
                    {
                        chosenIndices.Add(j);
                        totalWeight -= weights[j];
                        break;
                    }
                }
            }

            Debug.Assert(chosenIndices.Count == amount, string.Format("returning wrong list size! target:{0} actual:{1}", amount, chosenIndices.Count));

            return chosenIndices;
        }

        /// @param list 
        ///     Source list
        /// @param amount
        ///     Target list size
        /// @param weightingFunc
        ///     Function that takes a list element, index and retrieves the weight as a float.
        /// 
        /// @return A list of a specified size, populated using a weighted random exclusive choice (roulette wheel).
        /// 
        public static List<T> GetWeightedRandomItemsFromList<T>(this List<T> list, int amount, System.Func<T, int, float> weightingFunc)
        {
            List<int> chosenIndices = GetWeightedRandomIndicesFromList(list, amount, weightingFunc);

            List<T> returnList = new List<T>();
            for (int i = 0; i < amount; ++i)
            {
                returnList.Add(list[chosenIndices[i]]);
            }

            return returnList;
        }

        /// @param list 
        ///     Source list
        /// @param amount
        ///     Target list size
        /// @param weightingFunc
        ///     Function that takes a list element, index and retrieves the weight as a float.
        /// 
        /// @return A list of the specified amount of indices into the source list
        ///     populated using a weighted random choices (roulette wheel).
        /// 
        /// NOTE: in the case where all weights are zero, the first elements will be chosen
        /// 
        public static List<int> GetWeightedRandomIndicesFromList<T>(this List<T> list, int amount, System.Func<T, int, float> weightingFunc)
        {
            float totalWeight = 0.0f;
            float[] weights = new float[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                weights[i] = weightingFunc(list[i], i);
                totalWeight += weights[i];
            }

            List<int> chosenIndices = new List<int>();
            for (int i = 0; i < amount; ++i)
            {
                float rand = UnityEngine.Random.Range(0.0f, totalWeight);

                for (int j = 0; j < list.Count; ++j)
                {
                    rand -= weights[j];
                    if (rand <= 0.0f)
                    {
                        chosenIndices.Add(j);
                        break;
                    }
                }
            }

            Debug.Assert(chosenIndices.Count == amount, string.Format("returning wrong list size! target:{0} actual:{1}", amount, chosenIndices.Count));

            return chosenIndices;
        }
    }
}
