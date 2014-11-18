using System;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    /// <summary>
    ///     Provides data for the FtpUploadProgressChanged event.
    /// </summary>
    public class FtpUploadProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpUploadProgressChangedEventArgs" />
        ///     with the specified number of bytes sent,
        ///     total number of bytes to send
        ///     and sets the Transmission State to "Uploading".
        /// </summary>
        /// <param name="bytesSent">
        ///     The number of bytes sent so far during the transaction.
        /// </param>
        /// <param name="totalBytes">
        ///     The total number of bytes to send.
        /// </param>
        public FtpUploadProgressChangedEventArgs(long bytesSent, long totalBytes)
        {
            TransmissionState = TransmissionState.Uploading;
            BytesSent = bytesSent;
            TotalBytes = totalBytes;
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpUploadProgressChangedEventArgs" />
        ///     with the specified transmission State.
        /// </summary>
        /// <param name="transmissionState"></param>
        public FtpUploadProgressChangedEventArgs(TransmissionState transmissionState)
        {
            TransmissionState = transmissionState;
        }

        /// <summary>
        ///     Gets the number of bytes sent so far during the transaction.
        /// </summary>
        public long BytesSent { get; private set; }

        /// <summary>
        ///     Gets the total number of bytes to send (local file size).
        /// </summary>
        public long TotalBytes { get; private set; }

        /// <summary>
        ///     Gets the progress of the transaction as a percentage.
        /// </summary>
        public int Percentage
        {
            get
            {
                return MathUtils.GetCompletionPercentage(BytesSent, TotalBytes);
            }
        }

        /// <summary>
        ///     Gets the state of the transaction
        /// </summary>
        public TransmissionState TransmissionState { get; private set; }
    }
}