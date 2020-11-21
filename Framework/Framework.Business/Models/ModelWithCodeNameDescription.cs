namespace ZTR.Framework.Business.Models
{
    public abstract class ModelWithCodeNameDescription : ModelWithCodeName, IModelWithDescription
    {
        protected ModelWithCodeNameDescription()
        {
        }

        protected ModelWithCodeNameDescription(string code, string name, string description)
            : base(code, name)
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
