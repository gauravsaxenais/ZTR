namespace ZTR.Framework.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.Test.Fakes;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class ManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        : BaseTest<TFixture>
        where TFixture : class
        where TCommandManager : ICommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TQueryManager : IQueryManager<TReadModel>
        where TFaker : ModelFaker<TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModel
        where TUpdateModel : class, TCreateModel, IModelWithId
        where TReadModel : class, TUpdateModel
    {
        protected ManagerTest(ITestFixture<TFixture> integrationFixture, TErrorCode idDoesNotExist, TErrorCode idNotUnique)
            : base(integrationFixture)
        {
            IdDoesNotExist = idDoesNotExist;
            IdNotUnique = idNotUnique;
            CreateCommandManager = TestFixture.ServiceProvider.GetRequiredService<TCommandManager>();
            DeleteCommandManager = TestFixture.ServiceProvider.GetRequiredService<TCommandManager>();
            UpdateCommandManager = TestFixture.ServiceProvider.GetRequiredService<TCommandManager>();
            QueryManager = TestFixture.ServiceProvider.GetRequiredService<TQueryManager>();
            Faker = TestFixture.ServiceProvider.GetRequiredService<TFaker>();
        }

        protected TErrorCode IdDoesNotExist { get; }

        protected TErrorCode IdNotUnique { get; }

        protected TCommandManager CreateCommandManager { get; }

        protected TCommandManager DeleteCommandManager { get; }

        protected TFaker Faker { get; }

        protected TQueryManager QueryManager { get; }

        protected TCommandManager UpdateCommandManager { get; }

        [Fact]
        public virtual async Task CreateAndUpdateAsync()
        {
            var oneCreateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);

            var createResult = await CreateCommandManager.CreateAsync(oneCreateModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdId = createResult.Ids.Single();

            var getCreateResult = await QueryManager.GetByIdAsync(createdId).ConfigureAwait(false);

            var oneUpdateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            oneUpdateModel.Id = createdId;

            var updateResult = await UpdateCommandManager.UpdateAsync(oneUpdateModel).ConfigureAwait(false);
            updateResult.ThrowIfError();

            var getUpdateResult = await QueryManager.GetByIdAsync(createdId).ConfigureAwait(false);

            Assert.Single(getCreateResult);
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getCreateResult.Single(), oneCreateModel);
            Assert.Single(getUpdateResult);
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getUpdateResult.Single(), oneUpdateModel);
            Assert.Equal(getCreateResult.Single().Id, getUpdateResult.Single().Id);
        }

        [Fact]
        public virtual async Task GetAllAsync()
        {
            const int itemCount = 5;

            var createModels = await Faker.GenerateWithDependenciesAsync(itemCount).ConfigureAwait(false);

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdIds = createResult.Ids;

            var getAllResult = await QueryManager.GetAllAsync().ConfigureAwait(false);

            var selectedResults = getAllResult.Where(x => createdIds.Contains(x.Id)).ToList();

            Assert.True(getAllResult.Count() >= itemCount);
            Assert.Equal(itemCount, selectedResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(selectedResults, createModels);
        }

        [Fact]
        public virtual async Task GetByIdAsync()
        {
            const int itemCount = 2;

            var createModels = await Faker.GenerateWithDependenciesAsync(itemCount).ConfigureAwait(false);

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdIds = createResult.Ids;

            var getResult = await QueryManager.GetByIdAsync(createdIds).ConfigureAwait(false);

            Assert.Equal(getResult.Count(), itemCount);
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getResult, createModels);
        }

        [Fact]
        public virtual async Task DeleteByIdAsync()
        {
            const int itemCount = 5;

            var createModels = await Faker.GenerateWithDependenciesAsync(itemCount).ConfigureAwait(false);

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdIds = createResult.Ids;
            var selectedResults = await QueryManager.GetByIdAsync(createdIds).ConfigureAwait(false);

            // verify all the data was created correctly
            Assert.Equal(itemCount, selectedResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(selectedResults, createModels);

            var firstDeleteResult = await DeleteCommandManager.DeleteByIdAsync(createdIds[0]).ConfigureAwait(false);
            firstDeleteResult.ThrowIfError();

            var getAfterFirstDeleteResults = await QueryManager.GetByIdAsync(createdIds).ConfigureAwait(false);

            // verify all the data was created correctly
            Assert.Equal(itemCount - 1, getAfterFirstDeleteResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getAfterFirstDeleteResults, createModels.Skip(1));

            var secondDeleteResult = await DeleteCommandManager.DeleteByIdAsync(createdIds.Skip(1).TakeLast(2)).ConfigureAwait(false);
            secondDeleteResult.ThrowIfError();

            var getAfterSecondDeleteResults = await QueryManager.GetByIdAsync(createdIds).ConfigureAwait(false);

            // verify all the data was created correctly
            Assert.Equal(itemCount - 3, getAfterSecondDeleteResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getAfterSecondDeleteResults, createModels.Skip(1).Take(2));
        }

        [Fact]
        public virtual async Task UpdateWithNonExistingIdShouldThrowInvalidId()
        {
            var updateModels = await Faker.GenerateWithDependenciesAsync(4).ConfigureAwait(false);
            var i = 1000;
            updateModels.ForEach(x => x.Id = i++);

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModels).ConfigureAwait(false);
            Assert.True(updateResult.HasError);
            AssertExtensions.ContainsErrorCode(updateResult.ErrorRecords, IdDoesNotExist);
        }

        [Fact]
        public virtual async Task UpdateWithDuplicateIdFromExistingEntityShouldResultWithIdNotUniqueError()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            var firstCreatedId = createResult.Ids[0];
            var secondCreatedId = createResult.Ids[1];

            // Try updating 2nd item using id value from the 1st item.
            var updateModel = await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false);
            foreach (var item in updateModel)
            {
                item.Id = firstCreatedId;
            }

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(IdNotUnique, allErrorCodes);
        }
    }
}
