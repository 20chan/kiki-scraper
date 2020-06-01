using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace kiki.scraper {
    public class Html {
        public static async Task<string> GetAsync(string url) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public static async Task<HtmlDocument> ParseGetAsync(string url) {
            var doc = new HtmlDocument();
            doc.LoadHtml(await GetAsync(url));
            return doc;
        }
    }
}