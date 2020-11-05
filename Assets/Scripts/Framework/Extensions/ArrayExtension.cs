//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
	/// Extension class for additional functionalities to Arrays
	/// 
	public static class ArrayExtension
	{
        /// @param array
        /// 	The array to search
        /// @param item
        /// 	The item to look for
        /// 
        /// @return Whether the item has been found or not
        /// 
        public static bool Contains<T>(this T[] array, T item)
		{
            bool contained = false;
			foreach(T arrayItem in array)
			{
				if(arrayItem.Equals(item))
				{
                    contained = true;
                    break;
				}
			}

			return contained;
		}
	}
}
