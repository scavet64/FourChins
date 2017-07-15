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
        [XmlElement("post")]
        public Post Post { get; set; }

        [XmlElement("WalletAddress")]
        public string WalletAddress { get; set; }

        [XmlElement("NumberOfCoins")]
        public int NumberOfCoins { get; set; }
    }
}
