using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetFtp.Utils
{
    internal enum FileListStyle
    {
        UnixStyle,
        WindowsStyle,
        Unknown,
    }

    internal static class FtpListUtils
    {
        /// <summary>
        ///     Parses a list of string (usually from the result of LIST)
        ///     as FtpFiles.
        /// </summary>
        /// <param name="ftpRecords">
        ///     A List of string representing lines of a LIST result (segments).
        /// </param>
        /// <returns>List of FTP Files</returns>
        public static List<FtpFile> Parse(IList<string> ftpRecords)
        {
            var result = new List<FtpFile>();

            foreach (var ftpRecord in ftpRecords)
            {
                var ftpFile = Parse(ftpRecord);

                if (ftpFile == null
                    || string.IsNullOrEmpty(ftpFile.Name)
                    || ftpFile.Name == "."
                    || ftpFile.Name == "..")
                    continue;

                result.Add(ftpFile);
            }

            return result;
        }

        /// <summary>
        ///     Parses a single entry as a FtpFile
        /// </summary>
        /// <param name="ftpRecord">A string representing a file entry</param>
        /// <returns>An FtpFile corresponding to the ftpRecord string</returns>
        public static FtpFile Parse(string ftpRecord)
        {
            var ftpFile = new FtpFile();
            var fileListStyle = GuessFileListStyle(ftpRecord);
            if (fileListStyle == FileListStyle.Unknown || string.IsNullOrEmpty(ftpRecord))
                return null;
            ftpFile.Name = "..";
            switch (fileListStyle)
            {
                case FileListStyle.UnixStyle:
                    ftpFile = ParseFtpFileFromUnixStyleRecord(ftpRecord);
                    break;
                case FileListStyle.WindowsStyle:
                    ftpFile = ParseFtpFileFromWindowsStyleRecord(ftpRecord);
                    break;
            }
            return ftpFile;
        }

        /// <summary>
        ///     Parses a record formated as a Windows style FTP 
        /// </summary>
        private static FtpFile ParseFtpFileFromWindowsStyleRecord(string record)
        {
            var ftpFile = new FtpFile();
            var strArray1 = record.Trim()
                .Split(" \t".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
            // TODO: Parse other results
            //var date = strArray1[0];
            var str1 = strArray1[1];
            //var time = str1.Substring(0, 7);
            var str2 = str1.Substring(7, str1.Length - 7).Trim();
            //DateTime? dateTime;
            //if (ConvertDate.Parse(date, time, out dateTime))
            //    FtpFile.CreateTime = dateTime;
            //FtpFile.CreateTimeString = date + time;
            string str3;
            if (str2.Substring(0, 5) == "<DIR>")
            {
                ftpFile.IsDirectory = true;
                str3 = str2.Substring(5, str2.Length - 5).Trim();
            }
            else
            {
                ftpFile.IsDirectory = false;
                var strArray2 = str2.Split(new[]
                {
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries);
                long result;
                if (long.TryParse(strArray2[0].Trim(), out result))
                    ftpFile.Size = result;
                str3 = strArray2[1].Trim();
            }
            ftpFile.Name = str3;
            return ftpFile;
        }

        /// <summary>
        ///     Guesses the format of a string record.
        /// </summary>
        private static FileListStyle GuessFileListStyle(string record)
        {
            if (record.Length > 10 &&
                Regex.IsMatch(record.Substring(0, 10),
                    "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                return FileListStyle.UnixStyle;
            return record.Length > 8 &&
                   Regex.IsMatch(record.Substring(0, 8),
                       "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]")
                ? FileListStyle.WindowsStyle
                : FileListStyle.Unknown;
        }

        /// <summary>
        ///     Parses a record formated as a Unix style FTP
        /// </summary>
        private static FtpFile ParseFtpFileFromUnixStyleRecord(string record)
        {
            var ftpFile = new FtpFile();
            var str = record.Trim();

            ftpFile.Flags = str.Substring(0, 9);
            ftpFile.IsDirectory = ftpFile.Flags[0] == 100;
            var remainingToParse = str.Substring(11).Trim();
            CutSubstringFromStringWithTrim(ref remainingToParse, ' ', 0);
            ftpFile.Owner = CutSubstringFromStringWithTrim(ref remainingToParse, ' ', 0);
            ftpFile.Group = CutSubstringFromStringWithTrim(ref remainingToParse, ' ', 0);
            long result;
            if (long.TryParse(CutSubstringFromStringWithTrim(ref remainingToParse, ' ', 0), out result))
                ftpFile.Size = result;
            var dateStr = CutSubstringFromStringWithTrim(ref remainingToParse, ' ', 8);
            ftpFile.CreateTime = DateTime.Parse(dateStr);
            ftpFile.Name = remainingToParse;
            return ftpFile;
        }

        /// <summary>
        ///     Cuts a string from anything before the first occurrence of the specified
        ///     character after the specified start index, and trim what is left.
        /// </summary>
        /// <returns>
        ///     The part of the string below the first occurrence of the
        ///     specified character.
        /// </returns>
        private static string CutSubstringFromStringWithTrim(ref string s, char c,
            int startIndex)
        {
            var num = s.IndexOf(c, startIndex);
            var str = s.Substring(0, num);
            s = s.Substring(num).Trim();
            return str;
        }
    }
}