//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;

namespace CoreFramework
{
    /// Utils for Device
    /// 
	public static class BitUtils
    {
        #region Public functions
        /// @param number
        ///     The number to check
        ///     
        /// @return The bit size of the given number
        /// 
        public static int GetBitSize(int number)
        {
            int bitSize = 1;
            while (number != 0)
            {
                number /= 2;
                ++bitSize;
            }
            return bitSize;
        }

        /// @param number
        ///     The number to check
        /// @param [optional] indexShift
        ///     The shift to apply, if any
        ///     
        /// @return The indexes flagged in the given int
        /// 
        public static List<int> GetBitIndexes(int number, int indexShift = 0)
        {
            var bitIndexes = new List<int>();
            int localIndex = 0;
            int bit = number;
            while (bit != 0)
            {
                // Check first bit
                if ((bit & 1) != 0)
                {
                    bitIndexes.Add(indexShift + localIndex);
                }

                // Shift the int
                bit /= 2;
                ++localIndex;
            }
            return bitIndexes;
        }

        /// @param number
        ///     The number to check
        /// @param [optional] indexShift
        ///     The shift to apply, if any
        ///     
        /// @return The indexes flagged in the given int
        /// 
        public static List<int> GetBitIndexes(List<int> bitsList, int indexShift = 0)
        {
            int shift = 0;
            var bitIndexes = new List<int>();
            foreach (var bits in bitsList)
            {
                bitIndexes.AddRange(GetBitIndexes(bits, shift));
                shift += indexShift;
            }
            
            return bitIndexes;
        }

        /// @param numbers
        ///     The numbers to convert to bits
        /// @param bitSize
        ///     The bit size to cap at
        ///     
        /// @return The bit data for the given list of int
        /// 
        public static List<int> GetBitData(List<int> numbers, int bitSize)
        {
            List<int> bits = new List<int>();

            foreach (var number in numbers)
            {
                int bitIndex = number / bitSize;
                int bit = number % bitSize;

                while (bitIndex > bits.Count - 1)
                {
                    bits.Add(0);
                }

                int bitPosition = (1 << bit);
                bits[bitIndex] |= bitPosition;
            }

            return bits;
        }
        #endregion
    }
}
