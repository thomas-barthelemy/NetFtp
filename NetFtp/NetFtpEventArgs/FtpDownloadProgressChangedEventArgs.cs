using System;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    public class FtpDownloadProgressChangedEventArgs : EventArgs
    {
        public FtpDownloadProgressChangedEventArgs(long bytesReceived,
            long totalBytes)
        {
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        public long BytesReceived { get; private set; }

        public long TotalBytes { get; private set; }

        public int Percent
        {
            get { return MathUtil.GetCompletionPercentage(BytesReceived, TotalBytes); }
        }
    }
}