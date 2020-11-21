namespace Business.Models
{
    public class DeviceReadModel
    {
        public object Name { get; set; }
        public object Description { get; set; }
        public int Digital_Inputs { get; set; }
        public int Digital_Outputs { get; set; }
        public int Analog_Inputs { get; set; }
        public int Uarts { get; set; }

        public override string ToString()
        {
            return $"DeviceReadModel(${this.Name} {this.Description} {this.Digital_Inputs} {this.Digital_Outputs} {this.Analog_Inputs} {this.Uarts})";
        }
    }
}
