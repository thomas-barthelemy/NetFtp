using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetFtp.Utils;

namespace NetFtpUnitTest
{
    [TestClass]
    public class FtpListUnitTest
    {

        #region Helper Methods

        private static void CheckFtpfiles(ICollection<FtpFile> files)
        {
            Assert.IsNotNull(files);
            Debug.WriteLine("Files Found: " + files.Count);

            foreach (var ftpFile in files)
            {
                Assert.IsNotNull(ftpFile);
                Assert.IsFalse(string.IsNullOrEmpty(ftpFile.Name));

                Debug.WriteLine(ftpFile.Name);
            }
        }

        #endregion

        [TestMethod]
        public void ListSegments_ValidParams_ListOfRemoteFiles()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var ftpFiles = client.ListSegments(Utils.FtpDirToList);
            CheckFtpfiles(ftpFiles);
        }

        [TestMethod]
        public void ListSegments_NullDirParam_ListRootRemoteFiles()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var ftpFiles = client.ListSegments(null);

            CheckFtpfiles(ftpFiles);
        }

        [TestMethod]
        public void ListSegmentsAsync_ValidParams_ListOfRemoteFiles()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();
            IList<FtpFile> result = null;

            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            client.ListSegmentsCompleted += (sender, args) =>
            {
                result = args.FtpFiles;
                waitHandle.Set();
            };
            client.ListSegmentsAsync(Utils.FtpDirToList);
            waitHandle.WaitOne(TimeSpan.FromSeconds(15));

            CheckFtpfiles(result);
        }

    }
}
