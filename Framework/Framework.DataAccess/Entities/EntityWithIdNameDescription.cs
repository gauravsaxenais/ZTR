namespace ZTR.Framework.DataAccess.Entities
{
    public abstract class EntityWithIdNameDescription : EntityWithIdName, IEntityWithDescription
    {
        public EntityWithIdNameDescription()
        {
        }

        public EntityWithIdNameDescription(string name, string description)
            : base(name)
        {
            Description = description;
        }

        public EntityWithIdNameDescription(long id, string name, string description)
            : base(id, name)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
