using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using NetFtp.NetFtpEventArgs;
using NetFtp.Utils;

namespace NetFtp
{
    public class FtpClient
    {
        #region Fields

        private bool _abort;
        private string _host;
        private Thread _thread; // TODO: Thread pool

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

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string Host
        {
            get { return _host; }
            set { SetHost(value); }
        }

        public bool UsePassive { get; set; }

        public int TimeOut { get; set; }

        public int ReadWriteTimeOut { get; set; }

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

        public event EventHandler<FtpDownloadFileCompletedEventArgs> DownloadFileCompleted;

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
            string remoteDirectory, string remoteFileName)
        {
            var builder = new StringBuilder(remoteDirectory);
            if (!remoteDirectory.EndsWith("/"))
                builder.Append("/");
            builder.Append(remoteFileName);

            return CreateDefaultFtpRequest(ftpMethod, builder.ToString());
        }

        private FtpWebRequest CreateDefaultFtpRequest(string ftpMethod,
            string remoteDirectory
            )
        {
            var uri = new UriBuilder("ftp", Host, Port, remoteDirectory).Uri;
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

        public bool CreateDirectory(string remoteDirectory)
        {
            OnUploadProgressChanged(
                new FtpUploadProgressChangedEventArgs(TransmissionState.CreatingDir));
            
                var ftpWebRequest = CreateDefaultFtpRequest(
                    WebRequestMethods.Ftp.MakeDirectory,
                    remoteDirectory
                    );
                ftpWebRequest.GetResponse().Close();
            
            return true;
        }

        #endregion

        #region Ftp LIST function

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
                    list = FtpListUtil.Parse(str.Split('\n'));
                }
            }

            OnListSegmentsCompleted(new FtpListSegmentsCompletedEventArgs(list));
            return list;
        }

        public void ListSegmentsAsync(string remoteDirectory)
        {
            _thread = new Thread(() => ListSegments(remoteDirectory))
            {
                Name = ThreadNames.ListThreadName,
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _thread.Start();
        }

        public FtpFileExistsCompletedEventArgs FileExists(string remoteDirectory,
            string remoteFileName)
        {
            var ftpWebRequest = CreateDefaultFtpRequest(
                WebRequestMethods.Ftp.ListDirectoryDetails,
                remoteDirectory,
                remoteFileName);

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
                        var ftpFile = FtpListUtil.Parse(str.Split(new[]
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
                return new FtpFileExistsCompletedEventArgs {Exception = ex};
            }
            return new FtpFileExistsCompletedEventArgs
            {
                FileExists = true,
                RemotefileSize = remFileSize
            };
        }

        public bool DirectoryExits(string remoteDirectory)
        {
            if (UploadProgressChanged != null && !_abort)
                UploadProgressChanged(this,
                    new FtpUploadProgressChangedEventArgs(
                        TransmissionState.ProofingDirExits));

            return FileExists(remoteDirectory, string.Empty).FileExists;
        }

        #endregion

        #region Ftp STOR function

        public FtpUploadFileCompletedEventArgs Upload(string localDirectory, string localFilename,
            string remoteDirectory, string remoteFileName)
        {
            _abort = false;
            var fileInfo = new FileInfo(Path.Combine(localDirectory, localFilename));
            
            if(!fileInfo.Exists)
                throw new FileNotFoundException(
                    "Could not find the specified local file to upload",
                    fileInfo.FullName);

            var totalBytesSent = 0L;
            try
            {
                var ftpWebRequest =
                    CreateDefaultFtpRequest(
                        WebRequestMethods.Ftp.UploadFile,
                        remoteDirectory,
                        remoteFileName);

                if (!DirectoryExits(remoteDirectory))
                    CreateDirectoryRecursive(remoteDirectory);

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
                                return new FtpUploadFileCompletedEventArgs(totalBytesSent, TransmissionState.Aborted);

                            OnUploadProgressChanged(new FtpUploadProgressChangedEventArgs(
                                fileStream.Position, fileStream.Length));
                        } while (bytesSent != 0 && !_abort);
                        totalBytesSent = fileStream.Length;
                    }
                }
                var result = new FtpUploadFileCompletedEventArgs(totalBytesSent,
                    TransmissionState.Success);
                OnUploadFileCompleted(result);
                return result;
            }
            catch (WebException ex)
            {
                var result = new FtpUploadFileCompletedEventArgs(totalBytesSent,
                    TransmissionState.Failed, ex);
                OnUploadFileCompleted(result);
                return result;
            }
        }

        public void UploadAsync(string localDirectory,
            string localFilename,
            string remoteDirectory,
            string remoteFileName)
        {
            _thread =
                new Thread(() => Upload(localDirectory, localFilename, remoteDirectory,
                    remoteFileName))
                {
                    Name = ThreadNames.UploadThreadName,
                    IsBackground = true,
                    Priority = ThreadPriority.Normal
                };
            _thread.Start();
        }

        //[Obsolete(
        //    "Legacy function, will be refactored in next version. Method Signature won't change"
        //    )]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void UploadResume(string localDirectory, string localFilename,
        //    string remoteDirectory,
        //    string remoteFileName)
        //{
        //    _abort = false;
        //    var fileInfo = new FileInfo(Path.Combine(localDirectory, localFilename));
        //    var totalBytesSend = 0L;
        //    var ftpWebRequest =
        //        (FtpWebRequest)
        //            WebRequest.Create(
        //                new Uri("ftp://" + _host + ":" + Port + "/" + remoteDirectory +
        //                        "/" +
        //                        remoteFileName));
        //    ftpWebRequest.Credentials = new NetworkCredential(UserName, Password);
        //    ftpWebRequest.Timeout = TimeOut;
        //    ftpWebRequest.ReadWriteTimeout = ReadWriteTimeOut;
        //    ftpWebRequest.Proxy = null;
        //    ftpWebRequest.KeepAlive = KeepAlive;
        //    try
        //    {
        //        var fileExistsResult = FileExists(remoteDirectory, remoteFileName);
        //        if (fileExistsResult.State == TransmissionState.Failed)
        //            throw fileExistsResult.Exception;
        //        var remFileSize = fileExistsResult.RemotefileSize;
        //        if (fileExistsResult.FileExists)
        //        {
        //            ftpWebRequest.Method = WebRequestMethods.Ftp.AppendFile;
        //        }
        //        else
        //        {
        //            WebException webException;
        //            if (!DirectoryExits(remoteDirectory, out webException))
        //                CreateDirectory(remoteDirectory, out webException);
        //            ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
        //        }
        //        ftpWebRequest.ContentLength = fileInfo.Length - remFileSize;
        //        ftpWebRequest.UsePassive = UsePassive;
        //        using (var requestStream = ftpWebRequest.GetRequestStream())
        //        {
        //            using (
        //                var fileStream =
        //                    new FileStream(Path.Combine(localDirectory, localFilename),
        //                        FileMode.Open,
        //                        FileAccess.Read, FileShare.ReadWrite))
        //            {
        //                var streamReader = new StreamReader(fileStream);
        //                streamReader.BaseStream.Seek(remFileSize, SeekOrigin.Begin);
        //                var buffer = new byte[128000];
        //                int count;
        //                do
        //                {
        //                    count = streamReader.BaseStream.Read(buffer, 0, 128000);
        //                    requestStream.Write(buffer, 0, count);
        //                    if (UploadProgressChanged != null && !_abort)
        //                        UploadProgressChanged(this,
        //                            new FtpUploadProgressChangedEventArgs(
        //                                streamReader.BaseStream.Position,
        //                                streamReader.BaseStream.Length));
        //                } while (count != 0 && !_abort);

        //                totalBytesSend = streamReader.BaseStream.Length;
        //                Thread.Sleep(100);
        //            }
        //        }
        //        if (UploadFileCompleted == null || _abort)
        //            return;
        //        UploadFileCompleted(this,
        //            new FtpUploadFileCompletedEventArgs(totalBytesSend,
        //                TransmissionState.Success));
        //    }
        //    catch (WebException ex)
        //    {
        //        if (UploadFileCompleted == null || _abort)
        //            return;
        //        UploadFileCompleted(this,
        //            new FtpUploadFileCompletedEventArgs(totalBytesSend,
        //                TransmissionState.Failed, ex));
        //    }
        //}
        
        //[Obsolete(
        //    "Legacy function, will be refactored in next version. Method Signature won't change"
        //    )]
        //public void UploadResumeAsync(string localDirectory, string localFilename,
        //    string remoteDirectory,
        //    string remoteFileName)
        //{
        //    _thread =
        //        new Thread(
        //            () => UploadResume(localDirectory, localFilename, remoteDirectory,
        //                remoteFileName))
        //        {
        //            Name = ThreadNames.UploadThreadName,
        //            IsBackground = true,
        //            Priority = ThreadPriority.Normal
        //        };
        //    _thread.Start();
        //}

        #endregion

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Download(string localDirectory, string localFilename,
            string remoteDirectory, string remoteFileName)
        {
            _abort = false;
            var path = Path.Combine(localDirectory, localFilename);
            var num = 0L;
            try
            {
                var ftpWebRequest =
                    (FtpWebRequest)
                        WebRequest.Create(
                            new Uri("ftp://" + _host + ":" + Port + "/" + remoteDirectory +
                                    "/" + remoteFileName));
                ftpWebRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpWebRequest.UsePassive = UsePassive;
                ftpWebRequest.Timeout = TimeOut;
                ftpWebRequest.Proxy = null;
                ftpWebRequest.KeepAlive = KeepAlive;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                var fileSize = FileExists(remoteDirectory, remoteFileName).RemotefileSize;
                var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                using (
                    var responseStream = ftpWebRequest.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                        throw new WebException("Response stream was not received properly");
                    var buffer = new byte[128000];
                    var count = responseStream.Read(buffer, 0, 128000);
                    num = count;
                    while (count != 0 && !_abort)
                    {
                        fileStream.Write(buffer, 0, count);
                        count = responseStream.Read(buffer, 0, 128000);
                        num += count;
                        if (DownloadProgressChanged != null && !_abort)
                            DownloadProgressChanged(this,
                                new FtpDownloadProgressChangedEventArgs(num, fileSize));
                    }
                    fileStream.Close();
                }
                if (DownloadFileCompleted == null || _abort)
                    return;
                DownloadFileCompleted(this,
                    new FtpDownloadFileCompletedEventArgs(num, TransmissionState.Success));
            }
            catch (WebException ex)
            {
                if (DownloadFileCompleted != null && !_abort)
                    DownloadFileCompleted(this,
                        new FtpDownloadFileCompletedEventArgs(num,
                            TransmissionState.Failed, ex));
            }
        }

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        public void DownloadAsync(string localDirectory, string localFilename,
            string remoteDirectory,
            string remoteFileName)
        {
            var threadParameters = new FtpThreadTransferParameters(localDirectory,
                localFilename, remoteDirectory, remoteFileName);
            _thread = new Thread(DoDownloadAsync)
            {
                Name = "DownloadThread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _thread.Start(threadParameters);
        }

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DownloadResume(string localDirectory, string localFilename,
            string remoteDirectory,
            string remoteFileName)
        {
            _abort = false;
            var str = Path.Combine(localDirectory, localFilename);
            var fileInfo = new FileInfo(str);
            FileStream fileStream = null;
            var num1 = 0L;
            var num2 = 0L;
            try
            {
                var ftpWebRequest =
                    (FtpWebRequest)
                        WebRequest.Create(
                            new Uri("ftp://" + _host + ":" + Port + "/" + remoteDirectory +
                                    "/" + remoteFileName));
                ftpWebRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpWebRequest.UsePassive = UsePassive;
                ftpWebRequest.Timeout = TimeOut;
                ftpWebRequest.Proxy = null;
                ftpWebRequest.KeepAlive = KeepAlive;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                var fileSize = FileExists(remoteDirectory, remoteFileName).RemotefileSize;
                if (fileInfo.Exists)
                {
                    if (fileInfo.Length == fileSize)
                    {
                        if (DownloadFileCompleted != null)
                        {
                            DownloadFileCompleted(this,
                                new FtpDownloadFileCompletedEventArgs(0L,
                                    TransmissionState.Success));
                            return;
                        }
                    }
                    else if (fileInfo.Length > fileSize)
                    {
                        if (DownloadFileCompleted != null)
                        {
                            DownloadFileCompleted(this,
                                new FtpDownloadFileCompletedEventArgs(0L,
                                    TransmissionState.LocalFileBiggerAsRemoteFile));
                            return;
                        }
                    }
                    else
                    {
                        fileStream = new FileStream(str, FileMode.Append, FileAccess.Write);
                        ftpWebRequest.ContentOffset = fileInfo.Length;
                        num2 = fileInfo.Length;
                    }
                }
                else
                    fileStream = new FileStream(str, FileMode.Create, FileAccess.Write);
                using (
                    var responseStream = ftpWebRequest.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                        throw new WebException("Response stream was not received properly");
                    if (fileStream == null)
                        throw new IOException(
                            "Local file stream was not received properly");
                    var buffer = new byte[128000];
                    var count = responseStream.Read(buffer, 0, 128000);
                    num1 = num2 + count;
                    while (count != 0 && !_abort)
                    {
                        fileStream.Write(buffer, 0, count);
                        count = responseStream.Read(buffer, 0, 128000);
                        num1 += count;
                        if (DownloadProgressChanged != null && !_abort)
                            DownloadProgressChanged(this,
                                new FtpDownloadProgressChangedEventArgs(num1, fileSize));
                    }
                    fileStream.Close();
                }
                if (DownloadFileCompleted != null && !_abort)
                    DownloadFileCompleted(this,
                        new FtpDownloadFileCompletedEventArgs(num1,
                            TransmissionState.Success));
            }
            catch (WebException ex)
            {
                if (DownloadFileCompleted != null && !_abort)
                    DownloadFileCompleted(this,
                        new FtpDownloadFileCompletedEventArgs(num1,
                            TransmissionState.Failed, ex));
            }
        }

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DownloadResumeAsync(string localDirectory, string localFilename,
            string remoteDirectory,
            string remoteFileName)
        {
            var threadParameters = new FtpThreadTransferParameters(localDirectory,
                localFilename, remoteDirectory, remoteFileName);
            _thread = new Thread(DoDownloadResumeAsync)
            {
                Name = "DownloadThread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _thread.Start(threadParameters);
        }

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        public void Abort()
        {
            _abort = true;
            _thread.Abort();
        }

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        private void DoDownloadAsync(object threadParameters)
        {
            var param = (FtpThreadTransferParameters) threadParameters;
            Download(param.LocalDirectory, param.LocalFilename,
                param.RemoteDirectory, param.RemoteFilename);
        }

        [Obsolete(
            "Legacy function, will be refactored in next version. Method Signature won't change"
            )]
        private void DoDownloadResumeAsync(object threadParameters)
        {
            var param = (FtpThreadTransferParameters) threadParameters;
            DownloadResume(param.LocalDirectory,
                param.LocalFilename,
                param.RemoteDirectory, param.RemoteFilename);
        }

        #endregion
    }
}