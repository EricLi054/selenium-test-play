using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rac.TestAutomation.Common
{
    /// <summary>
    /// Do not inherit from this class directly.
    /// Use either UITestBase or NonUITestBase.
    /// </summary>
    abstract public class BaseTest
    {
        protected TestContext testContextInstance;
        protected Config     _testConfig;

        /// <summary>
        /// Provides access to the NUnit test context, for tracking current
        /// NUnit state.
        /// </summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        /// <summary>
        /// Initializes base controls that we want all tests to have.
        /// Child classes can define their own "OneTimeSetup" of a
        /// different method name, and they all get called.
        /// </summary>
        [OneTimeSetUp]
        protected void BaseClassInitState()
        {
            _testConfig = Config.Get();
        }

        /// <summary>
        /// Provides clean up that all test cases should perform.
        /// </summary>
        [OneTimeTearDown]
        protected void BaseClassFinalTearDown()
        {
            ExtentManager.Instance.Flush();
        }
    }

    /// <summary>
    /// Base class to be used for all NUnit tests that operate with a
    /// browser instance.
    /// </summary>
    abstract public class BaseUITest : BaseTest
    {
        protected Browser _browser;

        /// <summary>
        /// Performs the setup (pre each test) that is desired for
        /// each UI orientated test.
        /// Tests can define their own "Setup", and both will be run.
        /// </summary>
        [SetUp]
        public void BaseClassSetup()
        {
            Reporting.BeginTest(TestContext.CurrentContext.Test.Name);
            _browser = new Browser();
        }

        /// <summary>
        /// Performs the final clean up, including capturing a final screenshot
        /// if the test failed, and ensuring that it is captured in the HTML
        /// report, including untrapped exceptions. Also ensures that the
        /// browser instance is torn down between each test.
        /// Tests can define their own "TearDown", and both will be run.
        /// </summary>
        [TearDown]
        public void BaseClassTearDown()
        {
            string screenshot = null;
            if (_browser != null && _browser.Driver != null)
            {
                screenshot = TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Failed) ?
                             _browser.Driver.TakeSnapshot() : null;
            }
            Reporting.EndTest(TestContext.CurrentContext.Result, screenshot);
            _browser.CloseBrowser();
        }

    }

    /// <summary>
    /// Base class to be used with all NON-UI orientated tests, such as
    /// those verifying APIs.
    /// </summary>
    abstract public class BaseNonUITest : BaseTest
    {
        /// <summary>
        /// Basic setup for each non-UI test.
        /// Tests can define their own "Setup", and both will
        /// be run.
        /// </summary>
        [SetUp]
        public void BaseClassSetup()
        {
            Reporting.BeginTest(TestContext.CurrentContext.Test.Name);
        }

        /// <summary>
        /// Final summary to capture and log final result for current
        /// test, including recording any untrapped exceptions.
        /// Tests can define their own "TearDown", and both will be
        /// run.
        /// </summary>
        [TearDown]
        public void BaseClassTearDown()
        {
            Reporting.EndTest(TestContext.CurrentContext.Result, null);
        }

    }
}
