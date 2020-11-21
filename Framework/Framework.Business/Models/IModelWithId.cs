namespace ZTR.Framework.Business
{
    public interface IModelWithId : IModel
    {
        long Id { get; set; }
    }
}
