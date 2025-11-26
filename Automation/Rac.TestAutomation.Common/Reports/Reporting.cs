using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rac.TestAutomation.Common
{
    public class Reporting
    {
        private static string _reportFolder = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "") + "Reports\\";
        #region logging format constants
        public static readonly string NEW_SECTION_BAR = "==========================================================================<br>";
        public static readonly string SEPARATOR_BAR   = "--------------------------------------------------------------------------<br>";

        public static readonly string HTML_NEWLINE    = "<br>"; // Keep line formatting in Extent Reports html logs.
        #endregion

        public static string ReportFolder => _reportFolder;

        public static void BeginTest(string testName)
        {
            ExtentTestManager.CreateTest(testName);
        }

        /// <summary>
        /// Intended to be used in test teardown to mark when a test has finished
        /// so that ExtentReport can log final state, as well as process any
        /// uncaught exception that might have caused test termination.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="screenshot"></param>
        public static void EndTest(TestContext.ResultAdapter result, string screenshot = null)
        {
            var status = result.Outcome.Status;
            var stacktrace = result.StackTrace != null ? result.StackTrace.ToString() : "";
            var errorMessage = result.Message;

            try
            {
                switch (status)
                {
                    case TestStatus.Failed:
                        FailLog($"Test ended with failure - {errorMessage} - {stacktrace}", screenshot, true);
                        break;
                    case TestStatus.Skipped:
                        SkipLog($"Test skipped.");
                        break;
                    default:
                        PassLog($"Test passed.");
                        break;
                }
            }
            finally
            {

                if (File.Exists(GetConsoleLogFileName()))
                    TestContext.AddTestAttachment(GetConsoleLogFileName(), "Log file");
            }
        }

        #region formatted logging operations to support a more readable test log
        public static void LogTestData(string testName, string formattedTestDataString)
        {
            var formattedString = CreateSectionHeader($"Created test data for {testName}");
            formattedString.Append(formattedTestDataString);
            
            Log(formattedString.ToString());
        }

        public static void LogTestStart()
        {
            var formattedString = CreateSectionHeader("Begin test actions");
            Log(formattedString.ToString());
        }

        public static void LogPageChange(string pageName)
        {
            LogMinorSectionHeading($"Loaded page: {pageName}");
        }

        public static void LogMinorSectionHeading(string sectionTitle)
        {
            StringBuilder formattedString = new StringBuilder();
            CreateSubSectionHeader(formattedString, sectionTitle);
            Log(formattedString.ToString());
        }

        public static void LogTestMemberCentralValidations(string context, string mcDataName)
        {
            var formattedString = CreateSectionHeader($"Verifying {context} in Member Central: {mcDataName}");
            Log(formattedString.ToString());
        }

        public static void LogTestShieldValidations(string context, string shieldDataName)
        {
            var formattedString = CreateSectionHeader($"Verifying {context} in Shield: {shieldDataName}");
            Log(formattedString.ToString());
        }

        public static void LogFeatureToggle(List<KeyValueBool> toggles)
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(SEPARATOR_BAR);
            formattedString.AppendLine($"   Shield Environment:              {Config.Get().Shield.Environment}{HTML_NEWLINE}");
            formattedString.AppendLine($"--- Feature Toggle:{HTML_NEWLINE}");           
            if (toggles != null && toggles.Count > 0)
            {
                foreach (var toggle in toggles)
                {
                    formattedString.AppendLine($"    {toggle.Key}:              {toggle.Value}{HTML_NEWLINE}");
                }
            }
            else
            {
                formattedString.AppendLine($"    No feature toggles are set for this Test{HTML_NEWLINE}");
            }

            formattedString.AppendLine(SEPARATOR_BAR);
            Log(formattedString.ToString());
        }

        private static StringBuilder CreateSectionHeader(string headerMessage)
        {
            var stringBuilder = new StringBuilder(string.Empty);
            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine(NEW_SECTION_BAR);

            var barLength = NEW_SECTION_BAR.Length;
            if (headerMessage.Length < barLength)
            {
                headerMessage = string.Format($"{{0,-{barLength}}}", string.Format("{0," + ((barLength + headerMessage.Length) / 2).ToString() + "}", headerMessage));
            }
            stringBuilder.AppendLine($"{headerMessage}{HTML_NEWLINE}");
            stringBuilder.AppendLine(NEW_SECTION_BAR);
            return stringBuilder;
        }

        private static void CreateSubSectionHeader(StringBuilder stringBuilder, string headerMessage)
        {
            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine(SEPARATOR_BAR);

            var barLength = SEPARATOR_BAR.Length;
            if (headerMessage.Length < barLength)
            {
                headerMessage = string.Format($"{{0,-{barLength}}}", string.Format("{0," + ((barLength + headerMessage.Length) / 2).ToString() + "}", headerMessage));
            }
            stringBuilder.AppendLine($"{headerMessage}{HTML_NEWLINE}");
            stringBuilder.AppendLine(SEPARATOR_BAR);
        }
        #endregion

        /// <summary>
        /// General purpose `Log` method to have comments and informational notes
        /// put into the test report.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="screenshot"></param>
        public static void Log(string message, string screenshot = null)
        {
            if (screenshot == null)
            {
                ExtentTestManager.GetTest().Info(message);
                ConsoleLog($"INFO: {message}");
            }
            else
            {
                var screenshotFileName = screenshot.Split('\\').Last();
                ExtentTestManager.GetTest().Info(message, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotFileName).Build());
                ConsoleLog($"INFO: {message}. Screenshot: {screenshotFileName}");
                TestContext.AddTestAttachment(screenshot);
            }
        }

        /// <summary>
        /// Supports logging from within async tasks, as they can't fetch
        /// the Extent Report manager context. So will only log to 
        /// file/console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogAsyncTask(string message)
        {
            ConsoleLog($"[Async Task]: {message}");
        }

        /// <summary>
        /// For logging errors, and also will trigger an Assert.Fail() to ensure
        /// test is marked as failure. To be used when test logic captures a
        /// bad behaviour of the target system and doesn't want to proceed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="screenshot"></param>
        public static void Error(string message, string screenshot = null)
        {
            if (screenshot == null)
            {
                ExtentTestManager.GetTest().Error(message);
                ConsoleLog($"ERROR: {message}");
            }
            else
            {
                // We'd rather use a relative path, so remove absolute folder if it is
                // in the same reporting folder.
                var screenshotFileName = screenshot.Split('\\').Last();
                ExtentTestManager.GetTest().Error(message, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotFileName).Build());
                ConsoleLog($"ERROR: {message}. Screenshot: {screenshotFileName}");
                TestContext.AddTestAttachment(screenshot);
            }

            Assert.Fail(message);
        }

        /// <summary>
        /// Applicable for where NUnit runner is wrapping up a fixture
        /// and determined that a test was skipped.
        /// 
        /// NOTE: Causes test to exit with "inconclusive" state.
        /// </summary>
        public static void SkipLog(string message)
        {
            ExtentTestManager.GetTest().Skip(message);
            ConsoleLog($"SKIP: {message}");
            Assert.Inconclusive(message);
        }

        /// <summary>
        /// Compares two strings by passing 'Expected', "Actual' values.
        /// </summary>
        /// <param name="expected">Assert the expected value.</param>
        /// <param name="actual">Reference the value observed in the system.</param>
        /// <param name="ignoreCase">Optional boolean flag if this comparison need not be case-sensitive.</param>
        /// <param name="comparisonContext">This is a text message for meaningful logging, as it is not always obvious is being compared.</param>
        public static void AreEqual(string expected, string actual, bool ignoreCase = false, string comparisonContext = "")
        {
            bool equal = ignoreCase ?
                string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase) :
                string.Equals(expected, actual);
            if (equal)
            {
                PassLog($"Checking {comparisonContext} String '{expected}' and String '{actual}' are equal.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected string: '{expected}' and actual string: '{actual}' were expected to be equal.");
            }
        }

        /// <summary>
        /// Compares non-primitive object types. Pass 'Expected', "Actual' values.
        /// </summary>
        /// <param name="expected">Assert the expected value.</param>
        /// <param name="actual">Reference the value observed in the system.</param>
        /// <param name="comparisonContext">This is a text message for meaningful logging, as objects may not convert to useful strings.</param>
        public static void AreEqual(object expected, object actual, string comparisonContext)
        {
            if (expected.GetType() != actual.GetType())
            { 
                Reporting.Error($"Cannot compare objects of different types: {expected.GetType()} vs {actual.GetType()}");
            }
            if (object.Equals(expected, actual))
            {
                PassLog($"Checking {comparisonContext}. Both values were '{expected}'.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected value: '{expected}' and actual value: '{actual}' were expected to be equal.");
            }
        }

        public static void AreEqual(bool expected, bool actual, string comparisonContext)
        {
            if (expected == actual)
            {
                PassLog($"Checking {comparisonContext}. Both values were '{expected}'.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected boolean: '{expected}' and actual boolean: '{actual}' were expected to be equal.");
            }
        }

        public static void AreEqual(decimal expected, decimal actual, string comparisonContext)
        {
            if (Decimal.Equals(expected, actual))
            {
                PassLog($"Checking {comparisonContext}. Both values were '{expected}'.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected decimal: '{expected}' and actual decimal: '{actual}' were expected to be equal.");
            }
        }

        public static void AreEqual(int expected, int actual, string comparisonContext)
        {
            if (expected == actual)
            {
                PassLog($"Checking {comparisonContext}. Both values were '{expected}'.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected integer: '{expected}' and actual integer: '{actual}' were expected to be equal.");
            }
        }

        public static void AreEqual(double expected, double actual, string comparisonContext)
        {
            if (expected == actual)
            {
                PassLog($"Checking {comparisonContext}. Both values were '{expected}'.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected double: '{expected}' and actual double: '{actual}' were expected to be equal.");
            }
        }

        /// <summary>
        /// Primarily for comparing depreciating sum insured values, where
        /// B2C rounds differently depending on the screen.
        /// See Portfolio Summary and Endorsement sliders.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="comparisonContext"></param>
        public static void AreEqualWithPlusMinus1Tolerance(int expected, int actual, string comparisonContext)
        {
            if (expected == actual)
            {
                PassLog($"Checking {comparisonContext}. Integers {expected} and {actual} are equal");
            }
            else if ((expected + 1) == actual)
            {
                PassLog($"Checking {comparisonContext}. Integers {expected}+1 and {actual} are equal");
            }
            else if (expected == (actual + 1))
            {
                PassLog($"Checking {comparisonContext}. Integers {expected} and {actual}+1 are equal");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected integer: {expected} and actual integer: {actual} were expected to be equal");
            }
        }

        /// <summary>
        /// Compares two strings by passing 'Expected', "Actual' values, ensuring they are not equal, even if one is null.
        /// </summary>
        /// <param name="comparisonValue">Assert the value to compare against the actual value, expecting they will not be the same.</param>
        /// <param name="actual">Reference the value observed in the system.</param>
        /// <param name="ignoreCase">Optional boolean flag if this comparison need not be case-sensitive.</param>
        /// <param name="comparisonContext">This is a text message for meaningful logging, as it is not always obvious is being compared.</param>
        public static void AreNotEqual(string comparisonValue, string actual, bool ignoreCase = false, string comparisonContext = "")
        {
            bool equal = ignoreCase ?
                string.Equals(comparisonValue, actual, StringComparison.InvariantCultureIgnoreCase) :
                string.Equals(comparisonValue, actual);
            if (!equal)
            {
                PassLog($"Checking {comparisonContext} String '{comparisonValue}' and String '{actual}' are not equal.");
            }
            else
            {
                FailLog($"Checking {comparisonContext}. Expected string: '{comparisonValue}' and actual string: '{actual}' were expected to be not equal.");
            }
        }

        public static void IsTrue(bool a, string message)
        {
            if (a)
            {
                PassLog($"Checking {message}. Boolean {a} is TRUE (as expected)");
            }
            else
            {
                FailLog($"Checking {message}. Boolean was not found to be TRUE.");
            }
        }

        public static void IsFalse(bool a, string message)
        {
            if (!a)
            {
                PassLog($"Checking {message}. Boolean {a} is FALSE (as expected)");
            }
            else
            {
                FailLog($"Checking {message}. Boolean was not found to be FALSE.");
            }
        }

        public static void IsNull<T>(T a, string message)
        {
            if (a == null)
            {
                PassLog($"Checking {message}. Object is NULL (which is as expected)");
            }
            else
            {
                FailLog($"Checking {message}. Object was expected to be NULL, but it was not.");
            }
        }

        public static void IsNotNull<T>(T a, string message)
        {
            if (a != null)
            {
                PassLog($"Checking {message}. Object was not NULL (which is as expected)");
            }
            else
            {
                FailLog($"Checking {message}. Object was expected to have a value, but it did not.");
            }
        }

        public static void IsNullOrEmptyString<T>(T a, string message)
        {
            if (a == null)
            {
                PassLog($"Checking {message}. Object is NULL (which is as expected)");
            }
            else if (a.ToString() == "")
            {
                PassLog($"Checking {message}. Object is an empty string (which is as expected)");
            }
            else
            {
                FailLog($"Checking {message}. Object was expected to be NULL or an empty string, but it was not.");
            }
        }

        private static void FailLog(string message, string screenshot = null, bool testEnding = false)
        {
            if (screenshot == null)
            {
                ExtentTestManager.GetTest().Fail(message);
                ConsoleLog($"FAIL: {message}");
            }
            else
            {
                var screenshotFileName = screenshot.Split('\\').Last();
                ExtentTestManager.GetTest().Fail(message, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotFileName).Build());
                ConsoleLog($"FAIL: {message}. Screenshot: {screenshotFileName}");
                TestContext.AddTestAttachment(screenshot);
            }

            // We don't want to call Assert.Fail() if we're already wrapping up the test.
            if (!testEnding)
            {
                Assert.Fail(message);
            }
        }

        private static void PassLog(string message)
        {
            ExtentTestManager.GetTest().Pass(message);
            ConsoleLog($"PASS: {message}");
        }

        private static void ConsoleLog(string message)
        {
            var _log = ExtentTestManager.GetLogStream(TestContext.CurrentContext.Test.Name);
            _log.Info(message);
        }

        private static string GetConsoleLogFileName() => $"{Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory)}{TestContext.CurrentContext.Test.Name}.log";
    }
}