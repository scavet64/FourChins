using FChan.Library;
using FourChins.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FourChins
{
    public class FourChins
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Properties.Settings settings = Properties.Settings.Default;
        private XmlSerializer serializer = new XmlSerializer(typeof(List<AwardedPost>));
        private XmlSerializer LastCrawledserializer = new XmlSerializer(typeof(LastCrawledTracker));
        private XmlSerializer foundWalletSerializer = new XmlSerializer(typeof(HashSet<string>));
        private readonly string LastCrawledTrackerXMLFileName = "LastCrawledTracker.XML";
        private readonly string AwardedPostXMLFileName = "AwardedPostList.XML";
        private readonly string FoundWalletSetXMLFileName = "FoundWalletSet.XML";
        private List<AwardedPost> awardedPostsList;
        private LastCrawledTracker lastCrawledTracker;
        private HashSet<string> foundWalletSet;
        private bool firstrun = true;

        public FourChins()
        {
            LoadAwardedPostXML();
            LoadLastCrawledTracker();
            LoadFoundWalletSet();
        }

        /// <summary>
        /// Start running the bot.
        /// </summary>
        public void BotRunner()
        {
            do
            {
                StartWork();
                firstrun = false;

                logger.Info("Sleeping for: " + settings.WaitTimeMS + "ms");
                System.Threading.Thread.Sleep(settings.WaitTimeMS);
            } while (true);
        }

        /// <summary>
        /// Gets all of the boards
        /// </summary>
        private void StartWork()
        {
            logger.Info("Starting Crawl");
            BoardRootObject BoardsRootObject = FourChinCore.GetBoard();

            //for each board, parse it.
            foreach (Board board in BoardsRootObject.Boards)
            {
                //are we parsing threads older than the start of this bot?
                //if not set the time we last ran to now. threads updated passed this point will be considered.
                if (!settings.ParseOlderThreads && firstrun)
                {
                    SetLastRunTimeForBoard(board.BoardName);
                }

                ParseBoard(board.BoardName);
            }
        }

        /// <summary>
        /// Method that will parse the board for any gets.
        /// Starts by collecting all of the threads on the board. For each thread, if
        /// the last modified date is more recent than the previous run, request the posts for that thread.
        /// Iterate through the posts looking for posters that have the correct $4CHN: prefix. If they do,
        /// check for a get. Award the user if they got a get.
        /// </summary>
        /// <param name="board">Board to parse</param>
        public void ParseBoard(string board)
        {
            logger.Info("Crawling board: " + board);
            ulong lastCrawled = lastCrawledTracker.GetLastCrawledTimeFromBoard(board);

            //get all the threads from the passed in board
            List<Thread> threads = FourChinCore.GetAllThreadsFromBoard(board);

            //set the time we last gathered information from this board
            SetLastRunTimeForBoard(board);

            if (threads != null)
            {
                foreach (Thread thread in threads)
                {
                    //Check if the last modified time is greater than the last run of the bot
                    ulong LastModified = ulong.Parse(thread.LastModified);
                    if (LastModified > lastCrawled)
                    {
                        int attemptsLeft = settings.AttemptsGettingInformation;
                        bool success = false;
                        Thread fullThread;
                        do
                        {
                            //get the full details about the thread
                            fullThread = FourChinCore.GetThread(board, thread.ThreadNumber);
                            if (fullThread != null && fullThread.Posts != null)
                            {
                                success = true;
                            }
                            else
                            {
                                logger.Warn(string.Format("Error getting fullthread information for #[{0}] - {1} attempts left", thread.ThreadNumber, attemptsLeft));
                                attemptsLeft--;
                            }
                        } while (attemptsLeft > 0 && !success);

                        //check to see if we are still null after the 5 attempts. If so, skip this iteration of the loop
                        if (fullThread == null || fullThread.Posts == null)
                        {
                            logger.Error(string.Format("Could not get the posts for thread[{0}] - Thread might have been deleted or died", thread.ThreadNumber));
                            continue;
                        }

                        logger.Debug(string.Format("Crawling Thread: {0} - Board: {1}", thread.ThreadNumber, board));
                        foreach (Post post in fullThread.Posts)
                        {
                            //for each post, check to see if the poster's name has their address
                            string posterName = post.Name;
                            if (posterName != null && posterName.StartsWith("$4CHN:"))
                            {
                                string walletAddress = posterName.Replace("$4CHN:", "");
                                foundWalletSet.Add(walletAddress);
                                SaveFoundWalletSet();
                                logger.Info("Found wallet - " + walletAddress);

                                //if the user has their address, check to see if they got a get
                                CheckAndHandleGet(post, walletAddress);
                            }
                        }
                    }
                    else
                    {
                        logger.Debug(string.Format("Board [{0}] - Thread [{1}] has been crawled already", board, thread.ThreadNumber));
                    }
                }
            }
        }

        /// <summary>
        /// Simple algorithm that checks a post number for any consecutive final digits.
        /// </summary>
        /// <param name="postNumber">Post number to check</param>
        /// <returns>returns the number of consecutive final digits</returns>
        public int GetTheGet(int postNumber)
        {
            return GetTheGet(postNumber.ToString());
        }

        /// <summary>
        /// Simple algorithm that checks a post number for any consecutive final digits.
        /// </summary>
        /// <param name="postNumber">Post number to check</param>
        /// <returns>returns the number of consecutive final digits</returns>
        public int GetTheGet(string postNumber)
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
        private void CheckAndHandleGet(Post post, string walletAddress)
        {
            int postNumber = post.PostNumber;

            switch (GetTheGet(postNumber))
            {
                case 1:
                    logger.Info("singles - " + walletAddress);
                    break;
                case 2:
                    logger.Info("Dubs - " + walletAddress);
                    AwardPost(walletAddress, post, settings.DoubleAward);
                    break;
                case 3:
                    logger.Info("Trips - " + walletAddress);
                    AwardPost(walletAddress, post, settings.TripsAward);
                    break;
                case 4:
                    logger.Info("Quads - " + walletAddress);
                    AwardPost(walletAddress, post, settings.QuadsAward);
                    break;
                case 5:
                    logger.Info("quintuple - " + walletAddress);
                    AwardPost(walletAddress, post, settings.QuintsAward);
                    break;
                case 6:
                    logger.Info("sextuple - " + walletAddress);
                    AwardPost(walletAddress, post, settings.sextupleAward);
                    break;
                case 7:
                    logger.Info("septuple - " + walletAddress);
                    AwardPost(walletAddress, post, settings.septupleAward);
                    break;
                case 8:
                    logger.Info("octuple - " + walletAddress);
                    AwardPost(walletAddress, post, settings.octupleAward);
                    break;
                case 9:
                    logger.Info("nonuple - " + walletAddress);
                    break;
                case 10:
                    logger.Info("decuple - " + walletAddress);
                    break;
                case 11:
                    logger.Info("undecuple - " + walletAddress);
                    break;
                case 12:
                    logger.Info("duodecuple - " + walletAddress);
                    break;
                case 13:
                    logger.Info("tredecuple - " + walletAddress);
                    break;
                case 14:
                    logger.Info("quattuordecuple - " + walletAddress);
                    break;
                case 15:
                    logger.Info("quindecuple - " + walletAddress);
                    break;
                default:
                    logger.Error("Error in switch");
                    break;
            }
        }

        /// <summary>
        /// Wrapper method that handles the awarding of posts. This can be expanded to keep track of more data
        /// </summary>
        /// <param name="wallet">Wallet we are sending the coins to</param>
        /// <param name="postnumber">The post number that is getting the award</param>
        /// <param name="amount">the amount of coins we are sending</param>
        private void AwardPost(string wallet, Post post, double amount)
        {
            if (settings.Awarding)
            {
                AwardedPost potentialAwardedPost = new AwardedPost(post, wallet, amount);
                if (!awardedPostsList.Contains(potentialAwardedPost))
                {
                    //add to the list and total number of coins awarded
                    awardedPostsList.Add(new AwardedPost(post, wallet, amount));
                    settings.NumberOfCoinsAwarded += amount;

                    //log the event and send the coins
                    logger.Info(string.Format("Awarding wallet: {0} - with {1} Chancoins for post: {2}", wallet, amount, post.PostNumber));
                    //WalletController.SendAwardToWallet(wallet, amount, post.PostNumber, BuildURL());
                }
                else
                {
                    //logger.Warn(string.Format("PostNumber[{0}] was detected to be a duplicate. No awards were given.", post.PostNumber));
                }
            }
            else
            {
                logger.Info("Skipping award for postnumber: " + post.PostNumber);
            }
            
        }

        /// <summary>
        /// Builds the URL using the settings file
        /// </summary>
        /// <returns>String URL</returns>
        private string BuildURL()
        {
            var properties = Properties.Settings.Default;
            string url = string.Format("http://{0}:{1}", properties.WalletServerAddress, properties.WalletServerPort);
            return url;
        }

        #region saving and loading information

        /// <summary>
        /// set our last run to now and convert it to unix time. Unix time is very important as it is what the 4chan API uses.
        /// </summary>
        private void SetLastRunTimeForBoard(string board)
        {
            ulong lastRun = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            lastCrawledTracker.UpdateCrawledBoardTime(board);
            SaveLastCrawledTracker();
        }

        /// <summary>
        /// save the awarded post list to the xml file
        /// </summary>
        private void SaveAwardedPostXML()
        {
            using (FileStream stream = File.OpenWrite(AwardedPostXMLFileName))
            {
                serializer.Serialize(stream, awardedPostsList);
            }
        }

        /// <summary>
        /// Load the awarded post list from the xml file
        /// </summary>
        private void LoadAwardedPostXML()
        {
            if (File.Exists(AwardedPostXMLFileName))
            {
                using (FileStream stream = File.OpenRead(AwardedPostXMLFileName))
                {
                    awardedPostsList = (List<AwardedPost>)serializer.Deserialize(stream);
                }
            }
            else
            {
                logger.Warn("Could not find AwardedPostXML file. Creating new list");
                awardedPostsList = new List<AwardedPost>();
                SaveAwardedPostXML();
            }
        }

        private void SaveLastCrawledTracker()
        {
            using (FileStream stream = File.OpenWrite(LastCrawledTrackerXMLFileName))
            {
                LastCrawledserializer.Serialize(stream, lastCrawledTracker);
            }
        }

        private void LoadLastCrawledTracker()
        {
            if (File.Exists(LastCrawledTrackerXMLFileName))
            {
                using (FileStream stream = File.OpenRead(LastCrawledTrackerXMLFileName))
                {
                    lastCrawledTracker = (LastCrawledTracker)LastCrawledserializer.Deserialize(stream);
                }
            }
            else
            {
                logger.Warn("Could not find LastCrawledTracker file. Creating new object");
                lastCrawledTracker = new LastCrawledTracker();
                SaveLastCrawledTracker();
            }
        }

        private void SaveFoundWalletSet()
        {
            using (FileStream stream = File.OpenWrite(FoundWalletSetXMLFileName))
            {
                foundWalletSerializer.Serialize(stream, foundWalletSet);
            }
        }

        private void LoadFoundWalletSet()
        {
            if (File.Exists(FoundWalletSetXMLFileName))
            {
                using (FileStream stream = File.OpenRead(FoundWalletSetXMLFileName))
                {
                    foundWalletSet = (HashSet<string>)foundWalletSerializer.Deserialize(stream);
                }
            }
            else
            {
                logger.Warn("Could not find FoundWalletSet file. Creating new object");
                foundWalletSet = new HashSet<string>();
                SaveFoundWalletSet();
            }
        }
        #endregion
    }
}
