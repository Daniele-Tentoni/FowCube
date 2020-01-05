using System.Collections.Generic;

namespace FowCube.Models
{
    public class Collection
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<Card> CardsIn { get; set; }
    }
}
