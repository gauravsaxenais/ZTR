namespace ZTR.Framework.DataAccess
{
    public static class SuggestedConstants
    {
#pragma warning disable CA1034 // Nested types should not be visible

        public static class DataLengths
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public const int FirstName = 50;
            public const int LastName = 50;
            public const int EmailAddress = 256;
            public const int AddressLine = 100;
            public const int City = 100;
            public const int PostalCode = 25;
            public const int PhoneNumber = 25;

            public const int Username = 100;
            public const int Password = 100;
            public const int Token = 1024;
            public const int Url = 1024;
            public const int Text = int.MaxValue;
        }
    }
}
