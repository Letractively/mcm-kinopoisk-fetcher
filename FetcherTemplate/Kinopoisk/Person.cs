using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MCM_Common;

namespace FetcherTemplate.Kinopoisk
{
    class Person
    {
        protected string PageAddress = null;

        public Person(string pageAddress, string realName = null, string localName = null, string type = null, string role = null)
        {
            PageAddress = pageAddress;
            _type = type;
            _localName = localName;
            _role = role;
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

        public uint Id
        {
            get
            {
                var pattern  = new Regex(@"(\d+)\/$");
                var result = pattern.Match(PageAddress);
                return result.Success ? uint.Parse(result.Groups[1].Value) : 0;
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

        private readonly string _role = null;
        public string Role
        {
            get
            {
                if (_role == null)
                {
                    return "";
                }
                return _role;
            }
        }

        public string Thumb
        {
            get { return string.Format("http://st.kinopoisk.ru/images/actor/{0}.jpg", Id); }
        }

    }
}
