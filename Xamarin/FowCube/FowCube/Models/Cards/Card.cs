namespace FowCube.Models.Cards
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Realms;

    /// <summary>
    /// A simple card.
    /// </summary>
    public class Card : RealmObject
    {
        /// <summary>
        /// Card Identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        [PrimaryKey]
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

        /// <summary>
        /// All cards types.
        /// TODO: Implement this in APIs.
        /// </summary>
        [JsonProperty(PropertyName = "types")]
        public IList<int> Types { get; }
    }
}
