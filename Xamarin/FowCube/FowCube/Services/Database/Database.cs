namespace FowCube.Services.Database
{
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using SQLite;

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
        }
    }
}
