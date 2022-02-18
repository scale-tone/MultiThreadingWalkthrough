using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MTW.Common;

namespace MTW.LockFree
{
    class Program
    {
        static readonly LockFreeStack<string> Links = new LockFreeStack<string>();

        static void Main(string[] args)
        {
            string rootLink = args[0];
            Links.Push(rootLink);

            // Two threads would be enough
            new Thread(ThreadFunc).Start();
            ThreadFunc();
        }

        static void ThreadFunc()
        {
            // Never ending so far
            string link = Links.Pop();
            while (true)
            {
                if (link != null)
                {
                    ProcessNextLink(link);
                }
                link = Links.Pop();
            }
        }

        static void ProcessNextLink(string link)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": " + link);

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
            // Let's put the found links into a HashSet first, to reduce the chance for duplicates and distribute them more evenly.
            var linkList = new HashSet<string>();

            for (var match = Constants.UrlRegex.Match(html); match.Success; match = match.NextMatch())
            {
                linkList.Add(match.Groups[1].Value);
            }

            foreach (var link in linkList)
            {
                Links.Push(link);
            }
        }
    }
}
