using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Data;
using System.Threading;
using UIDriver.Helpers;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkQuoteImportantInformation : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            
            public class ImportantInformation
            {
                public const string Title             = "//h1[contains(text(),'Important information')]";
                public const string Paragraph1        = "//p[@id='bodytext-p1']";
                public const string Paragraph2        = "//p[@id='bodytext-p2']";
                public const string PdsLink           = "//a[contains(text(),'Product Disclosure Statement')]";
                public const string PrivacyPolicyLink = "//a[contains(text(),'Privacy Policy')]";
                
                public class Button
                {
                    public const string NextPage = "//button[@id='button']";
                }
            }
        }
        #endregion

        #region Constants
        private const string BODYTEXT_1ST_PARAGRAPH = "You must answer our questions honestly, accurately and to the best of your knowledge. This duty applies to you and anyone else insured under the policy. If you answer for another person, we'll treat your answers as theirs. Your duty continues until we insure you. If you don't meet this duty, we may cancel your policy or treat it as if it never existed. Your claim may also be rejected or not paid in full.";
        private const string BODYTEXT_2ND_PARAGRAPH = "By selecting 'Continue', you agree we've provided you with the Product Disclosure Statement. You also agree with our Privacy Policy.";
        public const string NPE_PRIVACY_POLICY_URL = "https://cdvnetd.ractest.com.au/about-rac/site-info/privacy";
        public const string NPE_BOAT_PDS_URL = "https://cdvnetd.ractest.com.au/products/insurance/policy-documents/boat-insurance";
        #endregion

        #region Settable properties and controls

        #endregion


        public SparkQuoteImportantInformation(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPaths.Header.PhoneNumber);
                GetElement(XPath.ImportantInformation.Title);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Quote page 0 - Important information");

            return true;
        }

        /// <summary>
        /// Supports Spark Boat Quotes.
        /// Verify that the RACI telephone number exists in the header 
        /// and that the Footer links are as expected.
        /// </summary>
        public void VerifyHeaderAndFooterContent()
        {
            Reporting.AreEqual(Header.Link.RACI_TELEPHONE_NUMBER, GetElement(XPaths.Header.PhoneNumber).GetAttribute("href"), "Help telephone number at the top right hand corner");
            Reporting.AreEqual(Footer.Link.PRIVACY_URL, GetElement(XPaths.Footer.PrivacyLink).GetAttribute("href"), "NPE Footer Privacy Policy URL");
            Reporting.AreEqual(Footer.Link.DISCLAIMER_URL, GetElement(XPaths.Footer.DisclaimerLink).GetAttribute("href"), "NPE Footer Disclaimer URL");
            Reporting.AreEqual(Footer.Link.SECURITY_URL, GetElement(XPaths.Footer.SecurityLink).GetAttribute("href"), "NPE Footer Security URL");
            Reporting.AreEqual(Footer.Link.ACCESSIBILITY_URL, GetElement(XPaths.Footer.AccessibilityLink).GetAttribute("href"), "NPE Footer Accessibility URL");
        }

        /// <summary>
        /// Supports Spark Boat Quotes.
        /// Verify text and elements found on the Important Information
        /// page.
        /// </summary>
        public void VerifyImportantInformationPageContent()
        {
            VerifyStandardHeaderAndFooterContent();

            Reporting.AreEqual("Important information", GetInnerText(XPath.ImportantInformation.Title), "Title of body text");

            Reporting.AreEqual(BODYTEXT_1ST_PARAGRAPH, GetElement(XPath.ImportantInformation.Paragraph1).Text, "Page body text (paragraph 1)");
            Reporting.AreEqual(BODYTEXT_2ND_PARAGRAPH, GetElement(XPath.ImportantInformation.Paragraph2).Text, "Page body text (paragraph 2)");

            Reporting.AreEqual(NPE_BOAT_PDS_URL, GetElement(XPath.ImportantInformation.PdsLink).GetAttribute("href"), "NPE Boat PDS URL");
            Reporting.AreEqual(NPE_PRIVACY_POLICY_URL, GetElement(XPath.ImportantInformation.PrivacyPolicyLink).GetAttribute("href"), "NPE Privacy Policy URL");

            
            Reporting.AreEqual("Continue", GetInnerText(XPath.ImportantInformation.Button.NextPage), "Continue button");

        }

        public void ContinueToLetsStart()
        {
            ClickControl(XPath.ImportantInformation.Button.NextPage);
        }
    }
}
