using System;
using System.Net;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    /// <summary>
    ///     Provides data for the FtpUploadFileCompleted event.
    /// </summary>
    public class FtpUploadFileCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpUploadFileCompletedEventArgs" />
        ///     with the specified transmission state
        ///     and total bytes sent.
        /// </summary>
        /// <param name="totalBytesSent">
        ///     The total number of bytes sent during the transaction.
        /// </param>
        /// <param name="transactionState">
        ///     The state of the transaction.
        /// </param>
        public FtpUploadFileCompletedEventArgs(long totalBytesSent,
            TransactionState transactionState)
        {
            TotalBytesSent = totalBytesSent;
            TransactionState = transactionState;
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpUploadFileCompletedEventArgs" />
        ///     with the specified transmission state, total bytes sent,
        ///     and WebException.
        /// </summary>
        /// <param name="totalBytesSent">
        ///     The total number of bytes sent during the transaction.
        /// </param>
        /// <param name="transactionState">
        ///     The state of the transaction.
        /// </param>
        /// <param name="webException">
        ///     The WebException that happened during the transaction.
        /// </param>
        public FtpUploadFileCompletedEventArgs(long totalBytesSent,
            TransactionState transactionState,
            WebException webException)
        {
            TotalBytesSent = totalBytesSent;
            TransactionState = transactionState;
            WebException = webException;
        }

        /// <summary>
        ///     Gets the total number of bytes sent during the transaction.
        /// </summary>
        public long TotalBytesSent { get; private set; }

        /// <summary>
        ///     Gets the state of the transaction.
        /// </summary>
        public TransactionState TransactionState { get; private set; }

        /// <summary>
        ///     Gets the exception that happened during the transaction,
        ///     will return null if no exception happened.
        /// </summary>
        public WebException WebException { get; private set; }
    }
}