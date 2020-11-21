namespace Business
{
    public class TomlConfigurationFile
    {
        public string DeviceFolder { get; set; }
        public string TomlConfigFolder { get; set; }
        public string DeviceTomlFile { get; set; }
        public string DefaultTomlFile { get; set; }
        public string ModulesUrl { get; set; }
        public string BlocksUrl { get; set; }

        public override string ToString()
        {
            return $"TomlConfigurationFile(${this.DeviceFolder} {this.TomlConfigFolder} {this.DeviceTomlFile} {this.DeviceFolder} {this.ModulesUrl} {this.BlocksUrl})";
        }
    }
}
