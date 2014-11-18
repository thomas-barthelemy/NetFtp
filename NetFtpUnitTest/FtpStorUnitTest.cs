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

            var localFile = new FileInfo(Utils.LocalPath);

            var result = client.Upload(Utils.LocalPath, Utils.RemotePath);

            var fileExistsResult = client.FileExists(Utils.RemotePath);
            Assert.IsTrue(fileExistsResult.FileExists);
            Assert.AreEqual(localFile.Length, fileExistsResult.RemotefileSize);
            Assert.AreEqual(localFile.Length, result.TotalBytesSent);
        }

        [TestMethod]
        public void UploadAsync_ValidParameters_FileUploaded()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var localFile = new FileInfo(Utils.LocalPath);

            FtpUploadFileCompletedEventArgs result = null;

            client.UploadFileCompleted += (sender, args) =>
            {
                result = args;
                Assert.IsNull(args.WebException);
                waitHandle.Set();
            };

            client.UploadAsync(Utils.LocalPath, Utils.RemotePath);

            waitHandle.WaitOne(TimeSpan.FromSeconds(15));

            var fileExistsResult = client.FileExists(Utils.RemotePath);
            Assert.IsTrue(fileExistsResult.FileExists);
            Assert.AreEqual(localFile.Length, fileExistsResult.RemotefileSize);
            Assert.AreEqual(localFile.Length, result.TotalBytesSent);
        }

        [TestMethod]
        public void Upload_WrongFileParameter_FileNotFoundException()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var localFile = new FileInfo(Utils.InvalidLocalPath);
            if(localFile.Exists)
                Assert.Inconclusive("The InvalidLocalFile constant should not point" +
                                    " to a file that exists to test this function");

            Exception ex = null;
            try
            {
                client.Upload(Utils.InvalidLocalPath, Utils.RemotePath);
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