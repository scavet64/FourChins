using FChan.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FourChins.Model
{
    public class AwardedPost
    {
        /// <summary>
        /// default empty constructor
        /// </summary>
        public AwardedPost()
        {

        }

        public AwardedPost(Post post, string walletAddress, double numberOfCoins)
        {
            this.Post = post;
            this.WalletAddress = walletAddress;
            this.NumberOfCoins = numberOfCoins;
        }

        [XmlElement("post")]
        public Post Post { get; set; }

        [XmlElement("WalletAddress")]
        public string WalletAddress { get; set; }

        [XmlElement("NumberOfCoins")]
        public double NumberOfCoins { get; set; }
    }
}
