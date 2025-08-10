using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MTW.Common;

namespace MTW.Tasks
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();

        // dotnet run https://learn.microsoft.com/en-gb/
        static void Main(string[] args)
        {
            Client.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgentString);

            string rootLink = args[0];

            Console.WriteLine("Press any key to proceed and then any key to cancel");
            Console.ReadKey();

            // ProcessLinkWithTasks(rootLink);
            ProcessLinkWithLimitedAmountOfTasks(rootLink);

            Console.ReadKey();
        }

        static void ProcessLinkWithTasks(string link)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": " + link);

            Client.GetStringAsync(link).ContinueWith(task =>
            {
                try
                {
                    foreach (var nextLink in ParseHtml(task.Result))
                    {
                        ProcessLinkWithTasks(nextLink);
                    }
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine("!Exception: " + ex.InnerException?.Message + ". (" + link + ")");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                }
            });
        }

        private const int MaxDegreeOfParallelism = 10;
        static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(MaxDegreeOfParallelism, MaxDegreeOfParallelism);

        static void ProcessLinkWithLimitedAmountOfTasks(string link)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": " + link);

            Semaphore.WaitAsync().ContinueWith(_ =>
            {
                Client.GetStringAsync(link).ContinueWith(task =>
                {
                    try
                    {
                        foreach (var nextLink in ParseHtml(task.Result))
                        {
                            ProcessLinkWithLimitedAmountOfTasks(nextLink);
                        }
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine("!Exception: " + ex.InnerException?.Message + ". (" + link + ")");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                    }
                });
            });
        }

        static IEnumerable<string> ParseHtml(string html)
        {
            for (var match = Constants.UrlRegex.Match(html); match.Success; match = match.NextMatch())
            {
                yield return match.Groups[1].Value;
            }
        }
    }
}
