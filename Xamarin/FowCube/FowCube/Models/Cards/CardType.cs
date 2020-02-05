using Newtonsoft.Json;
using Realms;

namespace FowCube.Models.Cards
{
    public class CardType : RealmObject
    {
        [JsonProperty(PropertyName = "id")]
        [PrimaryKey]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
    }
}
