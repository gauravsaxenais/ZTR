namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public abstract class EntityWithIdMasterKey : EntityWithId, IEntityWithMasterKey
    {
        public EntityWithIdMasterKey()
        {
        }

        public EntityWithIdMasterKey(Guid masterKey)
        {
            MasterKey = masterKey;
        }

        public EntityWithIdMasterKey(long id, Guid masterKey)
            : base(id)
        {
            MasterKey = masterKey;
        }

        public Guid MasterKey { get; set; }
    }
}
