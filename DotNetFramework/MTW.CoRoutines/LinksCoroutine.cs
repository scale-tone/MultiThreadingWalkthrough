using System.Collections.Generic;
using System.Linq;
using MTW.Common;

namespace MTW.CoRoutines
{
    /// <summary>
    /// Iterates through the list of found links
    /// </summary>
    class LinksCoroutine
    {
        private readonly Queue<string> _links = new Queue<string>();

        public LinksCoroutine(string rootLink)
        {
            this._links.Enqueue(rootLink);
        }

        /// <summary>
        /// Extracts and stores all links from HTML
        /// </summary>
        public void ParseHtml(string html)
        {
            for (var match = Constants.UrlRegex.Match(html); match.Success; match = match.NextMatch())
            {
                this._links.Enqueue(match.Groups[1].Value);
            }
        }

        /// <summary>
        /// Iterates through links
        /// </summary>
        public IEnumerable<string> GetLinks()
        {
            while (this._links.Any())
            {
                yield return this._links.Dequeue();
            }
        }
    }
}
