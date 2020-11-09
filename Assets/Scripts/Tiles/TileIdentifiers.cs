//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace MAGTask
{
    /// The possible colours of tiles
    /// 
    public enum TileColour
    {
        None = 0,
        Blue = 1,
        Yellow = 2,
        Green = 3,
        Pink = 4,
        Red = 5,
        Grey = 6,
    }

    /// The possible states of a board validation
    /// 
    public enum BoardState
    {
        Valid = 0,
        Reshuffle = 1,
        Recreate = 2,
    }
}