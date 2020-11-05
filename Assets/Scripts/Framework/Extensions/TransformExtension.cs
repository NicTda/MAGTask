//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Extension class for the Transform / RectTransform classes
    /// 
	public static class TransformExtension
    {
        #region Public functions
        /// @param name
        ///     The name of the child to find
        ///     
        /// @return The first child in the hierarchy with the given name
        ///
        public static Transform FindFirst(this Transform transform, string name)
        {
            Transform foundChild = null;
            if(transform.name == name)
            {
                foundChild = transform;
            }
            else
            {
                foreach (Transform child in transform)
                {
                    foundChild = child.FindFirst(name);
                    if (foundChild != null)
                    {
                        break;
                    }
                }
            }
            return foundChild;
        }

        /// @param name
        ///     The name of the children to find
        ///     
        /// @return The list of children in the hierarchy with the given name
        ///
        public static List<Transform> FindAll(this Transform transform, string name)
        {
            List<Transform> children = new List<Transform>();
            if (transform.name == name)
            {
                children.Add(transform);
            }

            foreach (Transform child in transform)
            {
                children.AddRange(child.FindAll(name));
            }

            return children;
        }

        /// @return The list of active children in the given transform 
        ///
        public static List<TransformType> ActiveChildren<TransformType>(this TransformType transform) 
            where TransformType : Transform
        {
            List<TransformType> children = new List<TransformType>();
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf == true)
                {
                    children.Add((TransformType)child);
                }
            }
            return children;
        }

        /// @return The amount of active children in the given transform 
        ///
        public static int ActiveChildrenCount(this Transform transform)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf == true)
                {
                    ++count;
                }
            }
            return count;
        }

        /// Destroy all active children of that transform
        ///
        public static void DestroyChildren(this Transform transform)
        {
            if(transform != null)
            {
                foreach (var child in transform.ActiveChildren())
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        /// @param rectTransform
        ///     The RectTransform to check
        ///     
        /// @return Whether the rectTransform can be seen on screen
        /// 
        public static bool IsVisibleOnScreen(this RectTransform rectTransform)
        {
            bool onScreen = false;
            var screenRect = ScreenUtils.k_rect;
            var worldRect = rectTransform.WorldRect();
            if(((-worldRect.size.y <= worldRect.position.y) && (worldRect.position.y <= screenRect.size.y + worldRect.size.y)) &&
               ((-worldRect.size.x <= worldRect.position.x) && (worldRect.position.x <= screenRect.size.x + worldRect.size.x)))
            {
                onScreen = true;
            }
            return onScreen;
        }

        /// @param rectTransform
        ///     The RectTransform to check
        ///     
        /// @return Whether the rectTransform is within screen bounds on the x axis
        /// 
        public static bool IsVisibleHorizontally(this RectTransform rectTransform)
        {
            bool onScreen = false;
            var worldRect = rectTransform.WorldRect();
            if ((-worldRect.size.x <= worldRect.position.x) && (worldRect.position.x <= ScreenUtils.k_rect.size.x + worldRect.size.x))
            {
                onScreen = true;
            }
            return onScreen;
        }

        /// @param rectTransform
        ///     The RectTransform to check
        ///     
        /// @return Whether the rectTransform is within screen bounds on the y axis
        /// 
        public static bool IsVisibleVertically(this RectTransform rectTransform)
        {
            bool onScreen = false;
            var worldRect = rectTransform.WorldRect();
            if ((-worldRect.size.y <= worldRect.position.y) && (worldRect.position.y <= ScreenUtils.k_rect.size.y + worldRect.size.y))
            {
                onScreen = true;
            }
            return onScreen;
        }

        /// @param rectTransform
        ///     The RectTransform to check
        /// @param boundsRect
        ///     The visible bounds rect
        ///     
        /// @return Whether the rectTransform can be seen within the bounds
        /// 
        public static bool IsVisibleWithin(this RectTransform rectTransform, Rect boundsRect)
        {
            return rectTransform.IsVisibleFrom(boundsRect);
        }

        /// @param rectTransform
        ///     The RectTransform to check
        /// @param boundsTransform
        ///     The transform of the visible bounds 
        ///     
        /// @return Whether the rectTransform can be seen within the bounds
        /// 
        public static bool IsVisibleWithin(this RectTransform rectTransform, RectTransform boundsTransform)
        {
            return rectTransform.IsVisibleFrom(boundsTransform);
        }
        #endregion

        #region Private functions
        /// @param rectTransform
        ///     The RectTransform to check
        /// @param boundsTransform
        ///     The transform of the visible bounds 
        ///     
        /// @return Whether the rectTransform can be seen within the bounds
        /// 
        private static bool IsVisibleFrom(this RectTransform rectTransform, RectTransform boundsTransform)
        {
            Vector3[] boundCorners = new Vector3[4];
            boundsTransform.GetWorldCorners(boundCorners);
            Rect boundsRect = new Rect(boundCorners[0], boundCorners[2] - boundCorners[0]);
            return rectTransform.IsVisibleFrom(boundsRect);
        }

        /// @param rectTransform
        ///     The RectTransform to check
        /// @param boundsRect
        ///     The visible bounds rect
        ///     
        /// @return Whether the rectTransform can be seen within the bounds
        /// 
        private static bool IsVisibleFrom(this RectTransform rectTransform, Rect boundsRect)
        {
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
            Rect objectRect = new Rect(objectCorners[0], objectCorners[2] - objectCorners[0]);
            return objectRect.Overlaps(boundsRect);
        }

        /// @param rectTransform
        ///     The RectTransform to check
        ///     
        /// @return The world rect for this transform
        /// 
        public static Rect WorldRect(this RectTransform rectTransform)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth * 0.5f, position.y - rectTransformHeight * 0.5f, rectTransformWidth, rectTransformHeight);
        }
        #endregion
    }
}
