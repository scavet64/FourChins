using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FourChins
{
    public class HTTPUtil
    {

        private static DateTime timeSinceLastAPICall = DateTime.UtcNow;

        private static TimeSpan waitInterval = new TimeSpan(0, 0, 1);

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
                TimeSpan tmp = timeSinceLastAPICall - DateTime.UtcNow;
                if (tmp < waitInterval)
                {
                    TimeSpan waitTime = waitInterval - tmp;
                    System.Threading.Thread.Sleep(waitTime);
                }
                
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
                timeSinceLastAPICall = DateTime.UtcNow;
                return Encoding.UTF8.GetString(array, 0, array.Length);
            }
        }
    }
}
