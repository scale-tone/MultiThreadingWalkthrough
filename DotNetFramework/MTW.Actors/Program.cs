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

        static void Main(string[] args)
        {
            var redis = RedisConnection.GetDatabase();

            if (args.Length > 0)
            {
                string rootLink = args[0];
                redis.ListLeftPush(ListKey, rootLink);
            }

            string link = redis.ListRightPop(ListKey);
            while (link != null)
            {
                ProcessNextLink(link);

                link = redis.ListRightPop(ListKey);
            }

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }

        static void ProcessNextLink(string link)
        {
            Console.WriteLine(link);

            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", Constants.UserAgentString);
                try
                {
                    string html = client.DownloadString(link);
                    ParseHtml(html);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                }
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
