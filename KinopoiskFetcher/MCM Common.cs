namespace MCM_Common
{
    public enum PluginTypes : int
    {
        MovieMetaFetcher = 1,
        TVMetaFetcher = 2,
        AdultMetaFetcher = 3,
        AnimeMetaFetcher = 4,
        TorrentDownloader = 5,
        TrailerDownloader = 6,
        Other = 0
    }

    public enum PluginSourceType : int
    {
        API = 1,
        Scrape = 2,
        Other = 0
    }

    [System.Serializable()]
    public class PluginDetails
    {
        public string Author = "(unknown)";
        public string Version = "(unknown)";
        public string Name = "(unknown)";
        public string ReleaseDate = "(unknown)";
        public PluginTypes PluginType = PluginTypes.Other;
        public double PluginSystemVersion = 1.0;
        public string LanguageCode = "en/English";
        public PluginSourceType SourceType = PluginSourceType.Other;
        public string Source = "(unknown)";
    }

    public enum OperatingModes : int
    {
        Movies = 1,
        TV = 2,
        Adult = 3,
        Mixed = 4,
        Unknown = 0
    }

    [System.Serializable()]
    public class MovieInfo
    {
        public string IMDB_ID = "";
        public string TMDB_ID = "";
        public string CDUniverse_ID = "";

        public string Title
        {
            get
            {
                return this.Original_Title;
            }
            set
            {
                this.Original_Title = value;
                this.Local_Title = value;
            }
        }

        public string Local_Title = "";
        public string Original_Title = "";
        public string Year = "";
        public string Runtime = "";
        public string IMDBscore = "";
        public string MPAArating = "";
        public string[] AllGenres = new string[] { };
        public string Budget = "";
        public string Revenue = "";
        public string Summary = "";
        public string AllCastAndCrew = "";
        public string Studios = "";

        public byte[] Poster = null;
        public byte[] Backdrop = null;
        public string[] Backdrops = new string[] { };
    }

    [System.Serializable()]
    public class MovieResult: object
    {
        public string ID = "";
        public string Title = "";
        public string Year = "";

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var mr = (MovieResult) obj;
            return mr.ID == ID && mr.Year == Year && mr.Title.ToLower() == Title.ToLower();
        }

        public override int GetHashCode()
        {
            var result = 0;
            if (ID != null) result ^= ID.GetHashCode();
            if (Title != null) result ^= Title.GetHashCode();
            if (Year != null) result ^= Year.GetHashCode();
            return result;
        }

    }
}