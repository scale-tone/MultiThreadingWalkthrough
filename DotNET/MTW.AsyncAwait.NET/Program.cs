using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MTW.Common;

namespace MTW.AsyncAwait
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();

        // dotnet run https://learn.microsoft.com/en-gb/
        static async Task Main(string[] args)
        {
            Client.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgentString);

            string rootLink = args[0];
            await ProcessLinkAsync(rootLink);

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }

        static async Task ProcessLinkAsync(string link)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": " + link);

            try
            {
                string html = await Client.GetStringAsync(link);

                foreach (string nextLink in ExtractLinks(html))
                {
                    await ProcessLinkAsync(nextLink);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
            }
        }

        static readonly ConcurrentDictionary<string, string> LinkSet = new ConcurrentDictionary<string, string>();

        static IEnumerable<string> ExtractLinks(string html)
        {
            for (var match = Constants.UrlRegex.Match(html); match.Success; match = match.NextMatch())
            {
                string link = match.Groups[1].Value;

                // Performing deduplication. With this approach deduplication is essential,
                // as the first link on microsoft.com site is actually https://microsoft.com :)
                if (LinkSet.TryAdd(link.ToLower(), link))
                {
                    yield return link;
                }
            }
        }
    }
}
