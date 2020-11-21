namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public abstract class EntityWithIdMasterKeyCodeName : EntityWithIdMasterKeyCode, IEntityWithName
    {
        public EntityWithIdMasterKeyCodeName()
        {
        }

        public EntityWithIdMasterKeyCodeName(Guid masterKey, string code, string name)
            : base(masterKey, code)
        {
            Name = name;
        }

        public EntityWithIdMasterKeyCodeName(long id, Guid masterKey, string code, string name)
            : base(id, masterKey, code)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
