﻿namespace NetFtpUnitTest
{
    class Utils
    {
        internal const string FtpUserName = "admin";
        internal const string FtpPassword = "password";
        internal const string FtpHost = "127.0.0.1";
        internal const int FtpPort = 21;

        internal const string FtpUnreachableHost = "0.0.0.0";
        internal const string FtpWrongUsername = "wronguser";
        internal const string FtpWrongPassword = "wrongpassword";

        internal const string FtpDirToList = "/";

        internal const string LocalPath = "d:\\tmp\\test.zip";
        internal const string InvalidLocalPath = "d:\\tmp\\thisFileDoesNot.exist";
        internal const string RemotePath = "/UploadTest/remoteTest.zip";
    }
}
