using NUnit.Framework.Internal;
using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIDriver.Pages.Spark.MemberRefund
{
    public class LetsGetSomeInformation : BaseMemberRefund
    {
        #region CONSTANTS
        private static new class Constants
        {

            public static class Label
            {
                public static readonly string RefundID = "Reimbursement ID\r\nYou can find this on your reimbursement letter";
                public static readonly string Dob = "What is your date of birth?";
                public static readonly string LastName = "Last name";

                public static readonly string RefundIDWarning = "Please enter a valid Reimbursement ID";
                public static readonly string DobWarning = "Please enter your date of birth";
                public static readonly string LastNameWarning = "Please enter a valid last name";
            }
        }
        #endregion

        #region XPATHS
        private static new class XPath
        {
            public static readonly string PageHeading = "//h2[text()=\"Let's get some information\"]";
            public static readonly string DatePicker = "id('meo-mid-term-endorsement-start-date')";

            public static class Label
            {
                public static readonly string RefundID = "//label[@id='label-refund-id-input']";
                public static readonly string Dob = "//label[@id='label-undefined']";
                public static readonly string LastName = "//label[@id='label-lastName']";

                public static readonly string RefundIDWarning = XPath.Label.RefundID+ "/../p";
                public static readonly string DobWarning = XPath.Label.Dob + "/../p";
                public static readonly string LastNameWarning = XPath.Label.LastName + "/../p";
            }

            public static class Input
            {
                public static readonly string RefundID = "id('refund-id-input')";
                public static readonly string Dob = "id('enter-details-dob')";
                public static readonly string LastName = "id('lastName')";
            }

            public static class Button
            {
                public static readonly string Next = "id('enter-details-submit')";
            }
        }
        #endregion

        public LetsGetSomeInformation(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);
                Reporting.Log($"Let's get some information page is Displayed");
                GetElement(XPath.Button.Next);
                Reporting.Log($"Next buttons is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Let's get some information");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Checking the label for the data entry fields
        /// </summary>
        public void VerifyUIFieldLabels()
        {
            Reporting.AreEqual(Constants.Label.RefundID, GetInnerText(XPath.Label.RefundID), $"the {Constants.Label.RefundID} is displayed on the page.");
            Reporting.AreEqual(Constants.Label.Dob, GetInnerText(XPath.Label.Dob), $"the {Constants.Label.Dob} is displayed on the page.");
            Reporting.AreEqual(Constants.Label.LastName, GetInnerText(XPath.Label.LastName), $"the {Constants.Label.LastName} is displayed on the page.");
        }

        /// <summary>
        /// Checking the active stepper on the current page
        /// </summary>
        public override void VerifyActiveStepper()
        {
            Reporting.IsTrue(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterRefund).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterRefund} is enabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperOTP).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperOTP} is disabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterBankDetails).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterBankDetails} is disabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperConfirmation).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperConfirmation} is disabled on the page.");
        }

        /// <summary>
        /// Entering refund id, dob and last name data supplied as part of test data
        /// </summary>
        public void EnterRefundDetails(RefundDetails testData)
        {
            string dob = testData.Dob.Date.ToString("dd/MM/yyyy");

            Reporting.Log($"Inputting '{Constants.Label.RefundID}': {testData.RefundID}");
            WaitForTextFieldAndEnterText(XPath.Input.RefundID, testData.RefundID, false);

            Reporting.Log($"Inputting '{Constants.Label.Dob}': {dob}");
            WaitForTextFieldAndEnterText(XPath.Input.Dob, dob, false);

            Reporting.Log($"Inputting '{Constants.Label.LastName}': {testData.LastName}");
            WaitForTextFieldAndEnterText(XPath.Input.LastName, testData.LastName, false);

            Reporting.Log($"Capturing populated fields before continuing.", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Checking the warning messages for Refund id, dob and last name fields
        /// </summary>
        public void VerifyWarningTextForMandatoryFields()
        {
            Reporting.Log($"Capturing state of the screen before continuing.", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.Label.RefundIDWarning, GetInnerText(XPath.Label.RefundIDWarning), $"the {Constants.Label.RefundIDWarning} warning text is displayed on the page.");
            Reporting.AreEqual(Constants.Label.DobWarning, GetInnerText(XPath.Label.DobWarning), $"the {Constants.Label.DobWarning} warning text is displayed on the page.");
            Reporting.AreEqual(Constants.Label.LastNameWarning, GetInnerText(XPath.Label.LastNameWarning), $"the {Constants.Label.LastNameWarning} warning text is displayed on the page.");
        }

        /// <summary>
        /// Select the "Next" button to navigate to the next page in the flow.
        /// </summary>
        public void ClickNext() => ClickControl(XPath.Button.Next);
    }
}
