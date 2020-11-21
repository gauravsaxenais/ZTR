namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMasterKeyCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        : ICommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModelWithMasterKey
        where TUpdateModel : class, TCreateModel, IModelWithId
    {
        Task<ManagerResponse<TErrorCode>> DeleteByMasterKeyAsync(Guid masterKey, params Guid[] masterKeys);

        Task<ManagerResponse<TErrorCode>> DeleteByMasterKeyAsync(IEnumerable<Guid> masterKeys);

        Task<ManagerResponse<TErrorCode>> CreateIfNotExistByMasterKeyAsync(TCreateModel model, params TCreateModel[] models);

        Task<ManagerResponse<TErrorCode>> CreateIfNotExistByMasterKeyAsync(IEnumerable<TCreateModel> models);

        Task<ManagerResponse<TErrorCode>> CreateOrUpdateByMasterKeyAsync(TUpdateModel model, params TUpdateModel[] models);

        Task<ManagerResponse<TErrorCode>> CreateOrUpdateByMasterKeyAsync(IEnumerable<TUpdateModel> models);
    }
}
