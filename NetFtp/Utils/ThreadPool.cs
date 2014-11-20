using System;
using System.Collections.Generic;
using System.Threading;

namespace NetFtp.Utils
{
    public class ThreadPool
    {
        private static readonly IDictionary<Guid, Thread> Threads
            = new Dictionary<Guid, Thread>();

        internal static Thread GetThread(Guid uid)
        {
            return Threads.ContainsKey(uid) ? Threads[uid] : null;
        }

        internal static ICollection<Thread> GetThreads()
        {
            return Threads.Values;
        }

        internal static Guid StartNewthread(string threadName, ThreadStart threadStart)
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

            Threads.Add(uid, thread);
            thread.Start();

            return uid;
        }

        internal static void AbortThread(Guid uid)
        {
            if (!Threads.ContainsKey(uid)) return;

            var thread = Threads[uid];
            if (thread == null || !thread.IsAlive)
            {
                Threads.Remove(uid);
                return;
            }

            thread.Abort();
            Threads.Remove(uid);
        }

        internal static void AbortAll()
        {
            var keysToAbort = new List<Guid>(Threads.Keys);

            foreach (var keyToAbort in keysToAbort)
            {
                AbortThread(keyToAbort);
            }
        }

        private static void ClearThread(Guid uid)
        {
            if (Threads.ContainsKey(uid))
                Threads.Remove(uid);
        }
    }
}
