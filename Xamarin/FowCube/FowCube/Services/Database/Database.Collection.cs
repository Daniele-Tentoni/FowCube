namespace FowCube.Services.Database
{
    using FowCube.Models.Collection;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public partial class Database
    {
        #region Create
        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<int> CreateCollectionAsync(Collection collection)
        {
            if (string.IsNullOrEmpty(collection.Id))
                collection.Id = Guid.NewGuid().ToString();
            return await this._database.InsertAsync(collection);
        }
        #endregion

        #region Read
        public async Task<List<Collection>> GetAllCollectionsAsync()
        {
            var res = await this._database.Table<Collection>().ToListAsync();
            return res;
        }
        /// <summary>
        /// Read.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<Collection>> GetCollectionsByUserAsync(string uid)
        {
            var res = await this._database.Table<Collection>().Where(w => w.User == uid).ToListAsync();
            return res;
        }

        public async Task<Collection> GetCollectionByIdAsync(string collId)
        {
            var res = await this._database.Table<Collection>().FirstOrDefaultAsync(w => w.Id == collId);
            return res;
        }
        #endregion

        #region Update
        /// <summary>
        /// Add a card to a collection.
        /// </summary>
        /// <param name="collId"></param>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task<int> AddCardToCollectionAsync(string collId, string cardId)
        {
            if (string.IsNullOrEmpty(collId) || string.IsNullOrEmpty(cardId))
                throw new ArgumentNullException();

            var coll = await this.GetCollectionByIdAsync(collId);
            var card = await this.GetCardByIdAsync(cardId);
            coll.CardsIn.Add(card);
            return await this._database.UpdateAsync(coll);
        }

        /// <summary>
        /// Remove a card from a collection.
        /// </summary>
        /// <param name="collId"></param>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task<int> RemoveCardToCollectionAsync(string collId, string cardId)
        {
            if (string.IsNullOrEmpty(collId) || string.IsNullOrEmpty(collId))
                throw new ArgumentNullException();

            var coll = await this.GetCollectionByIdAsync(collId);
            var card = await this.GetCardByIdAsync(cardId);
            coll.CardsIn.Remove(card);
            return await this._database.UpdateAsync(coll);
        }

        public async Task<int> RenameCollection(string collId, string newName)
        {
            if (string.IsNullOrEmpty(collId) || string.IsNullOrEmpty(newName))
                throw new ArgumentNullException();

            var coll = await this.GetCollectionByIdAsync(collId);
            coll.Name = newName;
            return await this._database.UpdateAsync(coll);
        }
        #endregion
    }
}
