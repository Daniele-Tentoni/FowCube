namespace FowCube.Models
{
    using SQLite;
    using System.Collections.Generic;

    public class User
    {
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Name displayed.
        /// </summary>
        public string DisplayName { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// User email used to communicate with.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Profile picture (only for google or facebook now).
        /// </summary>
        public string Picture { get; set; }

        public IList<Collection.Collection> Collections { get; }
    }
}
