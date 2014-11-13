using System.Collections.Generic;
using System.Diagnostics;
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
        public void ListFtpUnitTest()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var ftpFiles = client.ListSegments(Utils.FtpDirToList);
            CheckFtpfiles(ftpFiles);
        }

        [TestMethod]
        public void ListNullDirUnitTest()
        {
            var client = FtpClientUnitTest.GetDefaultFtpClient();

            var ftpFiles = client.ListSegments(null);

            CheckFtpfiles(ftpFiles);
        }

    }
}
