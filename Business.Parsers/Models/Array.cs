namespace Business.Parser.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Array
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isarray")]
        [JsonIgnore]
        public bool IsRepeated { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; } = new List<Field>();

        [JsonProperty("arrays")]
        public List<Array> Arrays { get; } = new List<Array>();
    }
}
