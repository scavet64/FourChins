using FChan.Library;
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
        private static Int32 lastRun = 0;
        private static ConcurrentBag<string> walletsFound;
        private static ConcurrentBag<Post> postsAwarded;
        private static int numberOfCoinsAwarded;

        public FourChins()
        {
            walletsFound = new ConcurrentBag<string>();
            postsAwarded = new ConcurrentBag<Post>();
            numberOfCoinsAwarded = 0;
        }

        public void DoTheThing()
        {
            BoardRootObject test = FourChinCore.GetBoard();
            String temp = test.Boards[0].BoardName;
            temp = "biz";
            List<Thread> threads = FourChinCore.GetAllThreadsFromBoard(temp);
            foreach (Thread thread in threads)
            {
                Int32 LastModified = Int32.Parse(thread.LastModified);
                if (LastModified > lastRun)
                {
                    //Console.WriteLine("LastModified: " + thread.LastModified);
                    //Console.WriteLine("ThreadNumber: " + thread.ThreadNumber);

                    Thread fullThread = FourChinCore.GetThread(temp, thread.ThreadNumber);

                    foreach (Post post in fullThread.Posts)
                    {
                        string posterName = post.Name;
                        if (posterName != null && posterName.StartsWith("$4CHN:"))
                        {
                            string walletAddress = posterName.Replace("$4CHN:", "");
                            Console.WriteLine("Found wallet - " + walletAddress);

                            switch (GetTheGet(post.PostNumber))
                            {
                                case 1:
                                    Console.WriteLine("singles - " + walletAddress);
                                    break;
                                case 2:
                                    Console.WriteLine("Dubs - " + walletAddress);
                                    break;
                                case 3:
                                    Console.WriteLine("Trips - " + walletAddress);
                                    break;
                                case 4:
                                    Console.WriteLine("Quads - " + walletAddress);
                                    break;
                                case 5:
                                    Console.WriteLine("quintuple - " + walletAddress);
                                    break;
                                case 6:
                                    Console.WriteLine("sextuple - " + walletAddress);
                                    break;
                                case 7:
                                    Console.WriteLine("septuple - " + walletAddress);
                                    break;
                                case 8:
                                    Console.WriteLine("octuple - " + walletAddress);
                                    break;
                                case 9:
                                    Console.WriteLine("nonuple - " + walletAddress);
                                    break;
                                case 10:
                                    Console.WriteLine("decuple - " + walletAddress);
                                    break;
                                case 11:
                                    Console.WriteLine("undecuple - " + walletAddress);
                                    break;
                                case 12:
                                    Console.WriteLine("duodecuple - " + walletAddress);
                                    break;
                                case 13:
                                    Console.WriteLine("tredecuple - " + walletAddress);
                                    break;
                                case 14:
                                    Console.WriteLine("quattuordecuple - " + walletAddress);
                                    break;
                                case 15:
                                    Console.WriteLine("quindecuple - " + walletAddress);
                                    break;
                                default:
                                    Console.WriteLine("Error");
                                    break;

                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("THREAD OLD!!!");
                }
            }

            lastRun = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
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
    }
}
