namespace ZTR.Framework.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.DataAccess;
    using ZTR.Framework.Test.Fakes;
    using FluentAssertions;
    using Xunit;

    public abstract class CodeManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        : ManagerTest<TFixture, TCommandManager, TQueryManager, TFaker, TErrorCode, TCreateModel, TUpdateModel, TReadModel>
        where TFixture : class
        where TCommandManager : ICodeCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TQueryManager : ICodeQueryManager<TReadModel>
        where TFaker : ModelFaker<TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModelWithCode
        where TUpdateModel : class, TCreateModel, IModelWithId
        where TReadModel : class, TUpdateModel
    {
        private readonly TErrorCode _codeRequired;
        private readonly TErrorCode _codeTooLong;
        private readonly TErrorCode _codeNotUnique;

        private readonly int _codeMaxLength;

        protected CodeManagerTest(
            ITestFixture<TFixture> integrationFixture,
            TErrorCode idDoesNotExist,
            TErrorCode idNotUnique,
            TErrorCode codeRequired,
            TErrorCode codeTooLong,
            TErrorCode codeNotUnique,
            int codeMaxLength = BaseConstants.DataLengths.Code)
            : base(integrationFixture, idDoesNotExist, idNotUnique)
        {
            _codeRequired = codeRequired;
            _codeTooLong = codeTooLong;
            _codeMaxLength = codeMaxLength;
            _codeNotUnique = codeNotUnique;
        }

        [Fact]
        public virtual async Task CreateWithNonUniqueCodeShouldResultWithCodeNotUniqueError()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false);
            createModels[1].Code = createModels[0].Code;

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);

            Assert.True(createResult.HasError);

            var inAllErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeNotUnique, inAllErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithAlreadyExistingCodeShouldResultWithCodeNotUniqueError()
        {
            var createFirstModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var existingCode = createFirstModel.Code;

            var createFirstResult = await CreateCommandManager.CreateAsync(createFirstModel).ConfigureAwait(false);
            createFirstResult.ThrowIfError();

            var createSecondModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createSecondModel.Code = existingCode;

            var createSecondResult = await CreateCommandManager.CreateAsync(createSecondModel).ConfigureAwait(false);

            var inAllErrorCodes = createSecondResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeNotUnique, inAllErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithEmptyCodeShouldResultWithCodeRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Code = string.Empty;

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithNullCodeShouldResultWithCodeRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Code = null;

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task CreateWithCodeTooLongShouldResultWithCodeTooLongError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModel.Code = new string('A', _codeMaxLength + 1);

            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);

            var allErrorCodes = createResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeTooLong, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithCodeFromExistingEntityShouldResultWithCodeNotUniqueError()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            var firstCreatedAppModuleId = createResult.Ids[0];
            var secondCreatedAppModuleId = createResult.Ids[1];

            // Try updating 2nd app mod using code value from the 1st app mod.
            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = secondCreatedAppModuleId;
            updateModel.Code = createModels[0].Code;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeNotUnique, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithEmptyCodeShouldResultWithCodeRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Code = string.Empty;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithNullCodeShouldResultWithCodeRequiredError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Code = null;

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeRequired, allErrorCodes);
        }

        [Fact]
        public virtual async Task UpdateWithCodeTooLongShouldResultWithCodeTooLongError()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResult = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            createResult.ThrowIfError();

            var updateModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            updateModel.Id = createResult.Ids.Single();
            updateModel.Code = new string('A', _codeMaxLength + 1);

            var updateResult = await UpdateCommandManager.UpdateAsync(updateModel).ConfigureAwait(false);

            var allErrorCodes = updateResult.ErrorRecords.SelectMany(x => x.Errors).Select(x => x.ErrorCode);
            Assert.Contains(_codeTooLong, allErrorCodes);
        }

        [Fact]
        public virtual async Task GetByCodeAsync()
        {
            const int itemCount = 2;

            var createModels = await Faker.GenerateWithDependenciesAsync(itemCount).ConfigureAwait(false);
            var codesToCreate = createModels.Select(x => x.Code);

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdIds = createResult.Ids;
            var idResult = await QueryManager.GetByIdAsync(createdIds).ConfigureAwait(false);
            var codeResult = await QueryManager.GetByCodeAsync(codesToCreate).ConfigureAwait(false);

            Assert.Equal(idResult.Count(), itemCount);
            Assert.Equal(codeResult.Count(), itemCount);

            AssertExtensions.EquivalentWithMissingMembersIgnoreId(idResult, createModels);
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(codeResult, createModels);
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(idResult, codeResult);
        }

        [Fact]
        public virtual async Task DeleteByCodeAsync()
        {
            const int itemCount = 5;

            var createModels = await Faker.GenerateWithDependenciesAsync(itemCount).ConfigureAwait(false);

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdCodes = createModels.Select(x => x.Code).ToArray();
            var selectedResults = await QueryManager.GetByCodeAsync(createdCodes).ConfigureAwait(false);

            // verify all the data was created correctly
            Assert.Equal(itemCount, selectedResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(selectedResults, createModels);

            var firstDeleteResult = await DeleteCommandManager.DeleteByCodeAsync(createdCodes[0]).ConfigureAwait(false);
            firstDeleteResult.ThrowIfError();

            var getAfterFirstDeleteResults = await QueryManager.GetByCodeAsync(createdCodes).ConfigureAwait(false);

            Assert.Equal(itemCount - 1, getAfterFirstDeleteResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getAfterFirstDeleteResults, createModels.Skip(1));

            var secondDeleteResult = await DeleteCommandManager.DeleteByCodeAsync(createdCodes.Skip(1).TakeLast(2)).ConfigureAwait(false);
            secondDeleteResult.ThrowIfError();

            var getAfterSecondDeleteResults = await QueryManager.GetByCodeAsync(createdCodes).ConfigureAwait(false);

            Assert.Equal(itemCount - 3, getAfterSecondDeleteResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getAfterSecondDeleteResults, createModels.Skip(1).Take(2));
        }

        [Fact]
        public virtual async Task CreateIfNotExistAsync()
        {
            const int createdCount = 3;
            const int createOrUpdateCount = 2;
            var allModelsCount = createdCount + createOrUpdateCount;

            var createModels = await Faker.GenerateWithDependenciesAsync(createdCount).ConfigureAwait(false);
            var createOrUpdateModels = await Faker.GenerateWithDependenciesAsync(createOrUpdateCount).ConfigureAwait(false);

            var createResult = await CreateCommandManager.CreateAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var createdCodes = createModels.Select(x => x.Code);
            var createdResults = await QueryManager.GetByCodeAsync(createdCodes).ConfigureAwait(false);

            // verify all the data was created correctly
            Assert.Equal(createdCount, createdResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(createModels, createdResults);

            var allModels = new List<TUpdateModel>(createdResults.Union(createOrUpdateModels));

            var createIfNotExistResult = await CreateCommandManager.CreateIfNotExistByCodeAsync(allModels).ConfigureAwait(false);
            createIfNotExistResult.ThrowIfError();

            var getAfterCreateIfNotExistResults = await QueryManager.GetByCodeAsync(allModels.Select(x => x.Code)).ConfigureAwait(false);

            // set all the ids to 0 for comparison since I can't find an easier way to ignore it
            foreach (var item in getAfterCreateIfNotExistResults)
            {
                item.Id = 0;
            }

            foreach (var item in allModels)
            {
                item.Id = 0;
            }

            Assert.Equal(allModelsCount, getAfterCreateIfNotExistResults.Count());
            AssertExtensions.EquivalentWithMissingMembersIgnoreId(getAfterCreateIfNotExistResults, allModels);
        }

        [Fact]
        public virtual async Task CreateIfNotExistByCodeCallingTwiceShouldSucceedAsync()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(1).ConfigureAwait(false);
            var adding2MoreModels = createModels.Union(await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false));

            var createResult = CreateCommandManager.CreateIfNotExistByCodeAsync(createModels).Result;
            createResult.ThrowIfError();

            var callingAgain = await CreateCommandManager.CreateIfNotExistByCodeAsync(adding2MoreModels).ConfigureAwait(false);
            callingAgain.ThrowIfError();

            createResult.Ids.Count().Should().Be(createModels.Count());
            callingAgain.Ids.Count().Should().Be(adding2MoreModels.Count());
        }

        [Fact]
        public virtual async Task CreateIfNotExistAsyncWithADuplicateListResultWithCodeNotUniqueAsync()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(5).ConfigureAwait(false);
            var duplicateCode = createModels.First().Code;
            createModels.ForEach(x => x.Code = duplicateCode);

            var result = await CreateCommandManager.CreateIfNotExistByCodeAsync(createModels).ConfigureAwait(false);
            Assert.True(result.HasError);

            var errorCode = result.ErrorRecords.SelectMany(x => x.Errors.Where(y => y.ErrorCode.Equals(_codeNotUnique)).Select(y => y.ErrorCode));
            result.ErrorRecords.Count().Should().Be(createModels.Count - 1);
            errorCode.Count().Should().Be(createModels.Count - 1);
            AssertExtensions.ContainsErrorCode(result.ErrorRecords, _codeNotUnique);
        }

        [Fact]
        public virtual async Task CreateIfNotExistByCodeShouldResultWithDuplicateCodeAsync()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResultOne = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            Assert.False(createResultOne.HasError);

            var modelCollection = new[] { createModel, createModel, createModel };
            var result = await CreateCommandManager.CreateIfNotExistByCodeAsync(modelCollection).ConfigureAwait(false);
            Assert.True(result.HasError);

            var errorCode = result.ErrorRecords.SelectMany(x => x.Errors.Where(y => y.ErrorCode.Equals(_codeNotUnique)).Select(y => y.ErrorCode));
            result.ErrorRecords.Count().Should().Be(modelCollection.Count() - 1);
            errorCode.Count().Should().Be(modelCollection.Count() - 1);
            AssertExtensions.ContainsErrorCode(result.ErrorRecords, _codeNotUnique);
        }

        [Fact]
        public virtual async Task CreateOrUpdateByCodeCallingTwiceShouldSucceedAsync()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(3).ConfigureAwait(false);
            var addingMoreModels = createModels.Union(await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false));

            var createResult = await CreateCommandManager.CreateOrUpdateByCodeAsync(createModels).ConfigureAwait(false);
            createResult.ThrowIfError();

            var addingMoreModelResult = await UpdateCommandManager.CreateOrUpdateByCodeAsync(addingMoreModels).ConfigureAwait(false);
            addingMoreModelResult.ThrowIfError();

            createResult.Ids.Count().Should().Be(createModels.Count());
            addingMoreModelResult.Ids.Count().Should().Be(addingMoreModels.Count());
        }

        [Fact]
        public virtual async Task CreateWithADuplicateListResultWithCodeNotUniqueAsync()
        {
            var createOrUpdateModels = await Faker.GenerateWithDependenciesAsync(5).ConfigureAwait(false);
            var duplicateCode = createOrUpdateModels.First().Code;
            createOrUpdateModels.ForEach(x => x.Code = duplicateCode);

            var result = await CreateCommandManager.CreateAsync(createOrUpdateModels).ConfigureAwait(false);
            Assert.True(result.HasError);

            var errorCode = result.ErrorRecords.SelectMany(x => x.Errors.Where(y => y.ErrorCode.Equals(_codeNotUnique)).Select(y => y.ErrorCode));
            result.ErrorRecords.Count().Should().Be(createOrUpdateModels.Count - 1);
            errorCode.Count().Should().Be(createOrUpdateModels.Count - 1);
            AssertExtensions.ContainsErrorCode(result.ErrorRecords, _codeNotUnique);
        }

        [Fact]
        public virtual async Task CreateOrUpdateByCodeWithADuplicateListResultWithCodeNotUniqueAsync()
        {
            var createOrUpdateModels = await Faker.GenerateWithDependenciesAsync(5).ConfigureAwait(false);
            var duplicateCode = "CreateOrUpdate";
            createOrUpdateModels.ForEach(x => x.Code = duplicateCode);

            var result = await CreateCommandManager.CreateOrUpdateByCodeAsync(createOrUpdateModels).ConfigureAwait(false);
            Assert.True(result.HasError);

            var errorCode = result.ErrorRecords.SelectMany(x => x.Errors.Where(y => y.ErrorCode.Equals(_codeNotUnique)).Select(y => y.ErrorCode));
            result.ErrorRecords.Count().Should().Be(createOrUpdateModels.Count - 1);
            errorCode.Count().Should().Be(createOrUpdateModels.Count - 1);
            AssertExtensions.ContainsErrorCode(result.ErrorRecords, _codeNotUnique);
        }

        [Fact]
        public virtual async Task CreateOrUpdateByCodeShouldResultWithCodeNotUniqueAsync()
        {
            var createModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            var createResultOne = await CreateCommandManager.CreateAsync(createModel).ConfigureAwait(false);
            Assert.False(createResultOne.HasError);

            var modelCollection = new[] { createModel, createModel, createModel };
            var result = await CreateCommandManager.CreateOrUpdateByCodeAsync(modelCollection).ConfigureAwait(false);
            Assert.True(result.HasError);
            var errorCode = result.ErrorRecords.SelectMany(x => x.Errors.Where(y => y.ErrorCode.Equals(_codeNotUnique)).Select(y => y.ErrorCode));
            result.ErrorRecords.Count().Should().Be(modelCollection.Count() - 1);
            errorCode.Count().Should().Be(modelCollection.Count() - 1);
            AssertExtensions.ContainsErrorCode(result.ErrorRecords, _codeNotUnique);
        }

        [Fact]
        public virtual async Task CreateIfNotExistShouldResultWithSameOrderAsync()
        {
            const int resultLength = 3;
            var createModels = await Faker.GenerateWithDependenciesAsync(resultLength).ConfigureAwait(false);
            var result = await CreateCommandManager.CreateIfNotExistByCodeAsync(createModels).ConfigureAwait(false);
            result.ThrowIfError();

            for (int i = 0; i < result.Ids.Length; i++)
            {
                createModels[i].Id = result.Ids[i];
            }

            var model1 = createModels[1];
            var model2 = createModels[2];
            createModels[2] = model1;
            createModels[1] = model2;

            var result2 = await CreateCommandManager.CreateIfNotExistByCodeAsync(createModels).ConfigureAwait(false);

            result2.ThrowIfError();
            Assert.True(result2.Ids.Length == resultLength);
            Assert.True(result2.Ids[2] == result.Ids[1]);
            Assert.True(result2.Ids[1] == result.Ids[2]);
        }

        [Fact]
        public virtual async Task CreateOrUpdateShouldResultWithSameOrderAsync()
        {
            var createModels = await Faker.GenerateWithDependenciesAsync(2).ConfigureAwait(false);

            var result = await CreateCommandManager.CreateOrUpdateByCodeAsync(createModels).ConfigureAwait(false);
            result.ThrowIfError();

            var rogueModel = await Faker.GenerateWithDependenciesAsync().ConfigureAwait(false);
            createModels.Insert(1, rogueModel);

            var result2 = await UpdateCommandManager.CreateOrUpdateByCodeAsync(createModels).ConfigureAwait(false);

            result2.ThrowIfError();
            Assert.True(result2.Ids[1] == rogueModel.Id);
        }
    }
}
