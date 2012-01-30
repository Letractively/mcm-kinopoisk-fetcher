using System;

namespace FetcherTemplate.Kinopoisk
{
    class FetchException : Exception
    {
        public FetchException(string message)
            : base(message)
        {}

        public FetchException(string message, Exception innerException)
            : base(message, innerException)
        {}

    }
}
