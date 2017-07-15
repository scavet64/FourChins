using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FChan.Library
{
    /// <summary>
    /// Chan.
    /// </summary>
    public static class FourChinCore
    {
        internal static T DownloadObject<T>(string url)
        {
            var task = DownloadObjectAsync<T>(url);
            task.Wait();
            return task.Result;
        }

        internal static async Task<T> DownloadObjectAsync<T>(string url)
        {
            try
            {
                string response = await GetStringAsync(url);
                string responseString = response;
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        private static async Task<string> GetStringAsync(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var task = new TaskCompletionSource<WebResponse>();

            request.BeginGetResponse(ac =>
            {
                try
                {
                    task.SetResult(request.EndGetResponse(ac));
                }
                catch (Exception e)
                {
                    task.SetException(e);
                }
            }, null);

            using (var response = await task.Task)
            using (var stream = response.GetResponseStream())
            using (var output = new MemoryStream())
            {

                await stream.CopyToAsync(output);
                var array = output.ToArray();
                return Encoding.UTF8.GetString(array, 0, array.Length);
            }
        }

        /// <summary>
        /// Gets boards info.
        /// </summary>
        /// <returns>The board.</returns>
        public static BoardRootObject GetBoard()
        {
            return DownloadObject<BoardRootObject>(Constants.BoardsUrl);
        }

        /// <summary>
        /// Gets boards info asynchronously.
        /// </summary>
        /// <returns>The board.</returns>
        public static async Task<BoardRootObject> GetBoardAsync()
        {
            return await DownloadObjectAsync<BoardRootObject>(Constants.BoardsUrl);
        }

        /// <summary>
        /// Gets thead root object.
        /// </summary>
        /// <returns>The thread page.</returns>
        /// <param name="board">Board.</param>
        /// <param name="page">Page.</param>
        public static ThreadRootObject GetThreadPage(string board, int page)
        {
            ThreadRootObject thread = DownloadObject<ThreadRootObject>(Constants.GetThreadPageUrl(board, page));

            foreach (Thread item in thread.Threads)
                foreach (Post post in item.Posts)
                    post.Board = board;

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
            ThreadRootObject thread = await DownloadObjectAsync<ThreadRootObject>(Constants.GetThreadPageUrl(board, page));

            foreach (Thread item in thread.Threads)
                foreach (Post post in item.Posts)
                    post.Board = board;

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
            Thread thread = DownloadObject<Thread>(Constants.GetThreadUrl(board, threadNumber));
            foreach (Post item in thread.Posts)
                item.Board = board;

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
            Thread thread = await DownloadObjectAsync<Thread>(Constants.GetThreadUrl(board, threadNumber));
            foreach (Post item in thread.Posts)
                item.Board = board;

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
            foreach (Page page in pages)
            {
                returnThreadList.AddRange(page.Threads);
            }

            return returnThreadList;
        }

        public static List<Page> GetPagesFromBoard(string board)
        {
            List<Page> pages = DownloadObject<List<Page>>(Constants.GetThreadsUrl(board));
            return pages;
        }

        /// <summary>
        /// Gets the thread asynchronously.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static async Task<Thread> GetThreadsAsync(string board)
        {
            //Thread thread = await DownloadObjectAsync<Thread>(Constants.GetThreadsUrl(board));
            //foreach (Post item in thread.Posts)
            //    item.Board = board;

            //return thread;
            throw new NotImplementedException();
        }
    }
}
