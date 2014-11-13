using System;

namespace NetFtp.Utils
{
    public static class MathUtil
    {
        public static int GetCompletionPercentage(long bytesSent, long totalBytes)
        {
            return totalBytes == 0L
                ? 100
                : Convert.ToInt32(bytesSent/totalBytes*100L);
        }
    }
}