namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public abstract class EntityWithIdMasterKeyName : EntityWithIdMasterKey, IEntityWithName
    {
        public EntityWithIdMasterKeyName()
        {
        }

        public EntityWithIdMasterKeyName(Guid masterKey, string name)
            : base(masterKey)
        {
            Name = name;
        }

        public EntityWithIdMasterKeyName(long id, Guid masterKey, string name)
            : base(id, masterKey)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
