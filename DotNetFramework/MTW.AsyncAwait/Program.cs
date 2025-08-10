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
        static void Main(string[] args)
        {
            string rootLink = args[0];
            ProcessLinkAsync(rootLink).Wait();

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }

        static async Task ProcessLinkAsync(string link)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": " + link);

            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", Constants.UserAgentString);
                try
                {
                    string html = await client.DownloadStringTaskAsync(link);

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
