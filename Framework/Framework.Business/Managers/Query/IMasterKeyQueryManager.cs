namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMasterKeyQueryManager<TReadModel> : IQueryManager<TReadModel>
        where TReadModel : class, IModelWithMasterKey
    {
        Task<IEnumerable<TReadModel>> GetByMasterKeyAsync(Guid masterKey, params Guid[] masterKeys);

        Task<IEnumerable<TReadModel>> GetByMasterKeyAsync(IEnumerable<Guid> masterKeys);
    }
}
