using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChins.Model
{
    public class CrawledBoard
    {
        public string BoardName { get; set; }
        public ulong LastCrawled { get; set; }

        public CrawledBoard() {}

        public CrawledBoard(string BoardName, ulong LastCrawled)
        {
            this.BoardName = BoardName;
            this.LastCrawled = LastCrawled;
        }
    }
}
