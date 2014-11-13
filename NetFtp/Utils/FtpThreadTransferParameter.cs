namespace NetFtp.Utils
{
    internal class FtpThreadTransferParameters
    {
        internal FtpThreadTransferParameters(string remoteDirectory)
        {
            RemoteDirectory = remoteDirectory;
        }

        internal FtpThreadTransferParameters(string localDirectory, string localFileName, string remoteDirectory,
            string remoteFileName)
        {
            LocalDirectory = localDirectory;
            LocalFilename = localFileName;
            RemoteDirectory = remoteDirectory;
            RemoteFilename = remoteFileName;
        }

        internal string LocalDirectory { get; set; }

        internal string LocalFilename { get; set; }

        internal string RemoteDirectory { get; set; }

        internal string RemoteFilename { get; set; }
    }
}
