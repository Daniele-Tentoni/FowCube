namespace FowCube.Models.Collection
{
    using FowCube.Models.Cards;
    using SQLiteNetExtensions.Attributes;

    public class CollectionCard
    {
        [ForeignKey(typeof(Collection))]
        public string CollectionId { get; set; }

        [ForeignKey(typeof(Card))]
        public string CardId { get; set; }
    }
}
