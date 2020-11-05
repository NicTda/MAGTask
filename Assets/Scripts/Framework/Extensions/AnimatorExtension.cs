//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using UnityEngine;

namespace CoreFramework
{
	/// Extension class for the Animator class
	///
	public static class AnimatorExtension
	{
		#region Public functions
		/// @param animator
		/// 	The animator to check
		/// @param animationName
		/// 	The name of the animation
		/// 
		/// @return The length of the requested animation
		///
		public static float GetAnimationLength(this Animator animator, string animationName)
		{
			float animationLength = 0.0f;
			if(animator != null)
			{
				var clipInfo = animator.GetClipInfo(animationName);
				if(clipInfo != null)
				{
					animationLength = clipInfo.length;
				}
			}
			return animationLength;
        }

        /// @param animator
        /// 	The animator to use
        /// @param animationName
        /// 	The name of the animation
        /// @param callback
        ///     The function to call once the animation has played once
        /// 
        /// @return The created coroutine
        ///
        public static Coroutine PlayAnimation(this Animator animator, string animationName, Action callback = null)
        {
            var time = animator.GetAnimationLength(animationName);
            if(animator.gameObject.activeInHierarchy)
            {
                animator.Play(animationName);
            }
            return GlobalDirector.WaitFor(time, callback);
        }

        /// @param animator
        /// 	The animator to check
        /// @param animationName
        /// 	The name of the animation
        /// @param eventName
        /// 	The name of the event
        /// 
        /// @return The length of the requested animation
        ///
        public static float GetAnimationEventTime(this Animator animator, string animationName, string eventName)
		{
			float eventTime = 0.0f;
			if(animator != null)
			{
				var clipInfo = animator.GetClipInfo(animationName);
				if(clipInfo != null)
				{
					foreach(var eventInfo in clipInfo.events)
					{
						if(eventInfo.functionName.Contains(eventName))
						{
							eventTime = eventInfo.time;
							break;
						}
					}
				}
			}
			return eventTime;
		}

		/// @param animationName
		/// 	The name of the animation to check
		/// @param firstEventName
		/// 	The name of the first event
		/// @param secondEventName
		/// 	The name of the second event
		/// 
		/// @return The time between the two given events for this animation, in the given order
		/// 
		public static float GetTimeBetweenEvents(this Animator animator, string animationName, string firstEventName, string secondEventName)
		{
			var firstEventTime = animator.GetAnimationEventTime(animationName, firstEventName);
			var secondEventTime = animator.GetAnimationEventTime(animationName, secondEventName);
			var difference = secondEventTime - firstEventTime;
			if(difference <= 0.0f)
			{
				Debug.LogError(string.Format("The events {0} and {1} are in the wrong order for the animation {2} on the object {3}, time difference = {4}", firstEventName, secondEventName, animationName, animator.gameObject.name, difference));
			}
			return difference;
		}
		#endregion

		#region Private functions
		/// @param animator
		/// 	The animator to check
		/// @param animationName
		/// 	The name of the animation
		/// 
		/// @return The clip info corresponding to the animation
		///
		private static AnimationClip GetClipInfo(this Animator animator, string animationName)
		{
			AnimationClip animationClip = null;
			if(animator != null)
			{
				var animationController = animator.runtimeAnimatorController;
				if(animationController != null)
				{
					foreach(var clipData in animationController.animationClips)
					{
						if(clipData.name.Contains(animationName))
						{
							animationClip = clipData;
							break;
						}
					}
				}
			}
			return animationClip;
		}
		#endregion
	}
}
