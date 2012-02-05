using KinopoiskFetcher.Kinopoisk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for PersonTest and is intended
    ///to contain all PersonTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PersonTest
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
        ///A test for Person Constructor
        ///</summary>
        [TestMethod()]
        public void PersonConstructorTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Id
        ///</summary>
        [TestMethod()]
        public void IdTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role); // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.Id;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LocalName
        ///</summary>
        [TestMethod()]
        public void LocalNameTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.LocalName;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RealName
        ///</summary>
        [TestMethod()]
        public void RealNameTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.RealName;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Role
        ///</summary>
        [TestMethod()]
        public void RoleTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Role;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Thumb
        ///</summary>
        [TestMethod()]
        public void ThumbTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Thumb;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Type
        ///</summary>
        [TestMethod()]
        public void TypeTest()
        {
            string pageAddress = string.Empty; // TODO: Initialize to an appropriate value
            string realName = string.Empty; // TODO: Initialize to an appropriate value
            string localName = string.Empty; // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string role = string.Empty; // TODO: Initialize to an appropriate value
            Person target = new Person(pageAddress, realName, localName, type, role); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Type;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
