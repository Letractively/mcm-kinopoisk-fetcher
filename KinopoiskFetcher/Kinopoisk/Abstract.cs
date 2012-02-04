using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using MCM_Common;

namespace KinopoiskFetcher.Kinopoisk
{
    class Abstract
    {
        public const string SiteName = "kinopoisk.ru";

        protected Helpers.CookieProvider Cookies = new Helpers.CookieProvider();

        public string PageFetch(string url)
        {
            string content;
            try
            {
                var oReq = WebRequest.Create(url);
                var ohReq = (HttpWebRequest)oReq;

                ohReq.Accept = "*/*";
                ohReq.Referer = url;
                ohReq.CookieContainer = Cookies.GetCookieContainer();
                oReq.Headers.Add("Accept-Language: ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3");
                oReq.Headers.Add("Accept-Encoding: plain");
                oReq.Headers.Add("Accept-Charset: windows-1251,utf-8;q=0.7,*;q=0.7");
                ohReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET CLR 1.1.4322)";
                ohReq.Connection = "false";
                oReq.Headers.Add("Pragma: no-cache");
                oReq.Method = "GET";
                //oReq.Timeout = 100000;
                
                //ohReq.Headers.Add("Host: " + GetHostName(url));
                ohReq.MaximumAutomaticRedirections = 3;
                try
                {
                    var oResp = (HttpWebResponse)oReq.GetResponse();
                    Cookies.SetCookie(oResp.Cookies);
                    var oSRead = new StreamReader(oResp.GetResponseStream(), System.Text.Encoding.GetEncoding(oResp.CharacterSet));
                    content = oSRead.ReadToEnd();
                }
                catch (WebException)
                {
                    throw new FetchException("Network access error");
                }

            }
            catch (TimeoutException)
            {
                throw new FetchException(String.Format("it appears that {0} is offline", SiteName));
            }
            catch (Exception ex)
            {
                throw new FetchException(string.Format("{0} is online, but experiencing technical difficulties at {1}", SiteName, url), ex);
            }

            return content;
        }

        protected string GetHostName(string url)
        {
            url = url.ToLower().Replace("http://", "");
            var endPos = url.IndexOf('/');
            if (endPos >= 0) url = url.Substring(0, endPos);
            return url;
        }

        /// <summary>
        /// \u00A0 - no break space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ExtTrim(string value)
        {
            return string.IsNullOrEmpty(value) ?
                    "" :
                    Regex.Replace(Regex.Replace(value, @"[ \t\n\r\0\x0B\u00A0]+\z", ""), @"\A[ \t\n\r\0\x0B\u00A0]+", "");
        }

        protected string Prepare(string value)
        {
            return string.IsNullOrEmpty(value) ? value : ExtTrim(Utils.UnHTML(value));
        }
    }
}
