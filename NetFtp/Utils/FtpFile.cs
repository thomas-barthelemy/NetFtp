using System;

namespace NetFtp.Utils
{
    /// <summary>
    ///     Represents a file on a remote FTP
    /// </summary>
    public class FtpFile
    {
        /// <summary>
        ///     Gets or Sets the creation date of the FTP File.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        ///     Gets or Sets the creation date as a string,
        ///     formated as in remote LIST result.
        /// </summary>
        public string CreateTimeString { get; set; }

        /// <summary>
        ///     Gets or Sets the FTP file flags.
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        ///     Gets or Sets the FTP file Group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     Gets or Sets if the FTP File is a Directory.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        ///     Gets or Sets the FTP File name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or Sets the FTP File Owner.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        ///     Gets or Sets the FTP File size (number of bytes).
        /// </summary>
        public long Size { get; set; }
    }
}