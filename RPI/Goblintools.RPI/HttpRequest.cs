using Goblintools.RPI.Processing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goblintools.RPI
{
    public static class HttpRequest
    {
        public static int Timeout { get; set; } = 15000;

        public static bool TryGet(string url, out string result)
        {
            result = null;

            try
            {
                using (var source = new CancellationTokenSource())
                {
                    source.CancelAfter(Timeout);

                    result = GetAsync(url, source.Token).Result;
                }
            }
            catch
            {
                //
            }

            return result != null;
        }

        private static async Task<string> GetAsync(string url, CancellationToken token)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url, token))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                    return await response.Content.ReadAsStringAsync();
                else
                    return null;
            }
        }

        public static bool TryPost(string url, string body, Dictionary<string, string> headers, out string result)
        {
            result = null;

            Processor.WriteToConsole($"POST {url} {body}");

            try
            {
                var content = new StringContent(body ?? string.Empty, Encoding.UTF8, "application/json");

                using (var source = new CancellationTokenSource())
                {
                    source.CancelAfter(Timeout);

                    result = PostAsync(url, content, headers, source.Token).Result;
                }
            }
            catch (Exception ex)
            {
                var message = $"POST {url} - {body} - {result}";

                Processor.WriteToConsole(ex.Message + Environment.NewLine + message, ConsoleColor.Red);
            }

            return result != null;
        }

        private static async Task<string> PostAsync(string url, HttpContent content, Dictionary<string, string> headers, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                using (var response = await client.PostAsync(url, content, token))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return await response.Content.ReadAsStringAsync();
                    else
                        return null;
                }
            }
        }
    }
}
