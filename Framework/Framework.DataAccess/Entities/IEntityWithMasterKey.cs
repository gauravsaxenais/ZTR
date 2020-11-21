namespace ZTR.Framework.DataAccess
{
    using System;

    public interface IEntityWithMasterKey : IEntity
    {
        Guid MasterKey { get; set; }
    }
}
