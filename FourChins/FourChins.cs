using FChan.Library;
using FourChins.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChins
{
    public class FourChins
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ulong lastRun = 1500093062;
        private static HashSet<string> walletsFound;
        private static HashSet<Post> postsAwarded;
        private static int numberOfCoinsAwarded;
        private static Dictionary<string, int> walletToEarnedCoinsMap;
        private static List<AwardedPost> awardedPostsList;

        public FourChins()
        {
            walletsFound = new HashSet<string>();
            postsAwarded = new HashSet<Post>();
            awardedPostsList = new List<AwardedPost>();
            numberOfCoinsAwarded = 0;

            int waitTimeMS = Properties.Settings.Default.WaitTimeMS;
            lastRun = Properties.Settings.Default.LastRun;

            do
            {
                DoTheThing();

                logger.Info("Sleeping for: " + waitTimeMS + "ms");
                WriteToLog("Sleeping for: " + waitTimeMS + "ms");
                System.Threading.Thread.Sleep(waitTimeMS);
            } while (true);
        }

        public void DoTheThing()
        {
            WriteToLog("Starting up");
            logger.Info("Starting up");
            BoardRootObject BoardsRootObject = FourChinCore.GetBoard();

            //for each board, parse it.
            foreach (Board board in BoardsRootObject.Boards)
            {
                ParseBoard(board.BoardName);
            }

            lastRun = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Properties.Settings.Default.LastRun = lastRun;
            Properties.Settings.Default.Save();
        }

        public static void ParseBoard(string board)
        {
            //get all the threads from the passed in board
            List<Thread> threads = FourChinCore.GetAllThreadsFromBoard(board);
            if (threads != null)
            {
                foreach (Thread thread in threads)
                {
                    //Check if the last modified time is greater than the last run of the bot
                    ulong LastModified = ulong.Parse(thread.LastModified);
                    if (LastModified > lastRun)
                    {
                        //get the full details about the thread
                        Thread fullThread = FourChinCore.GetThread(board, thread.ThreadNumber);
                        WriteToLog("Parsing Thread: " + thread.ThreadNumber + " - Board: " + board);

                        foreach (Post post in fullThread.Posts)
                        {
                            //for each post, check to see if the poster's name has their address
                            string posterName = post.Name;
                            if (posterName != null && posterName.StartsWith("$4CHN:"))
                            {
                                string walletAddress = posterName.Replace("$4CHN:", "");
                                WriteToLog("Found wallet - " + walletAddress);

                                //if the user has their address, check to see if they got a get
                                CheckAndHandleGet(post.PostNumber, walletAddress);
                            }
                        }
                    }
                    else
                    {
                        WriteToLog("Thread has been parsed already");
                    }
                }
            }
        }

        public static int GetTheGet(int postNumber)
        {
            return GetTheGet(postNumber.ToString());
        }

        public static int GetTheGet(string postNumber)
        {
            int iterations = 0;
            char lastDigit = postNumber[postNumber.Length - 1];

            bool stillGoing = true;
            do
            {
                int index = postNumber.Length - 1 - iterations;
                if (index < 0 || !lastDigit.Equals(postNumber[index]))
                {
                    stillGoing = false;
                }
                else
                {
                    iterations++;
                }

            } while (stillGoing);

            return iterations;
        }

        private static void CheckAndHandleGet(int postNumber, string walletAddress)
        {
            switch (GetTheGet(postNumber))
            {
                case 1:
                    WriteToLog("singles - " + walletAddress);
                    break;
                case 2:
                    WriteToLog("Dubs - " + walletAddress);
                    break;
                case 3:
                    WriteToLog("Trips - " + walletAddress);
                    break;
                case 4:
                    WriteToLog("Quads - " + walletAddress);
                    break;
                case 5:
                    WriteToLog("quintuple - " + walletAddress);
                    break;
                case 6:
                    WriteToLog("sextuple - " + walletAddress);
                    break;
                case 7:
                    WriteToLog("septuple - " + walletAddress);
                    break;
                case 8:
                    WriteToLog("octuple - " + walletAddress);
                    break;
                case 9:
                    WriteToLog("nonuple - " + walletAddress);
                    break;
                case 10:
                    WriteToLog("decuple - " + walletAddress);
                    break;
                case 11:
                    WriteToLog("undecuple - " + walletAddress);
                    break;
                case 12:
                    WriteToLog("duodecuple - " + walletAddress);
                    break;
                case 13:
                    WriteToLog("tredecuple - " + walletAddress);
                    break;
                case 14:
                    WriteToLog("quattuordecuple - " + walletAddress);
                    break;
                case 15:
                    WriteToLog("quindecuple - " + walletAddress);
                    break;
                default:
                    WriteToLog("Error");
                    break;
            }
        }

        private static void WriteToLog(string message)
        {
            Console.WriteLine(DateTime.UtcNow + ": " + message);
        }
    }
}
