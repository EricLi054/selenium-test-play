using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIDriver.Pages.Spark.MemberRefund
{
    public class Confirmation : BaseMemberRefund
    {
        #region CONSTANTS
        private static new class Constants
        {
            public static readonly string PageSubHeading = "A payment will be made into your bank account within 15 business days.\r\n" +
                                                            "If you haven't received it within this timeframe, " +
                                                            "please email us on insurancereimbursements@rac.com.au or call us on our dedicated reimbursement line - 1300 657 627.";
        }
        #endregion

        #region XPATHS
        private static new class XPath
        { 
            public static readonly string PageHeading = $"//h2[text()=\"You're all set!\"]";
            public static readonly string PageSubHeading = "id('confirmation-information')";

            public static class Button
            {
                public static readonly string BackToRACHomePage = "id('confirmation-go-to-rac-button')";
            }
        }
        #endregion

        public Confirmation(Browser browser) : base(browser) { }
        

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);
                Reporting.Log($"You're all set! confirmation page is Displayed");
                GetElement(XPath.Button.BackToRACHomePage);
                Reporting.Log($"Back To RAC Home Page buttons is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("You're all set!");
            Reporting.Log($"Page 3: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Checking the active stepper on the current page
        /// </summary>
        public override void VerifyActiveStepper()
        {
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterRefund).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterRefund} is disabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperOTP).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperOTP} is disabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterBankDetails).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterBankDetails} is disabled on the page.");
            Reporting.IsTrue(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperConfirmation).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperConfirmation} is enabled on the page.");
        }

        /// <summary>
        /// Verify the static labels displayed and buttons on the page
        /// </summary>
        public void VerifyUILabels()
        {
            Reporting.AreEqual(Constants.PageSubHeading, GetInnerText(XPath.PageSubHeading), $"the subtitle is displayed on the  page");
            Reporting.IsTrue(IsControlEnabled(XPath.Button.BackToRACHomePage), $"the 'Back to RAC homepage' is displyed and enabled on the page");
        }
    }
}
