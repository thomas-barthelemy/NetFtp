using System;
using System.Net;

namespace NetFtp
{
    /// <summary>
    ///     The exception that is thrown when the files size specified
    ///     for a resume (upload or download) are invalid.
    /// </summary>
    /// <remarks>
    ///     A FileSizeMismatchException will be thrown if you try to
    ///     resume a download/upload and the specified local/remote file is already
    ///     bigger than the file to download/upload.
    /// </remarks>
    public class FileSizeMismatchException : WebException
    {
        public FileSizeMismatchException(long remoteFileSize, long localFileSize)
        {
            RemoteFileSize = remoteFileSize;
            LocalFileSize = localFileSize;
        }

        /// <summary>
        ///     Gets the size of the remote file.
        /// </summary>
        public long RemoteFileSize { get; private set; }
        /// <summary>
        ///     Gets the size of the local file.
        /// </summary>
        public long LocalFileSize { get; private set; }
    }
}
