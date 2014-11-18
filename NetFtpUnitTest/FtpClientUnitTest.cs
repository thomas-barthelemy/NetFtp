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
        public static FtpClient GetDefaultFtpClient()
        {
            var client = new FtpClient(Utils.FtpHost, Utils.FtpUserName, Utils.FtpPassword, Utils.FtpPort);
            return client;
        }

        [TestMethod]
        public void FtpClientConstructor_ValidParams_NoException()
        {
            GetDefaultFtpClient();
        }

        [TestMethod]
        public void FtpClientConstructor_NoHost_UriFormatException()
        {
            Exception result = null;
            try
            {
                var client = new FtpClient(string.Empty, Utils.FtpUserName, Utils.FtpPassword, Utils.FtpPort);
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
        public void FtpClientConstructor_WrongHost_WebExceptionWithConnectFailure()
        {
            WebException result = null;
            try
            {
                var client = new FtpClient("0.0.0.0", Utils.FtpUserName, Utils.FtpPassword, Utils.FtpPort);
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
        public void FtpClientConstructor_WrongUsername_WebExceptionWithProtocolError()
        {
            WebException wrongUserResult = null;
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

            Assert.IsNotNull(wrongUserResult);
            Assert.AreEqual(WebExceptionStatus.ProtocolError, wrongUserResult.Status);
        }

        [TestMethod]
        public void FtpClientConstructor_WrongPassword_WebExceptionWithProtocolError()
        {
            WebException wrongPassResult = null;

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

            Assert.IsNotNull(wrongPassResult);
            Assert.AreEqual(WebExceptionStatus.ProtocolError, wrongPassResult.Status);
        }
    }
}
