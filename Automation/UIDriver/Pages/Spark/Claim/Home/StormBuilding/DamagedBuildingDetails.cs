using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class DamagedBuildingDetails : SparkBasePage
    {
        private class Constants
        {
            public static readonly string ActiveStepperLabel    = "Specific damage";
            public static readonly string Header                = "Tell us about any specific damage";
            public static readonly string NoSelection           = "Please select all options that apply";
            public class Categories
            {
                public static readonly string Flooring          = "Flooring";
                public static readonly string SolarPanels       = "Solar panels";
                public static readonly string GarageDoorOrMotor = "Garage door or motor";
                public static readonly string TvAerial          = "TV aerial";
                public static readonly string ClothesLine       = "Clothesline";
                public static readonly string SecuritySystem    = "Security system";
                public static readonly string Glass             = "Glass";
                public static readonly string LeadLight         = "Leadlight";
                public static readonly string OtherItems        = "Other items";
            }
        }
        private class XPath
        {
            public static readonly string ActiveStepper         = "//button[@aria-selected='true']";
            public static readonly string Header                = "id('header')";
            public static readonly string ClaimNumber           = "id('claimNumberDisplay')";
            public static readonly string NoSelection           = "id('helper-text-undefined')";
            public static readonly string NextButton            = "id('submit-button')";
            public class Categories
            {
                public static readonly string Flooring          = "id('Flooring')";
                public static readonly string SolarPanels       = "id('SolarPanels')";
                public static readonly string GarageDoorOrMotor = "id('GarageDoorOrMotor')";
                public static readonly string TvAerial          = "id('TvAerial')";
                public static readonly string ClothesLine       = "id('Clothesline')";
                public static readonly string SecuritySystem    = "id('SecuritySystem')";
                public static readonly string Glass             = "id('Glass')";
                public static readonly string LeadLight         = "id('Leadlight')";
                public static readonly string OtherItems        = "id('OtherItems')";
            }
        }

        public DamagedBuildingDetails(Browser browser) : base(browser)
        { }
        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("Spark Storm Claim - Specific damage");
                Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            }
            return isDisplayed;
        }

        /// <summary>
        /// If 'Other items' has been explicitly flagged for selection in this test, we select it and ignore all of the other options.
        /// 
        /// If 'Other items' has not been explicitly flagged for selection we don't want to select it as it will impact the 
        /// normal claim lifecycle.
        /// </summary>
        public void SelectSpecificDamages(ClaimHome claimHome)
        {
            if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.OtherItems))
            {
                Reporting.Log($"Selecting 'Other items'");
                ScrollElementIntoView(XPath.Categories.OtherItems);
                ClickControl(XPath.Categories.OtherItems);
            }
            else
            {
                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.Flooring))
                {
                    ClickControl(XPath.Categories.Flooring);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.SolarPanels))
                {
                    ClickControl(XPath.Categories.SolarPanels);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.GarageDoorOrMotor))
                {
                    ClickControl(XPath.Categories.GarageDoorOrMotor);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.TvAerial))
                {
                    ClickControl(XPath.Categories.TvAerial);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.ClothesLine))
                {
                    ClickControl(XPath.Categories.ClothesLine);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.SecuritySystem))
                {
                    ClickControl(XPath.Categories.SecuritySystem);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.Glass))
                {
                    ClickControl(XPath.Categories.Glass);
                }

                if (claimHome.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.LeadLight))
                {
                    ClickControl(XPath.Categories.LeadLight);
                }
            }
        }

        public void DetailedUiCheckingOfBuildingDamageTypes()
        {
            Reporting.LogMinorSectionHeading("Detailed UI Checking begins by triggering page validation error");
            ClickToContinue();
            ScrollElementIntoView(XPath.NoSelection);
            Reporting.Log($"Capturing screen with validation error", _driver.TakeSnapshot());

            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header),
                "expected page header text");

            Reporting.AreEqual(Constants.NoSelection, GetInnerText(XPath.NoSelection),
                "validation text displayed when no selection has been made");

            Reporting.AreEqual(Constants.Categories.Flooring, GetInnerText(XPath.Categories.Flooring),
                "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.SolarPanels, GetInnerText(XPath.Categories.SolarPanels),
                "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.GarageDoorOrMotor, GetInnerText(XPath.Categories.GarageDoorOrMotor),
                 "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.TvAerial, GetInnerText(XPath.Categories.TvAerial),
                 "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.ClothesLine, GetInnerText(XPath.Categories.ClothesLine),
                 "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.SecuritySystem, GetInnerText(XPath.Categories.SecuritySystem),
                 "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.Glass, GetInnerText(XPath.Categories.Glass),
                 "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.LeadLight, GetInnerText(XPath.Categories.LeadLight),
                 "expected copy with actual value displayed");

            Reporting.AreEqual(Constants.Categories.OtherItems, GetInnerText(XPath.Categories.OtherItems),
                 "expected copy with actual value displayed");
        }

        public void ClickToContinue()
        {
            ClickControl(XPath.NextButton);
        }
    }
}
