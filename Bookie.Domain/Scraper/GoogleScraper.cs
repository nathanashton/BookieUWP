using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;

namespace Bookie.Domain.Scraper
{
    public class GoogleScraper : IBookScraper
    {
        private static DateTime lastTimeIWasCalled = DateTime.Now;
        public SearchResult.Search SearchBy { get; set; }
        public object SearchQuery { get; set; }

        public async Task<ObservableCollection<SearchResult>> SearchBooks(object searchQuery)
        {
            var searchResults = new ObservableCollection<SearchResult>();

            SearchQuery = searchQuery;

            if (searchQuery == null)
            {
                throw new ArgumentNullException("searchQuery");
            }

            if (DateTime.Now.Subtract(lastTimeIWasCalled).Seconds < 1)
            {
                Task.Delay(TimeSpan.FromSeconds(1));
            }

            lastTimeIWasCalled = DateTime.Now;

            var foundBook = new Book();

            var result = await GetXml(GetUrlForSearch());

            Regex titleRegex = new Regex("<dc:title>(.*)</dc:title>");
            var titleMatch = titleRegex.Match(result);
            if (titleMatch.Success)
            {
                foundBook.Title = titleMatch.Groups[1].ToString();
            }

            Regex abstractRegex = new Regex("<dc:description>(.*)</dc:description>");
            var abstractMatch = abstractRegex.Match(result);
            if (abstractMatch.Success)
            {
                foundBook.Abstract = abstractMatch.Groups[1].ToString();
            }

            DateTime publishedDate;

            Regex dateRegex = new Regex("<dc:date>(.*)</dc:date>");
            var dateMatch = dateRegex.Match(result);
            if (dateMatch.Success)
            {
                DateTime.TryParse(dateMatch.Groups[1].ToString(), out publishedDate);
                if (publishedDate != null || publishedDate != DateTime.MinValue)
                {
                    foundBook.DatePublished = publishedDate;
                }
            }

            Regex authorRegex = new Regex("<dc:creator>(.*)</dc:creator>");
            var authorMatch = authorRegex.Match(result);
            if (authorMatch.Success)
            {
               foundBook.Author = authorMatch.Groups[1].ToString();
            }

            Regex publisherRegex = new Regex("<dc:publisher>(.*)</dc:publisher>");
            var publisherMatch = publisherRegex.Match(result);
            if (publisherMatch.Success)
            {
                foundBook.Publisher = publisherMatch.Groups[1].ToString();
            }

            //Regex pagesRegex = new Regex("<dc:format>(.*)</dc:format>");
            //var pagesMatch = pagesRegex.Match(result);
            //if (pagesMatch.Success)
            //{
            //    var p = pagesMatch.Groups[1].ToString();
            //    Regex re = new Regex(@"\d+");
            //    Match m = re.Match(p);
            //    if (m.Success)
            //    {
            //        foundBook.Pages = Convert.ToInt32(m.Groups[1].ToString());
            //    }

            //}


            SearchResult sresult = new SearchResult();

            if (String.IsNullOrEmpty(foundBook.Title))
            {
                sresult.Book = null;

            }
            else
            {
                sresult.Book = foundBook;

            }


            searchResults.Add(sresult);



            //try
            //{
            //    xdcDocument = XDocument.Parse(result);
            //    XNamespace ns = "http://www.w3.org/2005/Atom";
            //    var nodes = xdcDocument.Descendants(ns + "entry");
            //    if (nodes != null) // found a result
            //    {
            //        var t = nodes.Descendants();
            //        var title = t.Nodes();
            //    }

            //}
            //catch (Exception ex)
            //{

            //}








            // var ns = new XmlNamespaceManager(xdcDocument.NameTable);
            //ns.AddNamespace("openSearch", "http://a9.com/-/spec/opensearchrss/1.0/");
            //ns.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            //ns.AddNamespace("dc", "http://purl.org/dc/terms");

            //var xelRoot = xdcDocument.DocumentElement;
            //var xnlNodes = xelRoot.SelectNodes("//atom:entry", ns);

            //  var xnlNodes = xdcDocument.Element("//atom:entry");

            //foreach (XmlNode xndNode in xnlNodes)
            //{
            //    var searchResult = new SearchResult();
            //    var book = new Book();

            //    var xmlTitle = xndNode["title"];
            //    if (xmlTitle != null)
            //    {
            //        book.Title = xmlTitle.InnerText;
            //    }
            //    var xmlDatePublished = xndNode["dc:date"];
            //    if (xmlDatePublished != null)
            //    {
            //        var dp = xmlDatePublished.InnerText;
            //        if (dp.Length == 4)
            //        {
            //            var dt = Convert.ToDateTime(string.Format("01/01/,{0}", dp));
            //            book.DatePublished = dt;
            //        }
            //        else
            //        {
            //            DateTime dt;
            //            if (DateTime.TryParse(dp, out dt))
            //            {
            //                book.DatePublished = dt;
            //            }
            //        }
            //    }

            //    var xmlDescription = xndNode["dc:description"];
            //    if (xmlDescription != null)
            //    {
            //        // Remove ASCII Control Characters
            //        book.Abstract = xmlDescription.InnerText.Replace("&#39;", "'");
            //        book.Abstract = xmlDescription.InnerText.Replace("&quot;", string.Empty);
            //    }

            //    var formatNodes = xndNode.SelectNodes("dc:format", ns);
            //    if (formatNodes != null)
            //    {
            //        foreach (XmlNode node in formatNodes)
            //        {
            //            if (node.InnerText.Contains("pages"))
            //            {
            //                var resultString = Regex.Match(node.InnerText, @"\d+").Value;
            //                book.Pages = Convert.ToInt32(resultString);
            //            }
            //        }
            //    }

            //    var identifierNodes = xndNode.SelectNodes("dc:identifier", ns);
            //    if (identifierNodes != null)
            //    {
            //        foreach (XmlNode node in identifierNodes)
            //        {
            //            if (node.InnerText.Length == 18) // 13 digit ISBN Node
            //            {
            //                book.Isbn = Regex.Match(node.InnerText, @"\d+").Value;
            //            }
            //        }
            //    }

            //    var xmlPublisher = xndNode["dc:publisher"];
            //    if (xmlPublisher != null)
            //    {
            //     //   searchResult.Publishers.Add(new Publisher { Name = xmlPublisher.InnerText });
            //    }

            //    var xmlAuthor = xndNode["dc:creator"];
            //    if (xmlAuthor != null)
            //    {
            //        var words = xmlAuthor.InnerText.Split(' ');
            //        if (words.Length == 1)
            //        {
            //          //  searchResult.Authors.Add(new Author { LastName = words[0] });
            //        }
            //        else
            //        {
            //            //searchResult.Authors.Add(new Author
            //            //{
            //            //    FirstName = words[0],
            //            //    LastName = words[1]
            //            //});
            //        }
            //    }

            //   // book.Scraped = true;
            //    searchResult.Book = book;
            //    // searchResult.Percentage = MatchStrings.Compute(searchQuery.ToString(), book.Title)*100;
            //    searchResults.Add(searchResult);
            //}
            //  var s = searchResults.OrderByDescending(x => x.Percentage);
            // return new ObservableCollection<SearchResult>(s);
            return searchResults;
        }

        private string GetUrlForSearch()
        {
            SearchQuery = WebUtility.UrlEncode(SearchQuery.ToString());

            return "http://books.google.com/books/feeds/volumes?q=isbn:" + SearchQuery +
                   "&max-results=20&start-index=1&min-viewability=none";
        }

        private async Task<string> GetXml(string url)
        {
            var hClient = new HttpClient();
            var response = await hClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}