namespace FowCube.Models.Collection
{
    using FowCube.Models.Cards;
    using Newtonsoft.Json;
    using SQLite;
    using SQLiteNetExtensions.Attributes;
    using System.Collections.Generic;

    [Table("Collections")]
    public class Collection
    {
        /// <summary>
        /// This id is stored in Realm database.
        /// </summary>
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        [Column("Id")]
        public string Id { get; set; }

        /// <summary>
        /// This id is stored in Firebase Database.
        /// </summary>
        [JsonProperty(PropertyName = "fid")]
        [Column("FirebaseId")]
        public string FirebaseId { get; set; }

        /// <summary>
        /// This name is shown in the Main Page.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// This user is the manager of the list.
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        [Column("User")]
        public string User { get; set;  }

        /// <summary>
        /// This list contains all cards that compose the Collection.
        /// </summary>
        [JsonProperty(PropertyName = "cards_in")]
        [ManyToMany(typeof(CollectionCard))]
        public virtual List<Card> CardsIn { get; set; }
    }
}
