using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.EFT
{
    public class LinkExpired : SparkBasePage
    {
        #region XPATHS

        private static class XPath
        {
            public static class Error
            {
                public const string Heading = "//h1[contains(text(),'take you to your destination')]"; //TODO: SPK-4097 - When ID values have been added, update XPath
                public const string Message = "//h6[contains(text(),'It looks like your link has expired')]"; //TODO: SPK-4097 - When ID values have been added, update XPath
            }
            public static class Button
            {
                public const string HomePage = "id('btn-return-home')";
            }
        }

        #endregion XPATHS

        #region Settable properties and controls

        public string ErrorHeading => GetInnerText(XPath.Error.Heading);

        public string ErrorMessage => GetInnerText(XPath.Error.Message);

        #endregion Settable properties and controls

        public LinkExpired(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Button.HomePage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }
    }
}
