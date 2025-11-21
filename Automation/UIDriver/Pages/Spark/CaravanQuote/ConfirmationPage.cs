using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;
using System.Data;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class ConfirmationPage : SparkBasePage
    {
        private class XPath
        {
            public static class Confirmation
            {
                public const string FirstName    = "//*[@id='header']";
                public const string PolicyNumber = "//p[@data-testid='policyNumberLabel']";
            }
            public static class Button
            {
                public const string GoToHomePage = "id('racHomePageLinkButton')";
            }
        }

        #region Settable properties and controls

        public string FirstName
        {
            get
            {
                var urlRegex = new Regex(@"You're all set,\s*([\w\'\-\s]+)\!");
                var rawText = GetElement(XPath.Confirmation.FirstName).Text;
                Match match = urlRegex.Match(rawText);
                Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"ability to parse first name value from UI text: {rawText}");
                return match.Groups[1].Value;
            }
        }

        public string PolicyNumber
        {
            get
            {
                var urlRegex = new Regex(@"(MGV\d+)");
                var rawText = GetElement(XPath.Confirmation.PolicyNumber).Text;
                Match match = urlRegex.Match(rawText);
                Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"ability to parse policy number value from UI text: {rawText}");

                return match.Groups[1].Value;
            }
        }

        #endregion

        public ConfirmationPage(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Confirmation.FirstName);
                GetElement(XPath.Confirmation.PolicyNumber);
                GetElement(XPath.Button.GoToHomePage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Caravan Quote page - Policy Confirmation");

            return true;
        }
        /// <summary>
        /// Attempt to click the "Purchase Policy" button.
        /// </summary>
        /// <exception cref="ReadOnlyException">Thrown if button is present but disabled.</exception>
        public void ClickGoToRACHomepage()
        {
            ClickControl(XPath.Button.GoToHomePage);
        }

        /// <summary>
        /// Ignore CSS from visual testing
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "#header",  // Header with member's name
              "[data-testid='policyNumberLabel']", // Policy number
              "[data-icon='thumbs-up'] path"       // Animated thumbs up
          };
    }
}
