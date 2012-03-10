using System.Collections.Generic;
using System.Text.RegularExpressions;
using Fizzler;

namespace KinopoiskFetcher.Kinopoisk
{
    class Person:Abstract
    {
        private readonly string _pageAddress;
        protected override string PageAddress
        {
            get { return _pageAddress; }
        }

        protected readonly List<string> ShowImageFor = new List<string>(new string[] { "director", "actor" });

        public Person(string pageAddress, string realName = null, string localName = null, string type = null, string role = null)
        {
            _pageAddress = pageAddress;
            _type = type;
            _localName = Prepare(localName);
            _role = role;
            realName = Prepare(realName);
            _realName = string.IsNullOrEmpty(realName) ? _localName : realName;
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
            get
            {
                // we don't need to load images for whole crew.
                return ShowImageFor.Contains(Type) ? string.Format("http://st.kinopoisk.ru/images/actor/{0}.jpg", Id) : null;
            }
        }

    }
}
