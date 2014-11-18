
namespace NetFtp.Utils
{
    /// <summary>
    ///     Provides constant values to be used to identify threads.
    /// </summary>
    static class ThreadNames
    {
        /// <summary>
        ///     Represents the name used by the threads performing an async STOR.
        /// </summary>
        internal const string UploadThreadName = "NetFtp_STOR_Thread";

        /// <summary>
        ///     Represents the name used by the threads performing an async LIST.
        /// </summary>
        internal const string ListThreadName = "NetFtp_LIST_Thread";

        /// <summary>
        ///     Represents the name used by the threads performing an async RETR.
        /// </summary>
        internal const string DownloadThreadName = "NetFtp_RETR_Thread";
    }
}
