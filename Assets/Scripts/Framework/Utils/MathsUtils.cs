//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// Utils for Maths functions
    /// 
    public static class MathsUtils
    {
        #region Public functions
        /// @param number
        ///     The number to divide
        /// @param divider
        ///     The divider to use
        /// 
        /// @return The modulo of the two numbers
        ///
        public static int Mod(this int number, int divider)
        {
            int mod = number % divider;
            if((mod != 0) && Math.Sign(number) != Math.Sign(divider))
            {
                if(divider > 0)
                {
                    mod = (Math.Abs(mod) - divider) * -1;
                }
                else
                {
                    mod = Math.Abs(mod) + divider;
                }
            }
            return mod;
        }

        /// @param number
        ///     The number to divide
        /// @param divider
        ///     The divider to use
        /// 
        /// @return The modulo of the two numbers
        ///
        public static float Mod(this float number, float divider)
        {
            float mod = number % divider;
            if ((mod != 0.0f) && Mathf.Sign(number) != Mathf.Sign(divider))
            {
                if (divider > 0)
                {
                    mod = (Mathf.Abs(mod) - divider) * -1.0f;
                }
                else
                {
                    mod = Mathf.Abs(mod) + divider;
                }
            }
            return mod;
        }
        #endregion
    }
}
