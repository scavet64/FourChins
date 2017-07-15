using System.Collections.Generic;
using Newtonsoft.Json;

namespace FChan.Library
{
    /// <summary>
    /// Thread.
    /// </summary>
    public class Thread
    {
        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        /// <value>The posts.</value>
        [JsonProperty("posts")]
        public List<Post> Posts { get; set; }

        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        /// <value>The posts.</value>
        [JsonProperty("no")]
        public string ThreadNumber { get; set; }

        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        /// <value>The posts.</value>
        [JsonProperty("last_modified")]
        public string LastModified { get; set; }
    }
}
