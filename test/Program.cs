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
            //var listOfFilms = FetcherTemplate.MCMInterface.SearchByTitleAndYear("Список контактов", "2008");
            var filmInfo = FetcherTemplate.MCMInterface.FetchByIDs("258275", "0");
        }
    }
}
