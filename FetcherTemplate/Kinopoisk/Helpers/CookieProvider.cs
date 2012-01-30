using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FetcherTemplate.Kinopoisk.Helpers
{
    class CookieProvider
    {
        static protected List<Cookie> Cookies = new List<Cookie>();

        internal void SetCookie(CookieCollection cc)
        {
            Cookies = new List<Cookie>();
            foreach (Cookie c in cc)
            {
                Cookies.Add(c);
            }
            SaveCookieToFile(Cookies);
        }

        internal List<Cookie> GetCookie(bool load = true)
        {
            if (Cookies.Count == 0 && load)
            {
                Cookies = LoadCookieFromFile();
            }
            return Cookies;
        }

        internal bool HasCookie()
        {
            return GetCookie().Count > 0;
        }

        internal CookieContainer GetCookieContainer()
        {
            CookieContainer cc;
            var loadedCookies = GetCookie();
            if (loadedCookies.Count > 0)
            {
                cc = new CookieContainer(loadedCookies.Count);
                loadedCookies.ForEach(c => { if (c != null) cc.Add(c); });    
            } else
            {
                cc = new CookieContainer();
            }
            return cc;
        }

        protected string TempPath
        {
            get
            {
                var baseDirPath = Environment.GetEnvironmentVariable("TEMP");
                if (string.IsNullOrEmpty(baseDirPath)) baseDirPath = Environment.GetEnvironmentVariable("TMP");
                if (string.IsNullOrEmpty(baseDirPath)) baseDirPath = Path.GetTempPath();
                return baseDirPath;
            }
        }

        protected string CookieTempFileName
        {
            get { return TempPath + System.IO.Path.DirectorySeparatorChar + "mcm-kinoopoisk-cookie.dat"; }
        }

        internal List<Cookie> LoadCookieFromFile()
        {
            var loadedCookies = new List<Cookie>();

            if (File.Exists(CookieTempFileName))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (Stream stream = new FileStream(CookieTempFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        loadedCookies = new List<Cookie>(formatter.Deserialize(stream) as Cookie[]);
                    }
                    catch (Exception)
                    {
                        //suppress exceptions
                    }
                }
            }
            return loadedCookies;
        }

        internal void SaveCookieToFile(List<Cookie> cookies)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            try
            {
                using (var stream = new FileStream(CookieTempFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, cookies.ToArray());
                    stream.Close();
                }
            }
            catch (Exception)
            {
                //just do nothing
            }
        }

    }
}
