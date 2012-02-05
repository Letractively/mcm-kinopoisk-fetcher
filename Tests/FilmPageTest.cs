using KinopoiskFetcher.Kinopoisk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for FilmPageTest and is intended
    ///to contain all FilmPageTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmPageTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for FilmPage Constructor
        ///</summary>
        [TestMethod()]
        public void FilmPageConstructorTest()
        {
            uint filmId = 0; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(filmId);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for FilmPage Constructor
        ///</summary>
        [TestMethod()]
        public void FilmPageConstructorTest1()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetBackdrops
        ///</summary>
        [TestMethod()]
        public void GetBackdropsTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetBackdrops();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCrew
        ///</summary>
        [TestMethod()]
        public void GetCrewTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            List<Person> expected = null; // TODO: Initialize to an appropriate value
            List<Person> actual;
            actual = target.GetCrew();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetGenreList
        ///</summary>
        [TestMethod()]
        public void GetGenreListTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetGenreList();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetOnlyBackdrop
        ///</summary>
        [TestMethod()]
        public void GetOnlyBackdropTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetOnlyBackdrop();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetOnlyPoster
        ///</summary>
        [TestMethod()]
        public void GetOnlyPosterTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetOnlyPoster();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPosters
        ///</summary>
        [TestMethod()]
        public void GetPostersTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetPosters();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Load();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Budget
        ///</summary>
        [TestMethod()]
        public void BudgetTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Budget;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IMDBScore
        ///</summary>
        [TestMethod()]
        public void IMDBScoreTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.IMDBScore;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LocalTitle
        ///</summary>
        [TestMethod()]
        public void LocalTitleTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.LocalTitle;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MPAA
        ///</summary>
        [TestMethod()]
        public void MPAATest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.MPAA;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Revenue
        ///</summary>
        [TestMethod()]
        public void RevenueTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Revenue;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Runtime
        ///</summary>
        [TestMethod()]
        public void RuntimeTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Runtime;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Summary
        ///</summary>
        [TestMethod()]
        public void SummaryTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Summary;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Title
        ///</summary>
        [TestMethod()]
        public void TitleTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Title;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Year
        ///</summary>
        [TestMethod()]
        public void YearTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            FilmPage target = new FilmPage(sFilmId); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Year;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
