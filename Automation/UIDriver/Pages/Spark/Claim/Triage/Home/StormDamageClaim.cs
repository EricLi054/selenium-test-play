using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Triage
{
    public class StormDamageClaim : SparkBasePage
    {
        private class Constants
        {
            public class Checkbox
            {
                public static readonly string DamagedFence = "My fence";
                public static readonly string DamagedBuilding = "My building\r\nCould be things like walls, ceilings, roof, windows, doors, hard floors (and carpets if this is an investment property)";
                public static readonly string DamagedContents = "My contents\r\nCould be things like carpets, furniture, computers, appliances, clothing";
            }
        }
        private class XPath
        {
            public class Checkbox
            {
                public static readonly string DamagedFence      = "id('storm-damage-claim-Fence')";
                public static readonly string DamagedBuilding   = "id('storm-damage-claim-Building')";
                public static readonly string DamagedContents   = "id('storm-damage-claim-Contents')";
            }
            public static readonly string ButtonNext = "//button[@data-testid='storm-damage-claim-button']";
        }

        public StormDamageClaim(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Checkbox.DamagedFence);
                GetElement(XPath.Checkbox.DamagedBuilding);
                GetElement(XPath.Checkbox.DamagedContents);
                GetElement(XPath.ButtonNext);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Home Storm Claim Page - Tell us what's has been damaged");
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.Checkbox.DamagedBuilding, GetInnerText(XPath.Checkbox.DamagedBuilding), "My building checkbox copy");
            Reporting.AreEqual(Constants.Checkbox.DamagedContents, GetInnerText(XPath.Checkbox.DamagedContents), "My contents checkbox copy");
            Reporting.AreEqual(Constants.Checkbox.DamagedFence, GetInnerText(XPath.Checkbox.DamagedFence), "My fence checkbox copy");
        }

        public void CheckMyFenceCheckbox()
        {
            ClickControl(XPath.Checkbox.DamagedFence);
        }

        public void CheckMyBuildingCheckbox()
        {
            ClickControl(XPath.Checkbox.DamagedBuilding);
        }

        public void CheckMyContentsCheckbox()
        {
            ClickControl(XPath.Checkbox.DamagedContents);
        }

        public void ClickNextButton()
        {
            ClickControl(XPath.ButtonNext);
        }
    }
}
