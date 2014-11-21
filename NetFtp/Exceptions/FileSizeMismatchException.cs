using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NetFtp
{
    /// <summary>
    ///     The exception that is thrown when the files size specified
    ///     for a resume (upload or download) are invalid.
    /// </summary>
    /// <remarks>
    ///     A FileSizeMismatchException will be thrown if you try to
    ///     resume a download/upload and the specified local/remote file is already
    ///     bigger than the file to download/upload.
    /// </remarks>
    [Serializable]
    public class FileSizeMismatchException : WebException
    {
        public FileSizeMismatchException() { }
        protected FileSizeMismatchException(SerializationInfo serializationInfo,
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
        public FileSizeMismatchException(string message) : base(message) { }
        public FileSizeMismatchException(string message, Exception exception)
            : base(message, exception) { }
        public FileSizeMismatchException(long remoteFileSize, long localFileSize)
        {
            RemoteFileSize = remoteFileSize;
            LocalFileSize = localFileSize;
        }

        /// <summary>
        ///     Gets the size of the remote file.
        /// </summary>
        public long RemoteFileSize { get; private set; }
        /// <summary>
        ///     Gets the size of the local file.
        /// </summary>
        public long LocalFileSize { get; private set; }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }
    }
}
