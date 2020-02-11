namespace FowCube.Services.Collections
{
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the collection store properties calls, like collection renaming..
    /// </summary>
    public partial class CollectionStore
    {
        private async Task<bool> RemoteAddToCollection(string collId, string cardId)
        {
            var serializedItem = JsonConvert.SerializeObject(new { id = cardId });
            var response = await this.Client.PutAsync($"collection/addcard/{collId}",
                new StringContent(serializedItem, Encoding.UTF8, "application/json")
                );
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> RemoteRemoveFromCollection(string collId, string cardId)
        {
            var serializedItem = JsonConvert.SerializeObject(new { id = cardId });
            var response = await this.Client.PutAsync($"collection/removecard/{collId}",
                new StringContent(serializedItem, Encoding.UTF8, "application/json")
                );
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> RemoteRenameCollection(string collId, string newName)
        {
            if (collId == null || string.IsNullOrEmpty(newName))
                throw new ArgumentException();

            if (this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(new { name = newName });
                var response = await this.Client.PutAsync($"collection/rename/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }

            return false;
        }
    }
}