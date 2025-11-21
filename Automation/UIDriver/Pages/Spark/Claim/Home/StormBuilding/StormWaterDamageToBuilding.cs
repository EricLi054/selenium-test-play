using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class StormWaterDamageToBuilding : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string ActiveStepperLabel    = "Water damage";
            public static readonly string Header                = "Water damage inside the home";
            public static readonly string SelectAllProblems     = "Select all problems your home has.";
            public static readonly string NoSelection           = "Please select all options that apply";
            public class WaterDamageCheckboxes
            {
                public static readonly string NoWaterDamage                = "No water damage";
                public static readonly string DampPatchesOrDripping        = "Damp patches or dripping";
                public static readonly string SolidTimberFloorIsWet        = "Solid timber flooring is wet\r\nIncludes glued-down hardwoods and parquetry (excludes laminate or floating floorboards, vinyl and tiles)";
                public static readonly string BadlySoakedCarpets           = "Carpet is so badly soaked that you can't dry it";
                public static readonly string HouseIsFlooded               = "House is flooded";
                public static readonly string SewageOrDrainWaterInTheHouse = "There's sewage or drain water in the house";
                public static readonly string WaterInTheElectrics          = "Water in the electrics\r\nWater leaking into electrical switches or light fittings, some sparking";
                public static readonly string OtherWaterDamage             = "Other water damage";

                public class AdviseUser
                {
                    public static readonly string ExamplesTitle     = "Typical examples of serious water damage";
                    public static readonly string ExamplesBody      = "The house is flooded\r\nThe floor is wet in a large area\r\nThere's sewage or contaminated water in the house";
                    public static readonly string AssessYourDamage  = "Once you submit your claim, we'll get a repairer to contact you to assess your damage";
                }
            }
        }
        #endregion
        private class XPath
        {
            public static readonly string ActiveStepper     = "//button[@aria-selected='true']";
            public static readonly string Header            = "id('header')";
            public static readonly string ClaimNumber       = "id('claimNumberDisplay')";
            public static readonly string SelectAllProblems = "id('water-damage-problems-multi-choice-input-label')"; 
            public static readonly string NoSelection       = "id('water-damage-problems-multi-choice-input-error')"; 
            public class WaterDamageCheckboxes
            {
                public static readonly string NoWaterDamage                = "id('water-damage-problems-multi-choice-input-NoWaterDamage')";
                public static readonly string DampPatchesOrDripping        = "id('water-damage-problems-multi-choice-input-DampPatchesOrDripping')";
                public static readonly string SolidTimberFloorIsWet        = "id('water-damage-problems-multi-choice-input-SolidTimberFloorIsWet')";
                public static readonly string BadlySoakedCarpets           = "id('water-damage-problems-multi-choice-input-BadlySoakedCarpets')";
                public static readonly string HouseIsFlooded               = "id('water-damage-problems-multi-choice-input-HouseIsFlooded')";
                public static readonly string SewageOrDrainWaterInTheHouse = "id('water-damage-problems-multi-choice-input-SewageOrDrainWaterInTheHouse')";
                public static readonly string WaterInTheElectrics          = "id('water-damage-problems-multi-choice-input-WaterInTheElectrics')";
                public static readonly string OtherWaterDamage             = "id('water-damage-problems-multi-choice-input-OtherWaterDamage')";
            }
            public static readonly string NextButton = "id('submit-button')";
        }

        public StormWaterDamageToBuilding(Browser browser) : base(browser)
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
                Reporting.LogPageChange("Spark Storm Claim - Water Damage");
                Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            }
            return isDisplayed;
        }

        /// <summary>
        /// Select the checkboxes as expected from the test data.
        /// </summary>
        public void WaterDamageBuilding(ClaimHome claimHome)
        {
            HandleBadlySoakedCarpets(claimHome);

            if (claimHome.StormWaterDamageCheckboxes.NoWaterDamage)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.NoWaterDamage}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.NoWaterDamage);
            }
            
            if (claimHome.StormWaterDamageCheckboxes.DampPatchesOrDripping)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.DampPatchesOrDripping}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.DampPatchesOrDripping);
            }

            if (claimHome.StormWaterDamageCheckboxes.SolidTimberFloorIsWet)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.SolidTimberFloorIsWet}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.SolidTimberFloorIsWet);
            }

            if (claimHome.StormWaterDamageCheckboxes.HouseIsFlooded)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.HouseIsFlooded}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.HouseIsFlooded);
            }

            if (claimHome.StormWaterDamageCheckboxes.SewageOrDrainWaterInTheHouse)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.SewageOrDrainWaterInTheHouse}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.SewageOrDrainWaterInTheHouse);
            }

            if (claimHome.StormWaterDamageCheckboxes.WaterInTheElectrics)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.WaterInTheElectrics}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.WaterInTheElectrics);
            }

            if (claimHome.StormWaterDamageCheckboxes.OtherWaterDamage)
            {
                Reporting.Log($"Selecting '{Constants.WaterDamageCheckboxes.OtherWaterDamage}' checkbox.");
                ClickControl(XPath.WaterDamageCheckboxes.OtherWaterDamage);
            }
            Reporting.Log($"Capturing screenshot of page before continuing", _driver.TakeSnapshot());
        }

        /// <summary>
        /// This method checks more detail than the average test scenario, and is only invoked if 
        /// the detailedUiChecking flag is true for a given test scenario.
        /// 
        /// Badly Soaked Carpets is not included here as covered within HandleBadlySoakedCarpets.
        /// </summary>
        public void DetailedUiChecksOfWaterDamage(ClaimHome claimHome)
        {
            ClickControl(XPath.NextButton);

            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header));
            Reporting.AreEqual(Constants.NoSelection, GetInnerText(XPath.NoSelection),
                "expected validation error text with value displayed");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.NoWaterDamage,
                GetInnerText(XPath.WaterDamageCheckboxes.NoWaterDamage), "expected label matches the value displayed for No Water Damage");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.DampPatchesOrDripping,
                GetInnerText(XPath.WaterDamageCheckboxes.DampPatchesOrDripping), "expected label matches the value displayed for Damp Patches/Dripping");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.SolidTimberFloorIsWet,
                GetInnerText(XPath.WaterDamageCheckboxes.SolidTimberFloorIsWet), "expected label matches the value displayed for Solid Timber flooring");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.HouseIsFlooded,
                GetInnerText(XPath.WaterDamageCheckboxes.HouseIsFlooded), "expected label matches the value displayed for House is Flooded");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.SewageOrDrainWaterInTheHouse,
                GetInnerText(XPath.WaterDamageCheckboxes.SewageOrDrainWaterInTheHouse), "expected label matches the value displayed for Sewage/Drain water");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.WaterInTheElectrics,
                GetInnerText(XPath.WaterDamageCheckboxes.WaterInTheElectrics), "expected label matches the value displayed for Water in Electrics");

            Reporting.AreEqual(Constants.WaterDamageCheckboxes.OtherWaterDamage,
                GetInnerText(XPath.WaterDamageCheckboxes.OtherWaterDamage), "expected label matches the value displayed for Other Water Damage");
        }

        /// <summary>
        /// Most checkboxes on the Water damage page are always available, however 
        /// the there is one which is only displayed in some circumstances and must 
        /// not be in others. This method deals with that additional complication 
        /// after the other options are validated.
        /// </summary>
        public void HandleBadlySoakedCarpets(ClaimHome claimHome)
        {
            if (claimHome.PolicyDetails.HomeAsset.OccupancyType == "O"
                && (claimHome.DamagedCovers == AffectedCovers.FenceOnly ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingOnly ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingAndFence))
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.WaterDamageCheckboxes.BadlySoakedCarpets),
                    "Badly soaked carpet option should NOT be displayed when home is " +
                    "Owner Occupied AND Contents damage code is not involved.");
                claimHome.StormWaterDamageCheckboxes.BadlySoakedCarpets = false;
                Reporting.Log($"Enforcing BadlySoakedCarpets = false as a precaution for later verification." +
                    $"Logging actual value: '{claimHome.StormWaterDamageCheckboxes.BadlySoakedCarpets}'");
            }

            // If Badly Soaked Carpets is a valid option on display, confirm it is displayed
            // and select it if test data determines it should be.
            if (claimHome.PolicyDetails.HomeAsset.OccupancyType == "I" ||
                claimHome.PolicyDetails.HomeAsset.OccupancyType == "T"
                || (claimHome.DamagedCovers == AffectedCovers.ContentsOnly ||
                    claimHome.DamagedCovers == AffectedCovers.ContentsAndFence ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingAndContents ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.WaterDamageCheckboxes.BadlySoakedCarpets),
                    "'Carpet is so badly soaked that you can't dry it' is displayed for investment properties " +
                    "OR where Contents damage code is involved.");
                Reporting.AreEqual(Constants.WaterDamageCheckboxes.BadlySoakedCarpets, GetInnerText(XPath.WaterDamageCheckboxes.BadlySoakedCarpets),
                    "expected copy for badly soaked carpets option is displayed.");

                if (claimHome.StormWaterDamageCheckboxes.BadlySoakedCarpets)
                {
                    ClickControl(XPath.WaterDamageCheckboxes.BadlySoakedCarpets);
                }
            }
        }

        public void ClickToContinue()
        {
            ClickControl(XPath.NextButton);
        }
    }
}