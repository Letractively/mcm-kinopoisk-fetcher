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

        // Added with plugin system version 2.1
        public string Country = string.Empty;
        public string Language = string.Empty;
        public string ParentalRatingSummary = string.Empty;

        // Added with plugin system version 2.2
        public string PosterURL = string.Empty;
        public string BackdropURL = string.Empty;
        public string NumberOfVotes = string.Empty;
        public string FullMPAA = string.Empty;
        public string FullCertifications = string.Empty;
        public string TagLine = string.Empty;
        public string Outline = string.Empty;
        public string Plot = string.Empty;
        public string Top250 = string.Empty;
        public string Director = string.Empty;
        public string[] Writers = new string[] { };
        public string Awards = string.Empty;
        public string Website = string.Empty;
        public string Trailer = string.Empty;
    }

    [System.Serializable()]
    public class MovieResult
    {
        public string ID = "";
        public string Title = "";
        public string Year = "";
    }
}