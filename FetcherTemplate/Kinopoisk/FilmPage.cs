using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class FilmPage
    {
        protected readonly uint FilmId = 0;
        
        public FilmPage(string sFilmId)
        {
            FilmId = uint.Parse(sFilmId);
        }

        protected string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/1/film/{0}/", FilmId);
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

        private Dictionary<string, HtmlAgilityPack.HtmlNode> _properties = null;
        protected Dictionary<string, HtmlAgilityPack.HtmlNode> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<string, HtmlNode>();
                    var rows = Document.QuerySelectorAll("body div.movie table.info tr");
                    string value = null;
                    foreach (HtmlAgilityPack.HtmlNode row in rows)
                    {
                        _properties.Add(row.Elements().First().InnerText, row.Elements().Last());
                    }
                }
                return _properties;
            }

            
        }

        public string ExtTrim(string value)
        {
            return string.IsNullOrEmpty(value) ?
                    "" :
                    Regex.Replace(Regex.Replace(value, @"[ \t\n\r\0\x0B]*\z", ""), @"\A[ \t\n\r\0\x0B]*", "");
        }

        public List<string> GetGenreList()
        {
            var genres = new List<string>();
            var items = Document.QuerySelectorAll("body table.info td[itemprop=genre] a");
            var pattern = new Regex("[А-Яа-яёa-zA-Z]+");
            foreach (HtmlAgilityPack.HtmlNode genre in items)
            {
                if (pattern.Match(genre.InnerText).Success) genres.Add(genre.InnerText);
            }
            return genres;
        }

        public string Budget
        {
            get
            {
                if (Properties.ContainsKey("бюджет"))
                {
                    return ExtTrim(Properties["бюджет"].InnerText);
                }
                return null;
            }
        }

        public string MPAA
        {
            get
            {
                if (Properties.ContainsKey("рейтинг MPAA"))
                {
                    var item = Properties["рейтинг MPAA"].Elements().First();
                    return item.Attributes["href"].Value.Split(new char[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries).Last();
                }
                return null;
            }
        }

        public string Revenue
        {
            get
            {
                var item = Document.QuerySelector("td#div_rus_box_td2 a");
                return (item != null) ? item.InnerText : null;
            }
        }

        public string Runtime
        {
            get
            {
                string runtime = Document.QuerySelector("td#runtime").InnerText;
                return runtime;
            }
        }

        public string Summary
        {
            get
            {
                var item = Document.QuerySelector("div[itemprop=\"description\"]");
                return item != null ? item.InnerHtml : null;
            }
        }

        public string IMDBScore
        {
            get
            {
                var block = Document.QuerySelector("div#block_rating>div[class=\"block_2\"]").Elements().ToArray();
                if (block.Count() >= 2)
                {
                    var reg = new Regex(@"IMDb: ([0-9\.]+)");
                    var match = reg.Match(block[1].InnerText);
                    if (match.Success) return match.Groups[1].Value;
                }
                return null;
            }
        }

        public ushort Year
        {
            get
            {
                return Properties.ContainsKey("год")
                           ? ushort.Parse(Properties["год"].QuerySelector("a").InnerText)
                           : (ushort) 0;
            }
        }

        public string Title
        {
            get { return ExtTrim(Document.QuerySelector("h1[itemprop=\"name\"]").InnerText); }
        }


        private BackdropsList _backdrops = null;
        public string Backdrop
        {
            get
            {
                if (_backdrops == null) _backdrops = new BackdropsList(FilmId.ToString());
                return _backdrops.GetBackdropImageLink();
            }
        }

        private PostersList _posters = null;
        public string Poster
        {
            get
            {
                if (_posters == null) _posters = new PostersList(FilmId.ToString());
                return _posters.GetPosterImageLink();
            }
        }

        public List<Person> Crew
        {
            get
            {
                var crew = new List<Person>();
                var requireTypes = new string[] { "режиссер", "сценарий", "продюсер", "оператор", "композитор", "художник", "монтаж" };
                foreach (string type in requireTypes)
                {
                    if (Properties.ContainsKey(type))
                    {
                        var persons = Properties[type].QuerySelectorAll("a");
                        foreach (var person in persons)
                        {
                            if (person.InnerText != "...")
                                crew.Add(new Person(person.Attributes["href"].Value, person.InnerText, type));
                        }
                    }
                }
            }
        }

    }
}
