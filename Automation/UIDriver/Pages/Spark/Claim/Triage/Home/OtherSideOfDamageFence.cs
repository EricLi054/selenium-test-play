using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Triage
{
    public class OtherSideOfDamageFence : SparkBasePage
    {
        private class XPath
        {
            public class Checkbox
            {
                public const string SharedFence         = "id('storm-damaged-fence-claim-SharedFence')";
                public const string NonSharedFence      = "id('storm-damaged-fence-claim-NonSharedFence')";
            }
            public const string ButtonNext             = "//button[@data-testid='storm-damaged-fence-claim-button']";
        }

        public OtherSideOfDamageFence(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Checkbox.SharedFence);
                GetElement(XPath.ButtonNext);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Home Storm Claim Page - What sort of fence has been been damaged?");
            return true;
        }

        /// <summary>
        /// Indicate that the damaged fence divides two private properties.
        /// </summary>
        public void ClickCheckboxForDividingFence()
        {
            ClickControl(XPath.Checkbox.SharedFence);
        }

        /// <summary>
        /// Indicate that the damaged fence does NOT divide two private properties.
        /// </summary>
        public void ClickCheckboxForNonDividingFence()
        {
            ClickControl(XPath.Checkbox.NonSharedFence);
        }

        public void ClickNextButton()
        {
            ClickControl(XPath.ButtonNext);
        }
    }
}
