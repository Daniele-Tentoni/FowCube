using Newtonsoft.Json;
using Realms;
using System.Collections.Generic;

namespace FowCube.Models.Collection
{
    public class Collection : RealmObject
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        [JsonProperty(PropertyName = "cards_in")]
        public IList<string> CardsIn { get; }
    }
}
