//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Extension class for vectors
    ///
    public static class VectorExtension
    {
        #region Public functions
        /// @param vector
        ///     The vector to check
        /// @param point
        ///     The point to check
        /// 
        /// @return Whether the point is contained by the vector
        ///
        public static bool Contains(this Vector2 vector, float point)
        {
            return ((vector.x <= point) && (point <= vector.y)) || ((vector.y <= point) && (point <= vector.x));
        }

        /// @param vector
        ///     The vector to check
        /// 
        /// @return A random point within the vector
        ///
        public static float GetRandomValue(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }
        #endregion
    }
}
