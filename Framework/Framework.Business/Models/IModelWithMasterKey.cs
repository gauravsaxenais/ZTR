namespace ZTR.Framework.Business
{
    using System;

    public interface IModelWithMasterKey : IModel
    {
        Guid MasterKey { get; set; }
    }
}
