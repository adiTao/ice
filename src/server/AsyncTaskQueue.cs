using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Demo
{
    /// <summary>
    /// 非同步任務隊列
    /// </summary>
    public class AsyncTaskQueue : IDisposable
    {
        private bool _isDisposed;
        private readonly ConcurrentQueue<AwaitableTask> _queue = new ConcurrentQueue<AwaitableTask>();
        private Thread _thread;
        private AutoResetEvent _autoResetEvent;

        /// <summary>
        /// 非同步任務隊列
        /// </summary>
        public AsyncTaskQueue()
        {
            _autoResetEvent = new AutoResetEvent(false);
            _thread = new Thread(InternalRuning) { IsBackground = true };
            _thread.Start();
        }

        private bool TryGetNextTask(out AwaitableTask task)
        {
            task = null;
            while (_queue.Count > 0)
            {
                if (_queue.TryDequeue(out task) && (!AutoCancelPreviousTask || _queue.Count == 0)) return true;
                task.Cancel();
            }
            return false;
        }

        private AwaitableTask PenddingTask(AwaitableTask task)
        {
            lock (_queue)
            {
                Debug.Assert(task != null);
                _queue.Enqueue(task);
                _autoResetEvent.Set();
            }
            return task;
        }

        private void InternalRuning()
        {
            while (!_isDisposed)
            {
                if (_queue.Count == 0)
                {
                    _autoResetEvent.WaitOne();
                }
                while (TryGetNextTask(out var task))
                {
                    if (task.IsCancel) continue;

                    if (UseSingleThread)
                    {
                        task.RunSynchronously();
                    }
                    else
                    {
                        task.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 是否使用單線程完成任務.
        /// </summary>
        public bool UseSingleThread { get; set; } = true;

        /// <summary>
        /// 自動取消以前的任務。
        /// </summary>
        public bool AutoCancelPreviousTask { get; set; } = false;

        /// <summary>
        /// 執行任務
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public AwaitableTask Run(Action action)
            => PenddingTask(new AwaitableTask(new Task(action, new CancellationToken(false))));

        /// <summary>
        /// 執行任務
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public AwaitableTask<TResult> Run<TResult>(Func<TResult> function)
            => (AwaitableTask<TResult>)PenddingTask(new AwaitableTask<TResult>(new Task<TResult>(function)));


        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析構任務隊列
        /// </summary>
        ~AsyncTaskQueue() => Dispose(false);

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                _autoResetEvent.Dispose();
            }
            _thread = null;
            _autoResetEvent = null;
            _isDisposed = true;
        }

        /// <summary>
        /// 可等待的任務
        /// </summary>
        public class AwaitableTask
        {
            private readonly Task _task;

            /// <summary>
            /// 初始化可等待的任務。
            /// </summary>
            /// <param name="task"></param>
            public AwaitableTask(Task task) => _task = task;

            /// <summary>
            /// 任務的Id
            /// </summary>
            public int TaskId => _task.Id;

            /// <summary>
            /// 任務是否取消
            /// </summary>
            public bool IsCancel { get; private set; }

            /// <summary>
            /// 開始任務
            /// </summary>
            public void Start() => _task.Start();

            /// <summary>
            /// 同步執行開始任務
            /// </summary>
            public void RunSynchronously() => _task.RunSynchronously();

            /// <summary>
            /// 取消任務
            /// </summary>
            public void Cancel() => IsCancel = true;

            /// <summary>
            /// 獲取任務等待器
            /// </summary>
            /// <returns></returns>
            public TaskAwaiter GetAwaiter() => new TaskAwaiter(this);

            /// <summary>Provides an object that waits for the completion of an asynchronous task. </summary>
            [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
            public struct TaskAwaiter : INotifyCompletion
            {
                private readonly AwaitableTask _task;

                /// <summary>
                /// 任務等待器
                /// </summary>
                /// <param name="awaitableTask"></param>
                public TaskAwaiter(AwaitableTask awaitableTask) => _task = awaitableTask;

                /// <summary>
                /// 任務是否完成.
                /// </summary>
                public bool IsCompleted => _task._task.IsCompleted;

                /// <inheritdoc />
                public void OnCompleted(Action continuation)
                {
                    var This = this;
                    _task._task.ContinueWith(t =>
                    {
                        if (!This._task.IsCancel) continuation?.Invoke();
                    });
                }
                /// <summary>
                /// 獲取任務結果
                /// </summary>
                public void GetResult() => _task._task.Wait();
            }
        }

        /// <summary>
        /// 可等待的任務
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        public class AwaitableTask<TResult> : AwaitableTask
        {
            /// <summary>
            /// 初始化可等待的任務
            /// </summary>
            /// <param name="task">需要執行的任務</param>
            public AwaitableTask(Task<TResult> task) : base(task) => _task = task;


            private readonly Task<TResult> _task;

            /// <summary>
            /// 獲取任務等待器
            /// </summary>
            /// <returns></returns>
            public new TaskAwaiter GetAwaiter() => new TaskAwaiter(this);

            /// <summary>
            /// 任務等待器
            /// </summary>
            [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
            public new struct TaskAwaiter : INotifyCompletion
            {
                private readonly AwaitableTask<TResult> _task;

                /// <summary>
                /// 初始化任務等待器
                /// </summary>
                /// <param name="awaitableTask"></param>
                public TaskAwaiter(AwaitableTask<TResult> awaitableTask) => _task = awaitableTask;

                /// <summary>
                /// 任務是否已完成。
                /// </summary>
                public bool IsCompleted => _task._task.IsCompleted;

                /// <inheritdoc />
                public void OnCompleted(Action continuation)
                {
                    var This = this;
                    _task._task.ContinueWith(t =>
                    {
                        if (!This._task.IsCancel) continuation?.Invoke();
                    });
                }

                /// <summary>
                /// 獲取任務結果。
                /// </summary>
                /// <returns></returns>
                public TResult GetResult() => _task._task.Result;
            }
        }
    }
}