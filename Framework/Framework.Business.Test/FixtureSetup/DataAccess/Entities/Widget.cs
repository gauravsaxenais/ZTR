namespace ZTR.Framework.Business.Test.FixtureSetup.DataAccess.Entities
{
    using ZTR.Framework.DataAccess.Entities;

#pragma warning disable CA1724 // The type name Widget conflicts in whole or in part with the namespace

    public class Widget : EntityWithIdCodeNameDescription
#pragma warning restore CA1724 // The type name Widget conflicts in whole or in part with the namespace
    {
        public string SomeText { get; set; }

        public long ForeignKeyId { get; set; }
    }
}
