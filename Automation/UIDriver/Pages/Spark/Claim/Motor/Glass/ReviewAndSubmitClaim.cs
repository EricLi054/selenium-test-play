using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.Motor.Glass
{
    public class ReviewAndSubmitClaim : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public class DamagedGlass
            {
                public const string AlreadyFixed = "Yes, it's fixed";
                public const string NotFixed = "No";
                public const string RepairsBooked = "I've booked the repairs";
            }

            public class Glass
            {
                public const string FrontWindscreen = "Front windscreen";
                public const string OtherWindowGlass = "Any other window glass";               
            }
        }
        #endregion

        #region XPATHS

        private const string XP_CAR_MAKE_AND_MODEL = "id('policy-card-content-policy-details-header-title-summary')";
        private const string XP_CAR_REGO = "id('policy-card-content-policy-details-header-subtitle-summary')";
        private const string XP_POLICY_NUMBER = "id('policy-card-content-policy-details-property-0-policy-number-summary')";

        private const string XP_DATE_OF_DAMAGE = "id('claim-summary-date-of-damage-text')";
        private const string XP_GLASS_FIXED = "id('claim-summary-glass-repair-text')";
        private const string XP_GLASS_DAMAGED = "//p[contains(@id,'claim-summary-glass-damage-text')]";

        private const string XP_BTN_SUBMIT_CLAIM = "//button[text()='Submit claim']";

        #endregion

        #region Settable properties and controls

        public string CarMakeAndModel => GetInnerText(XP_CAR_MAKE_AND_MODEL);

        public string CarRego => GetInnerText(XP_CAR_REGO);

        /// <summary>
        /// Extract policy number from "Policy number: MGPXXXXXXXXX"
        /// </summary>
        public string PolicyNumber
        {
            get
            {
                var str = GetInnerText(XP_POLICY_NUMBER);
                return str.Substring(15);
            }
        }

        public DateTime DamageDate => Convert.ToDateTime(GetInnerText(XP_DATE_OF_DAMAGE));

        public string HasGlassBeenFixed => GetInnerText(XP_GLASS_FIXED);

        /// <summary>
        /// There can be multiple items based on the glass damaged
        /// First, it will find all the elements for glass damage
        /// Then, it will get the text from each elements and add
        /// into a list and return it
        /// </summary>
        public List<string> GlassDamaged
        {
            get
            {
                IReadOnlyList<IWebElement> elements = _driver.FindElements(By.XPath(XP_GLASS_DAMAGED));
                List<string> glassDamage = new List<string>();
                foreach (var element in elements)
                {
                    glassDamage.Add(element.Text);
                }
                return glassDamage;
            }
        }

        #endregion

        public ReviewAndSubmitClaim(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XP_CAR_MAKE_AND_MODEL);
                GetElement(XP_CAR_REGO);
                GetElement(XP_POLICY_NUMBER);
                GetElement(XP_BTN_SUBMIT_CLAIM);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motor Glass Claim Page - Review and Submit Claim");
            return true;
        }

        public void VerifyReviewAndSubmitClaim(ClaimCar claim)
        {            
            Reporting.AreEqual(CarMakeAndModel, $"{claim.Policy.Vehicle.Year} {claim.Policy.Vehicle.Make}", "Car Make and Model");
            
            if (claim.Policy.Vehicle.Registration != null)
            {
                Reporting.AreEqual(claim.Policy.Vehicle.Registration, CarRego, "Car Registration Number");
            }                
            
            Reporting.AreEqual(claim.Policy.PolicyNumber, PolicyNumber, "Policy number");

            Reporting.AreEqual(claim.EventDateTime.ToString("dd/MM/yyyy"), DamageDate.ToString("dd/MM/yyyy"), "Damage date");
           
            switch(claim.ClaimScenario)
            {
                case MotorClaimScenario.GlassDamageAlreadyFixed:
                    Reporting.AreEqual(Constants.DamagedGlass.AlreadyFixed, HasGlassBeenFixed , "Glass already fixed");
                    _browser.PercyScreenCheck(ClaimMotorGlass.ReviewClaim_GlassAlreadyFixed, GetPercyIgnoreCSS());
                    break;
                case MotorClaimScenario.GlassDamageNotFixed:
                    Reporting.AreEqual(Constants.DamagedGlass.NotFixed, HasGlassBeenFixed , "Glass not fixed");
                    _browser.PercyScreenCheck(ClaimMotorGlass.ReviewClaim_GlassNotFixed, GetPercyIgnoreCSS());
                    break;
                case MotorClaimScenario.GlassDamageRepairsBooked:
                    Reporting.AreEqual(Constants.DamagedGlass.RepairsBooked, HasGlassBeenFixed , "Glass repair booked");
                    _browser.PercyScreenCheck(ClaimMotorGlass.ReviewClaim_GlassRepairBooked, GetPercyIgnoreCSS());
                    break;
                default:
                    throw new NotSupportedException ($"{claim.ClaimScenario} not supported for motor glass claim");
            }

            if (claim.GlassDamageDetails.FrontWindscreenOnly)
            { 
                Reporting.IsTrue(GlassDamaged.Contains(Constants.Glass.FrontWindscreen), "Front windscreen");
            }
            if(claim.GlassDamageDetails.OtherWindowGlass)
            {
                Reporting.IsTrue(GlassDamaged.Contains(Constants.Glass.OtherWindowGlass), "Other window glass");
            }

            Reporting.Log("Motor Glass Claim - Review and Submit Claim", _browser.Driver.TakeSnapshot());

            ClickControl(XP_BTN_SUBMIT_CLAIM);

        }

        private List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "#claimNumberDisplay span",
               "#policy-card-content-policy-details-header-title-summary",
               "#policy-card-content-policy-details-header-subtitle-summary",
               "#policy-card-content-policy-details-property-0-policy-number-summary",
               "#claim-summary-date-of-damage-text",
               "#claim-summary-glass-repair-text",
               "#claim-summary-glass-damage-text-0"
          };


    }
}