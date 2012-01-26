using System.Collections.Generic;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class BackdropsList
    {
        protected readonly uint FilmId = 0;

        public BackdropsList(string sFilmId)
        {
            FilmId = uint.Parse(sFilmId);
        }

        protected string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/12/film/{0}/", FilmId);
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

        public List<string> GetBackdropsLinks()
        {
            var items = Document.QuerySelectorAll("table.fotos td u>a:first-child");
            return items.Select(item => "htp://http://www.kinopoisk.ru" + item.Attributes["href"]).ToList();
        }

        public string GetBackdropImageLink()
        {
            var links = GetBackdropsLinks();
            if (links.Count > 0)
            {
                string sMoviePageContents = Utils.PageFetch(links.First());
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sMoviePageContents);
                return document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value;
            }
            return null;
        }
    }
}
