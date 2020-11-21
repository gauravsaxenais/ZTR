namespace ZTR.Framework.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.DataAccess;
    using ZTR.Framework.Test.Fakes;
    using Xunit;

    public abstract class NameDescriptionManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        : NameManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>,
        IClassFixture<TFixture>
        where TFixture : class
        where TCommandManager : ICommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TQueryManager : IQueryManager<TReadModel>
        where TFaker : ModelFaker<TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModelWithName, IModelWithDescription
        where TUpdateModel : class, TCreateModel, IModelWithId
        where TReadModel : class, TUpdateModel
    {
        private readonly TErrorCode _descriptionTooLong;
        private readonly int _descriptionMaxLength;

        protected NameDescriptionManagerTest(
            ITestFixture<TFixture> integrationFixture,
            TErrorCode idDoesNotExist,
            TErrorCode idNotUnique,
            TErrorCode nameRequired,
            TErrorCode nameTooLong,
            TErrorCode descriptionTooLong,
            int nameMaxLength = BaseConstants.DataLengths.Name,
            int descriptionMaxLength = BaseConstants.DataLengths.Description)
            : base(integrationFixture, idDoesNotExist, idNotUnique, nameRequired, nameTooLong, nameMaxLength)
        {
            _descriptionTooLong = descriptionTooLong;
            _descriptionMaxLength = descriptionMaxLength;
        }

        [Fact]
        public virtual async Task CreateWithDescriptionTooLongShouldResultWithDescriptionTooLongError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Description = new string('A', _descriptionMaxLength + 1);

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_descriptionTooLong, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithDescriptionTooLongShouldResultWithDescriptionTooLongError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Description = new string('A', _descriptionMaxLength + 1);

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_descriptionTooLong, allErrorCodes);
        }
    }
}
