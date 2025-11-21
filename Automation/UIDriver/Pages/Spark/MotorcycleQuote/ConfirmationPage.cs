using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;
using System.Data;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class ConfirmationPage  : SparkBasePage
    {

        #region XPATHS
        private class XPath
        {
            public static class Confirmation
            {
                public const string FirstName = "//*[@id='header']";
                public const string PolicyNumber = "//div[@data-testid='policyNumberLabel']/p";
            }
            public static class Button
            {
                public const string GoToHomePage = "id('racHomePageLinkButton')";
            }
        }

        #endregion

        #region Settable properties and controls

        public string FirstName
        {
            get
            {
                var urlRegex = new Regex(@"You are ready to ride,\s*([\w\'\-]+(\s+[\w\'\-]+)*)\!");
                var rawText = GetElement(XPath.Confirmation.FirstName).Text;
                Match match = urlRegex.Match(rawText);

                Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"Unable to parse first name value from UI text: {rawText}");
                
                return match.Groups[1].Value;
            }
        }


        public string PolicyNumber
        {
            get
            {
                var urlRegex = new Regex(@"(MGC\d+)");
                var rawText = GetElement(XPath.Confirmation.PolicyNumber).Text;
                Match match = urlRegex.Match(rawText);

                Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"Unable to parse policy number value from UI text: {rawText}");
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

            Reporting.LogPageChange("Motorcycle Quote page - Policy Confirmation");
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
              "#header",
              "[data-testid='policyNumberLabel']",
              "[data-icon='thumbs-up'] path"
          };
    }
}
