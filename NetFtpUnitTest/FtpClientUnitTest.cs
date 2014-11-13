using System;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetFtp;

namespace NetFtpUnitTest
{
    [TestClass]
    public class FtpClientUnitTest
    {
        [TestMethod]
        public void FtpClientFailHostString()
        {
            Exception result = null;
            try
            {
                var client = new FtpClient("", Utils.FtpUserName, Utils.FtpPassword, Utils.FtpPort);
                client.ListSegments(Utils.FtpDirToList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                result = ex;
            }

            Assert.IsInstanceOfType(result, typeof(UriFormatException));
        }

        [TestMethod]
        public void FtpClientUnreachableHost()
        {
            WebException result = null;
            try
            {
                var client = new FtpClient("", Utils.FtpUserName, Utils.FtpPassword, Utils.FtpPort);
                client.ListSegments(Utils.FtpDirToList);
            }
            catch (WebException ex)
            {
                Debug.WriteLine(ex);
                result = ex;
            }

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Status == WebExceptionStatus.ConnectFailure);
        }

        [TestMethod]
        public void FtpClientWrongUsername()
        {
            WebException wrongUserResult = null;
            WebException wrongPassResult = null;
            try
            {
                var client = new FtpClient(Utils.FtpHost, Utils.FtpWrongUsername, Utils.FtpPassword, Utils.FtpPort);
                client.ListSegments(Utils.FtpDirToList);
            }
            catch (WebException ex)
            {
                Debug.WriteLine(ex);
                wrongUserResult = ex;
            }

            try
            {
                var client = new FtpClient(Utils.FtpHost, Utils.FtpUserName, Utils.FtpWrongPassword, Utils.FtpPort);
                client.ListSegments(Utils.FtpDirToList);
            }
            catch (WebException ex)
            {
                Debug.WriteLine(ex);
                wrongPassResult = ex;
            }

            Assert.IsNotNull(wrongUserResult);
            Assert.IsNotNull(wrongPassResult);

            Assert.AreEqual(WebExceptionStatus.ProtocolError, wrongUserResult.Status);
            Assert.AreEqual(WebExceptionStatus.ProtocolError, wrongPassResult.Status);
        }
    }
}
