using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class FilmSeach:Abstract
    {
        public List<MovieResult> Find(string title, string year)
        {
            var lstMovies = new List<MovieResult>();

            title = Utils.FixTitleForSearching(title); // fix wrnog symbols
            var pattern = new Regex(@"\s+");
            title = string.Join("+", pattern.Split(title));
            if (!string.IsNullOrEmpty(year)) title += "+" + year;
            string contents = PageFetch("http://www.kinopoisk.ru/level/7/type/film/list/1/find/" + System.Uri.EscapeUriString(title));

            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(contents);

            HtmlAgilityPack.HtmlNode document = html.DocumentNode;
            IEnumerable<HtmlAgilityPack.HtmlNode> films = document.QuerySelectorAll("body div.search_results div.info");

            var idPattern = new Regex(@"\/film\/(\d+)\/");
            var titlePattern = new Regex("[А-Яа-я]+");

            foreach (HtmlAgilityPack.HtmlNode filmHeader in films)
            {
                // define whether film title is local
                string confirmMovieTitle = titlePattern.Match(title).Success ? GetLocalFilmTitle(filmHeader) : GetGlobalFilmTitle(filmHeader);
                string confirmMovieYear = filmHeader.QuerySelector("p.name span.year").InnerText;

                string confirmMovieIDs = null;
                var matches = idPattern.Match(filmHeader.QuerySelector("a").Attributes["href"].Value);
                if (matches.Groups.Count == 2)
                {
                    confirmMovieIDs = matches.Groups[1].Value;
                }


                if (confirmMovieYear == year && !string.IsNullOrEmpty(confirmMovieIDs))
                {
                    lstMovies.Add(new MovieResult() { ID = confirmMovieIDs, Title = confirmMovieTitle, Year = confirmMovieYear });
                }
            }
            return lstMovies;
        }

        protected string GetLocalFilmTitle(HtmlAgilityPack.HtmlNode info)
        {
            return info.QuerySelector("a").InnerText;
        }

        protected string GetGlobalFilmTitle(HtmlAgilityPack.HtmlNode info)
        {
            var globalFilmTitle = info.QuerySelector("span.gray").InnerText;
            var filmTitleParts = globalFilmTitle.Split(',');
            if (filmTitleParts.Count() == 2)
            {
                return filmTitleParts[0].Trim();
            }
            return GetLocalFilmTitle(info);
        }

    }
}
