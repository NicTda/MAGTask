//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Factory for board Tiles
    /// 
	public sealed class TileFactory : Factory
    {
        private const string k_tilePrefabPath = "Prefabs/Tiles/TileView";
        private const string k_colourPrefabPath = "Prefabs/Tiles/Jelly{0}";

        #region Factory functions
        /// Callback for when the factory is being initialised
        /// 
        protected override void OnCompleteInitialisation()
        {
        }
        #endregion

        #region Public functions
        /// @param parent
        ///     The parent object of the room
        /// @param position
        ///     The position of the tile
        ///     
        /// @return The view of the newly created tile
        /// 
        public TileView CreateTile(Transform parent, Vector3 position)
        {
            // Random colour
            TileColour colour = RandomUtils.IsRandomSuccess(0.5f) ? TileColour.Blue : TileColour.Yellow;
            return CreateTile(colour, parent, position);
        }

        /// @param colour
        ///     The colour of tile to create
        /// @param parent
        ///     The parent object of the room
        /// @param position
        ///     The position of the tile
        ///     
        /// @return The view of the newly created tile
        /// 
        public TileView CreateTile(TileColour colour, Transform parent, Vector3 position)
        {
            // Create the tile holder
            var tileObject = ResourceUtils.LoadAndInstantiateGameObject(k_tilePrefabPath, parent);
            var tileView = tileObject.GetComponent<TileView>();
            tileObject.transform.position = position;

            // Set the colour of that tile
            SetColourForTile(colour, tileView);

            return tileView;
        }

        /// @param colour
        ///     The colour of tile to set
        /// @param tileView
        ///     The view of the tile to set
        /// 
        public void SetColourForTile(TileColour colour, TileView tileView)
        {
            // Create the requested colour item
            var colourObject = ResourceUtils.LoadAndInstantiateGameObject(string.Format(k_colourPrefabPath, colour.ToString()));

            // Link the two
            var tileItem = colourObject.GetComponent<TileItemView>();
            tileView.m_tileColour = colour;
            tileItem.transform.SetParent(tileView.TileItemHolder, false);
            tileView.m_tileItem = tileItem;
        }
        #endregion
    }
}
