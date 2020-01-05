using Newtonsoft.Json;
using System.Collections.Generic;

namespace FowCube.Models
{
    public class Collection
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "cards_in")]
        public List<string> CardsIn { get; set; }
    }
}
