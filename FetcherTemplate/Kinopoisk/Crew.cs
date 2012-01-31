using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class Crew : Abstract
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
                    string sMoviePageContents = PageFetch(PageAddress);
                    _document = new HtmlAgilityPack.HtmlDocument();
                    _document.LoadHtml(sMoviePageContents);
                }
                return _document.DocumentNode;
            }
        }

        public List<Person> GetCrew()
        {
            var crew = new List<Person>();
            var elements = Document.QuerySelector("td#block_left>div.block_left").Elements();
            string currentType = "";
            foreach (var element in elements)
            {
                if (element.Name == "table")
                {
                    var link = element.QuerySelectorAll("td>a").FirstOrDefault(item => (item.Attributes["name"] != null));
                    if (link != null) currentType = link.Attributes["name"].Value;
                }
                else if (element.Name == "div")
                {
                    var name = element.QuerySelector("div.actorInfo div.name");
                    var roleDiv = element.QuerySelector("div.actorInfo div.role");
                    string role = null;


                    if (roleDiv != null)
                    {
                        role = Prepare(roleDiv.InnerText);
                        var rolePattern = new Regex(@"[\. ]*(.+)");
                        var result = rolePattern.Match(role);
                        role = result.Success ? result.Groups[1].Value : null;
                    }

                    crew.Add(new Person(name.QuerySelector("a").Attributes["href"].Value,
                                realName: name.QuerySelector("span.gray").InnerText,
                                localName: name.QuerySelector("a").InnerText,
                                type: currentType,
                                role: role));
                }
            }
            return crew;
        }
    }
}
