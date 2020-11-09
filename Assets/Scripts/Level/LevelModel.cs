//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using CoreFramework.Json;

namespace MAGTask
{
    /// Container class for a level
    /// 
    public sealed class LevelModel : ISerializable
    {
        private const string k_keyIndex = "ID";
        private const string k_keyNodeState = "NS";
        private const string k_keyHighscore = "HS";

        public int m_index = 0;
        public int m_highscore = 0;
        public NodeState m_nodeState = NodeState.Locked;

        #region ISerializable functions
        /// @return The serialized data
        ///
        public object Serialize()
        {
            var jsonData = new JsonDictionary()
            {
                { k_keyIndex, m_index }
            };
            if (m_nodeState != NodeState.Locked)
            {
                jsonData.Add(k_keyNodeState, (int)m_nodeState);
            }
            if (m_highscore > 0)
            {
                jsonData.Add(k_keyHighscore, m_highscore);
            }
            return jsonData;
        }

        /// @param data
        ///     The json data
        /// 
        public void Deserialize(object data)
        {
            var jsonData = data.AsDictionary();
            if (jsonData.ContainsKey(k_keyIndex))
            {
                m_index = jsonData.GetInt(k_keyIndex);
            }
            if (jsonData.ContainsKey(k_keyNodeState))
            {
                m_nodeState = (NodeState)jsonData.GetInt(k_keyNodeState);
            }
            if (jsonData.ContainsKey(k_keyHighscore))
            {
                m_highscore = jsonData.GetInt(k_keyHighscore);
            }
        }
        #endregion
    }
}
