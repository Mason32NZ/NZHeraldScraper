using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace NZHeraldScraper
{
    public class Article
    {
        public Article(string json)
        {
            dynamic raw = JsonConvert.DeserializeObject(json);

            Title = raw.Title.ToString().Trim();
            Date = DateTime.ParseExact(Regex.Replace(raw.Date.ToString(), "(?<=pm|am).+", string.Empty).Trim(), "d MMMM, yyyy h:mmtt", CultureInfo.InvariantCulture);
            Author = "";
            Body = "";

            if (raw.Author != null)
            {
                Author = raw.Author.ToString().Trim();
                if (raw.AuthorBio != null)
                {
                    Author += $" ({raw.AuthorBio.ToString().Trim()})";
                }
            }
            foreach (var item in raw.Paragraphs)
            {
                Body += $"{Utility.RemoveHtmlTags(item.ToString())}\n\n";
            }
            Body = Body.Trim();
        }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }

        public override string ToString()
        {
            var str = $"{Title}\n{Date:d MMMM, yyyy h:mm tt}";
            if (!string.IsNullOrWhiteSpace(Author))
            {
                str += $"\n\nBy {Author}";
            }
            str += $"\n\n\n{Body}";

            return str;
        }
    }
}
