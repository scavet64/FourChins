using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FourChins;

namespace FChan.Library
{
    /// <summary>
    /// Chan.
    /// </summary>
    public static class FourChinCore
    {
        
        /// <summary>
        /// Gets boards info.
        /// </summary>
        /// <returns>The board.</returns>
        public static BoardRootObject GetBoard()
        {
            return HTTPUtil.DownloadObject<BoardRootObject>(Constants.BoardsUrl);
        }

        /// <summary>
        /// Gets boards info asynchronously.
        /// </summary>
        /// <returns>The board.</returns>
        public static async Task<BoardRootObject> GetBoardAsync()
        {
            return await HTTPUtil.DownloadObjectAsync<BoardRootObject>(Constants.BoardsUrl);
        }

        /// <summary>
        /// Gets thead root object.
        /// </summary>
        /// <returns>The thread page.</returns>
        /// <param name="board">Board.</param>
        /// <param name="page">Page.</param>
        public static ThreadRootObject GetThreadPage(string board, int page)
        {
            ThreadRootObject thread = HTTPUtil.DownloadObject<ThreadRootObject>(Constants.GetThreadPageUrl(board, page));

            if (thread != null)
            {
                foreach (Thread item in thread.Threads)
                    foreach (Post post in item.Posts)
                        post.Board = board;
            }
            return thread;
        }

        /// <summary>
        /// Gets thead root object asynchronously.
        /// </summary>
        /// <returns>The thread page.</returns>
        /// <param name="board">Board.</param>
        /// <param name="page">Page.</param>
        public static async Task<ThreadRootObject> GetThreadPageAsync(string board, int page)
        {
            ThreadRootObject thread = await HTTPUtil.DownloadObjectAsync<ThreadRootObject>(Constants.GetThreadPageUrl(board, page));

            if (thread != null)
            {
                foreach (Thread item in thread.Threads)
                    foreach (Post post in item.Posts)
                        post.Board = board;
            }
            return thread;
        }

        /// <summary>
        /// Gets the thread.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static Thread GetThread(string board, int threadNumber)
        {
            Thread thread = HTTPUtil.DownloadObject<Thread>(Constants.GetThreadUrl(board, threadNumber));
            if(thread != null)
            {
                foreach (Post item in thread.Posts)
                    item.Board = board;
            }
            return thread;
        }

        /// <summary>
        /// Gets the thread.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static Thread GetThread(string board, string threadNumber)
        {
            return GetThread(board, int.Parse(threadNumber));
        }

        /// <summary>
        /// Gets the thread asynchronously.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static async Task<Thread> GetThreadAsync(string board, int threadNumber)
        {
            Thread thread = await HTTPUtil.DownloadObjectAsync<Thread>(Constants.GetThreadUrl(board, threadNumber));

            if(thread != null)
            {
                foreach (Post item in thread.Posts)
                    item.Board = board;
            }

            return thread;
        }

        /// <summary>
        /// Gets the thread.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static List<Thread> GetAllThreadsFromBoard(string board)
        {
            List<Thread> returnThreadList = new List<Thread>();
            List<Page> pages = GetPagesFromBoard(board);

            if (pages != null)
            {
                foreach (Page page in pages)
                {
                    returnThreadList.AddRange(page.Threads);
                }
            }

            return returnThreadList;
        }

        /// <summary>
        /// Gets the thread.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static async Task<List<Thread>> GetAllThreadsFromBoardAsync(string board)
        {
            List<Thread> returnThreadList = new List<Thread>();
            List<Page> pages = await GetPagesFromBoardAsync(board);

            if (pages != null)
            {
                foreach (Page page in pages)
                {
                    returnThreadList.AddRange(page.Threads);
                }
            }
            return returnThreadList;
        }

        /// <summary>
        /// Gets the list of pages for a board.
        /// </summary>
        /// <param name="board">Board to find pages for</param>
        /// <returns>List of page objects that contain information about what threads are on each page</returns>
        public static List<Page> GetPagesFromBoard(string board)
        {
            return HTTPUtil.DownloadObject<List<Page>>(Constants.GetThreadsUrl(board));
        }

        /// <summary>
        /// Gets the list of pages for a board.
        /// </summary>
        /// <param name="board">Board to find pages for</param>
        /// <returns>List of page objects that contain information about what threads are on each page</returns>
        public static Task<List<Page>> GetPagesFromBoardAsync(string board)
        {
            return HTTPUtil.DownloadObjectAsync<List<Page>>(Constants.GetThreadsUrl(board));
        }

    }
}
