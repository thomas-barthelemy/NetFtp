using System;
using System.Collections.Generic;
using NetFtp.Utils;

namespace NetFtp.NetFtpEventArgs
{
    /// <summary>
    ///     Provides data for the FtpListSegmentsCompleted event.
    /// </summary>
    public class FtpListSegmentsCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="FtpListSegmentsCompletedEventArgs" />
        ///     with the specified ftp files list.
        /// </summary>
        /// <param name="ftpFiles">
        ///     The list of FTP files available
        ///     at the specified remote location.
        /// </param>
        public FtpListSegmentsCompletedEventArgs(IList<FtpFile> ftpFiles)
        {
            FtpFiles = ftpFiles;
        }

        /// <summary>
        ///     Gets the list of FTP files available
        ///     at the specified remote location.
        /// </summary>
        public IList<FtpFile> FtpFiles { get; private set; }
    }
}