namespace FowCube.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataStore<T>
    {
        /// <summary>
        /// Add the new <paramref name="item"/> of type <typeparamref name="T"/> to the DataStore.
        /// </summary>
        /// <param name="item">Item to add to the DataStore.</param>
        /// <returns>Return the Id of the added item.</returns>
        Task<string> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        /// <summary>
        /// Delete the <paramref name="id"/> item.
        /// </summary>
        /// <param name="id">Id of the selected element.</param>
        /// <returns>True if the element was deleted.</returns>
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
