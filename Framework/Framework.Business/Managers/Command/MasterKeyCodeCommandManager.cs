namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.DataAccess;
    using AutoMapper;
    using EnsureThat;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class MasterKeyCodeCommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>
        : MasterKeyCommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>, ICodeCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TDbContext : DbContext
        where TErrorCode : struct, Enum
        where TEntity : class, IEntityWithId, IEntityWithMasterKey, IEntityWithCode
        where TCreateModel : class, IModelWithMasterKey, IModelWithCode
        where TUpdateModel : class, TCreateModel, IModelWithId
    {
        protected MasterKeyCodeCommandManager(TDbContext databaseContext, ModelValidator<TCreateModel> createModelValidator, ModelValidator<TUpdateModel> updateModelValidator, ILogger<MasterKeyCodeCommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>> logger, IMapper mapper, TErrorCode idDoesNotExist, TErrorCode idNotUnique, TErrorCode masterKeyNotUnique, TErrorCode codeNotUnique)
            : base(databaseContext, createModelValidator, updateModelValidator, logger, mapper, idDoesNotExist, idNotUnique, masterKeyNotUnique)
        {
            CodeNotUnique = codeNotUnique;
        }

        protected TErrorCode CodeNotUnique { get; }

        public async Task<ManagerResponse<TErrorCode>> DeleteByCodeAsync(string code, params string[] codes)
        {
            try
            {
                EnsureArg.IsNotEmptyOrWhiteSpace(code, nameof(code));

                return await DeleteByCodeAsync(codes.Prepend(code)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByCodeAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> DeleteByCodeAsync(IEnumerable<string> codes)
        {
            try
            {
                EnsureArg.IsNotNull(codes, nameof(codes));
                EnsureArgExtensions.HasItems(codes, nameof(codes));

                return await DeleteByExpressionAsync(codes, x => codes.Contains(x.Code)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByCodeAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public async Task<ManagerResponse<TErrorCode>> CreateIfNotExistByCodeAsync(TCreateModel model, params TCreateModel[] models)
        {
            try
            {
                EnsureArg.IsNotNull(model, nameof(model));

                return await CreateIfNotExistByCodeAsync(models.Prepend(model)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateIfNotExistByCodeAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> CreateIfNotExistByCodeAsync(IEnumerable<TCreateModel> models)
        {
            try
            {
                EnsureArg.IsNotNull(models, nameof(models));
                EnsureArgExtensions.HasItems(models, nameof(models));

                var indexedModels = models.ToIndexedItems().ToList();
                var modelCodes = models.Select(y => y.Code);

                // Validate set for duplicates
                var duplicateCodes = ValidationHelpers.DuplicateValidation(indexedModels, item => item.Item.Code, CodeNotUnique);
                if (duplicateCodes.Any())
                {
                    return new ManagerResponse<TErrorCode>(duplicateCodes);
                }

                var existingKeys = await DatabaseContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(x => modelCodes.Contains(x.Code))
                    .Select(x => x.Code)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var missingIndexedModels = indexedModels.Where(x => !existingKeys.Select(y => y).Contains(x.Item.Code)).ToList();

                if (missingIndexedModels.Any())
                {
                    var errorRecords = CreateModelValidator.ExecuteCreateValidation<TErrorCode, TCreateModel>(missingIndexedModels);
                    var customErrorRecords = await CreateValidationAsync(missingIndexedModels).ConfigureAwait(false);

                    var mergedErrorRecords = errorRecords.Merge(customErrorRecords);

                    if (mergedErrorRecords.Any())
                    {
                        return new ManagerResponse<TErrorCode>(mergedErrorRecords);
                    }
                    else
                    {
                        var entities = new List<TEntity>();

                        Mapper.Map(missingIndexedModels.Select(x => x.Item), entities);
                        await CreateAfterMapAsync(missingIndexedModels, entities).ConfigureAwait(false);
                        DatabaseContext.AddRange(entities);

                        await DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                var finalEntitieIds = DatabaseContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(x => modelCodes.Contains(x.Code))
                    .OrderEntitiesByModelsOrder(models, entity => entity.Code, entity => entity.Id, model => model.Code);

                return new ManagerResponse<TErrorCode>(finalEntitieIds);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateIfNotExistByCodeAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public async Task<ManagerResponse<TErrorCode>> CreateOrUpdateByCodeAsync(TUpdateModel model, params TUpdateModel[] models)
        {
            try
            {
                EnsureArg.IsNotNull(model, nameof(model));

                return await CreateOrUpdateByCodeAsync(models.Prepend(model)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateOrUpdateByCodeAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> CreateOrUpdateByCodeAsync(IEnumerable<TUpdateModel> models)
        {
            try
            {
                EnsureArg.IsNotNull(models, nameof(models));
                EnsureArgExtensions.HasItems(models, nameof(models));

                var indexedModels = models.ToIndexedItems().ToList();
                var modelCodes = indexedModels.Select(y => y.Item.Code);
                var anyError = false;

                var existingEntities = await DatabaseContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(x => modelCodes.Contains(x.Code))
                    .Select(x => new { x.Id, x.Code })
                    .ToListAsync()
                    .ConfigureAwait(false);

                var updateIndexedModels = indexedModels.Join(existingEntities, indexedModel => indexedModel.Item.Code, entity => entity.Code, (model, entity) =>
                {
                    model.Item.Id = entity.Id;
                    return model;
                }).ToList();

                var errorRecords = Enumerable.Empty<ErrorRecord<TErrorCode>>();
                if (updateIndexedModels.Any())
                {
                    var updateErrorRecords = UpdateModelValidator.ExecuteUpdateValidation<TErrorCode, TUpdateModel>(updateIndexedModels);
                    var customUpdateErrorRecords = await UpdateValidationAsync(updateIndexedModels).ConfigureAwait(false);

                    errorRecords = errorRecords.Concat(updateErrorRecords).Concat(customUpdateErrorRecords);

                    if (errorRecords.Any())
                    {
                        anyError = true;
                    }
                    else
                    {
                        var updatedEntities = new List<TEntity>();

                        Mapper.Map(updateIndexedModels.Select(x => x.Item), updatedEntities);
                        await UpdateAfterMapAsync(updateIndexedModels, updatedEntities).ConfigureAwait(false);
                        DatabaseContext.UpdateRange(updatedEntities);
                    }
                }

                var existingCodes = existingEntities.Select(y => y.Code);

                var createIndexedModels = indexedModels.Where(x => !existingCodes.Contains(x.Item.Code)).Cast<IIndexedItem<TCreateModel>>().ToList();
                var createdEntities = new List<TEntity>();
                if (createIndexedModels.Any())
                {
                    var createErrorRecords = CreateModelValidator.ExecuteCreateValidation<TErrorCode, TCreateModel>(createIndexedModels);
                    var customCreateErrorRecords = await CreateValidationAsync(createIndexedModels).ConfigureAwait(false);

                    errorRecords = errorRecords.Concat(createErrorRecords).Concat(customCreateErrorRecords);

                    if (errorRecords.Any())
                    {
                        anyError = true;
                    }
                    else
                    {
                        Mapper.Map(createIndexedModels.Select(x => x.Item), createdEntities);
                        await CreateAfterMapAsync(createIndexedModels, createdEntities).ConfigureAwait(false);
                        DatabaseContext.AddRange(createdEntities);
                    }
                }

                if (anyError)
                {
                    var mergedErrors = errorRecords.Merge();

                    return new ManagerResponse<TErrorCode>(new ErrorRecords<TErrorCode>(mergedErrors));
                }
                else
                {
                    await DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    // update the models id
                    // must be after the save so the ids are correct
                    createdEntities.ForEach(x => indexedModels.Single(y => y.Item.Code == x.Code).Item.Id = x.Id);

                    var finalIds = indexedModels.Select(x => x.Item.Id);
                    return new ManagerResponse<TErrorCode>(finalIds);
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateOrUpdateByCodeAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        protected override async Task<ErrorRecords<TErrorCode>> CreateValidationAsync(IList<IIndexedItem<TCreateModel>> indexedModels)
        {
            Logger.LogDebug($"Calling {nameof(CreateValidationAsync)}");

            var baseErrorRecords = await base.CreateValidationAsync(indexedModels).ConfigureAwait(false);
            var uniqueErrorRecords = await ValidationHelpers.UniqueValidationAsync(
                async (keys) =>
                {
                    return await DatabaseContext.Set<TEntity>()
                        .AsNoTracking()
                        .Where(x => keys.Contains(x.Code))
                        .Select(x => new IdKey<string>(x.Id, x.Code))
                        .ToListAsync().ConfigureAwait(false);
                },
                indexedModels,
                x => x.Item.Code,
                CodeNotUnique).ConfigureAwait(false);

            return new ErrorRecords<TErrorCode>(baseErrorRecords.Concat(uniqueErrorRecords));
        }

        protected override async Task<ErrorRecords<TErrorCode>> UpdateValidationAsync(IList<IIndexedItem<TUpdateModel>> indexedModels)
        {
            Logger.LogDebug($"Calling {nameof(UpdateValidationAsync)}");

            var baseErrorRecords = await base.UpdateValidationAsync(indexedModels).ConfigureAwait(false);
            var uniqueErrorRecords = await ValidationHelpers.UniqueWithIdValidationAsync(
                async (keys) =>
                {
                    return await DatabaseContext.Set<TEntity>()
                        .AsNoTracking()
                        .Where(x => keys.Contains(x.Code))
                        .Select(x => new IdKey<string>(x.Id, x.Code))
                        .ToListAsync().ConfigureAwait(false);
                },
                indexedModels,
                x => x.Item.Code,
                CodeNotUnique).ConfigureAwait(false);

            return new ErrorRecords<TErrorCode>(baseErrorRecords.Concat(uniqueErrorRecords));
        }
    }
}
