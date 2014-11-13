namespace NetFtp.Utils
{
    public enum TransmissionState
    {
        Failed,
        Success,
        LocalFileBiggerAsRemoteFile,
        ProofingDirExits,
        CreatingDir,
        Uploading,
        Aborted
    }
}