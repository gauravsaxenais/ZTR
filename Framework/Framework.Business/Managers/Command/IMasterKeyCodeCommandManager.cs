namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMasterKeyCodeCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        : IMasterKeyCommandManager<TErrorCode, TCreateModel, TUpdateModel>
        where TErrorCode : struct, Enum
        where TCreateModel : class, IModelWithMasterKey, IModelWithCode
        where TUpdateModel : class, TCreateModel, IModelWithId
    {
        Task<ManagerResponse<TErrorCode>> DeleteByCodeAsync(string code, params string[] codes);

        Task<ManagerResponse<TErrorCode>> DeleteByCodeAsync(IEnumerable<string> codes);

        Task<ManagerResponse<TErrorCode>> CreateIfNotExistByCodeAsync(TCreateModel model, params TCreateModel[] models);

        Task<ManagerResponse<TErrorCode>> CreateIfNotExistByCodeAsync(IEnumerable<TCreateModel> models);

        Task<ManagerResponse<TErrorCode>> CreateOrUpdateByCodeAsync(TUpdateModel model, params TUpdateModel[] models);

        Task<ManagerResponse<TErrorCode>> CreateOrUpdateByCodeAsync(IEnumerable<TUpdateModel> models);
    }
}
