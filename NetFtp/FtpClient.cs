using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using NetFtp.NetFtpEventArgs;
using NetFtp.Utils;
using ThreadPool = NetFtp.Utils.ThreadPool;

namespace NetFtp
{
    public class FtpClient
    {
        #region Fields

        private bool _abort;
        private string _host;

        #endregion

        #region Constructors

        public FtpClient()
        {
            UsePassive = true;
            TimeOut = 30000;
            ReadWriteTimeOut = 30000;
            KeepAlive = false;
        }

        public FtpClient(string host, string userName, string password, int port)
        {
            Host = host;
            UserName = userName;
            Password = password;
            Port = port;
            UsePassive = true;
            TimeOut = 30000;
            ReadWriteTimeOut = 30000;
            KeepAlive = false;
        }

        public FtpClient(string host, string userName, string password, int port,
            bool usePassive)
        {
            Host = host;
            UserName = userName;
            Password = password;
            Port = port;
            UsePassive = usePassive;
            TimeOut = 30000;
            ReadWriteTimeOut = 30000;
            KeepAlive = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or Sets the Username used to login in the FTP Server.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or Sets the Password used to login in the FTP Server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or Sets the Port used to connect to the FTP Server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Gets or Sets the host address of the FTP Server.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { SetHost(value); }
        }

        /// <summary>
        ///     Gets or Sets the FTP Passive mode feature.
        /// </summary>
        public bool UsePassive { get; set; }

        /// <summary>
        ///     Gets or Sets the timeout (in ms) for FTP connections
        ///     (default: 30000 ms)
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        ///     Gets or Sets the IO timeout (in ms)
        ///     (default: 30000 ms)
        /// </summary>
        public int ReadWriteTimeOut { get; set; }

        /// <summary>
        ///     Gets or Sets the Keep-Alive feature.
        /// </summary>
        public bool KeepAlive { get; set; }

        #endregion

        #region Events

        public event EventHandler<FtpUploadProgressChangedEventArgs> UploadProgressChanged;

        protected void OnUploadProgressChanged(FtpUploadProgressChangedEventArgs args)
        {
            if (UploadProgressChanged == null) return;
            UploadProgressChanged(this, args);
        }

        public event EventHandler<FtpUploadFileCompletedEventArgs> UploadFileCompleted;

        protected void OnUploadFileCompleted(FtpUploadFileCompletedEventArgs args)
        {
            if (UploadFileCompleted == null) return;
            UploadFileCompleted(this, args);
        }

        public event EventHandler<FtpDownloadProgressChangedEventArgs>
            DownloadProgressChanged;

        protected void OnDownloadProgressChanged(FtpDownloadProgressChangedEventArgs args)
        {
            if (DownloadProgressChanged == null) return;
            DownloadProgressChanged(this, args);
        }

        public event EventHandler<FtpDownloadFileCompletedEventArgs> DownloadFileCompleted;

        protected void OnDownloadFileCompleted(FtpDownloadFileCompletedEventArgs args)
        {
            if (DownloadFileCompleted == null) return;
            DownloadFileCompleted(this, args);
        }

        public event EventHandler<FtpListSegmentsCompletedEventArgs> ListSegmentsCompleted;

        protected void OnListSegmentsCompleted(FtpListSegmentsCompletedEventArgs arg)
        {
            if (ListSegmentsCompleted == null) return;
            ListSegmentsCompleted(this, arg);
        }

        #endregion

        #region Private Helper Methods

        private void SetHost(string host)
        {
            _host = !host.ToLower().StartsWith("ftp://") ? host : _host.Substring(6);
        }

        private FtpWebRequest CreateDefaultFtpRequest(string ftpMethod,
            string remotePath
            )
        {
            var uri = new UriBuilder("ftp", Host, Port, remotePath).Uri;
            var ftpWebRequest = (FtpWebRequest) WebRequest.Create(uri);

            ftpWebRequest.Method = ftpMethod;
            ftpWebRequest.Credentials = new NetworkCredential(UserName, Password);
            ftpWebRequest.UsePassive = UsePassive;
            ftpWebRequest.Timeout = TimeOut;
            ftpWebRequest.KeepAlive = KeepAlive;

            return ftpWebRequest;
        }

        #endregion

        #region FTP functions

        #region Ftp MKD function

        /// <summary>
        ///     Creates the specified path (directory and subdirectories)
        ///     on the FTP Server.
        /// </summary>
        /// <param name="remoteDirectory">
        ///     The path to create.
        /// </param>
        /// <returns>Whether the directory was created successfully.</returns>
        public bool CreateDirectoryRecursive(string remoteDirectory)
        {
            while (remoteDirectory.Contains("//"))
                remoteDirectory = remoteDirectory.Replace("//", "/");

            var subDirectories = remoteDirectory.Split(new[] {'/'},
                StringSplitOptions.RemoveEmptyEntries);

            var createdPath = string.Empty;
            foreach (var subDirectory in subDirectories)
            {
                createdPath += subDirectory;
                CreateDirectory(createdPath);
                createdPath += "/";
            }
            return DirectoryExits(createdPath);
        }

        /// <summary>
        ///     Creates the specified directory on the FTP Server.
        /// </summary>
        /// <param name="remoteDirectory">
        ///     The path of the remote directory to create.
        /// </param>
        /// <returns>Whether the directory was created successfully</returns>
        public bool CreateDirectory(string remoteDirectory)
        {
            OnUploadProgressChanged(
                new FtpUploadProgressChangedEventArgs(TransactionState.CreatingDir));

            var ftpWebRequest = CreateDefaultFtpRequest(
                WebRequestMethods.Ftp.MakeDirectory,
                remoteDirectory
                );
            ftpWebRequest.GetResponse().Close();

            return true;
        }

        #endregion

        #region Ftp LIST function

        /// <summary>
        ///     Lists the files and directories on the specified path.
        /// </summary>
        /// <param name="remoteDirectory">The path to the directory to list</param>
        /// <returns>An IList of the FtpFiles in the specified directory</returns>
        public IList<FtpFile> ListSegments(string remoteDirectory)
        {
            List<FtpFile> list;

            var ftpWebRequest = CreateDefaultFtpRequest(
                WebRequestMethods.Ftp.ListDirectoryDetails,
                remoteDirectory
                );

            using (var ftpWebResponse = (FtpWebResponse) ftpWebRequest.GetResponse())
            {
                var responseStream = ftpWebResponse.GetResponseStream();
                if (responseStream == null)
                    throw new WebException("Response stream was not received properly");
                using (var streamReader = new StreamReader(responseStream))
                {
                    var str = streamReader.ReadToEnd();
                    list = FtpListUtils.Parse(str.Split('\n'));
                }
            }

            OnListSegmentsCompleted(new FtpListSegmentsCompletedEventArgs(list));
            return list;
        }

        /// <summary>
        ///     Lists asynchronously the files and directorties on the specified path.
        /// </summary>
        /// <param name="remoteDirectory">The path to the directory to list</param>
        public void ListSegmentsAsync(string remoteDirectory)
        {
            ThreadPool.StartNewthread(ThreadNames.ListThreadName,
                () => ListSegments(remoteDirectory));
        }

        /// <summary>
        ///     Checks if the file at the specified path exists.
        /// </summary>
        /// <param name="remotePath">The path of the remote file to check.</param>
        /// <returns>
        ///     <see cref="FtpFileExistsCompletedEventArgs" />
        /// </returns>
        public FtpFileExistsCompletedEventArgs FileExists(string remotePath)
        {
            var ftpWebRequest = CreateDefaultFtpRequest(
                WebRequestMethods.Ftp.ListDirectoryDetails,
                remotePath);

            long remFileSize;

            try
            {
                using (var ftpWebResponse = (FtpWebResponse) ftpWebRequest.GetResponse())
                {
                    var responseStream = ftpWebResponse.GetResponseStream();
                    if (responseStream == null)
                        throw new WebException("Response stream was not received properly");
                    using (
                        var streamReader = new StreamReader(responseStream)
                        )
                    {
                        var str = streamReader.ReadToEnd();
                        var ftpFile = FtpListUtils.Parse(str.Split(new[]
                        {
                            '\n'
                        })[0]);
                        remFileSize = ftpFile.Size;
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.WriteLine(ex.Message);
                return new FtpFileExistsCompletedEventArgs(ex);
            }
            return new FtpFileExistsCompletedEventArgs(true, remFileSize);
        }

        /// <summary>
        ///     Checks if the directory at the specified path exists.
        /// </summary>
        /// <param name="remoteDirectory">The path to the remote directory</param>
        /// <returns>Whether the directory exists</returns>
        public bool DirectoryExits(string remoteDirectory)
        {
            if (UploadProgressChanged != null && !_abort)
                UploadProgressChanged(this,
                    new FtpUploadProgressChangedEventArgs(
                        TransactionState.ProofingDirExits));

            return FileExists(remoteDirectory).FileExists;
        }

        #endregion

        #region Ftp STOR function

        /// <summary>
        ///     Uploads the file at the specified local path
        ///     to the specified remote FTP path.
        /// </summary>
        /// <param name="localPath">The path to the local file to upload</param>
        /// <param name="remotePath">The remote destination path</param>
        /// <returns>
        ///     <see cref="FtpUploadFileCompletedEventArgs" />
        /// </returns>
        /// <exception cref="FileNotFoundException">
        ///     Throws when the specified remote file was not found.
        /// </exception>
        public FtpUploadFileCompletedEventArgs Upload(string localPath,
            string remotePath)
        {
            _abort = false;
            var fileInfo = new FileInfo(localPath);

            if (!fileInfo.Exists)
                throw new FileNotFoundException(
                    "Could not find the specified local file to upload",
                    fileInfo.FullName);

            var totalBytesSent = 0L;
            try
            {
                var ftpWebRequest =
                    CreateDefaultFtpRequest(
                        WebRequestMethods.Ftp.UploadFile,
                        remotePath);

                var remoteDir = remotePath.Contains("/")
                    ? remotePath.Substring(0, remotePath.LastIndexOf('/'))
                    : "/";

                if (!DirectoryExits(remoteDir))
                    CreateDirectoryRecursive(remoteDir);

                using (var requestStream = ftpWebRequest.GetRequestStream())
                {
                    using (
                        var fileStream =
                            new FileStream(fileInfo.FullName,
                                FileMode.Open,
                                FileAccess.Read,
                                FileShare.Read))
                    {
                        fileStream.Seek(0L, SeekOrigin.Begin);
                        // TODO: Add buffer size as property
                        var buffer = new byte[128000];
                        int bytesSent;
                        do
                        {
                            bytesSent = fileStream.Read(buffer, 0, 128000);
                            requestStream.Write(buffer, 0, bytesSent);

                            if (_abort)
                                return new FtpUploadFileCompletedEventArgs(
                                    totalBytesSent, TransactionState.Aborted);

                            OnUploadProgressChanged(new FtpUploadProgressChangedEventArgs(
                                fileStream.Position, fileStream.Length));
                        } while (bytesSent != 0);
                        totalBytesSent = fileStream.Length;
                    }
                }
                var result = new FtpUploadFileCompletedEventArgs(totalBytesSent,
                    TransactionState.Success);
                OnUploadFileCompleted(result);
                return result;
            }
            catch (WebException ex)
            {
                var result = new FtpUploadFileCompletedEventArgs(totalBytesSent,
                    TransactionState.Failed, ex);
                OnUploadFileCompleted(result);
                return result;
            }
        }

        /// <summary>
        ///     Uploads asynchronously the file at the specified local path
        ///     to the specifed remote path.
        /// </summary>
        /// <param name="localPath">The path to the local file to upload</param>
        /// <param name="remotePath">The remote destination path</param>
        public void UploadAsync(string localPath,
            string remotePath)
        {
            ThreadPool.StartNewthread(ThreadNames.UploadThreadName,
                () => Upload(localPath, remotePath));
        }

        /// <summary>
        ///     Resumes an incomplete file upload,
        ///     if the remote file does not exist then a regular upload is performed.
        /// </summary>
        /// <param name="localPath">The path to the local file to upload</param>
        /// <param name="remotePath">The path to the remote destination file</param>
        /// <returns>
        ///     <see cref="FtpUploadFileCompletedEventArgs" />
        /// </returns>
        public FtpUploadFileCompletedEventArgs UploadResume(string localPath,
            string remotePath)
        {
            _abort = false;
            var fileInfo = new FileInfo(localPath);
            var totalBytesSent = 0L;
            var ftpWebRequest = CreateDefaultFtpRequest(WebRequestMethods.Ftp.UploadFile,
                remotePath);
            try
            {
                // Checks if remote file exists
                var fileExistsResult = FileExists(remotePath);
                if (fileExistsResult.State == TransactionState.Failed)
                    throw fileExistsResult.WebException;

                // Gets remote file size (or 0 if does not exist)
                var remFileSize = fileExistsResult.RemotefileSize;
                ftpWebRequest.ContentLength = fileInfo.Length - remFileSize;

                // Checks if remote is bigger than local file
                if (fileInfo.Length < remFileSize)
                {
                    var args = new FtpUploadFileCompletedEventArgs(
                        remFileSize,
                        TransactionState.RemoteFileBiggerThanLocalFile);
                    OnUploadFileCompleted(args);
                    return args;
                }

                // If the remote file exists we append to its content
                // else we perform a regular upload
                if (fileExistsResult.FileExists)
                    ftpWebRequest.Method = WebRequestMethods.Ftp.AppendFile;
                else
                    return Upload(localPath, remotePath);

                using (var requestStream = ftpWebRequest.GetRequestStream())
                {
                    using (
                        var fileStream =
                            new FileStream(localPath,
                                FileMode.Open,
                                FileAccess.Read,
                                FileShare.ReadWrite))
                    {
                        var streamReader = new StreamReader(fileStream);
                        streamReader.BaseStream.Seek(remFileSize, SeekOrigin.Begin);
                        var buffer = new byte[128000];
                        int count;
                        do
                        {
                            count = streamReader.BaseStream.Read(buffer, 0, 128000);
                            requestStream.Write(buffer, 0, count);

                            OnUploadProgressChanged(
                                new FtpUploadProgressChangedEventArgs(
                                    streamReader.BaseStream.Position,
                                    streamReader.BaseStream.Length));

                            if (!_abort) continue;
                            // Abortind
                            var args = new FtpUploadFileCompletedEventArgs(
                                streamReader.BaseStream.Position,
                                TransactionState.Aborted);
                            OnUploadFileCompleted(args);
                            return args;
                        } while (count != 0);

                        totalBytesSent = streamReader.BaseStream.Length;
                    }
                }
                var result = new FtpUploadFileCompletedEventArgs(totalBytesSent,
                    TransactionState.Success);
                OnUploadFileCompleted(result);
                return result;
            }
            catch (WebException ex)
            {
                var args = new FtpUploadFileCompletedEventArgs(totalBytesSent,
                    TransactionState.Failed, ex);
                OnUploadFileCompleted(args);
                return args;
            }
        }

        /// <summary>
        ///     Resumes asynchronously an incomplete file upload,
        ///     if the remote file does not exist then a regular upload is performed.
        /// </summary>
        /// <param name="localPath">The path to the local file to upload</param>
        /// <param name="remotePath">The path to the remote destination file</param>
        public void UploadResumeAsync(string localPath, string remotePath)
        {
            ThreadPool.StartNewthread(ThreadNames.UploadThreadName,
                () => UploadResume(localPath, remotePath));
        }

        #endregion

        #region RETR Function

        /// <summary>
        ///     Downloads a file from the remote ftp path
        ///     to the specified local path.
        /// </summary>
        /// <param name="localPath">Path where the FTP file will be saved</param>
        /// <param name="remotePath">Path of the remote FTP file to download</param>
        /// <returns>
        ///     <see cref="FtpDownloadFileCompletedEventArgs" />
        /// </returns>
        public FtpDownloadFileCompletedEventArgs Download(string localPath,
            string remotePath)
        {
            _abort = false;
            var bytesReceived = 0L;
            try
            {
                var ftpWebRequest = CreateDefaultFtpRequest(
                    WebRequestMethods.Ftp.DownloadFile,
                    remotePath);

                var fileExistResult = FileExists(remotePath);

                if (!fileExistResult.FileExists)
                {
                    throw new FileNotFoundException("FTP remote file does not exist",
                        remotePath);
                }
                var fileSize = fileExistResult.RemotefileSize;
                var fileStream = new FileStream(localPath, FileMode.Create,
                    FileAccess.Write);
                using (
                    var responseStream = ftpWebRequest.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                        throw new WebException("Response stream was not received properly");
                    var buffer = new byte[128000];
                    var count = responseStream.Read(buffer, 0, 128000);
                    bytesReceived = count;
                    while (count != 0)
                    {
                        fileStream.Write(buffer, 0, count);
                        count = responseStream.Read(buffer, 0, 128000);
                        bytesReceived += count;
                        OnDownloadProgressChanged(
                            new FtpDownloadProgressChangedEventArgs(bytesReceived,
                                fileSize));

                        if (!_abort) continue;

                        var args = new FtpDownloadFileCompletedEventArgs(
                            bytesReceived,
                            TransactionState.Success);
                        OnDownloadFileCompleted(args);
                        return args;
                    }
                    fileStream.Close();
                }
                var result = new FtpDownloadFileCompletedEventArgs(bytesReceived,
                    TransactionState.Success);
                OnDownloadFileCompleted(result);
                return result;
            }
            catch (WebException ex)
            {
                var args = new FtpDownloadFileCompletedEventArgs(bytesReceived,
                    TransactionState.Failed, ex);
                OnDownloadFileCompleted(args);
                return args;
            }
        }

        /// <summary>
        ///     Downloads asynchronously a file from the remote ftp path
        ///     to the specified local path.
        /// </summary>
        /// <param name="localPath">Path where the FTP file will be saved</param>
        /// <param name="remotePath">Path of the remote FTP file to download</param>
        /// <returns>
        ///     <see cref="FtpDownloadFileCompletedEventArgs" />
        /// </returns>
        public void DownloadAsync(string localPath, string remotePath)
        {
            ThreadPool.StartNewthread(ThreadNames.DownloadThreadName,
                () => Download(localPath, remotePath));
        }

        /// <summary>
        ///     Resumes the download of a file from the remote ftp path
        ///     to the specified local path.
        ///     If the local file does not exists a regular download is performed.
        /// </summary>
        /// <param name="localPath">Path where the FTP file will be saved</param>
        /// <param name="remotePath">Path of the remote FTP file to download</param>
        /// <returns>
        ///     <see cref="FtpDownloadFileCompletedEventArgs" />
        /// </returns>
        public FtpDownloadFileCompletedEventArgs DownloadResume(string localPath,
            string remotePath)
        {
            _abort = false;
            var fileInfo = new FileInfo(localPath);

            // If no local file exists, perform regular download
            if (!fileInfo.Exists)
                return Download(localPath, remotePath);

            var totalBytes = 0L;

            try
            {
                var ftpWebRequest = CreateDefaultFtpRequest(
                    WebRequestMethods.Ftp.DownloadFile,
                    remotePath);
                ftpWebRequest.ContentOffset = fileInfo.Length;

                var fileExistsResult = FileExists(remotePath);

                // If the specified file does not exists, throw FileNotFound
                if (!fileExistsResult.FileExists)
                    throw new FileNotFoundException("FTP remote file does not exist",
                        remotePath);

                var fileSize = fileExistsResult.RemotefileSize;

                if (fileInfo.Length == fileSize)
                {
                    var args = new FtpDownloadFileCompletedEventArgs(0L,
                        TransactionState.Success);
                    OnDownloadFileCompleted(args);
                    return args;
                }

                if (fileInfo.Length > fileSize)
                {
                    // TODO: Change to throw exception
                    var args = new FtpDownloadFileCompletedEventArgs(0L,
                        TransactionState.LocalFileBiggerThanRemoteFile);
                    OnDownloadFileCompleted(args);
                    return args;
                }

                using (
                    var fileStream = new FileStream(localPath, FileMode.Append,
                        FileAccess.Write))
                {
                    using (
                        var responseStream =
                            ftpWebRequest.GetResponse().GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new WebException(
                                "Response stream was not received properly");

                        var buffer = new byte[128000];
                        var count = responseStream.Read(buffer, 0, 128000);
                        totalBytes = fileInfo.Length + count;
                        while (count != 0)
                        {
                            fileStream.Write(buffer, 0, count);
                            count = responseStream.Read(buffer, 0, 128000);
                            totalBytes += count;

                            OnDownloadProgressChanged(
                                new FtpDownloadProgressChangedEventArgs(totalBytes,
                                    fileSize));

                            if (!_abort) continue;

                            var args = new FtpDownloadFileCompletedEventArgs(
                                totalBytes,
                                TransactionState.Aborted);
                            OnDownloadFileCompleted(args);
                            return args;
                        }
                    }
                }
                var result = new FtpDownloadFileCompletedEventArgs(totalBytes,
                    TransactionState.Success);
                OnDownloadFileCompleted(result);
                return result;
            }
            catch (WebException ex)
            {
                var args = new FtpDownloadFileCompletedEventArgs(totalBytes,
                    TransactionState.Failed, ex);
                OnDownloadFileCompleted(args);
                return args;
            }
        }

        /// <summary>
        ///     Resumes asynchronously the download of a file from the remote ftp path
        ///     to the specified local path.
        ///     If the local file does not exists a regular download is performed.
        /// </summary>
        /// <param name="localPath">Path where the FTP file will be saved</param>
        /// <param name="remotePath">Path of the remote FTP file to download</param>
        public void DownloadResumeAsync(string localPath, string remotePath)
        {
            ThreadPool.StartNewthread(ThreadNames.DownloadThreadName,
                () => DownloadResume(localPath, remotePath));
        }

        //[Obsolete(
        //    "Legacy function, will be refactored in next version. Method Signature won't change"
        //    )]
        //public void Abort()
        //{
        //    _abort = true;
        //    _thread.Abort();
        //}

        #endregion

        #endregion
    }
}