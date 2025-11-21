using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.UploadInvoice
{
    public class ErrorPage : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string ClaimNumberText = "Your claim number is ";
            public class FileLimitReached
            {
                public const string Header = "File limit reached";
                public const string SubHeader = "If you'd like to submit more files";
                public const string Message = "Email them to claims@rac.com.au with your claim number in the subject line.";
            }
            public class LinkExpired
            {
                public const string Header = "Uh oh!";
                public const string SubHeader = "This link has expired";
                public const string Message = "Please email your documents to claims@rac.com.au with your claim number in the subject line.";               
            }

            public class SomethingWentWrong
            {
                public const string Header = "Uh oh!";
                public const string SubHeader = "Something went wrong";
                public const string Message = "Please try again or email your documents to claims@rac.com.au with your claim number in the subject line.";
        }

            public class SessionTimeOut
            {
                public const string Header = "Uh oh!";
                public const string SubHeader = "It looks like your page timed out";
                public const string Message = "Please try again.";
            }

        }

        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Header = "//div[contains(@class,'MuiGrid-root MuiGrid-item css')]/h1";
            public const string SubHeader = "//div[contains(@class,'MuiGrid-root MuiGrid-item MuiGrid-grid')]/h1";
            public const string Message = "//div[contains(@class,'MuiTypography-root MuiTypography-subtitle1')]";
            public const string ClaimNumberText = "//p[@data-testid='claimNumberDisplay']";

        }

        #endregion

        public ErrorPage(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.SubHeader);
                GetElement(XPath.Message);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("Upload Invoice - Error Page");
            return true;
        }

        public void VerifyFileLimitReachedErrorPage(string claimNumber)
        {
            Reporting.Log("File Limit Reached", _browser.Driver.TakeSnapshot());
            _browser.PercyScreenCheck(DocumentUpload.FileLimitReachedErrorPage, GetPercyIgnoreCSS());

            Reporting.AreEqual(Constants.FileLimitReached.Header, GetInnerText(XPath.Header), "File limit reached header message");
            Reporting.AreEqual(Constants.FileLimitReached.SubHeader, GetInnerText(XPath.SubHeader), "File limit reached sub header message");
            Reporting.AreEqual(Constants.FileLimitReached.Message, GetInnerText(XPath.Message), "File limit reached text");
            Reporting.AreEqual($"{Constants.ClaimNumberText}{claimNumber}", GetInnerText(XPath.ClaimNumberText), "Claim number text");
        }

        public void VerifyLinkExpiredErrorPage(string claimNumber)
        {
            Reporting.Log("Link Expired Error Page", _browser.Driver.TakeSnapshot());
            _browser.PercyScreenCheck(DocumentUpload.LinkExpiredErrorPage, GetPercyIgnoreCSS());

            Reporting.AreEqual(Constants.LinkExpired.Header, GetInnerText(XPath.Header), "Link expired header message");
            Reporting.AreEqual(Constants.LinkExpired.SubHeader, GetInnerText(XPath.SubHeader), "Link expired sub header message");
            Reporting.AreEqual(Constants.LinkExpired.Message, GetInnerText(XPath.Message), "Link expired text");
            Reporting.AreEqual($"{Constants.ClaimNumberText}{claimNumber}", GetInnerText(XPath.ClaimNumberText), "Claim number text");
        }

        public void VerifySomethingWentWrongErrorPage(string claimNumber)
        {
            Reporting.Log("Something Went Wrong Error Page", _browser.Driver.TakeSnapshot());
            _browser.PercyScreenCheck(DocumentUpload.SomethingWentWrongErrorPage, GetPercyIgnoreCSS());

            Reporting.AreEqual(Constants.SomethingWentWrong.Header, GetInnerText(XPath.Header), "Something went wrong header message");
            Reporting.AreEqual(Constants.SomethingWentWrong.SubHeader, GetInnerText(XPath.SubHeader), "Something went wrong sub header message");
            Reporting.AreEqual(Constants.SomethingWentWrong.Message, GetInnerText(XPath.Message), "Something went wrong text");
            Reporting.AreEqual($"{Constants.ClaimNumberText}{claimNumber}", GetInnerText(XPath.ClaimNumberText), "Claim number text");
        }

        public void VerifySessionTimeOutErrorPage()
        {
            Reporting.Log("Session Time Out Error Page", _browser.Driver.TakeSnapshot());
            _browser.PercyScreenCheck(DocumentUpload.SessionTimeOutErrorPage);

            Reporting.AreEqual(Constants.SessionTimeOut.Header, GetInnerText(XPath.Header), "Session time out header message");
            Reporting.AreEqual(Constants.SessionTimeOut.SubHeader, GetInnerText(XPath.SubHeader), "Session time out sub header message");
            Reporting.AreEqual(Constants.SessionTimeOut.Message, GetInnerText(XPath.Message), "Session time out text");           
        }

        private List<string> GetPercyIgnoreCSS() =>
           new List<string>()
           {
                "#claimNumberDisplay span"
           };
    }
}
