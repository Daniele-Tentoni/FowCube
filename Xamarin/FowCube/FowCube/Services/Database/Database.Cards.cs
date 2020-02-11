namespace FowCube.Services.Database
{
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Partial class to access Card Entity.
    /// </summary>
    public partial class Database
    {
        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public async Task<int> CreateCardAsync(Card card) => await this._database.InsertOrReplaceAsync(card);

        /// <summary>
        /// Read.
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task<Card> GetCardByIdAsync(string cardId) => await this._database.Table<Card>().FirstOrDefaultAsync(w => w.Id == cardId);
        public async Task<List<Card>> GetAllCards() => await this._database.Table<Card>().ToListAsync();

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task<int> DeleteCardByIdAsync(string cardId) => await this._database.Table<Card>().DeleteAsync(d => d.Id == cardId);
    }
}
