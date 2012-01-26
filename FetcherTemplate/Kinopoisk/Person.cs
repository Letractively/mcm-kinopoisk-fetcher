using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class Person
    {
        protected string PageAddress = null;

        public Person(string pageAddress, string realName = null, string localName = null, string type = null)
        {
            PageAddress = pageAddress;
            _type = type;
            _localName = localName;
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

        private readonly string _realName = null;
        public string RealName
        {
            get
            {
                if (_realName == null)
                {
                    return "";
                }
                return _realName;
            }
        }

        private readonly string _type = null;
        public string Type
        {
            get
            {
                if (_type == null)
                {
                    return "";
                }
                return _type;
            }
        }

        private readonly string _localName = null;
        public string LocalName
        {
            get
            {
                if (_localName == null)
                {
                    return "";
                }
                return _localName;
            }
        }
    }
}
