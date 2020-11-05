//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
    /// Utils for Text
    /// 
	public static class TextUtils
    {
        private const float k_million = 1000000.0f;
        private const float k_billion = 1000000000.0f;
        private const string k_numberFormat = "{0}";
        private const string k_decimalFormat = "N0";
        private const string k_floatFormat = "N2";

        private const float k_thousand = 1000.0f;
        private const string k_meterFormat = "{0}m";
        private const string k_kilometerFormat = "{0}km";

        private const string k_kilosFormat = "{0}kg";

        private const string k_percentFormat = "{0}%";
        private const string k_percentFormat10 = "{0:0.0}%";
        private const string k_percentFormat100 = "{0:0.00}%";

        #region Public functions
        /// Returns a locally formatted string suitable to display for currency.
        /// Thousands are separated by either ',' or '.' depending on the device locale.
        /// 
        /// @param amount
        ///		the currency amount
        /// 
        /// @return a formatted string for the currency
        ///
        public static string GetFormattedCurrencyString(decimal amount)
        {
            return amount.ToString("N0");
        }

        /// Returns a locally formatted string suitable to display for big numbers.
        /// It'll show "million" or "billion", accordingly.
        /// 
        /// @param amount
        ///		the number to format
        /// 
        /// @return a formatted string for the number
        ///
        public static string GetNumberString(long amount)
        {
            string format = k_numberFormat;
            string parseFormat = k_decimalFormat;
            float number = amount;
            if (Math.Abs(amount) >= k_million)
            {
                number = amount / k_million;
                string formatID = LocalisedTextIdentifiers.k_millionFormat;
                var localisationService = GlobalDirector.Service<LocalisationService>();
                if (Math.Abs(amount) >= k_billion)
                {
                    number = amount / k_billion;
                    formatID = LocalisedTextIdentifiers.k_billionFormat;
                }
                format = localisationService.GetGameText(formatID);
            }

            if (Math.Ceiling(number) != Math.Floor(number))
            {
                number = Mathf.RoundToInt(number * 100) / 100.0f;
                parseFormat = k_floatFormat;
            }

            return string.Format(format, number.ToString(parseFormat));
        }

        /// Returns a locally formatted string suitable to display for meters.
        /// 
        /// @param meters
        ///		the meters to format
        /// 
        /// @return a formatted string for the meters
        ///
        public static string GetMetersString(long meters)
        {
            string parseFormat = k_decimalFormat;
            string format = k_meterFormat;
            return string.Format(format, meters.ToString(parseFormat));
        }

        /// Returns a locally formatted string suitable to display for distances.
        /// It'll show "m" or "km", accordingly.
        /// 
        /// @param meters
        ///		the meters to format
        /// 
        /// @return a formatted string for the distance
        ///
        public static string GetDistanceString(long meters)
        {
            string distanceString = string.Empty;
            if (Math.Abs(meters) >= k_thousand)
            {
                string parseFormat = k_decimalFormat;
                string format = k_kilometerFormat;
                float number = meters / k_thousand;

                if (Math.Ceiling(number) != Math.Floor(number))
                {
                    number = Mathf.RoundToInt(number * 100) / 100.0f;
                    parseFormat = k_floatFormat;
                }

                distanceString = string.Format(format, number.ToString(parseFormat));
            }
            else
            {
                distanceString = GetMetersString(meters);
            }
            return distanceString;
        }

        /// Returns a locally formatted string suitable to display for areas.
        /// It'll show "m²" or "km²", accordingly.
        /// 
        /// @param squaredMeters
        ///		The squared meters to format
        /// 
        /// @return a formatted string for the area
        ///
        public static string GetAreaString(long squaredMeters)
        {
            return GetDistanceString(squaredMeters) + Convert.ToChar(0x00B2);
        }

        /// @param amount
        ///		the percentage amount [0 - 10000]
        /// 
        /// @return a formatted percentage
        ///
        public static string GetFormattedPercent(long amount)
        {
            string valueText = string.Empty;
            string format = k_percentFormat100;

            if (amount % 100 == 0)
            {
                format = k_percentFormat;
            }
            else if (amount % 10 == 0)
            {
                format = k_percentFormat10;
            }
            valueText = string.Format(format, amount / 100.0f);
            return valueText;
        }

        /// Returns a string suitable to display for year.
        /// It'll show a localised "BC" if the year is a negative value.
        /// 
        /// @param year
        ///		the year to convert
        /// 
        /// @return a string for the year
        ///
        public static string GetYear(long year)
        {
            string valueText = string.Empty;

            valueText = Math.Abs(year).ToString();
            if (year < 0)
            {
                valueText = string.Format(LocalisedTextIdentifiers.k_yearBCFormat.LocaliseGame(), valueText);
            }

            return valueText;
        }

        /// Returns a locally formatted string suitable to display for kilos.
        /// 
        /// @param kilos
        ///		the kilos to format
        /// 
        /// @return a formatted string for the kilos
        ///
        public static string GetKilosString(long kilos)
        {
            string parseFormat = k_decimalFormat;
            string format = k_kilosFormat;
            return string.Format(format, kilos.ToString(parseFormat));
        }

        /// Returns a locally formatted string suitable to display for tons.
        /// Strictly rounded tons.
        /// 
        /// @param tons
        ///		the tons to format
        /// 
        /// @return a formatted string for the tons
        ///
        public static string GetTonsString(long tons)
        {
            string parseFormat = k_decimalFormat;
            string format = LocalisedTextIdentifiers.k_tonFormat;
            if (tons > 1)
            {
                format = LocalisedTextIdentifiers.k_tonsFormat;
            }

            return string.Format(format.LocaliseGame(), tons.ToString(parseFormat));
        }

        /// Returns a locally formatted string suitable to display for tons.
        /// Allows for floating point tons.
        /// 
        /// @param tons
        ///		the tons to format
        /// 
        /// @return a formatted string for the tons
        ///
        public static string GetTonsString(float tons)
        {
            string parseFormat = k_decimalFormat;
            if (Math.Ceiling(tons) != Math.Floor(tons))
            {
                tons = Mathf.RoundToInt(tons * 100) / 100.0f;
                parseFormat = k_floatFormat;
            }

            string format = LocalisedTextIdentifiers.k_tonFormat;
            if (tons > 1.0f)
            {
                format = LocalisedTextIdentifiers.k_tonsFormat;
            }

            return string.Format(format.LocaliseGame(), tons.ToString(parseFormat));
        }

        /// Returns a locally formatted string suitable to display for weight.
        /// It'll show "kg" or "ton", or "tons", accordingly.
        /// 
        /// @param kilos
        ///		the kilos to format
        /// 
        /// @return a formatted string for the weight
        ///
        public static string GetWeightString(long kilos)
        {
            string weightString = string.Empty;
            if (Math.Abs(kilos) >= 1000)
            {
                float tons = kilos / k_thousand;
                weightString = GetTonsString(tons);
            }
            else
            {
                weightString = GetKilosString(kilos);
            }
            return weightString;
        }

        /// @param time
        ///		The time to format
        /// 
        /// @return A formatted string for the time
        ///
        public static string GetTimeString(TimeSpan time)
        {
            string format = LocalisedTextIdentifiers.k_timeFormatSeconds;
            if (time.Days > 0)
            {
                format = LocalisedTextIdentifiers.k_timeFormatDays;
            }
            else if (time.Hours > 0)
            {
                format = LocalisedTextIdentifiers.k_timeFormatHours;
            }
            else if (time.Minutes > 0)
            {
                format = LocalisedTextIdentifiers.k_timeFormatMinutes;
            }
            return GetTimeString(time, format.LocaliseGame());
        }

        /// @param time
        ///		The time to format
        /// 
        /// @return A formatted short string for the time
        ///
        public static string GetTimeStringShort(TimeSpan time)
        {
            string format = LocalisedTextIdentifiers.k_timeFormatSeconds;
            if (time.Days > 0)
            {
                format = LocalisedTextIdentifiers.k_timeFormatDH;
            }
            else if (time.Hours > 0)
            {
                format = LocalisedTextIdentifiers.k_timeFormatHM;
            }
            else if (time.Minutes > 0)
            {
                format = LocalisedTextIdentifiers.k_timeFormatMS;
            }
            return GetTimeString(time, format.LocaliseGame());
        }

        /// @param time
        ///		The time to format
        /// @param format
        ///     The format to use
        /// 
        /// @return A formatted string for the time
        ///
        public static string GetTimeString(TimeSpan time, string format)
        {
            return string.Format(format, time.Seconds, time.Minutes, time.Hours, time.Days);
        }
        #endregion
    }
}
