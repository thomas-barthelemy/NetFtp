using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetFtp.NetFtpEventArgs;

namespace NetFtpUnitTest
{
    [TestClass]
    public class FtpStorUnitTest
    {
        [TestMethod]
        public void Upload_ValidParameters_FileUploaded()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var localFile = new FileInfo(Path.Combine(Utils.LocalDir, Utils.LocalFile));

            var result = client.Upload(Utils.LocalDir, Utils.LocalFile, Utils.RemoteDir,
                Utils.RemoteFile);

            var fileExistsResult = client.FileExists(Utils.RemoteDir, Utils.RemoteFile);
            Assert.IsTrue(fileExistsResult.FileExists);
            Assert.AreEqual(localFile.Length, fileExistsResult.RemotefileSize);
            Assert.AreEqual(localFile.Length, result.TotalBytesSent);
        }

        [TestMethod]
        public void UploadAsync_ValidParameters_FileUploaded()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var localFile = new FileInfo(Path.Combine(Utils.LocalDir, Utils.LocalFile));

            FtpUploadFileCompletedEventArgs result = null;

            client.UploadFileCompleted += (sender, args) =>
            {
                result = args;
                Assert.IsNull(args.WebException);
                waitHandle.Set();
            };

            client.UploadAsync(Utils.LocalDir, Utils.LocalFile, Utils.RemoteDir,
                Utils.RemoteFile);

            waitHandle.WaitOne(TimeSpan.FromSeconds(15));

            var fileExistsResult = client.FileExists(Utils.RemoteDir, Utils.RemoteFile);
            Assert.IsTrue(fileExistsResult.FileExists);
            Assert.AreEqual(localFile.Length, fileExistsResult.RemotefileSize);
            Assert.AreEqual(localFile.Length, result.TotalBytesSent);
        }

        [TestMethod]
        public void Upload_WrongFileParameter_FileNotFoundException()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var localFile = new FileInfo(Path.Combine(Utils.LocalDir, Utils.InvalidLocalFile));
            if(localFile.Exists)
                Assert.Inconclusive("The InvalidLocalFile constant should not point" +
                                    " to a file that exists to test this function");

            Exception ex = null;
            try
            {
                client.Upload(Utils.LocalDir, Utils.InvalidLocalFile, Utils.RemoteDir,
                    Utils.RemoteFile);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                ex = e;
            }

            Assert.IsInstanceOfType(ex, typeof(FileNotFoundException));
        }
    }
}