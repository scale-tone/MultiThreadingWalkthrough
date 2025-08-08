using System;
using System.Net;
using MTW.Common;
using StackExchange.Redis;

namespace MTW.Actors
{
    class Program
    {
        private static readonly ConnectionMultiplexer RedisConnection = ConnectionMultiplexer.Connect("localhost:6379");
        private const string ListKey = "link_queue";

        // dotnet run https://learn.microsoft.com/en-gb/
        static async Task Main(string[] args)
        {
            var redis = RedisConnection.GetDatabase();

            if (args.Length > 0)
            {
                string rootLink = args[0];
                redis.ListLeftPush(ListKey, rootLink);
            }

            string? link = redis.ListRightPop(ListKey);
            while (link != null)
            {
                await ProcessNextLink(link);

                link = redis.ListRightPop(ListKey);
            }

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }

        static async Task ProcessNextLink(string link)
        {
            Console.WriteLine(link);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgentString);

            try
            {
                using var response = await client.GetAsync(link);
                string html = await response.Content.ReadAsStringAsync();
                
                ParseHtml(html);
            }
            catch (Exception ex)
            {
                Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
            }
        }

        static void ParseHtml(string html)
        {
            var redis = RedisConnection.GetDatabase();

            for (var match = Constants.UrlRegex.Match(html); match.Success; match = match.NextMatch())
            {
                string link = match.Groups[1].Value;
                redis.ListLeftPush(ListKey, link);
            }
        }
    }
}
