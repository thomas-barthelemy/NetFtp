using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool = NetFtp.Utils.ThreadPool;

namespace NetFtpUnitTest
{
    [TestClass]
    public class ThreadPoolUnitTest
    {
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private readonly ThreadPool _threadPool = new ThreadPool();

        [TestMethod]
        public void StartNewThread_ValidParams_Success()
        {
            var worked = false;
            var uid = _threadPool.StartNewthread(string.Empty,
                () =>
                {
                    worked = true;
                    _resetEvent.Set();
                });

            Assert.IsNotNull(uid);
            _resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(worked);
            Assert.IsTrue(_threadPool.GetThreads().Count == 0);
        }

        [TestMethod]
        public void AbortThread_ValidParams_Aborted()
        {
            var isAborted = false;
            var thread = _threadPool.StartNewthread(string.Empty,
                () => FakeThread(ref isAborted));
            _threadPool.AbortThread(thread);
            Assert.IsNotNull(thread);
            Assert.IsTrue(isAborted);
        }

        [TestMethod]
        public void AbortAll_MultipleThreads_Aborted()
        {
            var thread1Aborted = false;
            var thread2Aborted = false;
            var thread3Aborted = false;

            _threadPool.StartNewthread(string.Empty,
                () => FakeThread(ref thread1Aborted));
            _threadPool.StartNewthread(string.Empty,
                () => FakeThread(ref thread2Aborted));
            _threadPool.StartNewthread(string.Empty,
                () => FakeThread(ref thread3Aborted));

            _threadPool.AbortAllThreads();
            Assert.IsTrue(thread1Aborted);
            Assert.IsTrue(thread2Aborted);
            Assert.IsTrue(thread3Aborted);
            Assert.IsTrue(_threadPool.GetThreads().Count == 0);
        }

        [TestMethod]
        public void AbortThread_Concurrent_Aborted()
        {
            var aborted = false;
            var id = _threadPool.StartNewthread(string.Empty,
                () => FakeThread(ref aborted));
            
            for (var i = 0; i < 10; i++)
            {
                _threadPool.StartNewthread(string.Empty, () =>
                    _threadPool.AbortThread(id));
            }

            Assert.IsTrue(aborted);
        }

        private static void FakeThread(ref bool isAborted)
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Assert.Fail("Should be aborted before the end");
            }
            catch (ThreadAbortException)
            {
                isAborted = true;
            }
        }
    }
}