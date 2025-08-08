using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MTW.Common;

namespace MTW.BeginEnd
{
    class Program
    {
        // dotnet run https://learn.microsoft.com/en-gb/
        static void Main(string[] args)
        {
            // limiting the thread pool size
//            ThreadPool.SetMinThreads(2, 2);
//            ThreadPool.SetMaxThreads(2, 2);

            string rootLink = args[0];
            CrawlLinksRecursively(0, rootLink);

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }

        private const int MaxDegreeOfParallelizm = 10;
        private static int _numOfParallelRequests;

        static void CrawlLinksRecursively(int recursionLevel, params string[] rootLinks)
        {
            Console.WriteLine("> Level of recursion: " + recursionLevel);

            // Starting a bunch of requests
            var asyncResults = rootLinks.Select(link =>
            {
                var request = (HttpWebRequest)WebRequest.Create(link);
                request.UserAgent = Constants.UserAgentString;

                // For curiosity, let's count how many parallel requests we have at each particular moment of time
                Interlocked.Increment(ref _numOfParallelRequests);
                Console.WriteLine(">> Num of parallel requests: " + _numOfParallelRequests);

                // passing HttpWebRequest instance via asyncState
                return request.BeginGetResponse(null, request);
            })
            .ToArray(); // Important! We want to start them _all_ first.

            // Extracting all the links from all the pages
            var childLinks = WaitAndExtractLinks(asyncResults).ToArray();

            // Now processing found links in batches (as childLinks can potentially be huge)
            while (childLinks.Any())
            {
                CrawlLinksRecursively(recursionLevel + 1, childLinks.Take(MaxDegreeOfParallelizm).ToArray());

                childLinks = childLinks.Skip(MaxDegreeOfParallelizm).ToArray();
            }
        }

        /// <summary>
        /// Extracts all the links from all HTML responses
        /// </summary>
        static IEnumerable<string> WaitAndExtractLinks(IAsyncResult[] asyncResults)
        {
            foreach (var asyncResult in asyncResults)
            {
                string html;
                try
                {
                    var request = (HttpWebRequest)asyncResult.AsyncState!;

                    // A typical boilerplate, when messing with HttpWebRequest directly.
                    using var response = request.EndGetResponse(asyncResult);
                    using var stream = response.GetResponseStream();
                    using var reader = new StreamReader(stream);

                    html = reader.ReadToEnd();
                }
                catch (WebException ex)
                {
                    // Another typical boilerplate, when messing with HttpWebRequest directly.
                    ex.Response?.Dispose();

                    Console.WriteLine("!WebException: " + ex.Message);
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!Exception: " + ex.Message);
                    continue;
                }
                finally
                {
                    Interlocked.Decrement(ref _numOfParallelRequests);
                }

                // Extracting all the links
                for (var match = Constants.UrlRegex.Match(html); match.Success; match = match.NextMatch())
                {
                    string link = match.Groups[1].Value;

                    Console.WriteLine(link);

                    yield return link;
                }
            }
        }
    }
}
