namespace DataAccess
{
#pragma warning disable CA1724
    public static class Constants
#pragma warning restore CA1724
    {
#pragma warning disable CA1034 // Nested types should not be visible

        public static class DataLengths
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public const int PasswordHash = 256;

            public const int SecurityStamp = 100;

            public const int SecretHash = 256;

            public const int ClientId = 200;

            public const int PersistentGrantKey = 64;

            public const int PersistentGrantSessionId = 40;

            public const int PersistentGrantData = int.MaxValue;

            public const int PersistentGrantType = 30;
        }
    }
}
