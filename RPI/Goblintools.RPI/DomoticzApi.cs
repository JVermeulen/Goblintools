using Flurl;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goblintools.RPI.Actors
{
    public class DomoticzApi
    {
        /// <summary>
        /// The timeout when http requests (GET, POST, PUT en DELETE) are send.
        /// </summary>
        public int Timeout { get; set; }

        public DomoticzApi(int timeout = 15000)
        {
            Timeout = timeout;
        }

        public HttpStatusCode UpdateDevice(string apiUrl, int idx, object nValue, string sValue)
        {
            var url = Url.Combine(apiUrl, "json.htm")
                .SetQueryParam("type", "command")
                .SetQueryParam("param", "udevice")
                .SetQueryParam("idx", idx)
                .SetQueryParam("nvalue", nValue)
                .SetQueryParam("svalue", sValue);

            return Get(url);
        }

        public HttpStatusCode Get(string url)
        {
            try
            {
                using (var source = new CancellationTokenSource())
                {
                    source.CancelAfter(Timeout);

                    return GetAsync(url, source.Token).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggregateException && aggregateException.InnerException != null && aggregateException.InnerException is TaskCanceledException taskCanceledException)
                    throw new ApplicationException($"Unable to get request '{url}' because the server did not respond within {Timeout} ms.", ex);
                else
                    throw new ApplicationException($"Unable to get request '{url}'.", ex);
            }
        }

        public async Task<HttpStatusCode> GetAsync(string url, CancellationToken token)
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(url, token))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"GET {url} {response.StatusCode}");

                    return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggregateException && aggregateException.InnerException != null && aggregateException.InnerException is TaskCanceledException taskCanceledException)
                    throw new ApplicationException($"Unable to request '{url}' because the server did not respond within {Timeout} ms.", ex);
                else
                    throw new ApplicationException($"Unable to request '{url}'.", ex);
            }
        }
    }
}