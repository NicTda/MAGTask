//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace MAGTask
{
    /// The possible states of a board validation
    /// 
    public enum BoardState
    {
        Valid = 0,
        Reshuffle = 1,
        Recreate = 2,
    }

    /// The possible objectives
    /// 
    public enum ObjectiveType
    {
        None = 0,
        Score = 1,
        Colour = 2,
        Chain = 3,
    }
}