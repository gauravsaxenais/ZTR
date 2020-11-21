namespace ZTR.Framework.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.DataAccess;
    using ZTR.Framework.Test.Fakes;
    using Xunit;

    public abstract class CodeNameManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        : CodeManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        where TFixture : class
        where TCommandManager : ICodeCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TQueryManager : ICodeQueryManager<TReadModel>
        where TFaker : ModelFaker<TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModelWithCode, IModelWithName
        where TUpdateModel : class, TCreateModel, IModelWithId
        where TReadModel : class, TUpdateModel
    {
        private readonly TErrorCode _nameRequired;
        private readonly TErrorCode _nameTooLong;
        private readonly int _nameMaxLength;

        protected CodeNameManagerTest(
            ITestFixture<TFixture> integrationFixture,
            TErrorCode idDoesNotExist,
            TErrorCode idNotUnique,
            TErrorCode codeRequired,
            TErrorCode codeTooLong,
            TErrorCode codeNotUnique,
            TErrorCode nameRequired,
            TErrorCode nameTooLong,
            int codeMaxLength = BaseConstants.DataLengths.Code,
            int nameMaxLength = BaseConstants.DataLengths.Name)
            : base(integrationFixture, idDoesNotExist, idNotUnique, codeRequired, codeTooLong, codeNotUnique, codeMaxLength)
        {
            _nameRequired = nameRequired;
            _nameTooLong = nameTooLong;
            _nameMaxLength = nameMaxLength;
        }

        [Fact]
        public virtual async Task CreateWithEmptyNameShouldResultWithNameRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Name = string.Empty;

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_nameRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithNullNameShouldResultWithNameRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Name = null;

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_nameRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithNameTooLongShouldResultWithNameTooLongError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Name = new string('A', _nameMaxLength + 1);

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_nameTooLong, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithEmptyNameShouldResultWithNameRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Name = string.Empty;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_nameRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithNullNameShouldResultWithNameRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Name = null;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_nameRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithNameTooLongShouldResultWithNameTooLongError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Name = new string('A', _nameMaxLength + 1);

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_nameTooLong, allErrorCodes);
        }
    }
}
