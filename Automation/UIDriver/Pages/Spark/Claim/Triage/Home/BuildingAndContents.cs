using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Triage
{
    public class BuildingAndContents : SparkBasePage
    {
        private class Constants
        {
            public class Control
            {
                public static readonly string StormDamage = "Storm damage\r\nIncludes building, contents and fence claims";
                public static readonly string OtherDamage = "Other\r\nAny other home insurance claim";
            }
        }
        private class XPath
        {
            public static readonly string Header = "//h2[contains(text(),'are you claiming for')]";
            public class Control
            {
                public static readonly string ButtonStormDamage = "//button[@id='claimType-storm-button']";
                public static readonly string ButtonOther = "//button[@id='claimType-other-button']";
                public static readonly string StormDamage = "id('building-and-contents-claim-cardgroup-0-StormDamage')";
                public static readonly string OtherDamage = "id('building-and-contents-claim-cardgroup-1-Other')";
                public static readonly string ButtonNext = "//button[@type='submit']";
            }
        }

        public BuildingAndContents(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Home Storm Claim Page - What are you claiming for?");
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.Control.StormDamage, GetInnerText(XPath.Control.StormDamage), "Storm damage copy");
            Reporting.AreEqual(Constants.Control.OtherDamage, GetInnerText(XPath.Control.OtherDamage), "Other damage copy");
        }

        /// <summary>
        /// Select the element for "Storm damage".
        /// 
        /// If the older element is available, select it.
        /// If the newer element is available, select it instead and then
        /// select the Next button.
        /// 
        /// If neither element is available, throw a NoSuchElementException
        /// and fail the test.
        /// 
        /// TODO: We're checking for both to support the Home Claims Triage for both BAU 
        /// and including changes to the application from the Home General Claims project.
        /// When HGC is BAU, this can be simplified.
        /// </summary>
        /// <exception cref="NoSuchElementException"></exception>
        public void ClickStormDamage()
        {
            if (IsControlDisplayed(XPath.Control.ButtonStormDamage))
            {
                Reporting.Log("Older 'Storm damage' element is available so selecting it.");
                ClickControl(XPath.Control.ButtonStormDamage);
            }
            else if (IsControlDisplayed(XPath.Control.StormDamage))
            {
                ClickControl(XPath.Control.StormDamage);
                Reporting.Log("Newer 'Storm damage' element is available so have selected it, " +
                    "capturing snapshot before selecting Next.", _browser.Driver.TakeSnapshot());
                ClickNextButton();
            }
            else
            {
                Reporting.Log("Could not detect 'Storm damage' element for selection, throwing exception.");
                throw new NoSuchElementException();
            }
        }

        /// <summary>
        /// Select the element for "Other" damage (not Storm damage).
        /// 
        /// If the older element is available, select it.
        /// If the newer element is available, select it instead and then
        /// select the Next button.
        /// 
        /// If neither element is available, throw a NoSuchElementException
        /// and fail the test.
        /// 
        /// TODO: We're checking for both to support the Home Claims Triage for both BAU 
        /// and including changes to the application from the Home General Claims project.
        /// When HGC is BAU, this can be simplified.
        /// </summary>
        /// <exception cref="NoSuchElementException"></exception>
        public void ClickOtherDamage()
        {
            if (IsControlDisplayed(XPath.Control.ButtonOther))
            {
                Reporting.Log("Older 'Other' element is available so selecting it.");
                ClickControl(XPath.Control.ButtonOther);
            }
            else if (IsControlDisplayed(XPath.Control.OtherDamage))
            {
                Reporting.Log("Newer 'Other' element is available so selecting it, then selecting Next.");
                ClickControl(XPath.Control.OtherDamage);
                ClickNextButton();
            }
            else
            {
                Reporting.Log("Could not detect 'Other' element for selection, throwing exception.");
                throw new NoSuchElementException();
            }
        }

        public void ClickNextButton()
        {
            ClickControl(XPath.Control.ButtonNext);
        }
    }
}
