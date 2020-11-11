//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System;
using System.Collections.Generic;

namespace MAGTask
{
    /// Identifiers for localised text
    /// 
    public static class GameTextIdentifiers
    {
        static BankService m_cachedBankService = null;
        static LocalisationService m_cachedLocalisationService = null;
        private static readonly List<string> k_currencies = new List<string>()
        {
            OverlayBankIdentifiers.k_currencyCoins,
            OverlayBankIdentifiers.k_currencyPremium,
        };

        /// String format
        /// 
        public const string k_progressFormat = "{0} / {1}";
        public const string k_currencyFormat = "<sprite name=\"{0}\">";
        public const string k_costFormat = "{0} {1}";
        public const string k_rewardFormat = "+{0}";
        public const string k_pluralFormat = "{0}_plural";

        /// Game keys
        /// 
        public const string k_cost = "Cost";
        public const string k_reward = "Reward";
        public const string k_free = "Free";
        public const string k_exitGame = "ExitGame";
        public const string k_costSuccess = "CostSuccess";
        public const string k_costFail = "CostFail";

        /// Game keys (supposed to be localised)
        /// 
        public const string k_mapExit = "Do you want to go back to the main menu?";
        public const string k_mapLocked = "This level is not available just yet!";
        public const string k_levelExit = "Do you want to go back to the level selection?";
        public const string k_levelDisplay = "Level {0}";
        public const string k_levelWinBody = "Well done! Level {0} completed!";
        public const string k_levelLostRetry = "Out of moves! Do you want to continue playing by adding {0} moves?";

        /// Objective keys
        /// 
        /// {0} -> Target or Value
        /// {1} -> Amount
        /// 
        public static readonly Dictionary<ObjectiveType, string> k_objectives = new Dictionary<ObjectiveType, string>()
        {
            { ObjectiveType.Score, "Score {1} points" },
            { ObjectiveType.Colour, "Match {1} <sprite name=\"{0}\">" },
            { ObjectiveType.Chain, "Chain {0} blocks" },
        };

        #region Public functions
        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted cost string
        /// 
        public static string LocaliseCost(this CurrencyItem currencyItem)
        {
            return GetLocalisedCost(currencyItem.m_currencyID, currencyItem.m_value);
        }

        /// @param currencyID
        ///     The ID of the currency to display
        /// @param amount
        ///     The amount of currency to display
        ///     
        /// @return The formatted cost string
        /// 
        public static string GetLocalisedCost(string currencyID, int amount)
        {
            string costString = string.Empty;
            if (amount <= 0)
            {
                costString = k_free.LocaliseGame();
            }
            else
            {
                costString = string.Format(k_cost.LocaliseGame(), currencyID, TextUtils.GetFormattedCurrencyString(amount));
            }
            return costString;
        }

        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted cost string
        /// 
        public static string LocaliseFullColouredCost(this CurrencyItem currencyItem)
        {
            string costString = currencyItem.LocaliseColouredCost();
            return string.Format(k_costFormat, costString, currencyItem.LocaliseFullName());
        }

        /// @param cost
        ///     The cost to display
        ///     
        /// @return The formatted cost string
        /// 
        public static string LocaliseColouredCost(this CurrencyItem cost)
        {
            if (m_cachedBankService == null)
            {
                m_cachedBankService = GlobalDirector.Service<BankService>();
            }

            string costString = string.Empty;
            if (k_currencies.Contains(cost.m_currencyID) == true)
            {
                costString = cost.LocaliseCost();
            }
            else
            {
                if (cost.IsFree() == true)
                {
                    costString = k_free.LocaliseGame();
                }
                else
                {
                    int current = Math.Min(m_cachedBankService.GetBalance(cost.m_currencyID), cost.m_value);
                    string currentString = TextUtils.GetFormattedCurrencyString(current);
                    string formatID = k_costFail;
                    if (m_cachedBankService.CanAfford(cost) == true)
                    {
                        formatID = k_costSuccess;
                    }
                    costString = string.Format(formatID.LocaliseGame(), cost.m_currencyID, TextUtils.GetFormattedCurrencyString(cost.m_value), currentString);
                }

            }
            return costString;
        }

        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted reward string
        /// 
        public static string LocaliseReward(this CurrencyItem currencyItem)
        {
            return currencyItem != null ? GetLocalisedReward(currencyItem.m_currencyID, currencyItem.m_value) : string.Empty;
        }

        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted currency string
        /// 
        public static string LocaliseFullCurrency(this CurrencyItem currencyItem)
        {
            string rewardString = string.Format(k_currencyFormat, currencyItem.m_currencyID);
            return string.Format(k_costFormat, rewardString, currencyItem.LocaliseFullNameAmount());
        }

        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted currency string
        /// 
        public static string LocaliseFullName(this CurrencyItem currencyItem)
        {
            string textID = currencyItem.m_currencyID;
            if (currencyItem.m_value > 1)
            {
                var pluralID = string.Format(k_pluralFormat, currencyItem.m_currencyID);
                textID = pluralID;
            }
            return textID.LocaliseGame();
        }

        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted currency string
        /// 
        public static string LocaliseFullNameAmount(this CurrencyItem currencyItem)
        {
            string textID = currencyItem.m_currencyID;
            if (currencyItem.m_value > 1)
            {
                var pluralID = string.Format(k_pluralFormat, currencyItem.m_currencyID);
                textID = pluralID;
            }
            return string.Format(k_costFormat, TextUtils.GetFormattedCurrencyString(currencyItem.m_value), textID.LocaliseGame());
        }

        /// @param currencyID
        ///     The ID of the currency to display
        /// @param amount
        ///     The amount of currency to display
        ///     
        /// @return The formatted reward string
        /// 
        public static string GetLocalisedReward(string currencyID, int amount)
        {
            return k_reward.LocaliseGame(currencyID, TextUtils.GetFormattedCurrencyString(amount));
        }

        /// @param currencyItem
        ///     The currency to display
        ///     
        /// @return The formatted reward amount string
        /// 
        public static string LocaliseRewardAmount(this CurrencyItem currencyItem)
        {
            return string.Format(k_rewardFormat, currencyItem.LocaliseReward());
        }
        #endregion
    }
}
