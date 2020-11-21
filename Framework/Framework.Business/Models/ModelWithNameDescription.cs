namespace ZTR.Framework.Business.Models
{
    public abstract class ModelWithNameDescription : ModelWithName, IModelWithDescription
    {
        protected ModelWithNameDescription()
        {
        }

        protected ModelWithNameDescription(string name, string description)
            : base(name)
        {
            Description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}
