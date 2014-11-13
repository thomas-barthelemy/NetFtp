namespace NetFtp.Utils
{
    public enum TransmissionState
    {
        Success,
        Failed,
        LocalFileBiggerAsRemoteFile,
        ProofingDirExits,
        CreatingDir,
        Uploading,
    }
}