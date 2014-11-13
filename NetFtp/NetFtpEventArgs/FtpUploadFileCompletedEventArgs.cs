using System;
using System.Net;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    public class FtpUploadFileCompletedEventArgs : EventArgs
    {
        public FtpUploadFileCompletedEventArgs(long totalBytesSent,
            TransmissionState transmissionState)
        {
            TotalBytesSent = totalBytesSent;
            TransmissionState = transmissionState;
            WebException = null;
            Exception = null;
        }

        public FtpUploadFileCompletedEventArgs(long totalBytesSent,
            TransmissionState transmissionState,
            WebException webException)
        {
            TotalBytesSent = totalBytesSent;
            TransmissionState = transmissionState;
            WebException = webException;
            Exception = null;
        }

        public FtpUploadFileCompletedEventArgs(long totalBytesSent,
            TransmissionState transmissionState,
            Exception exception)
        {
            TotalBytesSent = totalBytesSent;
            TransmissionState = transmissionState;
            WebException = null;
            Exception = exception;
        }

        public long TotalBytesSent { get; private set; }

        public TransmissionState TransmissionState { get; set; }

        public WebException WebException { get; private set; }

        public Exception Exception { get; private set; }
    }
}