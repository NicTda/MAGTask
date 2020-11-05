//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;

namespace CoreFramework
{
    /// Specialist serialisable for the metadata loader
    /// 
    public interface IMetaDataSerializable : ISerializable
    {
        string m_id { get; set; }
    }
}
