namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public abstract class EntityWithIdMasterKeyCodeNameDescription : EntityWithIdMasterKeyCodeName, IEntityWithDescription
    {
        public EntityWithIdMasterKeyCodeNameDescription()
        {
        }

        public EntityWithIdMasterKeyCodeNameDescription(Guid masterKey, string code, string name, string desription)
            : base(masterKey, code, name)
        {
            Description = desription;
        }

        public EntityWithIdMasterKeyCodeNameDescription(long id, Guid masterKey, string code, string name, string desription)
            : base(id, masterKey, code, name)
        {
            Description = desription;
        }

        public string Description { get; set; }
    }
}
