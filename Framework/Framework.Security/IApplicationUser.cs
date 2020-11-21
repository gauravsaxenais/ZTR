namespace ZTR.Framework.Security
{
    public interface IApplicationUser
    {
        long UserId { get; set; }

        string FormatIsoCode { get; set; }

        string CurrentTimeZone { get; set; }
    }
}
