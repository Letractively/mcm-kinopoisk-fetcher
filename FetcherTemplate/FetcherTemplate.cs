using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;
using MCM_Common;

#region Assembly versioning information
    // Fill in the following information about your plugin
    // (make sure you keep references to Media Center Master where appropriate)
    [assembly: AssemblyTitle("tMDB plugin, Media Center Master")]
    [assembly: AssemblyDescription("The Open Movie Database meta data fetcher for Media Center Master.")]
    [assembly: AssemblyCompany("Your Name")]
    [assembly: AssemblyCopyright("Copyright © YEAR Your Name")]
    [assembly: AssemblyVersion("1.0.0.0")]
    [assembly: AssemblyFileVersion("1.0.0.0")]
#endregion

namespace FetcherTemplate
{
    public class MCMInterface
    {
        #region Custom data specific to this plugin

        // Try to match the color scheme used at the website source for your data
        private const string Tag = "<color=#87A418><backcolor=#1A1A1A><b>themoviedb.org:</b></backcolor></color>  ";

        // For details on acquiring an API key, please visit http://api.themoviedb.org/2.0/docs/
        private const string API_Key = "THEMOVIEDB.ORG API KEY";

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
            vitals.Author = "Your Name";
            vitals.Name = "The Open Movie Database meta data fetcher for Media Center Master.";
            vitals.Version = "1.00";
            vitals.Source = "themoviedb.org";


            // Respect the user's date formatting by reparsing the date
            vitals.ReleaseDate = DateTime.Parse("2010/02/10").ToShortDateString();


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
        public static List<string> SearchByTitleAndYear(string sTitle, string sYear)
        {
            var lstMovies = new List<string>();

            try
            {
                sTitle = Utils.FixTitleForSearching(sTitle); // fix wrnog symbols
                var pattern = new Regex(@"\s+");
                sTitle = string.Join("+", pattern.Split(sTitle));
                if (string.IsNullOrEmpty(sYear)) sTitle += "+" + sYear;
                string sContents = Utils.PageFetch("http://www.kinopoisk.ru/level/7/type/film/list/1/find/" + System.Uri.EscapeUriString(sTitle));

                if (sContents == "[timeout]")
                {
                    Utils.Logger(Tag + "<color=#B00000><u><b>it appears that themoviedb.org is offline</b></u></color>");
                    return lstMovies;
                }

                if (sContents.StartsWith("[exception: "))
                {
                    Utils.Logger(Tag + "<color=#B00000><u><b>themoviedb.org is online, but experiencing technical difficulties</b></u></color>");
                    return lstMovies;
                }

                var html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(sContents);

                HtmlAgilityPack.HtmlNode document = html.DocumentNode;
                IEnumerable<HtmlAgilityPack.HtmlNode> films = document.QuerySelectorAll("body div.search_results p.name");

                Regex idPattern = new Regex(@"\/film\/(\d+)\/");
                

                foreach (HtmlAgilityPack.HtmlNode filmHeader in films)
                {
                    string confirm_movie_Title = filmHeader.QuerySelector("a").InnerText;
                    string confirm_movie_Year = filmHeader.QuerySelector("span.year").InnerText;

                    string confirm_movie_IDs = null;
                    var matches = idPattern.Match(filmHeader.QuerySelector("a").Attributes["href"].Value);
                    if (matches.Groups.Count == 2)
                    {
                        confirm_movie_IDs = matches.Groups[1].Value;
                    }


                    if (confirm_movie_Year == sYear && !string.IsNullOrEmpty(confirm_movie_IDs))
                    {
                        Utils.Logger(Tag + "Found #" + confirm_movie_IDs + ", \"" + confirm_movie_Title + "\" (" + confirm_movie_Year + ")");
                        lstMovies.Add(Utils.SerializeObject(new MovieResult() { ID = confirm_movie_IDs, Title = confirm_movie_Title, Year = confirm_movie_Year }));
                    }
                }
            }
            catch { }

            if (lstMovies.Count == 0)
            {
                Utils.Logger(Tag + "<color=#B00000><u>no results (by title)</u></color>");
                return lstMovies;
            }

            return lstMovies;
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
        public static string FetchByIDs(string sLocalID, string sExternalID)
        {
            MovieInfo movie = new MovieInfo();

            try
            {
                var filmInfo = new Kinopoisk.FilmPage(sLocalID);

                // This is a private function that will get a list of cast and crew as well as
                // download thumbnails, if the user so chooses (per AppSetting).
                // todo: Implement it later.
                movie.AllCastAndCrew = ProcessCastAndCrew(sLocalID);

                movie.TMDB_ID = sLocalID;
                movie.AllGenres = filmInfo.GetGenreList().ToArray();
                movie.Budget = filmInfo.Budget;
                movie.MPAArating = filmInfo.MPAA;
                movie.Revenue = filmInfo.Revenue;
                movie.Runtime = filmInfo.Runtime;
                movie.Summary = Utils.UnHTML(filmInfo.Summary);
                movie.IMDBscore = filmInfo.IMDBScore;
                movie.Year = Utils.SafeYear(filmInfo.Year);
                movie.Title = filmInfo.Title;
                if (filmInfo.Backdrop != null) movie.Backdrop = Utils.SerializeBitmap( Utils.LoadPictureFromURI(filmInfo.Backdrop) );
                if (filmInfo.Poster != null) movie.Poster = Utils.SerializeBitmap( Utils.LoadPictureFromURI(filmInfo.Poster) );
                
                //movie.IMDB_ID = Utils.ChopperBlank(sMoviePageContents, "<imdb>", "</imdb>");
                //movie.Studios  // not supported by tMDB API 2.0
            }
            catch (Exception ex)
            {
                Utils.Logger(Utils.GetAllErrorDetails(ex));
            }

            return Utils.SerializeObject(movie);
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
        public static string FetchByIDsNoImages(string sLocalID, string sExternalID)
        {
            MovieInfo movie = new MovieInfo();

            if ((Utils.CInt(sLocalID) < 1) && (!string.IsNullOrEmpty(sExternalID)))
            {
                List<MovieResult> lstMovies = new List<MovieResult>();

                try
                {
                    string sContents = Utils.PageFetch("http://api.themoviedb.org/2.0/Movie.imdbLookup?imdb_id=" + sExternalID.Replace("/", "") + "&api_key=" + API_Key);

                    if (sContents == "[timeout]")
                    {
                        Utils.Logger(Tag + "<color=#B00000><u><b>it appears that themoviedb.org is offline</b></u></color>");
                        return Utils.SerializeObject(movie);
                    }

                    if (sContents.StartsWith("[exception: "))
                    {
                        Utils.Logger(Tag + "<color=#B00000><u><b>themoviedb.org is online, but experiencing technical difficulties</b></u></color>");
                        return Utils.SerializeObject(movie);
                    }

                    if (sContents.Contains("Your query didn't return any results"))
                    {
                        Utils.Logger(Tag + "<color=#B00000><u>no results (by title)</u></color>");
                        return Utils.SerializeObject(movie);
                    }

                    string[] sResults = Utils.SubStrings(Utils.Chopper(sContents, "<moviematches>", "</moviematches>"), "<movie>");

                    foreach (string sMovie in sResults)
                    {
                        if (sMovie.Contains("</id>"))
                        {
                            string confirm_movie_Title = Utils.Chopper(sMovie, "<title>", "</title>");
                            string confirm_movie_Year = Utils.Chopper(sMovie, "<release>", "</release>");
                            string confirm_movie_IDs = Utils.ChopperBlank(sMovie, "<id>", "</id>");

                            Utils.Logger(Tag + "Found #" + confirm_movie_IDs + ", \"" + confirm_movie_Title + "\" (" + confirm_movie_Year + ")");
                            lstMovies.Add(new MovieResult() { ID = confirm_movie_IDs, Title = confirm_movie_Title, Year = confirm_movie_Year });
                        }
                    }
                }
                catch { }

                if (lstMovies.Count < 1)
                    return Utils.SerializeObject(movie);

                sLocalID = lstMovies[0].ID;
            }

            try
            {
                string sMoviePageContents = Utils.PageFetch("http://api.themoviedb.org/2.0/Movie.getInfo?id=" + sLocalID + "&api_key=" + API_Key);

                movie.AllCastAndCrew = ProcessCastAndCrew(sLocalID);

                try
                {
                    string sTMDB_Genre_List = "";
                    string s = Utils.Chopper(sMoviePageContents, "<categories>", "</categories>");
                    foreach (string s2 in Utils.SubStrings(s, "<category>"))
                        if (s2.Contains("<name>"))
                            sTMDB_Genre_List += Utils.Chopper(s2, "<name>", "</name>") + "|";
                    sTMDB_Genre_List = sTMDB_Genre_List.Trim();
                    movie.AllGenres = Utils.SubStrings(sTMDB_Genre_List, "|");
                }
                catch { }

                movie.Budget = Utils.ChopperBlank(sMoviePageContents, "<budget>", "</budget>");
                movie.IMDB_ID = Utils.ChopperBlank(sMoviePageContents, "<imdb>", "</imdb>");
                movie.IMDBscore = Utils.ChopperBlank(sMoviePageContents, "<rating>", "</rating>");
                //movie.MPAArating  // not supported by tMDB API 2.0
                movie.Revenue = Utils.ChopperBlank(sMoviePageContents, "<revenue>", "</revenue>");
                movie.Runtime = Utils.ChopperBlank(sMoviePageContents, "<runtime>", "</runtime>");
                //movie.Studios  // not supported by tMDB API 2.0
                movie.Summary = Utils.UnHTML(Utils.ChopperBlank(sMoviePageContents, "<short_overview>", "</short_overview>"));
                movie.Title = Utils.UnHTML(Utils.ChopperBlank(sMoviePageContents, "<title>", "</title>"));
                movie.TMDB_ID = sLocalID;
                movie.Year = Utils.SafeYear(Utils.ChopperBlank(sMoviePageContents, "<release>", "</release>"));
            }
            catch { }

            return Utils.SerializeObject(movie);
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
        public static string[] GetAllPosters(string sLocalID, string sExternalID)
        {
            try
            {
                string sPosters = string.Empty;


                string sMoviePageContents = MCM_Common.Utils.PageFetch("http://api.themoviedb.org/2.0/Movie.getInfo?id=" + sLocalID + "&api_key=" + API_Key);

                foreach (string poster in MCM_Common.Utils.SubStrings(sMoviePageContents, "<poster "))
                    if (poster.StartsWith("size=\"original"))
                        sPosters += "|" + MCM_Common.Utils.ChopperBlank(poster, "size=\"original\">", "</poster>");

                return MCM_Common.Utils.SubStrings(sPosters, "|");
            }
            catch { }

            return new string[] { "[Error with fetcher]" };
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
        public static string[] GetAllBackdrops(string sLocalID, string sExternalID)
        {
            try
            {
                string sPosters = string.Empty;

                string sMoviePageContents = MCM_Common.Utils.PageFetch("http://api.themoviedb.org/2.0/Movie.getInfo?id=" + sLocalID + "&api_key=" + API_Key);

                foreach (string poster in MCM_Common.Utils.SubStrings(sMoviePageContents, "<backdrop "))
                    if (poster.StartsWith("size=\"original"))
                        sPosters += "|" + MCM_Common.Utils.ChopperBlank(poster, "size=\"original\">", "</backdrop>");

                return MCM_Common.Utils.SubStrings(sPosters, "|");
            }
            catch { }

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
        private static string ProcessCastAndCrew(string sID)
        {
            string sCastAndCrewPage = Utils.PageFetch("http://www.themoviedb.org/movie/" + sID + "/cast");

            if (sCastAndCrewPage != string.Empty)
            {
                string sTMDB_People_List = string.Empty;
                foreach (string s in Utils.SubStrings(sCastAndCrewPage, "<div class=\"person\">"))
                {
                    if (s.Contains("/edit_character'") || s.Contains("/remove_character'"))
                    {
                        sTMDB_People_List += Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim() + "*";
                        sTMDB_People_List += "actor*";
                        if (s.Contains("title=\"Character: "))
                            sTMDB_People_List += Utils.ChopperBlank(s, "title=\"Character: ", "\">").Trim() + "|";
                        else
                            sTMDB_People_List += Utils.ChopperBlank(s, "{}; return false;\">", "</a>").Trim() + "|";
                        if (Utils.GetAppSetting("DownloadCastThumbs") != "False")
                            if (Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim().Contains("<") == false)
                                AddOrUpdatePerson(Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim(), Utils.ChopperBlank(s, "<a href=\"/person/", "\""));
                    }
                }
                sTMDB_People_List = sTMDB_People_List.Trim();
                foreach (string s in Utils.SubStrings(sCastAndCrewPage, "<div class=\"person\">"))
                {
                    if ((s.Contains("/edit_character'") == false) && (s.Contains("/remove_character'") == false))
                    {
                        sTMDB_People_List += Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim() + "*";
                        string sJob = Utils.ChopperBlank(s, "title=\"Job: ", "\"").ToLower().Replace("customer", "costume design").Trim();
                        if (sJob == string.Empty)
                            if (s.Contains("catch (e) {}; return false;"))
                                sJob = Utils.ChopperBlank(s, "of this cast\">", "</a>").ToLower().Replace("customer", "costume design").Trim();
                        sTMDB_People_List += sJob + "*|";
                        if (Utils.GetAppSetting("DownloadCrewThumbs") != "False")
                            if (Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim().Length > 0)
                                if (Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim().Contains("<") == false)
                                    AddOrUpdatePerson(Utils.ChopperBlank(s, "title=\"Person: ", "\"").Trim(), Utils.ChopperBlank(s, "<a href=\"/person/", "\""));
                    }
                }

                return sTMDB_People_List.Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// Finds the URL to the thumbnail of a given person, if one exists.
        /// 
        /// Required for:
        ///     none (private/utility)
        /// </summary>
        /// <returns>A URL</returns>
        private static string GetPictureURLForPerson(string sPersonID)
        {
            try
            {
                string sPersonPage = Utils.PageFetch("http://www.themoviedb.org/person/" + sPersonID);
                if (sPersonPage.Contains("<img id=\"left_image\" src=\"http:"))
                    return Utils.ChopperBlank(sPersonPage, "<img id=\"left_image\" src=\"", "\"");
                if (sPersonPage.Contains("<img alt=\"\" id=\"left_image\" src=\"http:"))
                    return Utils.ChopperBlank(sPersonPage, "<img alt=\"\" id=\"left_image\" src=\"", "\"");
                if (sPersonPage.Contains("<img id=\"left_image\" src=\""))
                    return "http://www.themoviedb.org" + Utils.ChopperBlank(sPersonPage, "<img id=\"left_image\" src=\"", "\"");
                if (sPersonPage.Contains("<img alt=\"\" id=\"left_image\" src=\""))
                    return "http://www.themoviedb.org" + Utils.ChopperBlank(sPersonPage, "<img alt=\"\" id=\"left_image\" src=\"", "\"");
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Downloads the actual thumbnail for the requested person if all of the appropriate
        /// settings are configured correctly, creating subfolders when required.
        /// 
        /// Required for:
        ///     none (private/utility)
        /// </summary>
        private static void AddOrUpdatePerson(string sName, string sPersonID)
        {
            string sProgramDataFolder = string.Empty;
            string sPathToUse = string.Empty;

            if (Utils.GetAppSetting("ThumbnailLocation") != "")
                if (System.IO.Directory.Exists(Utils.GetAppSetting("ThumbnailLocation")))
                    sPathToUse = Utils.GetAppSetting("ThumbnailLocation");

            sProgramDataFolder = Utils.ProgramDataFolder.Trim();


            // Check for existence of the %ProgramData% folder
            try
            {
                if (sPathToUse == string.Empty)
                {
                    if (sProgramDataFolder.Length > 0)
                    {
                        if (System.IO.Directory.Exists(sProgramDataFolder))
                        {
                            // Create the %ProgramData%\MediaBrowser folder if it doesn't exist
                            if (System.IO.Directory.Exists(sProgramDataFolder + "\\MediaBrowser") == false)
                                System.IO.Directory.CreateDirectory(sProgramDataFolder + "\\MediaBrowser");

                            // Create the %ProgramData%\MediaBrowser\ImagesByName folder if it doesn't exist
                            if (System.IO.Directory.Exists(sProgramDataFolder + "\\MediaBrowser\\ImagesByName") == false)
                                System.IO.Directory.CreateDirectory(sProgramDataFolder + "\\MediaBrowser\\ImagesByName");

                            // Create the %ProgramData%\MediaBrowser\ImagesByName folder if it doesn't exist
                            if (System.IO.Directory.Exists(sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People") == false)
                                System.IO.Directory.CreateDirectory(sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People");

                            // Create the %ProgramData%\MediaBrowser\ImagesByName\<name> folder if it doesn't exist
                            if (System.IO.Directory.Exists(sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + sName.Trim()) == false)
                                System.IO.Directory.CreateDirectory(sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + sName.Trim());

                            try
                            {
                                if ((System.IO.File.Exists(sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + sName.Trim() + "\\folder.jpg") == false)
                                    && (System.IO.File.Exists(sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + sName.Trim() + "\\folder.png") == false))
                                {
                                    Utils.Logger(Tag + "fetching cast/crew \"" + sName + "\"");
                                    string sImageURL = GetPictureURLForPerson(sPersonID);
                                    if (sImageURL != "")
                                    {
                                        System.Net.WebClient wc = new System.Net.WebClient();
                                        wc.DownloadFile(sImageURL, sProgramDataFolder + "\\MediaBrowser\\ImagesByName\\People\\" + sName.Trim() + "\\folder.jpg");
                                    }
                                }
                            }
                            catch (Exception ex) { Utils.Logger("<color=#ff0000>Exception/#1: " + ex.GetType().ToString() + "\r\n" + ex.Message + "</color>"); }
                        }
                    }
                }
                else
                {
                    try
                    {
                        // Create the [sPathToUse]\<name> folder if it doesn't exist
                        if (System.IO.Directory.Exists(sPathToUse + "\\" + sName.Trim()) == false)
                            System.IO.Directory.CreateDirectory(sPathToUse + "\\" + sName.Trim());

                        if ((System.IO.File.Exists(sPathToUse + "\\" + sName.Trim() + "\\folder.jpg") == false)
                            && (System.IO.File.Exists(sPathToUse + "\\" + sName.Trim() + "\\folder.png") == false))
                        {
                            Utils.Logger(Tag + "fetching cast/crew \"" + sName + "\"");
                            string sImageURL = GetPictureURLForPerson(sPersonID);
                            if (sImageURL != "")
                            {
                                System.Net.WebClient wc = new System.Net.WebClient();
                                wc.DownloadFile(sImageURL, sPathToUse + "\\" + sName.Trim() + "\\folder.jpg");
                            }
                        }
                    }
                    catch (Exception ex) { Utils.Logger("<color=#ff0000>Exception/#2: " + ex.GetType().ToString() + "\r\n" + ex.Message + "</color>"); }
                }
            }
            catch (Exception ex) { Utils.Logger("<color=#ff0000>Exception/#3: " + ex.GetType().ToString() + "\r\n" + ex.Message + "</color>"); }
        }

        #endregion
    }
}
