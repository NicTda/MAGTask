//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;

namespace CoreFramework
{
    /// Extension class for the System.Action class
    /// 
	public static class ActionExtension
	{
        #region Public functions
        /// Invoke the Action. If the Action is null, it does nothing.
        /// 
        public static void SafeInvoke(this Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        /// Invoke the Action. If the Action is null, it does nothing.
        /// 
        /// @param param
        /// 	The parameter of the Action
        ///
        public static void SafeInvoke<Type>(this Action<Type> action, Type param)
        {
            if (action != null)
            {
                action.Invoke(param);
            }
        }

        /// Invoke the Action. If the Action is null, it does nothing.
        /// 
        /// @param paramA
        /// 	The first parameter of the Action
        /// @param paramB
        /// 	The second parameter of the Action
        /// @param [optional] emptyAction
        ///     Whether the action should be emptied after invoking
        ///
        public static void SafeInvoke<TypeA, TypeB>(this Action<TypeA, TypeB> action, TypeA paramA, TypeB paramB)
        {
            if (action != null)
            {
                action.Invoke(paramA, paramB);
            }
        }

        /// Invoke the Action. If the Action is null, it does nothing.
        /// 
        /// @param paramA
        /// 	The first parameter of the Action
        /// @param paramB
        /// 	The second parameter of the Action
        /// @param paramC
        /// 	The third parameter of the Action
        ///
        public static void SafeInvoke<TypeA, TypeB, TypeC>(this Action<TypeA, TypeB, TypeC> action, TypeA paramA, TypeB paramB, TypeC paramC)
        {
            if (action != null)
            {
                action.Invoke(paramA, paramB, paramC);
            }
        }

        /// Invoke the Action in the end of the frame. If the Action is null, it does nothing.
        /// 
        public static void DelayInvoke(this Action action)
        {
            if (action != null)
            {
                GlobalDirector.WaitForFrame(action);
            }
        }

        /// Invoke the Action in the end of the frame. If the Action is null, it does nothing.
        /// 
        /// @param param
        /// 	The parameter of the Action
        ///
        public static void DelayInvoke<Type>(this Action<Type> action, Type param)
        {
            if (action != null)
            {
                GlobalDirector.WaitForFrame(() =>
                {
                    action.Invoke(param);
                });
            }
        }

        /// Invoke the Action in the end of the frame. If the Action is null, it does nothing.
        /// 
        /// @param paramA
        /// 	The first parameter of the Action
        /// @param paramB
        /// 	The second parameter of the Action
        /// @param [optional] emptyAction
        ///     Whether the action should be emptied after invoking
        ///
        public static void DelayInvoke<TypeA, TypeB>(this Action<TypeA, TypeB> action, TypeA paramA, TypeB paramB)
        {
            if (action != null)
            {
                GlobalDirector.WaitForFrame(() =>
                {
                    action.Invoke(paramA, paramB);
                });
            }
        }
        #endregion
    }
}
