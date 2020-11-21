namespace ZTR.Framework.DataAccess.Entities.Audit
{
    using System;

    public abstract class AuditableEntity : Entity
    {
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
