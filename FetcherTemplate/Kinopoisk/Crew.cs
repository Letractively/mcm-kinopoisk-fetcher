using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class Crew
    {
        protected readonly uint FilmId = 0;
        
        public Crew(string sFilmId)
        {
            FilmId = uint.Parse(sFilmId);
        }

        protected string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/19/film/{0}/", FilmId);
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

        public List<Person> GetCrew()
        {
            var crew = new List<Person>();
            var elements = Document.QuerySelectorAll("td#block_left>div.block_left").Elements();
            string currentType = "";
            foreach (var element in elements)
            {
                if (element.Name == "table")
                {
                    currentType = element.QuerySelectorAll("td>a").First(item => (item.Attributes["name"] != null)).Attributes["name"].Value;
                } else if (element.Name == "div")
                {
                    var name = element.QuerySelector("div.actorInfo div.name");
                    crew.Add(new Person(name.QuerySelector("a").Attributes["href"].Value,
                                realName:name.QuerySelector("span.gray").InnerText,
                                localName: name.QuerySelector("a").InnerText,
                                type:currentType));
                }
            }
            return crew;
        }
    }
}
