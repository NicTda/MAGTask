//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreFramework
{
    /// Extension class for renderers
    ///
    public static class RendererExtension
    {
        #region Public functions
        /// @param spriteRenderer
        ///     The spriteRenderer
        /// @param sprite
        ///     The sprite to set
        ///
        public static void SafeSprite(this SpriteRenderer spriteRenderer, Sprite sprite)
        {
            if(spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        /// @param image
        ///     The image
        /// @param sprite
        ///     The sprite to set
        ///
        public static void SafeSprite(this Image image, Sprite sprite)
        {
            if (image != null)
            {
                image.sprite = sprite;
            }
        }

        /// @param spriteRenderer
        ///     The spriteRenderer
        /// 
        /// @return A random point contained in the spriteRenderer
        ///
        public static Vector3 GetRandomPoint(this SpriteRenderer spriteRenderer)
        {
            Vector3 size = spriteRenderer.size * spriteRenderer.transform.lossyScale;
            Vector3 bottomLeft = spriteRenderer.transform.position - size * 0.5f;
            Vector3 topRight = bottomLeft + size;

            float randomX = Random.Range(bottomLeft.x, topRight.x);
            float randomY = Random.Range(bottomLeft.y, topRight.y);
            return new Vector3(randomX, randomY, bottomLeft.z);
        }

        /// @param vector
        ///     The spriteRenderer
        /// @param gridSize
        ///     The virtual size of the grid to use
        /// 
        /// @return A random point contained in the spriteRenderer's grid
        ///
        public static Vector3 GetRandomPoint(this SpriteRenderer spriteRenderer, int gridSize)
        {
            Vector3 size = spriteRenderer.size * spriteRenderer.transform.lossyScale;
            Vector3 bottomLeft = spriteRenderer.transform.position - size * 0.5f;

            var cellSize = size / gridSize;
            float randomX = bottomLeft.x + Random.Range(0, gridSize + 1) * cellSize.x;
            float randomY = bottomLeft.y + Random.Range(0, gridSize + 1) * cellSize.y;
            return new Vector3(randomX, randomY, bottomLeft.z);
        }

        /// @param vector
        ///     The spriteRenderer
        /// @param gridSize
        ///     The virtual size of the grid to use
        /// 
        /// @return The list of all points contained in the spriteRenderer's grid
        ///
        public static List<Vector3> GetGridPoints(this SpriteRenderer spriteRenderer, int gridSize)
        {
            Vector3 size = spriteRenderer.size * spriteRenderer.transform.lossyScale;
            Vector3 bottomLeft = spriteRenderer.transform.position - size * 0.5f;

            var cellSize = size / gridSize;
            var points = new List<Vector3>();
            for (int i = 0; i < gridSize; ++i)
            {
                for (int j = 0; j < gridSize; ++j)
                {
                    float pointX = bottomLeft.x + i * cellSize.x;
                    float pointY = bottomLeft.y + j * cellSize.y;
                    var point = new Vector3(pointX, pointY, bottomLeft.z);
                    points.Add(point);
                }
            }
            return points;
        }

        /// @param vector
        ///     The spriteRenderer
        /// @param gridSize
        ///     The virtual size of the grid to use
        /// @param amount
        ///     The amount of points to create
        /// 
        /// @return The list of random points contained in the spriteRenderer's grid
        ///
        public static List<Vector3> GetRandomPoints(this SpriteRenderer spriteRenderer, int gridSize, int amount)
        {
            Vector3 size = spriteRenderer.size * spriteRenderer.transform.lossyScale;
            Vector3 bottomLeft = spriteRenderer.transform.position - size * 0.5f;

            var cellSize = size / gridSize;
            var randomPoints = new List<Vector3>();
            for (int i = 0; i < amount; ++i)
            {
                float randomX = bottomLeft.x + Random.Range(0, gridSize + 1) * cellSize.x;
                float randomY = bottomLeft.y + Random.Range(0, gridSize + 1) * cellSize.y;
                var point = new Vector3(randomX, randomY, bottomLeft.z);
                randomPoints.Add(point);
            }
            return randomPoints;
        }

        /// @param vector
        ///     The spriteRenderer
        /// @param gridSize
        ///     The virtual size of the grid to use
        /// @param amount
        ///     The amount of points to create
        /// 
        /// @return The list of unique random points contained in the spriteRenderer's grid
        ///
        public static List<Vector3> GetDistinctRandomPoints(this SpriteRenderer spriteRenderer, int gridSize, int amount)
        {
            Vector3 size = spriteRenderer.size * spriteRenderer.transform.lossyScale;
            Vector3 bottomLeft = spriteRenderer.transform.position - size * 0.5f;

            var cellSize = size / gridSize;
            var randomPoints = new List<Vector3>();
            for (int i = 0; i < amount; ++i)
            {
                float randomX = bottomLeft.x + Random.Range(0, gridSize + 1) * cellSize.x;
                float randomY = bottomLeft.y + Random.Range(0, gridSize + 1) * cellSize.y;
                var point = new Vector3(randomX, randomY, bottomLeft.z);
                if(randomPoints.Contains(point) == true)
                {
                    --i;
                    continue;
                }
                randomPoints.Add(point);
            }
            return randomPoints;
        }
        #endregion
    }
}
