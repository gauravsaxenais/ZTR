namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DataAccess;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class CommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel> : BaseCommandManager<TDbContext, TErrorCode, TEntity>, ICommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TDbContext : DbContext
        where TErrorCode : struct, Enum
        where TEntity : class, IEntityWithId
        where TCreateModel : class, IModel
        where TUpdateModel : class, TCreateModel, IModelWithId
    {
        protected CommandManager(TDbContext databaseContext, ModelValidator<TCreateModel> createModelValidator, ModelValidator<TUpdateModel> updateModelValidator, ILogger<CommandManager<TDbContext, TErrorCode, TEntity, TCreateModel, TUpdateModel>> logger, IMapper mapper, TErrorCode idDoesNotExist, TErrorCode idNotUnique)
            : base(databaseContext, logger)
        {
            EnsureArg.IsNotNull(createModelValidator, nameof(createModelValidator));
            EnsureArg.IsNotNull(updateModelValidator, nameof(updateModelValidator));
            EnsureArg.IsNotNull(mapper, nameof(mapper));

            CreateModelValidator = createModelValidator;
            UpdateModelValidator = updateModelValidator;
            Mapper = mapper;
            IdDoesNotExist = idDoesNotExist;
            IdNotUnique = idNotUnique;
        }

        protected TErrorCode IdDoesNotExist { get; }

        protected TErrorCode IdNotUnique { get; }

        protected IMapper Mapper { get; }

        protected ModelValidator<TCreateModel> CreateModelValidator { get; }

        protected ModelValidator<TUpdateModel> UpdateModelValidator { get; }

        public async Task<ManagerResponse<TErrorCode>> CreateAsync(TCreateModel model, params TCreateModel[] models)
        {
            try
            {
                EnsureArg.IsNotNull(model, nameof(model));

                return await CreateAsync(models.Prepend(model)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> CreateAsync(IEnumerable<TCreateModel> models)
        {
            try
            {
                ValidateModel(models);
                var indexedModels = models.ToIndexedItems().ToList();
                var errorRecords = CreateModelValidator.ExecuteCreateValidation<TErrorCode, TCreateModel>(indexedModels);
                var customErrorRecords = await CreateValidationAsync(indexedModels).ConfigureAwait(false);

                return await CreateOrUpdateAsync(models, errorRecords, customErrorRecords, async entities =>
                {
                    await CreateAfterMapAsync(indexedModels, entities).ConfigureAwait(false);
                    DatabaseContext.AddRange(entities);
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(CreateAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public async Task<ManagerResponse<TErrorCode>> UpdateAsync(TUpdateModel model, params TUpdateModel[] models)
        {
            try
            {
                EnsureArg.IsNotNull(model, nameof(model));

                return await UpdateAsync(models.Prepend(model).ToArray()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(UpdateAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> UpdateAsync(IEnumerable<TUpdateModel> models)
        {
            try
            {
                ValidateModel(models);
                var indexedModels = models.ToIndexedItems().ToList();
                var errorRecords = UpdateModelValidator.ExecuteUpdateValidation<TErrorCode, TUpdateModel>(indexedModels);
                var customErrorRecords = await UpdateValidationAsync(indexedModels).ConfigureAwait(false);

                return await CreateOrUpdateAsync(models, errorRecords, customErrorRecords, async entities =>
                {
                    await UpdateAfterMapAsync(indexedModels, entities).ConfigureAwait(false);
                    DatabaseContext.UpdateRange(entities);
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(UpdateAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public async Task<ManagerResponse<TErrorCode>> DeleteByIdAsync(long id, params long[] ids)
        {
            try
            {
                EnsureArg.IsGt(id, 0, nameof(id));

                return await DeleteByIdAsync(ids.Prepend(id)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByIdAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        public virtual async Task<ManagerResponse<TErrorCode>> DeleteByIdAsync(IEnumerable<long> ids)
        {
            try
            {
                EnsureArg.IsNotNull(ids, nameof(ids));
                EnsureArgExtensions.HasItems(ids, nameof(ids));

                return await DeleteByExpressionAsync(ids, x => ids.Contains(x.Id)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByIdAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        protected virtual async Task<ErrorRecords<TErrorCode>> CreateValidationAsync(IList<IIndexedItem<TCreateModel>> indexedModels)
        {
            Logger.LogDebug($"Calling {nameof(CreateValidationAsync)}");

            return await Task.FromResult(new ErrorRecords<TErrorCode>()).ConfigureAwait(false);
        }

        protected virtual async Task<ErrorRecords<TErrorCode>> UpdateValidationAsync(IList<IIndexedItem<TUpdateModel>> indexedModels)
        {
            Logger.LogDebug($"Calling {nameof(UpdateValidationAsync)}");

            var existsValidationError = await ValidationHelpers.ExistsValidationAsync(
                async (keys) =>
                {
                    var modelIds = keys.Select(x => x).ToList();

                    return await DatabaseContext.Set<TEntity>()
                        .Where(x => modelIds.Contains(x.Id))
                        .Select(x => new IdKey<long>(x.Id, x.Id))
                        .ToListAsync().ConfigureAwait(false);
                },
                indexedModels,
                x => x.Item.Id,
                IdDoesNotExist).ConfigureAwait(false);

            var duplicateErrorIds = ValidationHelpers.DuplicateValidation(indexedModels, item => item.Item.Id, IdNotUnique);
            return new ErrorRecords<TErrorCode>(existsValidationError.Concat(duplicateErrorIds));
        }

        protected virtual Task CreateAfterMapAsync(IList<IIndexedItem<TCreateModel>> indexedItems, IList<TEntity> entities)
        {
            return Task.CompletedTask;
        }

        protected virtual Task UpdateAfterMapAsync(IList<IIndexedItem<TUpdateModel>> indexedItems, IList<TEntity> entities)
        {
            return Task.CompletedTask;
        }

        private async Task<ManagerResponse<TErrorCode>> CreateOrUpdateAsync<TModel>(IEnumerable<TModel> models, ErrorRecords<TErrorCode> errorRecords, IEnumerable<ErrorRecord<TErrorCode>> customErrorRecords, Func<List<TEntity>, Task> entityOperationsPreDatabaseCalling)
        {
            var mergedErrorRecords = errorRecords.Merge(customErrorRecords);

            if (mergedErrorRecords.Any())
            {
                return new ManagerResponse<TErrorCode>(mergedErrorRecords);
            }

            var entities = new List<TEntity>();
            Mapper.Map(models, entities);
            await entityOperationsPreDatabaseCalling(entities).ConfigureAwait(false);
            await DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
            return new ManagerResponse<TErrorCode>(entities.Select(x => x.Id));
        }

        private void ValidateModel<TModel>(IEnumerable<TModel> models)
        {
            EnsureArg.IsNotNull(models, nameof(models));
            EnsureArgExtensions.HasItems(models, nameof(models));
        }
    }
}
