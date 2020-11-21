namespace ZTR.Framework.Business
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueryManager<TReadModel> : IManager
        where TReadModel : class
    {
        Task<IEnumerable<TReadModel>> GetByIdAsync(long id, params long[] ids);

        Task<IEnumerable<TReadModel>> GetByIdAsync(IEnumerable<long> ids);

        Task<IEnumerable<TReadModel>> GetAllAsync();
    }
}
