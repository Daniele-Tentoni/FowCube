using Newtonsoft.Json;
using Realms;

namespace FowCube.Models.Cards
{
    public class CardTypes
    {
        [JsonProperty(PropertyName = "card_type")]
        public int CardType { get; }

        [JsonProperty(PropertyName = "card")]
        public string Card { get; }
    }
}
