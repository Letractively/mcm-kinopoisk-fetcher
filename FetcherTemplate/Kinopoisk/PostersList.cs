using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class PostersList
    {
        protected readonly uint FilmId = 0;

        public PostersList(string sFilmId)
        {
            FilmId = uint.Parse(sFilmId);
        }

        protected string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/17/film/{0}/", FilmId);
            }
        }

        private HtmlAgilityPack.HtmlDocument _document = null;
        protected HtmlAgilityPack.HtmlNode Document
        {
            get
            {
                if (_document == null)
                {
                    string sMoviePageContents = Utils.PageFetch(PageAddress);
                    _document = new HtmlAgilityPack.HtmlDocument();
                    _document.LoadHtml(sMoviePageContents);
                }
                return _document.DocumentNode;
            }
        }

        public List<string> GetPostersLinks()
        {
            var items = Document.QuerySelectorAll("table.fotos td a");
            return items.Select(item => "htp://http://www.kinopoisk.ru" + item.Attributes["href"]).ToList();
        }

        public string GetPosterImageLink()
        {
            var links = GetPostersLinks();
            if (links.Count > 0)
            {
                // last poster is usually localized
                string sMoviePageContents = Utils.PageFetch(links.Last());
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sMoviePageContents);
                return document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value;
            }
            return null;
        }

    }
}
