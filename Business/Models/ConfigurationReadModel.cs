namespace Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class ConfigurationReadModel
    {
        public List<ModuleReadModel> Module { get; set; } = new List<ModuleReadModel>();
        public DeviceReadModel Device { get; set; }

        public override string ToString()
        {
            return $"ConfigurationReadModel({this.Device} {string.Join(",", this.Module.Select(p => p.ToString()))})";
        }
    }
}
