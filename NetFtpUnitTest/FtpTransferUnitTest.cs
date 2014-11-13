using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetFtp;

namespace NetFtpUnitTest
{
    [TestClass]
    public class FtpTransferUnitTest
    {
        private const string FtpUserName = "admin";
        private const string FtpPassword = "password";
        private const string FtpHost = "127.0.0.1";
        private const int FtpPort = 21;

        private const string FtpDirToList = "/";

        [TestMethod]
        public FtpClient CreateFtpClient()
        {
            var client = new FtpClient(FtpHost, FtpUserName, FtpPassword, FtpPort);
            return client;
        }


        [TestMethod]
        public void ListFtpUnitTest()
        {
            var client = CreateFtpClient();

            var ftpFiles = client.ListSegments(FtpDirToList);
            Assert.IsNotNull(ftpFiles);
        }
    }
}
