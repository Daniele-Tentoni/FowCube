namespace FowCube.Services.Collections
{
    using FowCube.Models.Collection;
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
        public class RenameCollectionInfo
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        public async Task<bool> RenameCollection(string collId, string newName)
        {
            if (collId == null || string.IsNullOrEmpty(newName))
                throw new ArgumentException();

            bool remote = true;
            if (this.IsConnected && false)
            {
                var serializedItem = JsonConvert.SerializeObject(new RenameCollectionInfo { Name = newName });
                var response = await this.Client.PutAsync($"collection/rename/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                remote = response.IsSuccessStatusCode;
            }

            bool local = false;
            using (var trans = this.Realm.BeginWrite())
            {
                var coll = this.Realm.Find<Collection>(collId);
                coll.Name = newName;
                local = true;
                trans.Commit();
            }

            return remote && local;
        }
    }
}
