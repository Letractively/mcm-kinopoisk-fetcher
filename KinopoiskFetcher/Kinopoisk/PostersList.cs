using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace KinopoiskFetcher.Kinopoisk
{
    class PostersList : Abstract
    {
        const string BasePageAddress = "http://st.kinopoisk.ru";

        protected readonly uint FilmId = 0;

        public PostersList(string sFilmId)
        {
            FilmId = uint.Parse(sFilmId);
        }

        protected override string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/17/film/{0}/", FilmId);
            }
        }

        public List<string> GetPostersLinks()
        {
            var items = Document.QuerySelectorAll("table.fotos td a");
            return items.Select(item => "http://www.kinopoisk.ru" + item.Attributes["href"].Value).ToList();
        }

        public string GetPosterImageLink()
        {
            var links = GetPostersLinks();
            if (links.Count > 0)
            {
                // last poster is usually localized
                string sMoviePageContents = PageFetch(links.Last());
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sMoviePageContents);
                return GetRelativeUrl(document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value);

            }
            return null;
        }

        public List<string> GetAllPostersImageLinks()
        {
            var links = GetPostersLinks();
            var posters = new List<string>();
            foreach (var link in links)
            {
                var sMoviePageContents = PageFetch(link);
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sMoviePageContents);
                posters.Add(GetRelativeUrl(document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value));
            }
            return posters;
        }

        protected string GetRelativeUrl(string url)
        {
            if (url[0] == '/') url = BasePageAddress + url;
            return url;
        }

    }
}
