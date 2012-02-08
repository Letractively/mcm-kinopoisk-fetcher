using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

#region Assembly versioning information
    // Fill in the following information about your plugin
    // (make sure you keep references to Media Center Master where appropriate)
    [assembly: AssemblyTitle("Kinopoisk plugin, Media Center Master")]
    [assembly: AssemblyDescription("The Kinopoisk Database meta data fetcher for Media Center Master.")]
    [assembly: AssemblyCompany("Andrey Grebennikov")]
    [assembly: AssemblyCopyright("Copyright © 2012 Andrey Grebennikov")]
    [assembly: AssemblyVersion("1.0.0.0")]
    [assembly: AssemblyFileVersion("1.0.0.0")]
#endregion

namespace KinopoiskFetcher
{
    public class MCMInterface
    {
        #region Custom data specific to this plugin

        // Try to match the color scheme used at the website source for your data
        private const string Tag = "<color=#ff6600><backcolor=#eeeeee><b> kinopoisk.ru: </b></backcolor></color> ";

        private const ushort MaximumNumberOfCrewToLoadImages = 20;

        #endregion

        #region Public interface for Vitals()

        /// <summary>
        /// Returns extended plugin information that Media Center Master will read to negotiate
        /// how to properly use your plugin.  It's important that this information is always
        /// kept current and is formatted precisely.
        /// 
        /// Required for:
        ///     all plugin types
        /// </summary>
        /// <returns>A serialized MCM_Common.PluginDetails object</returns>
        public static string Vitals()
        {
            PluginDetails vitals = new PluginDetails();

            
            // Fill in the following information about your plugin
            vitals.Author = "Andre Grebennikov";
            vitals.Name = "The Kinopoisk Database meta data fetcher for Media Center Master.";
            vitals.Version = "1.00";
            vitals.Source = "kinopoisk.ru";
            vitals.LanguageCode = "ru/Russian";


            // Respect the user's date formatting by reparsing the date
            vitals.ReleaseDate = DateTime.Parse("2012/01/28").ToShortDateString();


            // Make sure you register your plugin as one of the available types so that future
            // versions of Media Center Master can display the plugin on the correct screen and so
            // that it knows how to interface with your plugin.
            vitals.PluginType = PluginTypes.MovieMetaFetcher;


            // Enter the plugin system version you support.  If unknown, leave it as the default.
            vitals.PluginSystemVersion = 2.0;


            // Set this to either Scrape or API depending on if your method uses HTML scrapes or an
            // actual API system (such as XML).
            vitals.SourceType = PluginSourceType.API;


            // Data needs to be serialized (converted to XML)
            return Utils.SerializeObject(vitals);
        }

        #endregion
        #region Public interface for SearchByTitleAndYear()

        /// <summary>
        /// Searches for media by title.  Year is provided but is not typically used because Media
        /// Center Master will automatically negotiate matches by year.  Some fetchers will need to
        /// submit this data during lookup, though, so it is provided for that purpose.
        /// 
        /// A full result list is sent back to Media Center Master and it is Media Center Master's
        /// role to evaluate the list and make a decision on what to do with the data.
        /// 
        /// Required for:
        ///     meta data fetches (all)
        /// </summary>
        /// <returns>A list of serialized MCM_Common.MovieResult objects</returns>
        public static List<string> SearchByTitleAndYear(string title, string year)
        {
            Utils.Logger(Tag + string.Format("<b>SearchByTitleAndYear</b> called with title = \"{0}\", year={1}", title, year));

            var search = new Kinopoisk.FilmSeach(title, year);
            var movies = new List<string>();
            try
            {
                var found = search.Find();
                if (found.Count == 0)
                {
                    Utils.Logger(Tag + "<b>SearchByTitleAndYear</b> no direct matches found. Returning all results.");
                    found = search.Find(false);
                }
                if (found.Count > 0)
                    found.ForEach(f => Utils.Logger(Tag + "Found #" + f.ID + ", \"" + f.Title + "\" (" + f.Year + ")"));
                movies = found.Select(Utils.SerializeObject).ToList();


            }
            catch(Kinopoisk.FetchException ex)
            {
                Utils.Logger(String.Format(Tag + "<color=#B00000><u><b>{0}</b></u></color>", ex.Message));
                if (ex.InnerException != null)
                {
                    Utils.Logger("MCM_Common.Utils.PageFetch() exception:\r\n" + ex.Message);
                    //Utils.Logger(Utils.GetAllErrorDetails(ex.InnerException));
                }
                    
                
            }
            catch (Exception ex)
            {
                Utils.Logger(Utils.GetAllErrorDetails(ex));
            }
            if (movies.Count == 0)
            {
                Utils.Logger(Tag + "<color=#B00000><u>no results (by title)</u></color>");
            }
            return movies;
        }

        #endregion
        #region Public interface for FetchByIDs() and FetchByIDsNoImages()

        /// <summary>
        /// Retrieves all data, posters, etc. for the media per its ID. The local ID will be the
        /// one returned in the list of IDs from SearchByTitleAndYear(). When available, the external
        /// ID will be an IMDB ID (will be string.Empty when unavailable).
        /// 
        /// For plugin system 1.0, it is acceptable for this function to download thumbnails for
        /// cast/crew.  A future version may move this to a new function.
        /// 
        /// Required for:
        ///     meta data fetches (all)
        /// </summary>
        /// <returns>A serialized MCM_Common.MovieInfo object (cannot be null)</returns>
        public static string FetchByIDs(string localId, string externalId)
        {
            Utils.Logger(Tag + string.Format("<b>FetchByIDs</b> called with localId = \"{0}\", externalId=\"{1}\"", localId, externalId));

            if (!string.IsNullOrEmpty(localId))
            {
                try
                {
                    return Utils.SerializeObject(FetchFilm(uint.Parse(localId), FetchOptions.FetchImages));
                }
                catch (Exception ex)
                {
                    Utils.Logger(Utils.GetAllErrorDetails(ex));
                }   
            }
            return null;
        }

        /// <summary>
        /// Retrieves all data for the media per its ID, excluding posters and backdrops.  The local
        /// ID will be the one returned in the list of IDs from SearchByTitleAndYear().  When available,
        /// the external ID will be an IMDB ID (will be string.Empty when unavailable).
        /// 
        /// For plugin system 1.0, it is acceptable for this function to download thumbnails for
        /// cast/crew.  A future version may move this to a new function.
        /// 
        /// Required for:
        ///     meta data fetches (all)
        /// </summary>
        /// <returns>A serialized MCM_Common.MovieInfo object (cannot be null)</returns>
        public static string FetchByIDsNoImages(string localId, string externalId)
        {
            Utils.Logger(Tag + string.Format("<b>FetchByIDsNoImages</b> called with localId = \"{0}\", externalId=\"{1}\"", localId, externalId));

            if (!string.IsNullOrEmpty(localId))
            {
                try
                {
                    return Utils.SerializeObject(FetchFilm(uint.Parse(localId)));
                }
                catch (Exception ex)
                {
                    Utils.Logger(Utils.GetAllErrorDetails(ex));
                }
            }
            return null;
        }

        [Flags]
        public enum FetchOptions
        {
            None = 0,
            FetchImages = 1
        }

        protected static MovieInfo FetchFilm(uint filmId, FetchOptions options = FetchOptions.None)
        {
            var movie = new MovieInfo();
            var filmInfo = new Kinopoisk.FilmPage(filmId);

            movie.AllGenres = filmInfo.GetGenreList().ToArray();
            movie.Budget = filmInfo.Budget;
            movie.MPAArating = filmInfo.MPAA;
            movie.Revenue = filmInfo.Revenue;
            movie.Runtime = filmInfo.Runtime;
            movie.Summary = Utils.UnHTML(filmInfo.Summary);
            movie.IMDBscore = filmInfo.IMDBScore;
            movie.Year = Utils.SafeYear(filmInfo.Year);
            movie.Local_Title = filmInfo.LocalTitle;
            movie.Original_Title = filmInfo.Title;

            // Added with plugin system version 2.1
            var countries = filmInfo.GetContries();
            if (countries.Count() > 0)
                movie.Country = string.Join(", ", countries);
            
            //public string Language = string.Empty;
            //public string ParentalRatingSummary = string.Empty;
            

            // Added with plugin system version 2.2
            movie.TagLine = filmInfo.TagLine;
            movie.FullMPAA = filmInfo.FullMPAA;
            movie.PosterURL = filmInfo.GetOnlyPoster();
            movie.BackdropURL = filmInfo.GetOnlyBackdrop();

            movie.Director = string.Join(", ", filmInfo.GetCrew().Where(p => p.Type == "director").Select(p=>p.LocalName).ToArray());
            movie.Writers = filmInfo.GetCrew().Where(p => p.Type == "writer").Select(p => p.LocalName).ToArray();
            movie.NumberOfVotes = filmInfo.Rating.ImdbRating.Votes.ToString();

            //public string FullCertifications = string.Empty;
            //public string Outline = string.Empty;
            //public string Plot = string.Empty;
            //public string Top250 = string.Empty;
            //public string Awards = string.Empty;
            //public string Website = string.Empty;
            //public string Trailer = string.Empty;


            if ((options & FetchOptions.FetchImages)==FetchOptions.FetchImages)
            {
                var link = filmInfo.GetOnlyBackdrop();
                if (link != null) movie.Backdrop = Utils.SerializeBitmap(Utils.LoadPictureFromURI(link));
                link = filmInfo.GetOnlyPoster();
                if (link != null) movie.Poster = Utils.SerializeBitmap(Utils.LoadPictureFromURI(link));
            }
                
            movie.AllCastAndCrew = ProcessCastAndCrew(filmInfo);

            //movie.IMDB_ID = Utils.ChopperBlank(sMoviePageContents, "<imdb>", "</imdb>");
            //movie.Studios  // not supported by tMDB API 2.0

            return movie;
        }

        #endregion
        #region Public interface for GetAllPosters() and GetAllBackdrops()

        /// <summary>
        /// Retrieves a list of all available posters in a string array.
        /// 
        /// This is an optional interface and can be removed if it is not applicable to your fetcher.
        /// 
        /// Optional for:
        ///     meta data fetches (all)
        /// </summary>
        /// <returns>An array of string's (can be empty, but not null)</returns>
        public static string[] GetAllPosters(string localId, string externalId)
        {
            Utils.Logger(Tag + string.Format("<b>GetAllPosters</b> called with localId = \"{0}\", externalId=\"{1}\"", localId, externalId));
            try
            {
                var filmInfo = new Kinopoisk.FilmPage(localId);
                List<string> posters = filmInfo.GetPosters();
                Utils.Logger(Tag + string.Format("<b>GetAllPosters</b> returned \"{0}\" results.", localId, posters.Count));
                return posters.ToArray();
            }
            catch (Exception ex)
            {
                Utils.Logger(Utils.GetAllErrorDetails(ex));
            }

            return new string[]{};
        }

        /// <summary>
        /// Retrieves a list of all available backdrops in a string array.
        /// 
        /// This is an optional interface and can be removed if it is not applicable to your fetcher.
        /// 
        /// Optional for:
        ///     meta data fetches (all)
        /// </summary>
        /// <returns>An array of string's (can be empty, but not null)</returns>
        public static string[] GetAllBackdrops(string localId, string externalId)
        {
            Utils.Logger(Tag + string.Format("<b>GetAllBackdrops</b> called with localId = \"{0}\", externalId=\"{1}\"", localId, externalId));
            try
            {
                var filmInfo = new Kinopoisk.FilmPage(localId);
                List<string> backdrops = filmInfo.GetBackdrops();

                Utils.Logger(Tag + string.Format("<b>GetAllBackdrops</b> returned \"{0}\" results.", localId, backdrops.Count));
                return backdrops.ToArray();
            }
            catch (Exception ex)
            {
                Utils.Logger(Utils.GetAllErrorDetails(ex));
            }

            return new string[] { "[Error with fetcher]" };
        }

        #endregion
        #region Public interface for GetSetting()
        
        /// <summary>
        /// Used to pass custom settings to and from fetchers for special circumstances.
        /// 
        /// Plugin system version 2.0 supports only the following setting:
        ///     WantExternalIMDB   Return 'True' (case-insensitive) if your fetcher requires 
        ///                        the IMDB ID to be passed as the external ID during a call to
        ///                        FetchByIDs() or FetchByIDsNoImages().
        /// 
        /// This is an optional interface and can be removed if it is not applicable to your fetcher.
        /// 
        /// Optional for:
        ///     meta data fetches (all)
        /// </summary>
        /// <param name="sSettingName">The in-bound setting name.</param>
        /// <returns>Your response to the setting request (string.Empty or null is prefered if
        /// your fetcher does not want to answer a setting request).</returns>
        public static string GetSetting(string sSettingName)
        {
            if (sSettingName.Trim().ToLower() == ("WantExternalIMDB").ToLower())
                return "True";

            return string.Empty;
        }

        #endregion

        #region Private methods specific to this plugin


        /// <summary>
        /// Creates a delimited list of cast and crew.  This version also downloads thumbnails
        /// if the AppSetting is turned on.
        /// 
        /// The cast/crew list should be delimited as such:
        ///     real name*type*role|real name*type*role|real name*type*role
        /// 
        /// The role may be blank for crew.
        /// 
        /// NOTE: since themoviedb.org does not provide an API for cast/crew thumbnails, this
        ///       part of the fetcher is actually a scraper.  In general, this fetcher uses API,
        ///       though so its overall qualification is API.
        /// 
        /// Required for:
        ///     none (private/utility)
        /// </summary>
        /// <returns>A specially-formatted list of cast and crew</returns>
        private static string ProcessCastAndCrew(Kinopoisk.FilmPage film)
        {
            var crew = film.GetCrew();
            
            if (Utils.GetAppSetting("DownloadCastThumbs") != "False")
                crew.Take(MaximumNumberOfCrewToLoadImages).ToList().ForEach(AddOrUpdatePerson);
            
            return string.Join("|",
                               (from p in crew
                                select String.Join("*", new string[] { p.LocalName, p.Type, p.Role })).
                                   ToArray());

        }
        
        /// <summary>
        /// Downloads the actual thumbnail for the requested person if all of the appropriate
        /// settings are configured correctly, creating subfolders when required.
        /// 
        /// Required for:
        ///     none (private/utility)
        /// </summary>
        private static void AddOrUpdatePerson(Kinopoisk.Person person)
        {
            var programDataFolder = Utils.ProgramDataFolder.Trim();
            var pathToUse = string.Empty;
            var name = person.RealName;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(person.Thumb)) return;

            if (Utils.GetAppSetting("ThumbnailLocation") != "")
                if (System.IO.Directory.Exists(Utils.GetAppSetting("ThumbnailLocation")))
                    pathToUse = Utils.GetAppSetting("ThumbnailLocation");


            // Check for existence of the %ProgramData% folder
            try
            {
                if (pathToUse == string.Empty)
                {
                    if (programDataFolder.Length > 0)
                    {
                        if (System.IO.Directory.Exists(programDataFolder))
                        {
                            // Create the %ProgramData%\MediaBrowser folder if it doesn't exist
                            if (System.IO.Directory.Exists(programDataFolder + "\\MediaBrowser") == false)
                                System.IO.Directory.CreateDirectory(programDataFolder + "\\MediaBrowser");

                            // Create the %ProgramData%\MediaBrowser\ImagesByName folder if it doesn't exist
                            if (System.IO.Directory.Exists(programDataFolder + "\\MediaBrowser\\ImagesByName") == false)
                                System.IO.Directory.CreateDirectory(programDataFolder + "\\MediaBrowser\\ImagesByName");

                            // Create the %ProgramData%\MediaBrowser\ImagesByName folder if it doesn't exist
                            if (System.IO.Directory.Exists(programDataFolder + "\\MediaBrowser\\ImagesByName\\People") == false)
                                System.IO.Directory.CreateDirectory(programDataFolder + "\\MediaBrowser\\ImagesByName\\People");

                            // Create the %ProgramData%\MediaBrowser\ImagesByName\<name> folder if it doesn't exist
                            if (System.IO.Directory.Exists(programDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + name) == false)
                                System.IO.Directory.CreateDirectory(programDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + name);

                            try
                            {
                                if ((System.IO.File.Exists(programDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + name + "\\folder.jpg") == false)
                                    && (System.IO.File.Exists(programDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + name + "\\folder.png") == false))
                                {
                                    Utils.Logger(Tag + "fetching cast/crew \"" + name + "\"");
                                    string imageUrl = person.Thumb;
                                    if (imageUrl != "")
                                    {
                                        var wc = new System.Net.WebClient();
                                        wc.DownloadFile(imageUrl, programDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + name.Trim() + "\\folder.jpg");
                                    }
                                }
                            }
                            catch (Exception ex) { Utils.Logger("<color=#ff0000>Exception/#1: " + ex.GetType() + "\r\n" + ex.Message + "</color>"); }
                        }
                    }
                }
                else
                {
                    try
                    {
                        // Create the [sPathToUse]\<name> folder if it doesn't exist
                        if (System.IO.Directory.Exists(pathToUse + "\\" + name.Trim()) == false)
                            System.IO.Directory.CreateDirectory(pathToUse + "\\" + name.Trim());

                        if ((System.IO.File.Exists(pathToUse + "\\" + name.Trim() + "\\folder.jpg") == false)
                            && (System.IO.File.Exists(pathToUse + "\\" + name.Trim() + "\\folder.png") == false))
                        {
                            Utils.Logger(Tag + "fetching cast/crew \"" + name + "\"");
                            string imageUrl = person.Thumb;
                            if (imageUrl != "")
                            {
                                System.Net.WebClient wc = new System.Net.WebClient();
                                wc.DownloadFile(imageUrl, pathToUse + "\\" + name.Trim() + "\\folder.jpg");
                            }
                        }
                    }
                    catch (Exception ex) { Utils.Logger("<color=#ff0000>Exception/#2: " + ex.GetType() + "\r\n" + ex.Message + "</color>"); }
                }
            }
            catch (Exception ex) { Utils.Logger("<color=#ff0000>Exception/#3: " + ex.GetType() + "\r\n" + ex.Message + "</color>"); }
        }

        #endregion
    }
}
