namespace ZTR.Framework.Security
{
    public static class CustomClaimTypes
    {
        public const string CompanyMasterKey = "company_master_key";

        public const string CompanyId = "company_id_key";

        public const string LanguageIsoCode = "language_iso_code";

        public const string FormatIsoCode = "format_iso_code";

        public const string LoginProviderTypeCode = "login_type_code";

        public const string LoginProviderCode = "login_provider_code";

        public const string IsPasswordRecovery = "password_recovery";

        public const string SecurityRights = "security_rights";

        public const string TimeZone = "time_zone";

        /// <summary>
        /// This is used during Client Credients flow to impersonate the user.
        /// It is not to be used during interactive flow.
        /// </summary>
        // TODO: change user key when upgrade to 3.1 --> "user_id_key";
        public const string UserId = nameof(UserId);
    }
}
