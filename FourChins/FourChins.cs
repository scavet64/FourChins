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
        private static Properties.Settings settings = Properties.Settings.Default;

        private static HashSet<string> walletsFound;
        private static HashSet<Post> postsAwarded;
        private static int numberOfCoinsAwarded;

        //unused as of now
        private static Dictionary<string, double> walletToEarnedCoinsMap;
        private static List<AwardedPost> awardedPostsList;

        public FourChins()
        {
            walletsFound = new HashSet<string>();
            postsAwarded = new HashSet<Post>();
            awardedPostsList = new List<AwardedPost>();
            numberOfCoinsAwarded = 0;
        }

        /// <summary>
        /// Start running the bot.
        /// </summary>
        public void BotRunner()
        {
            do
            {
                StartWork();

                logger.Info("Sleeping for: " + settings.WaitTimeMS + "ms");
                System.Threading.Thread.Sleep(settings.WaitTimeMS);
            } while (true);
        }

        /// <summary>
        /// Gets all of the boards
        /// </summary>
        private void StartWork()
        {
            WriteToLog("Starting work");

            ////////////////////////////////////////////
            //Test Code
            //AwardPost("CdEG5wxA93h7jj2BjBunLWgZy1pdfPoHAN", "22323232", 0.1);
            ////////////////////////////////////////////

            BoardRootObject BoardsRootObject = FourChinCore.GetBoard();

            //for each board, parse it.
            foreach (Board board in BoardsRootObject.Boards)
            {
                ParseBoard(board.BoardName);
            }

            //set our last run to now and convert it to unix time. Unix time is very important as it is what the 4chan API uses.
            settings.LastRun = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Method that will parse the board for any gets.
        /// Starts by collecting all of the threads on the board. For each thread, if
        /// the last modified date is more recent than the previous run, request the posts for that thread.
        /// Iterate through the posts looking for posters that have the correct $4CHN: prefix. If they do,
        /// check for a get. Award the user if they got a get.
        /// </summary>
        /// <param name="board">Board to parse</param>
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
                    if (LastModified > settings.LastRun)
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
                                walletsFound.Add(walletAddress);

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

        /// <summary>
        /// Simple algorithm that checks a post number for any consecutive final digits.
        /// </summary>
        /// <param name="postNumber">Post number to check</param>
        /// <returns>returns the number of consecutive final digits</returns>
        public static int GetTheGet(int postNumber)
        {
            return GetTheGet(postNumber.ToString());
        }

        /// <summary>
        /// Simple algorithm that checks a post number for any consecutive final digits.
        /// </summary>
        /// <param name="postNumber">Post number to check</param>
        /// <returns>returns the number of consecutive final digits</returns>
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

        /// <summary>
        /// Checks to see if the post was a Get. If there was a successfull get, we send the coins.
        /// </summary>
        /// <param name="postNumber">Post number to check</param>
        /// <param name="walletAddress">Address to send the coins to if successful</param>
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
                    WriteToLog("Error in switch");
                    break;
            }
        }

        /// <summary>
        /// Wrapper method that handles the awarding of posts. This can be expanded to keep track of more data
        /// </summary>
        /// <param name="wallet">Wallet we are sending the coins to</param>
        /// <param name="postnumber">The post number that is getting the award</param>
        /// <param name="amount">the amount of coins we are sending</param>
        private static void AwardPost(string wallet, string postnumber, double amount)
        {
            //walletToEarnedCoinsMap.Add(wallet, amount);
            WalletController.SendAwardToWallet(wallet, amount, postnumber, BuildURL());
        }

        /// <summary>
        /// Builds the URL using the settings file
        /// </summary>
        /// <returns>String URL</returns>
        private static string BuildURL()
        {
            var properties = Properties.Settings.Default;
            string url = string.Format("http://{0}:{1}", properties.WalletServerAddress, properties.WalletServerPort);
            return url;
        }

        /// <summary>
        /// This currently writes to the console and a txt file where the exe is located.
        /// </summary>
        /// <param name="message">Message to log</param>
        private static void WriteToLog(string message)
        {
            logger.Info(message);
            Console.WriteLine(DateTime.UtcNow + ": " + message);
        }
    }
}
