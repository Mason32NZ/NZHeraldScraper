using System;

namespace NZHeraldScraper
{
    public class Article
    {
        public string Country { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }

        public override string ToString()
        {
            return $"{Country}\n\n{Title}\n{Date:d MMMM, yyyy h:mm tt}\n\nBy {Author}\n\n\n{Body}";
        }
    }
}
