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

    public abstract class MasterKeyCommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>
        : CommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>, IMasterKeyCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TDbContext : DbContext
        where TErrorCode : struct, Enum
        where TEntity : class, IEntityWithId, IEntityWithMasterKey
        where TCreateModel : class, IModelWithMasterKey
        where TUpdateModel : class, TCreateModel, IModelWithId
    {
        protected MasterKeyCommandManager(TDbContext databaseContext, ModelValidator<TCreateModel> createModelValidator, ModelValidator<TUpdateModel> updateModelValidator, ILogger<MasterKeyCommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>> logger, IMapper mapper, TErrorCode idDoesNotExist, TErrorCode idNotUnique, TErrorCode masterKeyNotUnique)
            : base(databaseContext, createModelValidator, updateModelValidator, logger, mapper, idDoesNotExist, idNotUnique)
        {
            MasterKeyNotUnique = masterKeyNotUnique;
        }

        protected TErrorCode MasterKeyNotUnique { get; }

        public async Task<ManagerResponse<TErrorCode>> DeleteByMasterKeyAsync(Guid masterKey, params Guid[] masterKeys)
        {
            try
            {
                EnsureArg.IsNotDefault(masterKey, nameof(masterKey));

                return await DeleteByMasterKeyAsync(masterKeys.Prepend(masterKey)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByMasterKeyAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> DeleteByMasterKeyAsync(IEnumerable<Guid> masterKeys)
        {
            try
            {
                EnsureArg.IsNotNull(masterKeys, nameof(masterKeys));
                EnsureArgExtensions.HasItems(masterKeys, nameof(masterKeys));

                return await DeleteByExpressionAsync(masterKeys, x => masterKeys.Contains(x.MasterKey)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByMasterKeyAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public async Task<ManagerResponse<TErrorCode>> CreateIfNotExistByMasterKeyAsync(TCreateModel model, params TCreateModel[] models)
        {
            try
            {
                EnsureArg.IsNotNull(model, nameof(model));

                return await CreateIfNotExistByMasterKeyAsync(models.Prepend(model)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateIfNotExistByMasterKeyAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> CreateIfNotExistByMasterKeyAsync(IEnumerable<TCreateModel> models)
        {
            try
            {
                EnsureArg.IsNotNull(models, nameof(models));
                EnsureArgExtensions.HasItems(models, nameof(models));

                var indexedModels = models.ToIndexedItems().ToList();
                var modelMasterKeys = models.Select(y => y.MasterKey);

                // Validate set for duplicates
                var duplicateCodes = ValidationHelpers.DuplicateValidation(indexedModels, item => item.Item.MasterKey, MasterKeyNotUnique);
                if (duplicateCodes.Any())
                {
                    return new ManagerResponse<TErrorCode>(duplicateCodes);
                }

                var existingKeys = await DatabaseContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(x => modelMasterKeys.Contains(x.MasterKey))
                    .Select(x => x.MasterKey)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var missingIndexedModels = indexedModels.Where(x => !existingKeys.Select(y => y).Contains(x.Item.MasterKey)).ToList();

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

                var finalEntityIds = DatabaseContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(x => modelMasterKeys.Contains(x.MasterKey))
                    .OrderEntitiesByModelsOrder(models, entity => entity.MasterKey, entity => entity.Id, model => model.MasterKey);

                return new ManagerResponse<TErrorCode>(finalEntityIds);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateIfNotExistByMasterKeyAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public async Task<ManagerResponse<TErrorCode>> CreateOrUpdateByMasterKeyAsync(TUpdateModel model, params TUpdateModel[] models)
        {
            try
            {
                EnsureArg.IsNotNull(model, nameof(model));

                return await CreateOrUpdateByMasterKeyAsync(models.Prepend(model)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateOrUpdateByMasterKeyAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> CreateOrUpdateByMasterKeyAsync(IEnumerable<TUpdateModel> models)
        {
            try
            {
                EnsureArg.IsNotNull(models, nameof(models));
                EnsureArgExtensions.HasItems(models, nameof(models));

                var indexedModels = models.ToIndexedItems().ToList();
                var modelMasterKeys = indexedModels.Select(y => y.Item.MasterKey);
                var anyError = false;

                var existingEntities = await DatabaseContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(x => modelMasterKeys.Contains(x.MasterKey))
                    .Select(x => new { x.Id, x.MasterKey })
                    .ToListAsync()
                    .ConfigureAwait(false);

                var updateIndexedModels = indexedModels.Join(existingEntities, indexedModel => indexedModel.Item.MasterKey, entity => entity.MasterKey, (model, entity) =>
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

                var existingCodes = existingEntities.Select(y => y.MasterKey);

                var createIndexedModels = indexedModels.Where(x => !existingCodes.Contains(x.Item.MasterKey)).Cast<IIndexedItem<TCreateModel>>().ToList();
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
                    createdEntities.ForEach(x => indexedModels.Single(y => y.Item.MasterKey == x.MasterKey).Item.Id = x.Id);

                    var finalIds = indexedModels.Select(x => x.Item.Id);
                    return new ManagerResponse<TErrorCode>(finalIds);
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateOrUpdateByMasterKeyAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }
    }
}
