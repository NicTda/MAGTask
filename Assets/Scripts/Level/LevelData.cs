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

        private const string k_keyTiles = "Tiles";
        private const string k_keyHeight = "Height";
        private const string k_keyWidth = "Width";
        private const string k_keyMoves = "Moves";
        private const string k_keyScores = "Scores";
        private const string k_keyObjectives = "Objectives";

        public string m_id { get; set; } = string.Empty;
        public int m_index = 0;
        public List<ObjectiveData> m_objectives = new List<ObjectiveData>();
        public List<TileColour> m_tiles = new List<TileColour>();
        public List<int> m_scores = new List<int>() { 0, 0, 0};
        public int m_width = 5;
        public int m_height = 5;
        public int m_moves = 10;

        #region Public functions
        /// @return A clone of this level data
        /// 
        public LevelData Clone()
        {
            return new LevelData()
            {
                m_id = m_id,
                m_index = m_index,
                m_width = m_width,
                m_height = m_height,
                m_moves = m_moves,
                m_objectives = new List<ObjectiveData>(m_objectives),
                m_tiles = new List<TileColour>(m_tiles),
                m_scores = new List<int>(m_scores)
            };
        }
        #endregion

        #region ISerializable functions
        /// @return The serialized data
        ///
        public object Serialize()
        {
            var jsonData = new JsonDictionary()
            {
                { k_keyWidth, m_width },
                { k_keyHeight, m_height },
                { k_keyMoves, m_moves },
            };
            if (m_tiles.Count > 0)
            {
                var tilesArray = JsonWrapper.CreateJsonArray(m_tiles.Count);
                for (int i = 0; i < m_tiles.Count; ++i)
                {
                    tilesArray[i] = m_tiles[i].ToString();
                }
                jsonData.Add(k_keyTiles, tilesArray);
            }
            if (m_scores.Count > 0)
            {
                jsonData.Add(k_keyScores, JsonWrapper.SerializeListOfTypes(m_scores));
            }
            if (m_objectives.Count > 0)
            {
                jsonData.Add(k_keyObjectives, JsonWrapper.SerializeListOfSerializables(m_objectives));
            }
            return jsonData;
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
        }
        #endregion
    }
}
