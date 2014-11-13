using System;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    public class FtpFileExistsCompletedEventArgs
    {
        public bool FileExists { get; set; }
        public Exception Exception { get; set; }
        public long RemotefileSize { get; set; }

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