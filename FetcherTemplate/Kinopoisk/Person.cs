using System.Text.RegularExpressions;

namespace FetcherTemplate.Kinopoisk
{
    class Person:Abstract
    {
        protected string PageAddress = null;

        public Person(string pageAddress, string realName = null, string localName = null, string type = null, string role = null)
        {
            PageAddress = pageAddress;
            _type = type;
            _localName = Prepare(localName);
            _role = role;
            _realName = Prepare(realName);
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
                return _realName ?? "";
            }
        }

        private readonly string _type = null;
        public string Type
        {
            get
            {
                return _type ?? "";
            }
        }

        private readonly string _localName = null;
        public string LocalName
        {
            get
            {
                return _localName ?? "";
            }
        }

        private readonly string _role = null;
        public string Role
        {
            get
            {
                return _role ?? "";
            }
        }

        public string Thumb
        {
            get { return string.Format("http://st.kinopoisk.ru/images/actor/{0}.jpg", Id); }
        }

    }
}
