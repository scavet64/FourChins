using FChan.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChan.Library
{
    public class Page
    {
        /// <summary>
        /// Gets or sets the boards.
        /// </summary>
        /// <value>The boards.</value>
        [JsonProperty("page")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the boards.
        /// </summary>
        /// <value>The boards.</value>
        [JsonProperty("threads")]
        public List<Thread> Threads { get; set; }
    }
}
