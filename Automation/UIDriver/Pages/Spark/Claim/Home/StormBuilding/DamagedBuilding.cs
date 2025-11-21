using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class DamagedBuilding : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string ActiveStepperLabel    = "Your building damage";
            public static readonly string Header                = "First, let's take care of your building damage";
            public static readonly string NoSelection           = "Please select all options that apply";
            public class DamageLevel
            {
                public class Title
                {
                    public static readonly string SpecificItemsDamaged  = "Level 1: Specific items have been damaged";
                    public static readonly string HomeHasBeenDamaged    = "Level 2: My home has been damaged";
                    public static readonly string HomeIsNotSafe         = "Level 3: My home isn't safe";
                }
                public class Body
                {
                    public static readonly string SpecificItemsDamaged  = "Typical examples include:\r\nFlooring\r\nSolar panels\r\nGarage doors and motors\r\nTV aerial\r\nClothesline\r\nSecurity system\r\nGlass or leadlight\r\nOther items";
                    public static readonly string HomeHasBeenDamaged    = "Typical examples include:\r\nSome broken roof tiles\r\nDamage to gutters and eaves\r\nExternal walls damaged\r\nStaining on internal walls or ceiling\r\nElectrical failure to permanent fixtures such as aircon, pool pump, or power points";
                    public static readonly string HomeIsNotSafe         = "Typical examples include:\r\nPart of the roof has been crushed, come off or collapsed (not just a few roof tiles)\r\nLarge cracks are spreading in the walls or ceilings\r\nThe ceiling has caved in or is sagging";
                }
            }
        }
        #endregion
        private class XPath
        {
            public static readonly string ActiveStepper = "//button[@aria-selected='true']";
            public static readonly string Header        = "id('header')";
            public static readonly string ClaimNumber   = "id('claimNumberDisplay')";
            public static readonly string NoSelection   = "id('helper-text-undefined')";
            public static readonly string NextButton    = "id('submit-button')";
            public class DamageLevel
            {
                public class Title
                {
                    public static readonly string SpecificItemsDamaged  = "id('1-selection-card-title')";
                    public static readonly string HomeHasBeenDamaged    = "id('2-selection-card-title')";
                    public static readonly string HomeIsNotSafe         = "id('3-selection-card-title')";
                }
                public class Body
                {
                    public static readonly string SpecificItemsDamaged  = "id('1-selection-card-content')";
                    public static readonly string HomeHasBeenDamaged    = "id('2-selection-card-content')";
                    public static readonly string HomeIsNotSafe         = "id('3-selection-card-content')";
                }
                public class Button
                {
                    public static readonly string SpecificItemsDamaged  = "id('1-selection-card-select-btn')";
                    public static readonly string HomeHasBeenDamaged    = "id('2-selection-card-select-btn')";
                    public static readonly string HomeIsNotSafe         = "id('3-selection-card-select-btn')";
                }
            }
        }

        public DamagedBuilding(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
                GetElement(XPath.DamageLevel.Title.SpecificItemsDamaged);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("Spark Storm Claim - Building Damage");
                Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            }
            return isDisplayed;
        }

        public void AnswerBuildingDamageLevel(Browser browser, ClaimHome claimHome)
        {
            Reporting.Log($"Evaluating what to select");
            if (claimHome.BuildingDamage.IsSpecificItemsDamaged)
            {
                ScrollElementIntoView(XPath.DamageLevel.Button.SpecificItemsDamaged);
                ClickControl(XPath.DamageLevel.Button.SpecificItemsDamaged);
                Reporting.Log($"Selected '{Constants.DamageLevel.Title.SpecificItemsDamaged}'", _browser.Driver.TakeSnapshot());
            }
            else 
            {
                Reporting.Log($"Damage '{Constants.DamageLevel.Title.SpecificItemsDamaged}' was NOT selected.");
            }

            
            if (claimHome.BuildingDamage.IsHomeBadlyDamaged)
            {
                ScrollElementIntoView(XPath.DamageLevel.Button.HomeHasBeenDamaged);
                ClickControl(XPath.DamageLevel.Button.HomeHasBeenDamaged);
                Reporting.Log($"Selected '{Constants.DamageLevel.Title.HomeHasBeenDamaged}'", _browser.Driver.TakeSnapshot());
            }
            else
            {
                Reporting.Log($"Damage '{Constants.DamageLevel.Title.HomeHasBeenDamaged}' was NOT selected.");
            }

            if (claimHome.BuildingDamage.IsHomeUnsafe)
            {
                ScrollElementIntoView(XPath.DamageLevel.Button.HomeIsNotSafe);
                ClickControl(XPath.DamageLevel.Button.HomeIsNotSafe);
                Reporting.Log($"Selected '{Constants.DamageLevel.Title.HomeIsNotSafe}'", _browser.Driver.TakeSnapshot());
            }
            else
            {
                Reporting.Log($"Damage '{Constants.DamageLevel.Title.HomeIsNotSafe}' was NOT selected.");
            }
        }

        public void DetailedUiCheckingOfDamageLevels()
        {
            Reporting.LogMinorSectionHeading("Detailed UI checking begins by triggering page validation error");

            ClickToContinue();
            ScrollElementIntoView(XPath.DamageLevel.Button.HomeIsNotSafe);
            Reporting.Log($"Capturing screen with validation error", _driver.TakeSnapshot());

            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header),
                "expected page header text");

            Reporting.AreEqual(Constants.NoSelection, GetInnerText(XPath.NoSelection),
                "validation text displayed when no selection has been made");

            Reporting.AreEqual(Constants.DamageLevel.Title.SpecificItemsDamaged,
                GetInnerText(XPath.DamageLevel.Title.SpecificItemsDamaged),
                $"against expected title text of first level of damage");

            Reporting.AreEqual(Constants.DamageLevel.Body.SpecificItemsDamaged,
                GetInnerText(XPath.DamageLevel.Body.SpecificItemsDamaged),
                $"against expected body text of first level of damage");

            Reporting.AreEqual(Constants.DamageLevel.Title.HomeHasBeenDamaged,
                GetInnerText(XPath.DamageLevel.Title.HomeHasBeenDamaged),
                $"against expected title text of second level of damage");

            Reporting.AreEqual(Constants.DamageLevel.Body.HomeHasBeenDamaged,
                GetInnerText(XPath.DamageLevel.Body.HomeHasBeenDamaged),
                $"against expected body text of second level of damage");

            Reporting.AreEqual(Constants.DamageLevel.Title.HomeIsNotSafe,
                GetInnerText(XPath.DamageLevel.Title.HomeIsNotSafe),
                $"against expected title text of third level of damage");

            Reporting.AreEqual(Constants.DamageLevel.Body.HomeIsNotSafe,
                GetInnerText(XPath.DamageLevel.Body.HomeIsNotSafe),
                $"against expected body text of third level of damage");
        }

        public void ClickToContinue()
        {
            ClickControl(XPath.NextButton);
        }
    }
}
