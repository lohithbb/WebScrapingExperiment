﻿using AngleSharp.Html.Dom;
using System.Collections.Generic;
using System.Linq;

namespace WebScrapingExperiment
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            #region Initial Trials
            {
                //// URL - of the novel ToC
                ////var url = "https://novelfull.com/release-that-witch.html";

                //// create the webscraper object
                //var webScraper = new WebScraper();

                //// get the page as an IDocument
                //var document = webScraper.GetWebpageAsync(url).Result;

                //var y = document.All
                //    // get section with the list of chapters
                //    .Where(e => e.LocalName == "ul" && e.HasAttribute("id") && e.GetAttribute("id") == "list-chapter");
                ////.Where(e => e.LocalName == "ul" && e.HasAttribute("class") && e.GetAttribute("class") == "row");
                ////.Where(e => e.LocalName == "a" && e.HasAttribute("href"));
                ////.Select((e,f) => (e.GetAttribute("href"),(e.GetAttribute("title"))));
                ////.Where(e => e.LocalName == "a")
                ////.Select(e => e.LocalName);

                //var z = document
                //    .QuerySelector("#list-chapter > .row")
                //    .QuerySelectorAll("a")
                //    .OfType<IHtmlAnchorElement>()
                //    //.Select(x => x.GetAttribute("href"))
                //    .ToList()
                //    .Select(x => x.Href)
                ////    .QuerySelectorAll("a");
                //    ;
                //// this worked ...
            }
            #endregion Initial Trials

            // so lets try to get a list of URLs for each of the chapter names
            // once it gets written to file, the call to get chapters can be disabled.
            // Note that the file will persist in the bin folder
            #region Get Chapter Urls

            {
                var listOfChapterUrls = new List<string>();

                //var urlTableOfContents = "https://novelfull.com/release-that-witch.html";
                var urlTableOfContents = "https://readnovelfull.me/the-regressed-demon-lord-is-kind/";

                var webScraper = new WebScraper();
                listOfChapterUrls = webScraper.GetChapterUrls(urlTableOfContents).Result;

                // store in file
                System.IO.File.WriteAllLines("chapterUrls.txt", listOfChapterUrls);
            }

            #endregion Get Chapter Urls


            #region Experiment - Get content in one chapter
            {
                ////var inputWorkingWith = "https://novelfull.com/release-that-witch/chapter-1226-the-prison-of-the-heart.html";
                //var inputWorkingWith = "https://readnovelfull.me/the-regressed-demon-lord-is-kind/chapter-1/";

                //var webScraper = new WebScraper();
                //var chapterContent = webScraper.ExtractChapterContent(inputWorkingWith);

                //// sanitise
                //// remove leading/trailing whitespace/newlines
                //chapterContent = chapterContent.Trim(System.Environment.NewLine.ToCharArray());

                //// markdown treats 2 newlines as a new line ...
                //chapterContent = chapterContent.Replace(System.Environment.NewLine, System.Environment.NewLine + System.Environment.NewLine);

                //// write file to disk
                //System.IO.File.WriteAllText("experimentchapter.md", chapterContent);
            }
            #endregion Experiment - Get content in one chapter

            #region Get content for all chapters
            {

                // read chapter URLs from saved file (stored a copy of this)
                var chapterUrls = System.IO.File.ReadAllLines("chapterUrls.txt");

                var webScraper = new WebScraper();

                //get each chapter and save it
                foreach (var chapterUrl in chapterUrls)
                {
                    // get chapter title for file name
                    //var chapterTitle = webScraper.ExtractChapterTitle(chapterUrl);
                    // this was not fit for purpose as its not guarenteed to be safe; better to use the URL
                    //var chapterTitle = chapterUrl.Split('/').Last().Replace(".html", "");
                    // another pattern, where there is a trailing '/'
                    var chapterTitle = chapterUrl.Split('/').SkipLast(1).Last();

                    var chapterContent = webScraper.ExtractChapterContent(chapterUrl);

                    // sanitise
                    // remove leading/trailing whitespace/newlines
                    chapterContent = chapterContent.Trim(System.Environment.NewLine.ToCharArray());

                    // markdown treats 2 newlines as a new line ...
                    chapterContent = chapterContent.Replace(System.Environment.NewLine, System.Environment.NewLine + System.Environment.NewLine);

                    // write file to disk
                    System.IO.File.WriteAllText($"{chapterTitle}.txt", chapterContent);
                }

            }
            #endregion
        }


    }
}