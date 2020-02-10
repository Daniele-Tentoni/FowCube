namespace FowCube.Models.Collection
{
    using FowCube.Models.Cards;
    using Newtonsoft.Json;
    using Realms;
    using System.Collections.Generic;
    using System.Linq;

    public class Collection : RealmObject
    {
        /// <summary>
        /// This id is stored in Realm database.
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// This id is stored in Firebase Database.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string FirebaseId { get; set; }

        /// <summary>
        /// This name is shown in the Main Page.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// This user is the manager of the list.
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        [Backlink(nameof(Models.User.Collections))]
        public IQueryable<User> User { get; }

        /// <summary>
        /// This list contains all cards that compose the Collection.
        /// </summary>
        [JsonProperty(PropertyName = "cards_in")]
        public IList<Card> CardsIn { get; }
    }
}
