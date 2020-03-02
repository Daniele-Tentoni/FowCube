namespace FowCube.Services.Database
{
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using SQLite;
    using System;

    /// <summary>
    /// Primary Database class.
    /// </summary>
    public partial class Database
    {
        private readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            this._database = new SQLiteAsyncConnection(dbPath);
            this._database.CreateTableAsync<Collection>().Wait();
            this._database.CreateTableAsync<Card>().Wait();
            this._database.CreateTableAsync<CollectionCard>().Wait();

            if (this._database.Table<Card>().CountAsync().Result <= 0)
            {
                for (int i = 0; i < 1000; i++)
                {
                    var card = new Card { Id = $"{i}", Name = $"Card {i}", Description = $"Desc card {i}" };
                    this._database.InsertOrReplaceAsync(card);
                }
            }

            var coll1 = new Collection { Id = "1", Name = "Collection 1", User = "11" };
            coll1.CardsIn = new System.Collections.Generic.List<Card>();
            var c1 = this._database.FindAsync<Card>("1").Result;
            coll1.CardsIn.Add(c1);
            this._database.InsertOrReplaceAsync(coll1);

            var coll2 = new Collection { Id = "2", Name = "Collection 2", User = "11" };
            var c2 = this._database.FindAsync<Card>("2").Result;
            var c3 = this._database.FindAsync<Card>("3").Result;
            coll1.CardsIn.Add(c2);
            coll1.CardsIn.Add(c3);
            this._database.InsertOrReplaceAsync(coll2);
        }
    }
}
