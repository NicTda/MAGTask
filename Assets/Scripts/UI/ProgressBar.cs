//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MAGTask
{
    /// Component that controls a progress bar
    /// 
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public sealed class ProgressBar : MonoBehaviour
    {
        public enum ProgressStyle
        {
            Rect,
            FillImage,
        }

        public enum FillStyle
        {
            Horizontal,
            Vertical
        }

        public TextMeshProUGUI Text { get { return m_text; } }

        [SerializeField]
        private ProgressStyle m_progressStyle = ProgressStyle.Rect;
        [SerializeField]
        private FillStyle m_fillStyle = FillStyle.Horizontal;
        [SerializeField]
        private RectTransform m_fillRect = null;
		[SerializeField]
        private Image m_fillImage = null;
        [SerializeField]
        private bool m_overrideColour = true;
        [SerializeField]
        private Color m_fillerColour = Color.white;
        [SerializeField]
        private Color m_completeColour = Color.white;
        [SerializeField]
        private TextMeshProUGUI m_text = null;
        [SerializeField] [Range(0.0f, 1.0f)]
        private float m_progress = 0.0f;
        [SerializeField]
		private bool m_update = false;

		private Tweener m_tween = null;

#if UNITY_EDITOR
        private float m_lastProgress = 0.0f;
#endif

        #region Unity functions
        /// Destroy function
        ///
        private void Awake()
        {
            if(m_completeColour == Color.white)
            {
                m_completeColour = m_fillerColour;
            }
            SetProgress(m_progress);
        }

        /// Destroy function
        ///
        private void OnDestroy()
        {
            StopTween();
        }

#if UNITY_EDITOR
        /// Update function
        ///
        private void Update()
        {
            if (m_update == true)
            {
                m_update = false;
                SetProgress(m_progress);
            }
        }

        /// OnGUI function
        ///
        private void OnGUI()
        {
            if(m_lastProgress != m_progress && Application.isPlaying == false)
            {
                m_lastProgress = m_progress;
                m_update = true;
            }
        }
#endif
        #endregion

        #region Public functions
		/// @param color
		////	the color to set the fill image to
		///
		public void SetFillColor(Color color)
        {
            if (m_progressStyle == ProgressStyle.FillImage)
            {
                m_fillImage.color = color;
            }
			else if (m_progressStyle == ProgressStyle.Rect)
            {
				var image = m_fillRect.GetComponent<Image>();
				if(image != null)
				{
					image.color = color;
				}
			}
        }

        /// Stop tweening
        ///
        public void StopTween()
        {
            m_tween.Stop();
        }

        /// @param endProgress
        ///		The end value of progress clamped between [0.0f, 1.0f]
        /// @param duration
        ///		The duration of the tween
        /// @param [optional] delay
        ///		Delay before tween begins - default 0.0f
        /// @param [optional] callback
        ///		The function to call when the tween is done
        ///
        public void TweenToProgress(float endProgress, float duration, float delay = 0.0f, TweenCallback callback = null)
        {
            TweenToProgress(m_progress, endProgress, duration, delay, callback);
        }

        /// @param startProgress
        ///		The starting progress value between [0.0f, 1.0f]
        /// @param endProgress
        ///		The end value of progress clamped between [0.0f, 1.0f]
        /// @param duration
        ///		The duration of the tween
        /// @param [optional] delay
        ///		Delay before tween begins - default 0.0f
        /// @param [optional] callback
        ///		The function to call when the tween is done
        ///
        public void TweenToProgress(float startProgress, float endProgress, float duration, float delay = 0.0f, TweenCallback callback = null)
		{
            m_tween.Stop();
			m_tween = DOTween.To((x)=> SetProgress(Mathf.Clamp01(x)), startProgress, endProgress, duration).SetDelay(delay).SetEase(Ease.InCubic).OnComplete(callback);
		}

		/// @param progress
		///		The current progress [0.0f, 1.0f]
		///
        public void SetProgress(float progress)
		{
			m_progress = Mathf.Clamp01(progress);
            UpdateFillProgress();

            if (m_overrideColour == true)
            {
                if (m_progress >= 1.0f)
                {
                    SetFillColor(m_completeColour);
                }
                else
                {
                    SetFillColor(m_fillerColour);
                }
            }
		}
 
		/// @return The current progress [0.0f, 1.0f]
		///
		public float GetProgress()
		{
			return m_progress;
		}
        #endregion

        #region Private functions
        /// Update the rect
        /// 
        private void UpdateFillProgress()
        {
            switch (m_progressStyle)
            {
                case ProgressStyle.Rect:
                {
                    UpdateFillRect();
                    break;
                }
                case ProgressStyle.FillImage:
                {
                    UpdateFillImage();
                    break;
                }
            }
        }

        /// Update the rect
        /// 
        private void UpdateFillRect()
        {
            Vector2 newAnchor = m_fillRect.anchorMax;
            switch (m_fillStyle)
            {
                case FillStyle.Horizontal:
                {
                    newAnchor.x = m_progress;
                    break;
                }
                case FillStyle.Vertical:
                {
                    newAnchor.y = m_progress;
                    break;
                }
            }
            m_fillRect.anchorMax = newAnchor;
        }

        /// Update the raw image
        ///
        private void UpdateFillImage()
        {
            m_fillImage.fillAmount = m_progress;
        }
		#endregion
	}
}