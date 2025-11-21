using Rac.TestAutomation.Common;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.Motor.Glass
{
    public class StartYourClaim : SparkStartYourClaim
    {
        #region XPATHS
        protected new class XPath
        {
            public static readonly string Header = "id('start-claim-header')";

            public class PolicyCard
            {
                public static readonly string CarMakeAndModel = "//h3[starts-with(@id,'policy-card-content-policy-details-header-title-')]";
                public static readonly string CarRego         = "//h4[starts-with(@id,'policy-card-content-policy-details-header-subtitle-')]";
                public static readonly string PolicyNumber    = "//p[starts-with(@id,'policy-card-content-policy-details-property-0-policy-number-')]";
            }
        }

        #endregion

        #region Settable properties and controls

        public string CarMakeAndModel => GetInnerText(XPath.PolicyCard.CarMakeAndModel);

        public string CarRego => GetInnerText(XPath.PolicyCard.CarRego);

        /// <summary>
        /// Extract policy number from "Policy number: MGPXXXXXXXXX"
        /// </summary>
        public string PolicyNumber
        {
            get
            {
                var str = GetInnerText(XPath.PolicyCard.PolicyNumber);
                return str.Substring(15);
            }
        }

        #endregion

        public StartYourClaim(Browser browser) : base(browser)
        { }

        public void VerifyCarModel(ClaimCar claim)
        {
            Reporting.AreEqual($"{claim.Policy.Vehicle.Year} {claim.Policy.Vehicle.Make}", CarMakeAndModel, "expected Car Make and Model details with the value displayed");

            if (DataHelper.IsRegistrationNumberConsideredValid(claim.Policy.Vehicle.Registration))
            {
                Reporting.AreEqual(claim.Policy.Vehicle.Registration, CarRego, "expected Car Registration Number with the value displayed");
            }
            Reporting.AreEqual(claim.Policy.PolicyNumber, PolicyNumber, "expected Policy number with the value displayed");
        }

        public void CapturePercyScreenShot()
        {
            _browser.PercyScreenCheck(ClaimMotorGlass.StartYourClaim, GetPercyIgnoreCSS());
            ClickNext();
            _browser.PercyScreenCheck(ClaimMotorGlass.StartYourClaimErrorPage, GetPercyIgnoreCSS());
        }

        private List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "[id^='policy-card-content-policy-details-header-title-']",
               "[id^='policy-card-content-policy-details-header-subtitle-']",
               "[id^='policy-card-content-policy-details-property-0-policy-number-']"
          };
    }
}