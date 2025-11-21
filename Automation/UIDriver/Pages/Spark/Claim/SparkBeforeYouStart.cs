using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

namespace UIDriver.Pages.Spark.Claim
{
    public class SparkBeforeYouStart : SparkBasePage
    {

        #region XPATHS
        protected class XPath
        {
            public static readonly string SubHeader = "//h2[text()=\"Before you start\"]";
            public static readonly string NextButton = "//button[@data-testid='next']";
        }
        #endregion

        public SparkBeforeYouStart(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.SubHeader);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        public void ClickNext()
        {
            Reporting.Log("Capturing Before You Start before continuing to the next page.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.NextButton);
        }
    }


}
