using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScrapingExperiment
{
    internal class WebScraper
    {
        /// <summary>
        ///     A HTTP Get request of the URL is performed.
        /// </summary>
        /// <param name="Url"></param>
        /// <returns>
        ///     AngleSharp.Dom.IDocument
        /// </returns>
        public async Task<IDocument> GetWebpageAsync(string Url)
        {
            // Load default configuration
            var config = Configuration.Default.WithDefaultLoader();

            // Create a new browsing context
            var context = BrowsingContext.New(config);

            // This is where the HTTP request happens, returns <IDocument> that // we can query later
            var document = await context.OpenAsync(Url);

            // Check the HTML content that has been returned
            //Console.WriteLine(document.DocumentElement.OuterHtml);

            // Return the IDocument
            return document;
        }

        /// <summary>
        ///     Returns a list of the URLs for each chapter in the Table of Contents
        /// </summary>
        /// <param name="urlTableOfContents"></param>
        /// <returns>
        ///     A list of string containing th URLs of each chapter
        /// </returns>
        public Task<List<string>> GetChapterUrls(string urlTableOfContents)
        {
            // store the URLs; will be returned
            var listOfChapterUrls = new List<string>();

            var response = GetWebpageAsync(urlTableOfContents).Result;

            do
            {
                response = GetWebpageAsync(urlTableOfContents).Result;

                var chapterUrls = response
                    .QuerySelector("#list-chapter > .row")
                    .QuerySelectorAll("a")
                    .OfType<IHtmlAnchorElement>()
                    .Select(x => x.Href)
                    .ToList()
                    ;

                listOfChapterUrls.AddRange(chapterUrls); // as I know it only allows 50 per page

                // move to the next page (as there are 50 chapters per page)
                // this will error if there is no next page
                try
                {
                    var nextUrl = response
                                .QuerySelector("li.next")
                                .QuerySelectorAll("a")
                                .OfType<IHtmlAnchorElement>()
                                .Select(x => x.Href)
                                .First()
                                ;

                    urlTableOfContents = nextUrl;
                }
                catch (System.Exception)
                {
                    break;
                }
            } while (response.QuerySelector("#disabled") == null);

            return Task.FromResult(listOfChapterUrls);
        }

        /// <summary>
        ///     Gets the chapter name
        /// </summary>
        /// <param name="urlChapter"></param>
        /// <returns>string</returns>
        public string ExtractChapterTitle(string urlChapter)
        {
            var response = GetWebpageAsync(urlChapter).Result;

            var chapterTitle = response
                .QuerySelector(".chapter-text")
                .TextContent
                ;

            return chapterTitle;
        }

        /// <summary>
        ///     Returns the text in HTML element '#chapter-content'
        ///     Collapses the string array into single string separated by newlines
        /// </summary>
        /// <param name="urlChapter"></param>
        /// <returns>string</returns>
        public string ExtractChapterContent(string urlChapter)
        {
            var response = GetWebpageAsync(urlChapter).Result;

            //var chapterContentAsList = response
            //    .QuerySelector("#chapter-content")
            //    .QuerySelectorAll("p")
            //    .Select(x => x.TextContent)
            //    .ToList()
            //    ;

            var chapterContentAsList = response
                .QuerySelector("#chr-content")
                .QuerySelectorAll("p")
                .Select(x => x.TextContent)
                .ToList()
                ;

            var chapterContent = string.Join(System.Environment.NewLine, chapterContentAsList);

            return chapterContent;
        }
    }
}