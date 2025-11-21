using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class BeforeYouStart : SparkBeforeYouStart
    {
        #region XPATHS
        private new class XPath :SparkBeforeYouStart.XPath
        {
            //TODO - Add XPaths for verification of page content
        }
        #endregion

        public BeforeYouStart(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.SubHeader);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Home Storm Claim Page - Before You Start");
            return true;
        }
    }


}
