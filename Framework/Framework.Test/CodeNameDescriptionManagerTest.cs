namespace ZTR.Framework.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.DataAccess;
    using ZTR.Framework.Test.Fakes;
    using Xunit;

    public abstract class CodeNameDescriptionManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        : CodeNameManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        where TFixture : class
        where TCommandManager : ICodeCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TQueryManager : ICodeQueryManager<TReadModel>
        where TFaker : ModelFaker<TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModelWithCode, IModelWithName, IModelWithDescription
        where TUpdateModel : class, TCreateModel, IModelWithId
        where TReadModel : class, TUpdateModel
    {
        private readonly TErrorCode _descriptionRequired;
        private readonly TErrorCode _descriptionTooLong;
        private readonly int _descriptionMaxLength;

        protected CodeNameDescriptionManagerTest(
            ITestFixture<TFixture> integrationFixture,
            TErrorCode idDoesNotExist,
            TErrorCode idNotUnique,
            TErrorCode codeRequired,
            TErrorCode codeTooLong,
            TErrorCode codeNotUnique,
            TErrorCode nameRequired,
            TErrorCode nameTooLong,
            TErrorCode descriptionRequired,
            TErrorCode descriptionTooLong,
            int codeMaxLength = BaseConstants.DataLengths.Code,
            int nameMaxLength = BaseConstants.DataLengths.Name,
            int descriptionMaxLength = BaseConstants.DataLengths.Description)
            : base(integrationFixture, idDoesNotExist, idNotUnique, codeRequired, codeTooLong, codeNotUnique, nameRequired, nameTooLong, codeMaxLength, nameMaxLength)
        {
            _descriptionTooLong = descriptionTooLong;
            _descriptionMaxLength = descriptionMaxLength;
            _descriptionRequired = descriptionRequired;
        }

        [Fact]
        public async Task CreateWithEmptyDescriptionShouldResultWithDescriptionRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Description = string.Empty;

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_descriptionRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithNullDescriptionShouldResultWithDescriptionRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Description = null;

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_descriptionRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithEmptyDescriptionShouldResultWithDescriptionRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Description = string.Empty;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_descriptionRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithNullDescriptionShouldResultWithDescriptionRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Description = null;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_descriptionRequired, allErrorCodes);
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
