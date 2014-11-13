using System;

namespace NetFtp.Utils
{
    public class FtpFile
    {
        public DateTime? CreateTime { get; set; }

        public string CreateTimeString { get; set; }

        public string Flags { get; set; }

        public string Group { get; set; }

        public bool IsDirectory { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public long Size { get; set; }
    }
}