using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bookie.Domain.Scraper
{
    public class Scraper
    {

        public async Task<GoogleResult> Scrape(string isbn)
        {
            var result = await GetJson(GetUrlForSearch(isbn));
            var gresult = JsonConvert.DeserializeObject<GoogleResult>(result);
            return gresult;
        }


        private string GetUrlForSearch(string isbn)
        {
            return "https://www.googleapis.com/books/v1/volumes?q=isbn:" + isbn;
        }

        private async Task<string> GetJson(string url)
        {
            var hClient = new HttpClient();
            var response = await hClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

    }
}
