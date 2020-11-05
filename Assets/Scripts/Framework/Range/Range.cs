//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System;
using UnityEngine;

namespace CoreFramework
{
	/// Class to represent a range of values, as well as a means to get
	/// a random value in that range
	/// 
    public abstract class Range<TType> : ISerializable where TType : IComparable, IConvertible
	{
        protected const string k_format = "({0},{1})";

        public TType Lower {  get { return m_lowerBound; } }
        public TType Upper {  get { return m_upperBound; } }

		[SerializeField]
		protected TType m_lowerBound = default(TType);
		[SerializeField]
        protected TType m_upperBound = default(TType);

        #region Public
        /// Empty Constructor
        ///
        public Range()
        {
        }

        /// Constructor
        /// 
        /// @param lowerBound
        ///		lower bound of the range (inclusive)
        /// @param upperBound
        ///		upper bound of the range (inclusive)
        ///
        public Range(TType lowerBound, TType upperBound)
		{
			if(lowerBound.CompareTo(upperBound) <= 0)
			{
				m_lowerBound = lowerBound;
				m_upperBound = upperBound;
			}
			else 
			{
                Debug.LogWarning("Range Parameters were mixed up");
				m_lowerBound = upperBound;
				m_upperBound = lowerBound;
			}
		}

        /// @return String representation of this range
        /// 
        public override string ToString()
        {
            return string.Format(k_format, m_lowerBound, m_upperBound);
        }

		/// @param value
		///		Value to check
		///	
		///	@return if the given value is within bounds [lower, upper]
        ///     The value is inclusive of the upper and lower bounds
		///
		public bool IsInRange(TType value)
		{
			return (m_lowerBound.CompareTo(value) <= 0) && (m_upperBound.CompareTo(value) >= 0);
        }

        /// @param otherRange
        ///		Other range to check
        ///	
        ///	@return Whether the range contains the other range
        ///
        public bool Contains(Range<TType> otherRange)
        {
            return IsInRange(otherRange.m_lowerBound) && IsInRange(otherRange.m_upperBound);
        }

        /// @return A random value in bounds
        ///     The value is inclusive of the upper and lower bounds
        ///
        public TType GetRandomValue()
		{
			TypeCode currentType = m_lowerBound.GetTypeCode();

			TType result = m_lowerBound;

			switch(currentType)
			{
				//Including TypeCode.Int64 and TypeCode.UInt64 for convenience, but will lose precision. Random only deals with 32bit int's
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				{
					int lBoundAsInt = (int)Convert.ChangeType(m_lowerBound, typeof(int));
					int uBoundAsInt = (int)Convert.ChangeType(m_upperBound, typeof(int));

					//Return a random int in range, totally ignoring precision for convenience
					result = (TType)Convert.ChangeType(UnityEngine.Random.Range(lBoundAsInt, uBoundAsInt), typeof(TType));
					break;
				}
				case TypeCode.Single:
				case TypeCode.Double:
				{
					double lBoundAsDouble = (double)Convert.ChangeType(m_lowerBound, typeof(double));
					double uBoundAsDouble = (double)Convert.ChangeType(m_upperBound, typeof(double));
					float floatResult = UnityEngine.Random.Range((float)lBoundAsDouble, (float)uBoundAsDouble);

					result = (TType)Convert.ChangeType(floatResult, typeof(TType));
					break;
				}
				default:
				{
					Debug.Assert(false, string.Format("Type not handled - {0}", currentType.ToString()));
					break;
				}
			}
			return result;
		}

        /// @return length of this range
        /// 
        public abstract TType Length();
        #endregion

        #region ISerializable
        /// @return The serialized data
        /// 
        public object Serialize()
        {
            return ToString();
        }

        /// @param data
        ///     The json data
        /// 
        public abstract void Deserialize(object data);
        #endregion
	}
}
