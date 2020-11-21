namespace ZTR.Framework.DataAccess
{
    public interface IEntityWithName : IEntity
    {
        string Name { get; set; }
    }
}
