namespace ZTR.Framework.Configuration.Ftp
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FtpConnection
    {
        public string Host { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int? Port { get; set; }

        public string KeyFile { get; set; }

        public string Folder { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
