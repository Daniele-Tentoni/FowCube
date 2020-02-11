namespace FowCube.Models.Cards
{
    using FowCube.Models.Collection;
    using Newtonsoft.Json;
    using SQLite;
    using SQLiteNetExtensions.Attributes;
    using System.Collections.Generic;

    /// <summary>
    /// A simple card.
    /// </summary>
    [Table("Cards")]
    public class Card
    {
        /// <summary>
        /// Card Identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        [PrimaryKey]
        [Column("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Card name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Long description.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        [Column("Description")]
        public string Description { get; set; }

        [ManyToMany(typeof(CollectionCard))]
        public virtual List<Collection> Collections { get; set; }
    }
}
