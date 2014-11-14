using System;
using System.Net;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    /// <summary>
    ///     Provides data for the FtpDownloadFileCompleted event
    /// </summary>
    public class FtpDownloadFileCompletedEventArgs : EventArgs
    {
        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransmissionState transmissionState)
        {
            TotalBytesReceived = totalBytesReceived;
            TransmissionState = transmissionState;
        }

        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransmissionState transmissionState,
            WebException webException)
        {
            TotalBytesReceived = totalBytesReceived;
            TransmissionState = transmissionState;
            WebException = webException;
        }

        /// <summary>
        ///     Gets the total number of bytes received
        /// </summary>
        public long TotalBytesReceived { get; private set; }

        /// <summary>
        ///     Gets the status of the transmission
        /// </summary>
        public TransmissionState TransmissionState { get; private set; }

        /// <summary>
        ///     Gets the WebException if an error happened during the transfer,
        ///     check exception messange and transmission state for more information.
        /// </summary>
        public WebException WebException { get; private set; }
    }
}