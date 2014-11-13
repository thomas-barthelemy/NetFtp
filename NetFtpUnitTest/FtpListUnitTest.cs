using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetFtp;

namespace NetFtpUnitTest
{
    [TestClass]
    public class FtpListUnitTest
    {

        [TestMethod]
        public FtpClient CreateFtpClient()
        {
            var client = new FtpClient(Utils.FtpHost, Utils.FtpUserName, Utils.FtpPassword, Utils.FtpPort);
            return client;
        }


        [TestMethod]
        public void ListFtpUnitTest()
        {
            var client = CreateFtpClient();

            var ftpFiles = client.ListSegments(Utils.FtpDirToList);
            Assert.IsNotNull(ftpFiles);

            Debug.WriteLine("Files Found: " + ftpFiles.Count);

            foreach (var ftpFile in ftpFiles)
            {
                Assert.IsNotNull(ftpFile);
                Assert.IsFalse(string.IsNullOrEmpty(ftpFile.Name));

                Debug.WriteLine(ftpFile.Name);
            }
        }
    }
}
