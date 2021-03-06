﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MCM_Common;

namespace KinopoiskFetcher.Kinopoisk
{
    class FilmPage : Abstract
    {
        protected readonly uint FilmId = 0;
        
        public FilmPage(string sFilmId) { FilmId = uint.Parse(sFilmId); }
        public FilmPage(uint filmId) { FilmId = filmId; }

        protected override string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/level/1/film/{0}/", FilmId);
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
                    foreach (HtmlAgilityPack.HtmlNode row in rows)
                    {
                        _properties.Add(row.Elements().First().InnerText, row.Elements().Last());
                    }
                }
                return _properties;
            }

            
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


        public string TagLine
        {
            get
            {
                return Properties.ContainsKey("слоган") ? Prepare(Properties["слоган"].InnerText) : null;
            }
        }

        public string Budget
        {
            get
            {
                return Properties.ContainsKey("бюджет") ? Prepare(Properties["бюджет"].InnerText) : null;
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


        public string FullMPAA
        {
            get
            {
                if (Properties.ContainsKey("рейтинг MPAA"))
                {
                    var item = Properties["рейтинг MPAA"].QuerySelector("span");
                    return Prepare(item.InnerText);
                }
                return null;
            }
        }

        public string Revenue
        {
            get
            {
                if (Properties.ContainsKey("сборы в России"))
                {
                    // for old films where local revenue is not a link
                    return Prepare(Properties["сборы в России"].Elements().First().InnerText);
                }
                var item = Document.QuerySelector("td#div_rus_box_td2 a");
                return (item != null) ? Prepare(item.InnerText) : null;
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
                return item != null ? Prepare(item.InnerHtml) : null;
            }
        }

        public string IMDBScore
        {
            get
            {
                /*var block = Document.QuerySelector("div#block_rating>div[class=\"block_2\"]").Elements().ToArray();
                if (block.Count() >= 2)
                {
                    var reg = new Regex(@"IMDb: ([0-9\.]+)");
                    var match = reg.Match(block[1].InnerText);
                    if (match.Success) return match.Groups[1].Value;
                }
                return null;*/
                return Rating.ImdbRating.Score.ToString();
            }
        }

        public int Year
        {
            get
            {
                return Properties.ContainsKey("год")
                           ? int.Parse(Properties["год"].QuerySelector("a").InnerText)
                           : 0;
            }
        }

        public string Title
        {
            get
            {
                var local = Document.QuerySelector("span[itemprop=\"alternativeHeadline\"]");
                return ExtTrim(local != null ? local.InnerText : LocalTitle);
            }
        }

        public string LocalTitle
        {
            get { return ExtTrim(Document.QuerySelector("h1[itemprop=\"name\"]").InnerText); }
        }

        public string[] GetContries()
        {
            string[] contries = null;
            if (Properties.ContainsKey("страна"))
            {
                var nodes = Properties["страна"].QuerySelectorAll("a");
                contries = nodes.Select(c => c.InnerText).ToArray();
            }
            return contries ?? new string[]{};
        }

        private FilmRating _ratings = null;
        public FilmRating Rating
        {
            get { return _ratings ?? (_ratings = new FilmRating(FilmId)); }
        }

#region Backdrops
        private BackdropsList _backdrops = null;

        protected BackdropsList BackdropsProvider
        {
            get { return _backdrops ?? (_backdrops = new BackdropsList(FilmId.ToString())); }
        }
        public string GetOnlyBackdrop() { return BackdropsProvider.GetBackdropImageLink(); }
        public List<string> GetBackdrops() { return BackdropsProvider.GetAllBackdropsLinks(); }
#endregion
        

#region Posters
        private PostersList _posters = null;

        protected PostersList PostersProvider
        {
            get { return _posters ?? (_posters = new PostersList(FilmId.ToString())); }
        }
        public string GetOnlyPoster() { return PostersProvider.GetPosterImageLink(); }
        public List<string> GetPosters() { return PostersProvider.GetAllPostersImageLinks(); }
#endregion
        


        protected Crew FilmCrew = null;

        public List<Person> GetCrew()
        {
            if (FilmCrew == null) FilmCrew = new Crew(FilmId.ToString());
            return FilmCrew.GetCrew();
        }

    }
}
