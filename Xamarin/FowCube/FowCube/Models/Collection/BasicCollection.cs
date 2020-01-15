namespace FowCube.Models.Collection
{
    using Newtonsoft.Json;

    public class BasicCollection
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
