using KinopoiskFetcher.Kinopoisk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            const uint filmId = 526;
            var target = new FilmPage(filmId);
            Assert.AreEqual(target.LocalTitle, "Большой куш");
            Assert.AreEqual(target.Title, "Snatch.");
            Assert.AreEqual(target.Year, 2000);
            Assert.AreEqual(target.MPAA, "R");
            Assert.AreEqual(target.Runtime, "102 мин. / 01:42");
            Assert.IsTrue(target.Summary.Length > 10);
        }
    }
}
