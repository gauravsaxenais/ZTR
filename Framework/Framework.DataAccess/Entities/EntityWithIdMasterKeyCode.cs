namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public abstract class EntityWithIdMasterKeyCode : EntityWithIdMasterKey, IEntityWithCode
    {
        public EntityWithIdMasterKeyCode()
        {
        }

        public EntityWithIdMasterKeyCode(Guid masterKey, string code)
            : base(masterKey)
        {
            Code = code;
        }

        public EntityWithIdMasterKeyCode(long id, Guid masterKey, string code)
            : base(id, masterKey)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
