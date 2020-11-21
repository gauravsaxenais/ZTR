namespace ZTR.Business.Test
{
    using Xunit;

    [CollectionDefinition(nameof(IntegrationFixture))]
    public sealed class IntegrationFixtureCollection : ICollectionFixture<IntegrationFixture>
    {
    }
}
