using KinopoiskFetcher.Kinopoisk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for BackdropsListTest and is intended
    ///to contain all BackdropsListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BackdropsListTest
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
        ///A test for GetAllBackdropsLinks
        ///</summary>
        [TestMethod()]
        public void GetAllBackdropsLinksTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            BackdropsList target = new BackdropsList(sFilmId); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetAllBackdropsLinks();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBackdropImageLink
        ///</summary>
        [TestMethod()]
        public void GetBackdropImageLinkTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            BackdropsList target = new BackdropsList(sFilmId); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetBackdropImageLink();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBackdropsLinks
        ///</summary>
        [TestMethod()]
        public void GetBackdropsLinksTest()
        {
            string sFilmId = string.Empty; // TODO: Initialize to an appropriate value
            BackdropsList target = new BackdropsList(sFilmId); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetBackdropsLinks();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
