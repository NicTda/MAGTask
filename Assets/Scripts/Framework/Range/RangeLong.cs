//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;

namespace CoreFramework
{
    /// Serializable Range for long values
    ///
    [Serializable]
    public sealed class RangeLong : Range<long>
    {
        #region Range functions
        /// Empty Constructor
        ///
        public RangeLong()
        {
            m_lowerBound = long.MinValue;
            m_upperBound = long.MaxValue;
        }

        /// @param lowerBound
        ///     Lower bound value (inclusive)
        /// @param upperBound
        ///     Upper bound value (inclusive)
        /// 
        public RangeLong(long lowerBound, long upperBound)
            : base(lowerBound, upperBound)
        {
        }

        /// @return length of this range
        /// 
        public override long Length()
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
            m_lowerBound = (long)vector.x;
            m_upperBound = (long)vector.y;
        }
        #endregion
    }
}
