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
        /// <param name="transactionState">
        ///     The state of the transfer.
        /// </param>
        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransactionState transactionState)
        {
            TotalBytesReceived = totalBytesReceived;
            TransactionState = transactionState;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpDownloadFileCompletedEventArgs" />
        ///     with the specified state and total number of bytes received.
        /// </summary>
        /// <param name="totalBytesReceived">
        ///     The total number of bytes received during the transfer.
        /// </param>
        /// <param name="transactionState">
        ///     The state of the transfer.
        /// </param>
        /// <param name="webException">
        ///     An instance of a <see cref="System.Net.WebException"/>
        ///     that happened during the transfer (or null if no error
        ///     happened).
        /// </param>
        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransactionState transactionState,
            WebException webException)
        {
            TotalBytesReceived = totalBytesReceived;
            TransactionState = transactionState;
            WebException = webException;
        }

        /// <summary>
        ///     Gets the total number of bytes received.
        /// </summary>
        public long TotalBytesReceived { get; private set; }

        /// <summary>
        ///     Gets the status of the transmission.
        /// </summary>
        public TransactionState TransactionState { get; private set; }

        /// <summary>
        ///     Gets the WebException if an error happened during the transfer,
        ///     check exception message and transmission state for more information.
        /// </summary>
        public WebException WebException { get; private set; }
    }
}