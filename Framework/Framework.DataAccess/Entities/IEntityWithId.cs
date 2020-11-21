namespace ZTR.Framework.DataAccess
{
    public interface IEntityWithId : IEntity
    {
        long Id { get; set; }
    }
}
