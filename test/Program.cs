using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var listOfFilms = FetcherTemplate.MCMInterface.SearchByTitleAndYear("Deception", "2008");
            //var filmInfo = FetcherTemplate.MCMInterface.FetchByIDs("258275", "0");
            //var filmInfo = FetcherTemplate.MCMInterface.FetchByIDsNoImages("258275", "0");
            var info = FetcherTemplate.MCMInterface.FetchByIDsNoImages("258275", "");
        }
    }
}
