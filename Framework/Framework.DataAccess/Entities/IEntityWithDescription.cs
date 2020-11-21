namespace ZTR.Framework.DataAccess
{
    public interface IEntityWithDescription : IEntity
    {
        string Description { get; set; }
    }
}
