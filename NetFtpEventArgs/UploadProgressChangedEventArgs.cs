﻿using System;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    public class UploadProgressChangedEventArgs : EventArgs
    {
        public UploadProgressChangedEventArgs(long bytesSent, long totalBytes)
        {
            TransmissionState = TransmissionState.Uploading;
            BytesSent = bytesSent;
            TotalBytes = totalBytes;
        }

        public UploadProgressChangedEventArgs(TransmissionState transmissionState)
        {
            TransmissionState = transmissionState;
            BytesSent = 0L;
            TotalBytes = 0L;
        }

        public long BytesSent { get; private set; }

        public long TotalBytes { get; private set; }

        public int Percentage
        {
            get
            {
                return MathUtil.GetCompletionPercentage(BytesSent, TotalBytes);
            }
        }

        public TransmissionState TransmissionState { get; private set; }
    }
}