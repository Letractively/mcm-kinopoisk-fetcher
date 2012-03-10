using System.Collections.Generic;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace KinopoiskFetcher.Kinopoisk
{
    class BackdropsList : ImagesListAbstract
    {
        protected readonly uint FilmId = 0;

        public BackdropsList(string sFilmId)
        {
            FilmId = uint.Parse(sFilmId);
        }

        protected override string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/12/film/{0}/", FilmId);
            }
        }

        public List<string> GetBackdropsLinks()
        {
            var items = Document.QuerySelectorAll("table.fotos td u>a:first-child");
            return items.Select(item => "http://www.kinopoisk.ru" + item.Attributes["href"].Value).ToList();
        }

        public string GetBackdropImageLink()
        {
            var links = GetBackdropsLinks();
            if (links.Count > 0)
            {
                string sMoviePageContents = PageFetch(links.First());
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sMoviePageContents);
                return GetRelativeUrl(document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value);
            }
            return null;
        }

        public List<string> GetAllBackdropsLinks()
        {
            var links = GetBackdropsLinks();
            var backdrops = new List<string>();
            foreach (var link in links)
            {
                string sMoviePageContents = PageFetch(link);
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sMoviePageContents);
                backdrops.Add(GetRelativeUrl(document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value));
            }
            return backdrops;
        }
    }
}
