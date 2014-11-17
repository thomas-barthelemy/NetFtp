using System;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    /// <summary>
    ///     Provides data for the FtpDownloadProgressChanged event
    /// </summary>
    public class FtpDownloadProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpDownloadProgressChangedEventArgs" />
        ///     with the specified state and total number of bytes received.
        /// </summary>
        /// <param name="bytesReceived">
        ///     The number of bytes received so far during the transaction.
        /// </param>
        /// <param name="totalBytes">
        ///     The total number of bytes of the file being downloaded.
        /// </param>
        public FtpDownloadProgressChangedEventArgs(long bytesReceived,
            long totalBytes)
        {
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        /// <summary>
        ///     Gets the number of bytes received so far during the transaction.
        /// </summary>
        public long BytesReceived { get; private set; }

        /// <summary>
        ///     Gets the total number of bytes of the file being downloaded.
        /// </summary>
        public long TotalBytes { get; private set; }

        /// <summary>
        ///     Gets the download progress as a percentage.
        /// </summary>
        public int Percent
        {
            get { return MathUtil.GetCompletionPercentage(BytesReceived, TotalBytes); }
        }
    }
}