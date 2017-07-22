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
        /// default empty constructor. Used for serialization.
        /// </summary>
        public AwardedPost()
        {

        }

        /// <summary>
        /// Constructor that sets the properties of this object using the passed in parameters
        /// </summary>
        /// <param name="post"></param>
        /// <param name="walletAddress"></param>
        /// <param name="numberOfCoins"></param>
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

        /// <summary>
        /// override of Equals. Only takes into consideration the post object and wallet address.
        /// We do not want to send more coins to the same address for the same post. Regardless of coin amount
        /// </summary>
        /// <param name="obj">obj to compare to</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            bool areEqual = false;
            if(obj is AwardedPost)
            {
                AwardedPost postToCompare = (AwardedPost)obj;
                if(this.Post.Equals(postToCompare.Post) && WalletAddress.Equals(postToCompare.WalletAddress))
                {
                    areEqual = true;
                }
            }
            return areEqual;
        }

        /// <summary>
        /// override of GetHashCode. Only takes into consideration the post object and wallet address.
        /// We do not want to send more coins to the same address for the same post. Regardless of coin amount
        /// </summary>
        /// <returns>HashCode integer</returns>
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Post.GetHashCode();
            hash = (hash * 7) + WalletAddress.GetHashCode();
            return hash;
        }
    }
}
