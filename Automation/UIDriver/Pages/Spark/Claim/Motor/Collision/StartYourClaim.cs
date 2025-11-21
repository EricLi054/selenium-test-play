using System;
using System.Collections.Generic;
using Rac.TestAutomation.Common;


namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class StartYourClaim : SparkStartYourClaim
    {
        #region CONSTANTS
        private class Constant
        {
            public static readonly string ActiveStepperLabel = "Start your claim";
            public static readonly string ApproximateTimeLabel = "Approximate time";

            public class Message
            {
                public static readonly string PolicyNotActive = "Unfortunately, your policy wasn't active on this dateIf you'd like to discuss this or if you think there's been a mistake, call us on 13 17 03.";
                public static readonly string PolicyNotCover = "Sorry, you've hit a road blockPlease call us on 13 17 03 to make a claim.";
                public static string DuplicateClaim(string claimNumber) => $"This policy has an existing car insurance claim for this dateTo continue this claim, please call 13 17 03 and quote the previous claim number {claimNumber}.";
                public static string SimilarClaim(string date, string claimNumber) => $"You've already made a claim for a similar dateThat claim was for car damage on {date}.To continue that claim or ask about it, please call 13 17 03 and quote claim number {claimNumber}.Or make a new claim";
                public static readonly string CantClaimOnline = "Sorry, you can't claim onlinePlease call us on 13 17 03 so we can help you with your claim.";
            }
        }
        

        #endregion

        #region XPATHS
        private new class XPath : SparkStartYourClaim.XPath
        {
            public class Label
            {
                public static readonly string ApproximateTime = "//label[text()='Approximate time']";
            }

            public class Dialog
            {
                public static readonly string SimilarClaim = "//div[@aria-labelledby='similar-claim-confirmation-dialog-title']";
            }

            public class PolicyCard
            {              
                public static string CarMakeAndModel(string policyNumber) => $"(id('policy-card-content-policy-details-header-title-{policyNumber}'))";
                public static string CarRego(string policyNumber) => $"(id('policy-card-content-policy-details-header-subtitle-{policyNumber}'))";
                public static readonly string PolicyNumber = "//p[contains(@id,'policy-card-content-policy-details-property-0-policy-number-')]";
            }
        }

        #endregion

        #region Settable properties and controls

        public string CarMakeAndModel => GetInnerText(XPath.PolicyCard.CarMakeAndModel(PolicyNumber));

        public string CarRego => GetInnerText(XPath.PolicyCard.CarRego(PolicyNumber));

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


    public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constant.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.HeaderText, GetInnerText(XPath.Header), "Header text on Let's start your claim page");
            Reporting.AreEqual(Constants.DateOfAccidentLabel, GetInnerText(XPath.Field.Label.DateOfAccident), "Date of the accident label");
            Reporting.AreEqual(Constant.ApproximateTimeLabel, GetInnerText(XPath.Label.ApproximateTime), "Approximate Time label");
            VerifyMotorClaimOnlineNotificationCard();
        }

        public void VerifyCarModel(ClaimCar claim)
        {
            Reporting.AreEqual($"{claim.Policy.Vehicle.Year} {claim.Policy.Vehicle.Make}", CarMakeAndModel, "expected Car Make and Model details with the value displayed");

            if (DataHelper.IsRegistrationNumberConsideredValid(claim.Policy.Vehicle.Registration))
            {
                Reporting.AreEqual(claim.Policy.Vehicle.Registration.Replace(" ", ""), CarRego.Replace(" ", ""), "expected Car Registration Number with the value displayed");
            }
            Reporting.AreEqual(claim.Policy.PolicyNumber, PolicyNumber, "expected Policy number with the value displayed");
        }          

        /// <summary>
        /// Select the event time using the pop-up time picker interface.
        /// </summary>
        public void SelectEventTime(DateTime dateTime)
        {
            if (DataHelper.RandomNumber(0, 2) == 1)
            {
                InputTimeAsText(dateTime); 
            }
            else
            { 
                SelectTimeFromTimePicker(dateTime); 
            }
        }

        /// <summary>
        /// Verify motor policy is not active error message
        /// </summary>
        public void VerifyPolicyNotActiveErrorMessage()
        {
            Reporting.Log("Policy not active error message", _driver.TakeSnapshot());
            Reporting.AreEqual(Constant.Message.PolicyNotActive, GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "Policy not active error message");
        }

        /// <summary>
        /// Verify policy not cover error message
        /// </summary>
        public void VerifyPolicyNotCoverErrorMessage()
        {
            Reporting.Log("Policy not cover error message", _driver.TakeSnapshot());
            Reporting.AreEqual(Constant.Message.PolicyNotCover, GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "Policy not cover error message");
        }


        /// <summary>
        /// Verify duplicate claim error message
        /// </summary>
        public void VerifyDuplicateClaimErrorMessage(string claimNumber)
        {
            Reporting.Log("Duplicate claim error message", _driver.TakeSnapshot());
            Reporting.AreEqual(Constant.Message.DuplicateClaim(claimNumber), GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "Duplicate claim error message");
        }
        
        /// <summary>
        /// Verify similar claim warning message
        /// </summary>
        public void VerifySimilarClaimErrorMessage(string date, string claimNumber)
        {
            Reporting.Log("Similar claim message", _driver.TakeSnapshot());
            Reporting.AreEqual(Constant.Message.SimilarClaim(date, claimNumber), GetInnerText(XPath.Dialog.SimilarClaim).StripLineFeedAndCarriageReturns(false), "Similar claim message");
        }

        /// <summary>
        /// Verify Can't claim online error message
        /// </summary>
        public void VerifyCantClaimOnlineErrorMessage()
        {
            Reporting.Log("Sotty you can't claim online error message", _driver.TakeSnapshot());
            Reporting.AreEqual(Constant.Message.CantClaimOnline, GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "Can't claim online error message");
        }

        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "#policy-card-content-policy-details-header-title-policy-0",
               "#policy-card-content-policy-details-header-subtitle-policy-0",
               "#policy-card-content-policy-details-property-0-policy-number-policy-0"
          };
    }
}
