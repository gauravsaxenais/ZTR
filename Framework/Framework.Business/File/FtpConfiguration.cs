namespace ZTR.Framework.Business.File
{
    using System;
    using System.IO;
    using ZTR.Framework.Configuration.Ftp;

    public class FtpConfiguration : FtpConnection
    {
        public FtpConfiguration()
        {
        }

        public string FolderPath { get; set; }

        public string FileNamePreFix { get; set; }

        public string FileExtension { get; set; }

        public string[] ExcludeFileList { get; set; }

        public int NoOfFilesToBeDownloadedParallel { get; set; } = 1;

        public string DecryptionPrivateKeyFilePath { get; set; }

        public string DecryptionPassword { get; set; }

        public Func<Stream, Stream> DecryptionHandler { get; set; }
    }
}
