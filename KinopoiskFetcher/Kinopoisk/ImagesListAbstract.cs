using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinopoiskFetcher.Kinopoisk
{
    abstract class ImagesListAbstract : Abstract
    {
        const string BasePageAddress = "http://st.kinopoisk.ru";

        protected string GetRelativeUrl(string url)
        {
            if (url[0] == '/')
            {
                url = BasePageAddress + url;
            }

            return url;
        }
    }
}
