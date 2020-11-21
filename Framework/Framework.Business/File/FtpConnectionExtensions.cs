namespace ZTR.Framework.Business.File
{
    using ZTR.Framework.Configuration.Ftp;
    using Renci.SshNet;

    public static class FtpConnectionExtensions
    {
        public static SftpClient ToSftpClient(this FtpConnection ftpConnection)
        {
            var resolvedPort = ftpConnection.Port ?? 22;
            if (string.IsNullOrWhiteSpace(ftpConnection.Password))
            {
                var privateKeyFile = new PrivateKeyFile(ftpConnection.KeyFile);
                return new SftpClient(ftpConnection.Host, resolvedPort, ftpConnection.User, privateKeyFile);
            }

            return new SftpClient(ftpConnection.Host, resolvedPort, ftpConnection.User, ftpConnection.Password);
        }
    }
}
