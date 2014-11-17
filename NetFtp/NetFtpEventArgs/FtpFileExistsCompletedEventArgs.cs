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
        ///     with the specified state and total number of bytes received.
        /// </summary>
        /// <param name="fileExists"></param>
        /// <param name="remoteFileSize"></param>
        public FtpFileExistsCompletedEventArgs(bool fileExists, long remoteFileSize)
        {
            FileExists = fileExists;
            RemotefileSize = remoteFileSize;

        }

        public FtpFileExistsCompletedEventArgs(WebException ex)
        {
            Exception = ex;
        }

        public bool FileExists { get; private set; }
        public WebException Exception { get; private set; }
        public long RemotefileSize { get; private set; }

        public TransmissionState State
        {
            get
            {
                return Exception == null
                    ? TransmissionState.Success
                    : TransmissionState.Failed;
            }
        }
    }
}