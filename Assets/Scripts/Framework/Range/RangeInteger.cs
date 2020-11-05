//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;
  
namespace CoreFramework
{
    /// Serializable Range for integer values
    ///
    [Serializable]
    public sealed class RangeInteger : Range<int>
    {
        #region Public
        /// Empty Constructor
        ///
        public RangeInteger()
        {
        }

		/// Constructor
        /// 
        /// @param lowerBound
        ///     Lower bound value (inclusive)
        /// @param upperBound
        ///     Upper bound value (inclusive)
        /// 
		public RangeInteger(int lowerBound, int upperBound)
		    : base(lowerBound, upperBound)
		{
		}

        /// @return length of this range
        /// 
        public override int Length()
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
            m_lowerBound = (int)vector.x;
            m_upperBound = (int)vector.y;
        }
        #endregion
    }
}
