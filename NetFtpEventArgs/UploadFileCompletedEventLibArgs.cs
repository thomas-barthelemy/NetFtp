using System;
using System.Net;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    public class UploadFileCompletedEventArgs : EventArgs
    {
        public UploadFileCompletedEventArgs(long totalBytesSend,
            TransmissionState transmissionState)
        {
            TotalBytesSend = totalBytesSend;
            TransmissionState = transmissionState;
            WebException = null;
            Exception = null;
        }

        public UploadFileCompletedEventArgs(long totalBytesSend,
            TransmissionState transmissionState,
            WebException webException)
        {
            TotalBytesSend = totalBytesSend;
            TransmissionState = transmissionState;
            WebException = webException;
            Exception = null;
        }

        public UploadFileCompletedEventArgs(long totalBytesSend,
            TransmissionState transmissionState,
            Exception exception)
        {
            TotalBytesSend = totalBytesSend;
            TransmissionState = transmissionState;
            WebException = null;
            Exception = exception;
        }

        public long TotalBytesSend { get; private set; }

        public TransmissionState TransmissionState { get; set; }

        public WebException WebException { get; private set; }

        public Exception Exception { get; private set; }
    }
}