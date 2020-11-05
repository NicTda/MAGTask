//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using System.Collections.Generic;

namespace CoreFramework
{
    /// Helper class that automatically compacts data (list of ints) into a bit field.
    /// This is useful to store compressed data on file / server.
    /// 
	public sealed class CompactListInt : List<int>, ISerializable
	{
        private const int k_bitSize = 16;

        #region ISerializable functions
        /// @return The serialized data
        /// 
        public object Serialize()
        {
            object data = null;
            if (Count > 0)
            {
                var bitData = BitUtils.GetBitData(this, k_bitSize);
                data = JsonWrapper.SerializeListOfTypes(bitData);
            }
            return data;
        }

        /// @param jsonData
        /// 	The json data
        /// 
        public void Deserialize(object jsonData)
        {
            if (jsonData != null)
            {
                Clear();
                var bitData = jsonData.AsList<int>();
                AddRange(BitUtils.GetBitIndexes(bitData, k_bitSize));
            }
        }
        #endregion
    }
}
