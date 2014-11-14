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
        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpDownloadFileCompletedEventArgs" />
        ///     with the specified state and total number of bytes received.
        /// </summary>
        /// <param name="totalBytesReceived">
        ///     The total number of bytes received during the transfer.
        /// </param>
        /// <param name="transmissionState">
        ///     The state of the transfer.
        /// </param>
        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransmissionState transmissionState)
        {
            TotalBytesReceived = totalBytesReceived;
            TransmissionState = transmissionState;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpDownloadFileCompletedEventArgs" />
        ///     with the specified state and total number of bytes received.
        /// </summary>
        /// <param name="totalBytesReceived">
        ///     The total number of bytes received during the transfer.
        /// </param>
        /// <param name="transmissionState">
        ///     The state of the transfer.
        /// </param>
        /// <param name="webException">
        ///     An instance of a <see cref="System.Net.WebException"/>
        ///     that happened during the transfer (or null if no error
        ///     happened).
        /// </param>
        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransmissionState transmissionState,
            WebException webException)
        {
            TotalBytesReceived = totalBytesReceived;
            TransmissionState = transmissionState;
            WebException = webException;
        }

        /// <summary>
        ///     Gets the total number of bytes received.
        /// </summary>
        public long TotalBytesReceived { get; private set; }

        /// <summary>
        ///     Gets the status of the transmission.
        /// </summary>
        public TransmissionState TransmissionState { get; private set; }

        /// <summary>
        ///     Gets the WebException if an error happened during the transfer,
        ///     check exception messange and transmission state for more information.
        /// </summary>
        public WebException WebException { get; private set; }
    }
}