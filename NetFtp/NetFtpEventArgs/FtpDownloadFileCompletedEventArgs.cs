﻿using System;
using System.Net;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
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

        public FtpDownloadFileCompletedEventArgs(long totalBytesReceived,
            TransmissionState transmissionState,
            Exception exception)
        {
            TotalBytesReceived = totalBytesReceived;
            TransmissionState = transmissionState;
            Exception = exception;
        }

        public long TotalBytesReceived { get; private set; }

        public TransmissionState TransmissionState { get; set; }

        public WebException WebException { get; private set; }

        public Exception Exception { get; private set; }
    }
}