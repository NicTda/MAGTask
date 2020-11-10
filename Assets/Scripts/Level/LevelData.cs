//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using CoreFramework.Json;
using System;
using System.Collections.Generic;

namespace MAGTask
{
    /// Container class for a level
    /// 
    public sealed class LevelData : IMetaDataSerializable
    {
        public const string k_levelPrefix = "Level";

        private const string k_keyType = "Type";
        private const string k_keyTiles = "Tiles";
        private const string k_keyHeight = "Height";
        private const string k_keyWidth = "Width";
        private const string k_keyMoves = "Moves";
        private const string k_keyScores = "Scores";
        private const string k_keyObjectives = "Objectives";
        private const string k_keyReward = "Reward";

        public string m_id { get; set; } = string.Empty;
        public int m_index { get; private set; } = 0;
        public string m_type { get; private set; } = string.Empty;
        public List<ObjectiveData> m_objectives { get; private set; } = new List<ObjectiveData>();
        public List<TileColour> m_tiles { get; private set; } = new List<TileColour>();
        public List<int> m_scores { get; private set; } = new List<int>();
        public int m_width { get; private set; } = 5;
        public int m_height { get; private set; } = 5;
        public int m_moves { get; private set; } = 10;
        public CurrencyItem m_reward { get; private set; } = null;

        #region ISerializable functions
        /// @return The serialized data
        ///
        public object Serialize()
        {
            return null;
        }

        /// @param data
        ///     The json data
        /// 
        public void Deserialize(object data)
        {
            // Get the index from the ID
            var indexString = m_id.Substring(k_levelPrefix.Length, m_id.Length - k_levelPrefix.Length);
            if (Int32.TryParse(indexString, out int integer))
            {
                m_index = integer;
            }

            // Deserialize the rest of the data
            var jsonData = data.AsDictionary();
            if (jsonData.ContainsKey(k_keyType))
            {
                m_type = jsonData.GetString(k_keyType);
            }
            if (jsonData.ContainsKey(k_keyHeight))
            {
                m_height = jsonData.GetInt(k_keyHeight);
            }
            if (jsonData.ContainsKey(k_keyWidth))
            {
                m_width = jsonData.GetInt(k_keyWidth);
            }
            if (jsonData.ContainsKey(k_keyMoves))
            {
                m_moves = jsonData.GetInt(k_keyMoves);
            }
            if (jsonData.ContainsKey(k_keyScores))
            {
                m_scores = jsonData.GetValue(k_keyScores).AsList<int>();
            }
            if (jsonData.ContainsKey(k_keyTiles))
            {
                m_tiles = jsonData.GetValue(k_keyTiles).AsEnumList<TileColour>();
            }
            if (jsonData.ContainsKey(k_keyObjectives))
            {
                m_objectives = jsonData.GetValue(k_keyObjectives).AsListOfSerializables<ObjectiveData>();
            }
            if (jsonData.ContainsKey(k_keyReward))
            {
                m_reward = new CurrencyItem();
                m_reward.Deserialize(jsonData[k_keyReward]);
            }
        }
        #endregion
    }
}
