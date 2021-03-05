namespace ZTR.M7Config.Business.Parsers
{
    using System;

    public class Global
    {
        public static string WebRoot => AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        
    }
}
