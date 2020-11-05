//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// Collection of helpers for randomness
    ///
    public static class RandomUtils
    {
		public const int k_defaultSeed = -1;

        private static System.Random s_random = new System.Random();

        /// Use to give something a "percentageChance" chance of success
        /// 
        /// @param percentageChance
        ///     the percantage chance of success, range [0,100]
        /// 
        /// @return true if randomly generated percentage is lower than the percentage chance given
        /// 
        public static bool IsRandomSuccess(int percentageChance)
        {
            Debug.Assert((percentageChance >= 0) && (percentageChance <= 100), string.Format("percentageChance ({0}) out of range [0,100]", percentageChance));
            return (UnityEngine.Random.Range(0, 100) < percentageChance);
        }

        /// Use to give something a "probability" chance of success
        /// 
        /// @param probability
        ///     the probability of success, range [0.0,1.0]
        /// 
        /// @return true if randomly generated number is lower than the probability given
        /// 
        public static bool IsRandomSuccess(float probability)
        {
            Debug.Assert((probability >= 0.0f) && (probability <= 1.0f), string.Format("probability ({0}) out of range [0.0f,1.0f]", probability));
            return (UnityEngine.Random.Range(0.0f, 1.0f) <= probability);
        }

        /// @return Either -1 or 1
        /// 
        public static int GetRandomSign()
        {
            return UnityEngine.Random.Range(0, 2) * 2 - 1;
        }

        /// @param min
        ///     The mnimum value of each component
        /// @param max
        ///     The maximum value of each component
        /// 
        /// @return A random Vector2 with each component ranging [min,max]
        /// 
        public static Vector2 GetRandomVector2(float min = 0.0f, float max = 1.0f)
        {
            return new Vector2(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
        }

        /// @param min
        ///     The mnimum value of each component
        /// @param max
        ///     The maximum value of each component
        /// 
        /// @return A random Vector3 with each component ranging [min,max]
        /// 
        public static Vector3 GetRandomVector3(float min = 0.0f, float max = 1.0f)
        {
            return new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
        }

        /// @return A random value of an enum type
        /// 
        public static T RandomEnum<T>()
        {
            Type type = typeof(T);
            Array values = Enum.GetValues(type);
            lock (s_random)
            {
                object value = values.GetValue(s_random.Next(values.Length));
                return (T)Convert.ChangeType(value, type);
            }
        }
    }
}
