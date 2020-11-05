//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CoreFramework
{
    /// Extension class for text
    ///
    public static class TextExtension
    {
        private static readonly string k_defaultTextFormat = "{0}";

        #region Public functions
        /// @param textMesh
        /// 	The text to modify
        /// @param text
        /// 	The text to safely set
        ///
        public static void SafeText(this TMP_Text textMesh, string text)
        {
            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }

        /// @param textMesh
        /// 	The text to modify
        /// @param text
        /// 	The text to safely set
        /// @oaram args
        ///     The format arguments
        ///
        public static void SafeText(this TMP_Text textMesh, string text, params object[] args)
        {
            if (textMesh != null)
            {
                textMesh.text = string.Format(text, args);
            }
        }

        /// @param inputField
        /// 	The UI to modify
        /// @param text
        /// 	The text to safely set
        ///
        public static void SafeText(this TMP_InputField inputField, string text)
        {
            if (inputField != null && inputField.text != null)
            {
                inputField.text = text;
            }
        }

        /// @param inputField
        /// 	The UI to modify
        /// @param text
        /// 	The text to safely set
        /// @oaram args
        ///     The format arguments
        ///
        public static void SafeText(this TMP_InputField inputField, string text, params object[] args)
        {
            if (inputField != null)
            {
                inputField.text = string.Format(text, args);
            }
        }

        /// @param textMesh
        /// 	The text to modify
        /// @oaram amount
        ///     The amount to mount up to
        /// @oaram duration
        ///     The duration of the tween
        /// @oaram [optional] callback
        ///     The function to call when the tweener finished
        ///     
        /// @return The count tweener
        ///
        public static Tweener DOCount(this TMP_Text textMesh, int amount, float duration, Action callback = null)
        {
            return textMesh.DOCount(0, amount, duration, callback);
        }

        /// @param textMesh
        /// 	The text to modify
        /// @param from
        /// 	The amount to start up as
        /// @oaram to
        ///     The amount to mount up to
        /// @oaram duration
        ///     The duration of the tween
        /// @oaram [optional] callback
        ///     The function to call when the tweener finished
        ///     
        /// @return The count tweener
        ///
        public static Tweener DOCount(this TMP_Text textMesh, int from, int to, float duration, Action callback = null)
        {
            return textMesh.DOCount(k_defaultTextFormat, from, to, duration, callback);
        }

        /// @param textMesh
        /// 	The text to modify
        /// @param format
        /// 	The string format to use
        /// @oaram amount
        ///     The amount to mount up to
        /// @oaram duration
        ///     The duration of the tween
        /// @oaram [optional] callback
        ///     The function to call when the tweener finished
        ///     
        /// @return The count tweener
        ///
        public static Tweener DOCount(this TMP_Text textMesh, string format, int amount, float duration, Action callback = null)
        {
            return textMesh.DOCount(format, 0, amount, duration, callback);
        }

        /// @param textMesh
        /// 	The text to modify
        /// @param format
        /// 	The string format to use
        /// @param from
        /// 	The amount to start up as
        /// @oaram to
        ///     The amount to mount up to
        /// @oaram duration
        ///     The duration of the tween
        /// @oaram [optional] callback
        ///     The function to call when the tweener finished
        ///     
        /// @return The count tweener
        ///
        public static Tweener DOCount(this TMP_Text textMesh, string format, int from, int to, float duration, Action callback = null)
        {
            if (textMesh != null)
            {
                int progress = from;
                return DOTween.To(() => progress, (Value) =>
                {
                    textMesh.SafeText(string.Format(format, TextUtils.GetNumberString(progress)));
                    progress = Value;

                }, to, duration).OnComplete(() =>
                {
                    textMesh.SafeText(string.Format(format, TextUtils.GetNumberString(to)));
                    callback.SafeInvoke();
                });
            }
            return null;
        }

        /// @param textMesh
        /// 	The text to reveal
        /// @oaram duration
        ///     The duration of the tween
        /// @param callback
        /// 	The function to call when the reveal is done
        /// 
        public static Tweener DORevealCharacters(this TMP_Text textMesh, float duration = 1.0f, Action callback = null)
        {
            // Hide the text
            var colour = textMesh.color;
            colour.a = 0.0f;
            textMesh.color = colour;
            textMesh.ForceMeshUpdate();

            TMP_TextInfo textInfo = textMesh.textInfo;
            int characterCount = textInfo.characterCount;
            Color32[] newVertexColors;
            int currentIndex = 0;
            int startingRange = currentIndex;
            int spread = Mathf.Min(characterCount, 10);
            int endRange = characterCount + spread;
            var tweener = DOTween.To(() => currentIndex, (index) =>
            {
                byte fadeSteps = (byte)(255 / spread);
                for (int i = startingRange; i < currentIndex + 1 && i < characterCount; ++i)
                {
                    // Skip characters that are not visible
                    if (textInfo.characterInfo[i].isVisible == false)
                    {
                        continue;
                    }

                    // Get the vertex colors of the current mesh
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                    // Get the current character's alpha value
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);

                    // Set new alpha values
                    newVertexColors[vertexIndex + 0].a = alpha;
                    newVertexColors[vertexIndex + 1].a = alpha;
                    newVertexColors[vertexIndex + 2].a = alpha;
                    newVertexColors[vertexIndex + 3].a = alpha;

                    if (alpha == 255)
                    {
                        ++startingRange;
                    }
                }

                // Apply the colour changes
                textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                currentIndex = index;

            }, endRange, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                // End of the reveal
                colour.a = 1.0f;
                textMesh.color = colour;
                textMesh.ForceMeshUpdate();
                callback.SafeInvoke();
            });
            return tweener;
        }
        #endregion
    }
}
