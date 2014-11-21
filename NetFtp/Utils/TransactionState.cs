namespace NetFtp.Utils
{
    /// <summary>
    ///     Represents the state of a transaction.
    /// </summary>
    public enum TransactionState
    {
        /// <summary>
        ///     The transaction terminated unexpectedly. 
        /// </summary>
        Failed,
        /// <summary>
        ///     The transaction terminated successfully.
        /// </summary>
        Success,
        /// <summary>
        ///     The FTP Client is checking if the specified remote directory exists.
        /// </summary>
        ProofingDirExits,
        /// <summary>
        ///     The FTP Client is creating a necessary remote directory.
        /// </summary>
        CreatingDir,
        /// <summary>
        ///     The upload transaction is active and in progress.
        /// </summary>
        Uploading,
        /// <summary>
        ///     The transaction terminated due to an abort request.
        /// </summary>
        Aborted
    }
}