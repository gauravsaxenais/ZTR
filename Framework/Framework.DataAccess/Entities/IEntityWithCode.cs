namespace ZTR.Framework.DataAccess
{
    public interface IEntityWithCode : IEntity
    {
        string Code { get; set; }
    }
}
