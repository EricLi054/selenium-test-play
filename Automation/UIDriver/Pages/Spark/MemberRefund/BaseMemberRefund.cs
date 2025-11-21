using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIDriver.Pages.Spark.MemberRefund
{
    abstract public class BaseMemberRefund : SparkBasePage
    {
        #region CONSTANTS
        public static class Constants
        {
            public static class Label
            {
                public static readonly string StepperEnterRefund = "Identify your reimbursement";
                public static readonly string StepperOTP = "Verify it's you";
                public static readonly string StepperEnterBankDetails = "Enter your bank details";
                public static readonly string StepperConfirmation = "Confirmation";
            }
        }
        #endregion

        #region XPATH
        public static class XPath
        {
            public static class Label
            {
                public static readonly string StepperEnterRefund = "id('identify-your-reimbursement-step')";
                public static readonly string StepperOTP = "//button[contains(@id,'verify-it') and contains(@id,'s-you-step')]";
                public static readonly string StepperEnterBankDetails = "id('enter-your-bank-details-step')";
                public static readonly string StepperConfirmation = "id('confirmation-step')";
            }
        }
        #endregion

        protected BaseMemberRefund(Browser browser) : base(browser) { }

        /// <summary>
        /// Checking the active stepper on the current page
        /// </summary>
        public abstract void VerifyActiveStepper();

        public void VerifyStepperName()
        {
            Reporting.AreEqual(Constants.Label.StepperEnterRefund, GetInnerText(XPath.Label.StepperEnterRefund), $"the expected stepper {Constants.Label.StepperEnterRefund} against the value displayed on the page.");
            Reporting.AreEqual(Constants.Label.StepperOTP, GetInnerText(XPath.Label.StepperOTP), $"the expected stepper {Constants.Label.StepperOTP} against the value displayed on the page.");
            Reporting.AreEqual(Constants.Label.StepperEnterBankDetails, GetInnerText(XPath.Label.StepperEnterBankDetails), $"the expected stepper {Constants.Label.StepperEnterBankDetails} against the value displayed on the page.");
            Reporting.AreEqual(Constants.Label.StepperConfirmation, GetInnerText(XPath.Label.StepperConfirmation), $"the expected stepper {Constants.Label.StepperConfirmation} against the value displayed on the page.");
        }
    }
}
