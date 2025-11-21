using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;

namespace UIDriver.Pages.Spark.EFT
{
    public class Confirmation : SparkBasePage
    {
        #region XPATHS
        private static class XPath
        {
            public static class General
            {
                public const string ConfirmationMessage = "//div[@id='stepper-content']//span[contains(@class,'MuiStepLabel-label')]"; //TODO: SPK-4097 - When ID values have been added, update XPath
                public const string ClaimNumber = "//div[@data-testid='claimNumberLabel']/p"; //TODO: SPK-4097 - When ID values have been added, update XPath
            }
            public static class Button
            {
                public const string HomePage = "id('racHomePageLinkButton')";
            }
        }
        #endregion XPATHS
        
        #region Settable properties and controls

        public string ConfirmationMessage => GetInnerText(XPath.General.ConfirmationMessage);

        public string ClaimNumber
        {
            get
            {
                var urlRegex = new Regex(@"(\d+)");
                var rawText = GetElement(XPath.General.ClaimNumber).Text;
                Match match = urlRegex.Match(rawText);
                Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"ability to parse claim number value from UI text: {rawText}");

                return match.Groups[1].Value;
            }
        }

        #endregion Settable properties and controls

        public Confirmation(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.ConfirmationMessage);
                GetElement(XPath.General.ClaimNumber);
                GetElement(XPath.Button.HomePage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempt to click the "Go To RAC homepage" button.
        /// </summary>
        /// <exception cref="ReadOnlyException">Thrown if button is present but disabled.</exception>
        public void ClickGoToRACHomepage()
        {
            ClickControl(XPath.Button.HomePage);
        }
    }
}