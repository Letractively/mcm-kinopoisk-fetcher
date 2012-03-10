using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace KinopoiskFetcher.Kinopoisk
{
    class FilmSeach:Abstract
    {

        protected readonly string FilmTitle;
        protected readonly string FilmYear;

        public FilmSeach(string title, string year = null)
        {
            FilmTitle = title;
            FilmYear = year;
        }

        protected override string PageAddress
        {
            get
            {
                var title = Utils.FixTitleForSearching(FilmTitle); // fix wrnog symbols
                var pattern = new Regex(@"\s+");
                title = string.Join("+", pattern.Split(title));
                if (!string.IsNullOrEmpty(FilmYear)) title += "+" + FilmYear;
                return "http://www.kinopoisk.ru/index.php?first=no&what=&kp_query=" + Uri.EscapeUriString(title);
                //return "http://www.kinopoisk.ru/level/7/type/film/list/1/find/" + Uri.EscapeUriString(title);
            }
        }

        public List<MovieResult> Find(bool showOnlyRelatedFilms = true)
        {
            var lstMovies = new List<MovieResult>();

            IEnumerable<HtmlAgilityPack.HtmlNode> films = showOnlyRelatedFilms
                                                        ? Document.QuerySelectorAll("body div.search_results div.most_wanted div.info")
                                                        : Document.QuerySelectorAll("body div.search_results div.info");

            foreach (HtmlAgilityPack.HtmlNode filmHeader in films)
            {
                MovieResult film = GetMovieResult(filmHeader);
                if (film != null)
                {
                    if (string.IsNullOrEmpty(FilmYear)) film.Year = FilmYear;
                    else if (FilmYear != film.Year) continue;
                    lstMovies.Add(film);
                }
            }
            return lstMovies;
        }

        protected MovieResult GetMovieResult(HtmlAgilityPack.HtmlNode filmNode)
        {
            var idPattern = new Regex(@"\/film\/(\d+)\/");
            var titlePattern = new Regex("[А-Яа-я]+");

            string confirmMovieTitle = null,
                   confirmMovieYear = null,
                   confirmMovieIDs = null,
                   filmId;
            
            try
            {
                // define whether film title is local
                confirmMovieTitle = titlePattern.Match(FilmTitle).Success ? GetLocalFilmTitle(filmNode) : GetGlobalFilmTitle(filmNode);
                confirmMovieYear = filmNode.QuerySelector("p.name span.year").InnerText;
                filmId = filmNode.QuerySelector("a").Attributes["href"].Value;
            }
            catch (Exception)
            {
                filmId = confirmMovieTitle = confirmMovieYear = null;
            }
            if (!string.IsNullOrEmpty(filmId) )
            {
                var matches = idPattern.Match(filmId);
                if (matches.Groups.Count == 2)
                {
                    confirmMovieIDs = matches.Groups[1].Value;
                }
            }
            if (confirmMovieIDs != null && confirmMovieTitle != null && confirmMovieYear != null)
                return new MovieResult() {ID = confirmMovieIDs, Title = confirmMovieTitle, Year = confirmMovieYear};
            return null;
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
