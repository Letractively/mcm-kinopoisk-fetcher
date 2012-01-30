﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class PostersList : Abstract
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
                    string sMoviePageContents = PageFetch(PageAddress);
                    _document = new HtmlAgilityPack.HtmlDocument();
                    _document.LoadHtml(sMoviePageContents);
                }
                return _document.DocumentNode;
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
                return document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value;
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
                posters.Add(document.DocumentNode.QuerySelector("img#image").Attributes["src"].Value);
            }
            return posters;
        }

    }
}
