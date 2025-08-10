using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using MTW.Common;

namespace MTW.CoRoutines
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootLink = args[0];
            var coroutine = new LinksCoroutine(rootLink);

            foreach (string link in coroutine.GetLinks())
            {
                Console.WriteLine(link);

                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", Constants.UserAgentString);
                    try
                    {
                        string html = client.DownloadString(link);
                        coroutine.ParseHtml(html);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                    }
                }
            }

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }
    }
}
