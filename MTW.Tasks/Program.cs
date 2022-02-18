using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MTW.Common;

namespace MTW.Tasks
{
    class Program
    {
        static void Main(string[] args)
        {
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

            var client = new WebClient();
            client.Headers.Add("user-agent", Constants.UserAgentString);

            client.DownloadStringTaskAsync(link).ContinueWith(task =>
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
                    Console.WriteLine("!Exception: " + ex.InnerException.Message + ". (" + link + ")");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                }
                finally
                {
                    client.Dispose();
                }
            });
        }

        private const int MaxDegreeOfParallelism = 10;
        static readonly SemaphoreSlim WebClientSemaphore = new SemaphoreSlim(MaxDegreeOfParallelism, MaxDegreeOfParallelism);

        static void ProcessLinkWithLimitedAmountOfTasks(string link)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": " + link);

            var client = new WebClient();
            client.Headers.Add("user-agent", Constants.UserAgentString);

            WebClientSemaphore.WaitAsync().ContinueWith(_ =>
            {
                client.DownloadStringTaskAsync(link).ContinueWith(task =>
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
                        Console.WriteLine("!Exception: " + ex.InnerException.Message + ". (" + link + ")");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                    }
                    finally
                    {
                        client.Dispose();
                        WebClientSemaphore.Release();
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
