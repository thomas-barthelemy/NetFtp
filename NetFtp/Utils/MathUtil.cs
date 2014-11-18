using System;

namespace NetFtp.Utils
{
    /// <summary>
    ///     Provides Math related helper function
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        ///     Gets the transfer progress percentage from the current bytes
        ///     transfered and the total number of bytes to transfer.
        /// </summary>
        /// <param name="bytesTransfered">The current number of bytes transfered</param>
        /// <param name="totalBytes">The total number of bytes to transfer</param>
        /// <returns>The transfer progress percentage</returns>
        public static int GetCompletionPercentage(long bytesTransfered, long totalBytes)
        {
            return totalBytes == 0L
                ? 100
                : Convert.ToInt32(bytesTransfered/totalBytes*100L);
        }
    }
}