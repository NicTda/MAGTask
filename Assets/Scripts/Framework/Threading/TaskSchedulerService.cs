//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WEBGL == false
using System.Threading;
#endif

namespace CoreFramework
{
    /// Provides the means to schedule tasks on a background thread or the 
    /// main thread. Also allows for creation of coroutines from classes that do
    /// not inherit from MonoBehaviour.
    /// 
    /// This is thread safe, though it must be constructed on the main thread.
    /// 
    public sealed class TaskSchedulerService : Service
    {
		private ITaskHandler m_taskHandler;

		/// Private interface for different platform implementations
		/// 
		private interface ITaskHandler
		{
			Queue<Action> m_queuedTasks { get; set; }

			/// Update
			/// 
			void Update();

			/// @return Whether or not the current thread is the main thread.
			/// 
			bool IsMainThread();

			/// Schedules a new task on a background thread.
			/// 
			/// @param task
			/// 	The task that should be executed on a background thread.
			/// 
			void ScheduleBackgroundTask(Action task);

			/// Schedules a new task on the main thread. The task will be executed during the
			/// next update.
			/// 
			/// @param task
			/// 	The task that should be executed on the main thread.
			/// 
			void ScheduleMainThreadTask(Action task);
		}

        #region Service functions
        /// Update
        /// 
        public override void ServiceUpdate()
        {
			m_taskHandler.Update();
        }
        #endregion

        #region Public functions
        /// Constructor
        /// 
        public TaskSchedulerService()
        {
#if UNITY_WEBGL
			m_taskHandler = new TaskHandlerImmediate();
#else
			m_taskHandler = new TaskHandlerThreaded();
#endif
        }

        /// @return Whether or not the current thread is the main thread.
        /// 
        public bool IsMainThread()
        {
			return m_taskHandler.IsMainThread();
        }

        /// Schedules a new task on a background thread.
        /// 
        /// @param task
        /// 	The task that should be executed on a background thread.
        /// 
        public void ScheduleBackgroundTask(Action task)
        {
			m_taskHandler.ScheduleBackgroundTask(task);
        }

        /// Schedules a new task on the main thread. The task will be executed during the
        /// next update.
        /// 
        /// @param task
        /// 	The task that should be executed on the main thread.
        /// 
        public void ScheduleMainThreadTask(Action task)
        {
			m_taskHandler.ScheduleMainThreadTask(task);
        }
        #endregion

		#region Private classes
#if UNITY_WEBGL
		/// Private implementation of an immediate task handler
		/// 
		private class TaskHandlerImmediate : ITaskHandler
		{
			public Queue<Action> m_queuedTasks { get; set; }

			#region Service functions
			/// Update
			/// 
			public void Update()
			{
				Queue<Action> taskQueue = null;

				taskQueue = new Queue<Action>(m_queuedTasks);
				m_queuedTasks.Clear();

				foreach(Action action in taskQueue)
				{
					action();
				}
			}
			#endregion

			#region Public functions
			/// Constructor
			/// 
			public TaskHandlerImmediate()
			{
				m_queuedTasks = new Queue<Action>();
			}

			/// @return Whether or not the current thread is the main thread.
			/// 
			public bool IsMainThread()
			{
				return true;
			}

			/// Schedules a new task on a background thread.
			/// 
			/// @param task
			/// 	The task that should be executed on a background thread.
			/// 
			public void ScheduleBackgroundTask(Action task)
			{
				ScheduleMainThreadTask(task);
			}

			/// Schedules a new task on the main thread. The task will be executed during the
			/// next update.
			/// 
			/// @param task
			/// 	The task that should be executed on the main thread.
			/// 
			public void ScheduleMainThreadTask(Action task)
			{
				Debug.Assert(task != null, "A scheduled task should not be null.");
				m_queuedTasks.Enqueue(task);
			}
			#endregion
		}
#else
		/// Private implementation of a threaded task handler
		/// 
		private class TaskHandlerThreaded : ITaskHandler
		{
			public Queue<Action> m_queuedTasks { get; set; }

			private object m_lock = new object();
			private Thread m_mainThread = null;

			#region Service functions
			/// Update
			/// 
			public void Update()
			{
				Queue<Action> taskQueue = null;

				Debug.Assert(IsMainThread(), "Update must be run from the main thread.");
				lock(m_lock)
				{
					taskQueue = new Queue<Action>(m_queuedTasks);
					m_queuedTasks.Clear();
				}

				foreach(Action action in taskQueue)
				{
					action();
				}
			}
			#endregion

			#region Public functions
			/// Constructor
			/// 
			public TaskHandlerThreaded()
			{
				m_queuedTasks = new Queue<Action>();
				m_mainThread = System.Threading.Thread.CurrentThread;
				Debug.Assert(m_mainThread != null, "Failed to initialise Task Scheduler.");
			}

			/// @return Whether or not the current thread is the main thread.
			/// 
			public bool IsMainThread()
			{
				return (m_mainThread == System.Threading.Thread.CurrentThread);
			}

			/// Schedules a new task on a background thread.
			/// 
			/// @param task
			/// 	The task that should be executed on a background thread.
			/// 
			public void ScheduleBackgroundTask(Action task)
			{
				Debug.Assert(task != null, "A scheduled task should not be null.");

				ThreadPool.QueueUserWorkItem((object state) =>
				{
					task();
				});
			}

			/// Schedules a new task on the main thread. The task will be executed during the
			/// next update.
			/// 
			/// @param task
			/// 	The task that should be executed on the main thread.
			/// 
			public void ScheduleMainThreadTask(Action task)
			{
				Debug.Assert(task != null, "A scheduled task should not be null.");

				lock(m_lock)
				{
					m_queuedTasks.Enqueue(task);
				}
			}
		#endregion
		}
#endif
	#endregion
    }
}
