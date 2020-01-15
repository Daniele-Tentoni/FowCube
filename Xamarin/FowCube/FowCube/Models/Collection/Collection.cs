using Newtonsoft.Json;
using System.Collections.Generic;

namespace FowCube.Models.Collection
{
    public class Collection : BasicCollection
    {
        [JsonProperty(PropertyName = "cards_in")]
        public List<string> CardsIn { get; set; }
    }
}
