using System.Net;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    /// <summary>
    ///     Provides data for the FtpFileExistsCompleted event.
    /// </summary>
    public class FtpFileExistsCompletedEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpFileExistsCompletedEventArgs" />
        ///     with the specified file exist status and size.
        /// </summary>
        /// <param name="fileExists">
        ///     Specifies if the file exists on the remote server.
        /// </param>
        /// <param name="remoteFileSize">
        ///     Specifies the remote file size (or 0 if it does not exist).
        /// </param>
        public FtpFileExistsCompletedEventArgs(bool fileExists, long remoteFileSize)
        {
            FileExists = fileExists;
            RemotefileSize = remoteFileSize;
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpFileExistsCompletedEventArgs" />
        ///     with the specified WebException.
        /// </summary>
        /// <param name="ex"></param>
        public FtpFileExistsCompletedEventArgs(WebException ex)
        {
            WebException = ex;
        }

        /// <summary>
        ///     Gets if the file exists on the remote server
        /// </summary>
        public bool FileExists { get; private set; }

        /// <summary>
        ///     Gets
        /// </summary>
        public WebException WebException { get; private set; }
        public long RemotefileSize { get; private set; }

        public TransactionState State
        {
            get
            {
                return WebException == null
                    ? TransactionState.Success
                    : TransactionState.Failed;
            }
        }
    }
}