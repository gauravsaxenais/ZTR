namespace Business.Models
{
    public class ModuleReadModel
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public override string ToString()
        {
            return $"ModuleReadModel(${this.Name} {this.UUID})";
        }
    }
}
