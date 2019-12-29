namespace FowCube.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// A simple card.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Card Identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Card name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Long description.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
