using KinopoiskFetcher.Kinopoisk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MCM_Common;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for FilmSeachTest and is intended
    ///to contain all FilmSeachTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmSeachTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///Test seach to find international film using international film title.
        ///</summary>
        [TestMethod()]
        public void FindTestRelatedInternationalFilm()
        {
            const string title = "Under Siege"; // why not :)
            const string year = "1992";
            const bool showOnlyRelatedFilms = true;

            var target = new FilmSeach(title, year);
            var expected = new List<MovieResult> {new MovieResult() {ID = "4053", Title = "Under Siege", Year = "1992"}};
            var actual = target.Find(showOnlyRelatedFilms);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test seach to find international film using local film title.
        ///</summary>
        [TestMethod()]
        public void FindTestRelatedInternationalFilmByRussianTitle()
        {
            const string title = "Вердикт за деньги";
            const string year = "2003";
            const bool showOnlyRelatedFilms = true;

            var target = new FilmSeach(title, year);
            var expected = new List<MovieResult> { new MovieResult() { ID = "3621", Title = "Вердикт за деньги", Year = "2003" } };
            var actual = target.Find(showOnlyRelatedFilms);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Find
        ///</summary>
        [TestMethod()]
        public void FindTestRelatedRussianFilm()
        {
            const string title = "Берегись автомобиля";
            const string year = "1966";
            var target = new FilmSeach(title, year);
            const bool showOnlyRelatedFilms = true;
            var expected = new List<MovieResult> { new MovieResult() { ID = "46089", Title = "Берегись автомобиля", Year = "1966" } };
            var actual = target.Find(showOnlyRelatedFilms);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Find
        ///</summary>
        [TestMethod()]
        public void FindTestRelatedRussianFilmByGlobalTitle()
        {
            const string title = "Piter FM";
            const string year = "2006";
            var target = new FilmSeach(title, year);
            const bool showOnlyRelatedFilms = true;
            var expected = new List<MovieResult> { new MovieResult() { ID = "252063", Title = "Питер FM", Year = "2006" } };
            var actual = target.Find(showOnlyRelatedFilms);
            CollectionAssert.AreEqual(expected, actual);
        }
        
        /// <summary>
        ///A test for Find
        ///</summary>
        [TestMethod()]
        public void FindTestAllRussianFilm()
        {
            const string title = "Питер FM";
            const string year = "2006";
            var target = new FilmSeach(title, year);
            const bool showOnlyRelatedFilms = false;
            var expected = new MovieResult() { ID = "252063", Title = "Питер FM", Year = "2006" };
            var actual = target.Find(showOnlyRelatedFilms);
            Assert.IsTrue(actual.Contains(expected));
        }
    }
}
