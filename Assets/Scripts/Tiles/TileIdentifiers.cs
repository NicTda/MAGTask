//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;

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

    /// List identifiers for tiles
    /// 
    public sealed class TileIdentifiers
    {
        public static readonly List<TileColour> k_availableTiles = new List<TileColour>()
        {
            TileColour.Blue,
            TileColour.Yellow,
            TileColour.Green,
            TileColour.Pink,
            TileColour.Red,
            TileColour.Grey,
        };
    }
}