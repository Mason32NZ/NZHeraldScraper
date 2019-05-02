using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using RestSharp;

namespace NZHeraldScraper
{
    class Program
    {
        static void Main()
        {
            Console.Write("Please enter the NZ Herald article URL: ");
            var url = Console.ReadLine();
            Console.WriteLine();

            if (url.Contains("https://www.nzherald.co.nz/nz/news/article.cfm"))
            {
                var uri = new Uri(url);

                Console.WriteLine("INFO: Processing...");

                try
                {
                    var article = GetArticle(uri);
                    var filename = $"{article.Date.ToString("ddMMyyyy")}_{Utility.MakeFileNameSafe(article.Title).Replace(" ", "_")}.txt";

                    FileInfo file = new FileInfo($"articles/{filename}");
                    file.Directory.Create();
                    File.WriteAllText(file.FullName, article.ToString());

                    Console.WriteLine($"INFO: File saved as '{filename}'.");
                    Console.WriteLine("INFO: Success!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: {e}");
                }
            }
            else
            {
                Console.WriteLine("ERROR: The URL you have provided is not a NZ Herald article URL!");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        static Article GetArticle(Uri uri)
        {
            // Get the article HTML.
            var client = new RestClient("https://www.nzherald.co.nz");

            var request = new RestRequest($"{string.Join(string.Empty, uri.Segments).Substring(1)}{uri.Query}", Method.GET);
            var response = client.Execute(request);
            var html = response.Content;

            // Scrap the article HTML.
            var stream = new StreamReader(new MemoryStream(Properties.Resources.config));
            var config = StructuredDataConfig.ParseJsonString(stream.ReadToEnd());
            var scraper = new StructuredDataExtractor(config);
            dynamic raw = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(scraper.Extract(html)));

            // Clean raw data and bind to object.
            var article = new Article
            {
                Country = raw.Country.ToString().Trim(),
                Title = raw.Title.ToString().Trim(),
                Date = DateTime.ParseExact(Regex.Replace(raw.Date.ToString(), "(?<=pm|am).+", string.Empty).Trim(), "d MMMM, yyyy h:mmtt", CultureInfo.InvariantCulture),
                Author = $"{raw.Author.ToString().Trim()} ({raw.AuthorBio.ToString().Trim()})",
                Body = ""
            };
            foreach (var item in raw.Paragraphs.Text)
            {
                article.Body += $"{Utility.RemoveHtmlTags(item.ToString())}\n\n";
            }
            article.Body = article.Body.Trim();

            return article;
        }
    }
}
