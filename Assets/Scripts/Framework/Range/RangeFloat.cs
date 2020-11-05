//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;

namespace CoreFramework
{
    /// Serializable Range for float values
    ///
    [Serializable]
    public sealed class RangeFloat : Range<float>
    {
        #region Public
        /// Empty Constructor
        ///
        public RangeFloat()
        {
        }

		/// Constructor
		///
		/// @param lowerBound
        /// 	Lower bound value (inclusive)
		/// @param upperBound
        /// 	Upper bound value (inclusive)
		/// 
		public RangeFloat(float lowerBound, float upperBound)
			: base(lowerBound, upperBound)
		{
		}

        /// @return length of this range
        /// 
        public override float Length()
        {
            return (m_upperBound - m_lowerBound);
        }
		#endregion

        #region ISerializable
        /// Deserializes the data
        ///
        /// @param data
        ///     The json data
        /// 
        public override void Deserialize(object data)
        {
            var vector = data.AsVector2();
            m_lowerBound = vector.x;
            m_upperBound = vector.y;
        }
        #endregion
    }
}
