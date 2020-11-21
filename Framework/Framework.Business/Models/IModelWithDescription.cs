namespace ZTR.Framework.Business
{
    public interface IModelWithDescription : IModel
    {
        string Description { get; set; }
    }
}
