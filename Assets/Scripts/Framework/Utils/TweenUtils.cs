//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using DG.Tweening;

namespace CoreFramework
{
	/// Utils for tweens
	///
	public static class TweenUtils
	{
		#region Unity functions
		/// Stops the tween
		/// 
		/// @param tween
		/// 	The tween to stop
		/// @param [optional] complete
		/// 	Whether to complete the tween or not. Defaults to false.
		/// 
		public static void Stop(this Tweener tween, bool complete = false)
		{
			if(tween != null)
			{
				tween.Kill(complete);
			}
		}

		/// Stops the sequence
		/// 
		/// @param sequence
		/// 	The sequence to stop
		/// @param [optional] complete
		/// 	Whether to complete the tweens or not. Defaults to false.
		/// 
		public static void Stop(this Sequence sequence, bool complete = false)
		{
			if(sequence != null)
			{
				sequence.Kill(complete);
			}
		}
		#endregion
	}
}
