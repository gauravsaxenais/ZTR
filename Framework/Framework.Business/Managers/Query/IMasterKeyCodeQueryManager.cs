namespace ZTR.Framework.Business
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMasterKeyCodeQueryManager<TReadModel> : IMasterKeyQueryManager<TReadModel>
        where TReadModel : class, IModelWithMasterKey, IModelWithCode
    {
        Task<IEnumerable<TReadModel>> GetByCodeAsync(string code, params string[] codes);

        Task<IEnumerable<TReadModel>> GetByCodeAsync(IEnumerable<string> codes);
    }
}
