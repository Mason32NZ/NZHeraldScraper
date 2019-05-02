using System.IO;
using System.Text.RegularExpressions;

namespace NZHeraldScraper
{
    public class Utility
    {
        public static string RemoveHtmlTags(string html)
        {
            var regex = "(</?(!(DOCTYPE|doctype).+?|\\w+((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[\\^'\">\\s]+))?)+\\s*|\\s*))/?>)";
            return Regex.Replace(html, regex, string.Empty).Trim();
        }

        public static string MakeFileNameSafe(string text)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                text = text.Replace(c.ToString(), string.Empty);
            }
            return text;
        }
    }
}
