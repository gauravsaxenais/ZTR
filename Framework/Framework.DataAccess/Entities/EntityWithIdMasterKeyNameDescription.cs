namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public abstract class EntityWithIdMasterKeyNameDescription : EntityWithIdMasterKeyName, IEntityWithDescription
    {
        public EntityWithIdMasterKeyNameDescription()
        {
        }

        public EntityWithIdMasterKeyNameDescription(Guid masterKey, string name, string desription)
            : base(masterKey, name)
        {
            Description = desription;
        }

        public EntityWithIdMasterKeyNameDescription(long id, Guid masterKey, string name, string desription)
            : base(id, masterKey, name)
        {
            Description = desription;
        }

        public string Description { get; set; }
    }
}
