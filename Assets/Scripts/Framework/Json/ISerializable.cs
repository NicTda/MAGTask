//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    namespace Json
    {
        /// Interface for objects to serialize
        /// 
        public interface ISerializable
        {
            /// @return The serialized data
            /// 
            object Serialize();
            
            /// @param data
            /// 	The json data
            /// 
            void Deserialize(object data);
        }

        /// Extension class for serializable interface
        /// 
        public static class SerializableExtensions
        {
            /// @return The serialized data as string
            /// 
            public static string SerializeToString(this ISerializable serializable)
            {
                var jsonData = serializable.Serialize();
                return jsonData.Serialize();
            }

            /// @param data
            /// 	The json data
            /// 
            public static void DeserializeString(this ISerializable serializable, string data)
            {
                var jsonData = JsonWrapper.Deserialize(data);
                serializable.Deserialize(jsonData);
            }
        }
    }
}
