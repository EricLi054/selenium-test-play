using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.UploadInvoice
{
    public class Confirmation : SparkBasePage
    {

        #region CONSTANTS

        private class Constants
        {
            public const string Header = "Received - thanks, ";           
            public const string Message = "We'll be in touch within 5 business days. However during busy claim periods, we may take a little longer.";
            public const string ClaimNumberText = "Your claim number is ";
        }

        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Header = "//h2[@data-testid='header']";
            public const string Message = "//p[@data-testid='subHeader']";
            public const string ClaimNumber = "//p[@data-testid='claimNumberDisplay']";


            public class Button
            {
                public const string GoToHomePage = "//a[@data-testid='rac-home-page-link-button']";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string Message => GetInnerText(XPath.Message);
        private string ClaimNumberText => GetInnerText(XPath.ClaimNumber);

        #endregion

        public Confirmation(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.ClaimNumber);
                GetElement(XPath.Button.GoToHomePage);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("Upload Invoice Page - Confirmation");
            return true;
        }

        public void VerifyConfirmationPage(ClaimUploadFile claim)
        {
            _browser.PercyScreenCheck(DocumentUpload.Confirmation, GetPercyIgnoreCSS());
            Reporting.Log("Confirmation Page", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.Header}{claim.ClaimantFirstName}", Header, "Header on the Confirmation page");
            Reporting.AreEqual($"{Constants.Message}", Message, "Message on the Confirmation page");
            Reporting.AreEqual($"{Constants.ClaimNumberText}{claim.ClaimNumber}", ClaimNumberText, "Claim number on the Confirmation page");
        }

        private List<string> GetPercyIgnoreCSS() =>
           new List<string>()
           {
               "#header",
               "#claimNumberDisplay span"
           };
    }
}
