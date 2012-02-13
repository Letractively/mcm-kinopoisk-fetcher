using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace KinopoiskFetcher.Kinopoisk
{
    class FilmRating:Abstract
    {
        protected readonly uint FilmId = 0;
        
        public FilmRating(string sFilmId) { FilmId = uint.Parse(sFilmId); }
        public FilmRating(uint filmId) { FilmId = filmId; }

        protected string PageAddress
        {
            get
            {
                return string.Format("http://www.kinopoisk.ru/rating/{0}.xml", FilmId);
            }
        }

        private XDocument _document = null;
        protected XDocument Document
        {
            get { return _document ?? (_document = XDocument.Parse(PageFetch(PageAddress))); }
        }

        public Rating KpRating
        {
            get
            {
                var xNode = Document.Descendants("kp_rating").Single();
                if (xNode != null)
                {
                    const NumberStyles style = NumberStyles.AllowDecimalPoint;
                    var culture = CultureInfo.CreateSpecificCulture("en-US");
                    var r = new Rating {Score = double.Parse(xNode.Value, style, culture)};
                    var xAttribute = xNode.Attribute("num_vote");
                    if (xAttribute != null) r.Votes = uint.Parse(xAttribute.Value);
                    return r;
                }


                return null;
            }
        }

        public Rating ImdbRating
        {
            get
            {
                var xNode = Document.Descendants("imdb_rating").Single();
                if (xNode != null)
                {
                    const NumberStyles style = NumberStyles.AllowDecimalPoint;
                    var culture = CultureInfo.CreateSpecificCulture("en-US");
                    var r = new Rating {Score = double.Parse(xNode.Value, style, culture)};
                    var xAttribute = xNode.Attribute("num_vote");
                    if (xAttribute != null) r.Votes = uint.Parse(xAttribute.Value);
                    return r;
                }


                return null;
            }
        }

    }
}
