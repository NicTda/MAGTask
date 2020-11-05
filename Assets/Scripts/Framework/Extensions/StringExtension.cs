//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CoreFramework
{
    /// Available comparator for strings
    ///
	public enum StringComparator
	{
		Identical,
		Contains,
		StartsWith,
		EndsWith
	}

    /// Extension class for strings
    ///
    public static class StringExtension
    {
		#region Public functions
        /// @param firstString
		/// 	The string to check
		/// @param otherString
		/// 	The string to compare with
		/// @param comparator
		/// 	The method of comparison to use
		/// 
		/// @return Whether the strings are matching the comparison criterion
        ///
		public static bool Matches(this string firstString, string otherString, StringComparator comparator = StringComparator.Identical)
        {
			bool match = false;

			switch(comparator)
			{
				case StringComparator.Identical:
				{
					match = (firstString == otherString);
					break;
				}
				case StringComparator.Contains:
				{
					match = firstString.Contains(otherString);
					break;
				}
				case StringComparator.StartsWith:
				{
					match = firstString.StartsWith(otherString);
                    break;
				}
				case StringComparator.EndsWith:
				{
					match = firstString.EndsWith(otherString);
                    break;
				}
				default:
				{
					Debug.LogError(string.Format("Unknown comparator '{0}'. Could not compare '{1}' and '{2}'", comparator.ToString(), firstString, otherString));
					break;
				}
			}

			return match;
        }

        /// @param text
        ///     The string to check
        /// @param list
        ///     The list containing the strings to check against
        /// 
        /// @return Whether the string contains any of the given strings
        /// 
        public static bool ContainsAny(this string text, List<string> list)
        {
            foreach (var item in list)
            {
                if (text.Contains(item) == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// @param text
        ///     The string to check
        /// @param list
        ///     The list containing the strings to check against
        /// 
        /// @return Whether the string contains all of the given strings
        /// 
        public static bool ContainsAll(this string text, List<string> list)
        {
            foreach (var item in list)
            {
                if (text.Contains(item) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// @param text
        ///     The string to trim spaces from
        /// 
        /// @return The trimmed string
        /// 
        public static string TrimSpaces(this string text)
        {
            return new string(text.Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
        }

        /// @param text
        ///     The string to trim spaces from
        /// @param characters
        ///     The characters to trim
        /// 
        /// @return The trimmed string
        /// 
        public static string TrimChars(this string text, string characters)
        {
            return new string(text.Where(c => !characters.Contains(c)).ToArray());
        }

        /// @param url
        ///     The URL to format
        /// 
        public static string FormatURL(this string url)
        {
            return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }

        /// @param text
        ///     The string to encode
        ///     
        public static string ToBase64(this string text)
        {
            return ToBase64(text, Encoding.UTF8);
        }

        /// @param text
        ///     The string to encode
        /// @param endcoding
        ///     The encoding to use
        ///     
        public static string ToBase64(this string text, Encoding encoding)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        /// @param text
        ///     The string to decode
        /// @param endcoding
        ///     The encoding to use
        ///    
        /// @return The decoded string
        /// 
        public static string ParseBase64(this string text)
        {
            return ParseBase64(text, Encoding.UTF8);
        }

        /// @param text
        ///     The string to decode
        /// @param endcoding
        ///     The encoding to use
        ///    
        /// @return The decoded string
        /// 
        public static string ParseBase64(this string text, Encoding encoding)
        {
            string decodedText = string.Empty;
            if (string.IsNullOrEmpty(text) == false)
            {
                try
                {
                    byte[] textAsBytes = Convert.FromBase64String(text);
                    decodedText = encoding.GetString(textAsBytes);
                }
                catch (Exception)
                {
                }
            }
            return decodedText;
        }
        #endregion
    }
}
