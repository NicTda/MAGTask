//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace MAGTask
{
    /// The possible node types
    /// 
    public enum NodeType
    {
        None = 0,
        Level = 1,
        Gate = 2,
    }

    /// The possible node states
    /// 
    public enum NodeState
    {
        Locked = 0,
        Unlocked = 1,
        Open = 2,
        Completed = 3,
    }
}