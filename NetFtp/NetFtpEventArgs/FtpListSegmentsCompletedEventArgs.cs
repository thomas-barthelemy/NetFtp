using System;
using System.Collections.Generic;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    public class FtpListSegmentsCompletedEventArgs : EventArgs
    {
        public FtpListSegmentsCompletedEventArgs(IList<FtpFile> ftpFiles)
        {
            FtpFiles = ftpFiles;
        }
        public IList<FtpFile> FtpFiles { get; private set; }
    }
}