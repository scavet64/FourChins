using FourChins.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChins
{
    public class LastCrawledTracker
    {
        public List<CrawledBoard> ListOfCrawledBoards { get; set; }

        public LastCrawledTracker()
        {
            this.ListOfCrawledBoards = new List<CrawledBoard>();
        }

        public CrawledBoard GetCrawledBoard(string boardName)
        {
            foreach(CrawledBoard cb in ListOfCrawledBoards)
            {
                if (!string.IsNullOrEmpty(cb.BoardName) && cb.BoardName.Equals(boardName))
                {
                    return cb;
                }
            }
            return null;
        }

        public ulong GetLastCrawledTimeFromBoard(string boardName)
        {
            return GetCrawledBoard(boardName).LastCrawled;
        }

        public void AddCrawledBoard(string boardName)
        {
            if (!Contains(boardName))
            {
                ListOfCrawledBoards.Add(new CrawledBoard(boardName, GetTimeNowInUnixTime()));
            }
        }

        public void UpdateCrawledBoardTime(CrawledBoard board)
        {
            if(board != null)
            {
                board.LastCrawled = GetTimeNowInUnixTime();
            }
        }

        public void UpdateCrawledBoardTime(string boardName)
        {
            if (!Contains(boardName))
            {
                AddCrawledBoard(boardName);
            } else
            {
                UpdateCrawledBoardTime(GetCrawledBoard(boardName));
            }
        }

        private ulong GetTimeNowInUnixTime()
        {
            return (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private bool Contains(string boardName)
        {
            foreach(CrawledBoard cb in ListOfCrawledBoards)
            {
                if (cb.BoardName.Equals(boardName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
