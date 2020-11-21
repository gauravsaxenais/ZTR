namespace ZTR.Framework.DataAccess.Entities
{
    public abstract class EntityWithIdCodeNameDescription : EntityWithIdCodeName, IEntityWithDescription
    {
        public EntityWithIdCodeNameDescription()
        {
        }

        public EntityWithIdCodeNameDescription(string code, string name, string description)
            : base(code, name)
        {
            Description = description;
        }

        public EntityWithIdCodeNameDescription(long id, string code, string name, string description)
            : base(id, code, name)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
