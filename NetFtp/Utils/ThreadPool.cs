using System;
using System.Collections.Generic;
using System.Threading;

namespace NetFtp.Utils
{
    internal class ThreadPool
    {
        private readonly IDictionary<Guid, Thread> _threads
            = new Dictionary<Guid, Thread>();

        internal Thread GetThread(Guid uid)
        {
            return _threads.ContainsKey(uid) ? _threads[uid] : null;
        }

        internal ICollection<Thread> GetThreads()
        {
            return _threads.Values;
        }

        internal Thread StartNewthread(string threadName, ThreadStart threadStart)
        {
            var uid = Guid.NewGuid();

            var thread = new Thread(() =>
            {
                threadStart.Invoke();
                ClearThread(uid);
            })
            {
                IsBackground = true,
                Name = threadName,
                Priority = ThreadPriority.Normal
            };

            _threads.Add(uid, thread);
            thread.Start();

            return thread;
        }

        internal void AbortThread(Thread thread)
        {
            if (thread == null) return;

            thread.Abort();

            Guid? toDelete = null;
            foreach (var item in _threads)
            {
                if (item.Value != thread)
                    continue;
                
                toDelete = item.Key;
                break;
            }

            if (toDelete.HasValue)
                _threads.Remove(toDelete.Value);
        }

        internal void AbortThread(Guid uid)
        {
            if (!_threads.ContainsKey(uid)) return;

            var thread = _threads[uid];
            if (thread == null || !thread.IsAlive)
            {
                _threads.Remove(uid);
                return;
            }

            thread.Abort();
            _threads.Remove(uid);
        }

        internal void AbortAllThreads()
        {
            var keysToAbort = new List<Guid>(_threads.Keys);

            foreach (var keyToAbort in keysToAbort)
            {
                AbortThread(keyToAbort);
            }
        }

        private void ClearThread(Guid uid)
        {
            if (_threads.ContainsKey(uid))
                _threads.Remove(uid);
        }
    }
}
