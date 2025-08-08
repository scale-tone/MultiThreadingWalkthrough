using System.Net;
using System.Text;
using MTW.Common;

namespace MTW.CoRoutines
{
    class Program
    {
        // dotnet run https://learn.microsoft.com/en-gb/
        static async Task Main(string[] args)
        {
            string rootLink = args[0];
            var coroutine = new LinksCoroutine(rootLink);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgentString);

            foreach (string link in coroutine.GetLinks())
            {
                Console.WriteLine(link);

                try
                {
                    using var response = await client.GetAsync(link);
                    string html = await response.Content.ReadAsStringAsync();

                    coroutine.ParseHtml(html);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!Exception: " + ex.Message + ". (" + link + ")");
                }
            }

            // Actually, it will never be finished :)
            Console.WriteLine("Finished!");
        }
    }
}
