namespace ZTR.Framework.DataAccess.Entities
{
    using ZTR.Framework.DataAccess.Entities.Audit;
    public abstract class EntityWithId : AuditableEntity, IEntityWithId
    {
        public EntityWithId()
        {
        }

        public EntityWithId(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
