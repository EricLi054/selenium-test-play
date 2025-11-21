using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.MemberRefund
{
    public class BankAccountDetails : BaseMemberRefund
    {
        #region CONSTANTS
        private static new class Constants
        {
            public static readonly string PageHeading = "Enter your bank details";

            public static class Label
            {
                public static readonly string Amount = "You are entitled to a reimbursement of $";

                public static readonly string AmountIncludeText = "This is inclusive of all applicable taxes and interest. " +
                                                                    "For more details please refer to our correspondence containing your reimbursement ID.";
                public static readonly string ProvideBankDetailsText = "Please provide your current bank account details to receive your reimbursement";

                public static readonly string BSB = "BSB";
                public static readonly string AccountNumber = "Account number";
                public static readonly string AccountName = "Account name";
            }

            public static class Warning
            {
                public static readonly string BSB = "Please enter a valid BSB";
                public static readonly string AccountNumber = "Please enter a valid account number";
                public static readonly string AccountName = "Please enter a valid account name";
            }

            public static class NotificationCard
            {
                public static readonly string Heading = "Check your bank details are correct";
                public static readonly string Content = "Please check that the bank details you've entered are correct as entering " +
                                                        "incorrect details may mean the wrong account is credited and " +
                                                         "it may not be possible to recover the funds. RAC Insurance is not responsible for loss caused by providing incorrect bank details.";
            }

        }
        #endregion

        #region XPATHS
        private static new class XPath
        {
            public static readonly string PageHeading = "//h2[text()='" + Constants.PageHeading + "']";

            public static class Label
            {
                public static readonly string Amount = "id('add-account-subtitle')";

                public static readonly string AmountIncludeText = "id('add-account-taxes-interest')";
                public static readonly string ProvideBankDetailsText = "id('add-account-provide-bank-details')";

                public static readonly string BSB = "//input[@id='bsb']/../../label";
                public static readonly string BSB_DETAILS_CHECKING = "//p[@id='bsb-message' and contains(text(),'Checking BSB')]";
                public static readonly string BANK_BSB_DETAILS = "id('bsb-message')";
                public static readonly string AccountNumber = "//input[@id='accountNumber']/../../label";
                public static readonly string AccountName = "//input[@id='accountName']/../../label";
            }

            public static class Warning
            {
                public static readonly string BSB = "id('bsb-message')";
                public static readonly string AccountNumber = XPath.Label.AccountNumber + "/../p";
                public static readonly string AccountName = XPath.Label.AccountName + "/../p";
            }

            public static class Input
            {
                public static readonly string BSB = "id('bsb')";
                public static readonly string AccountNumber = "id('accountNumber')";
                public static readonly string AccountName = "id('accountName')";
            }

            public static class NotificationCard
            {
                public static readonly string Heading = "id('add-account-check-your-bank-details-notification-title')";
                public static readonly string Content = "id('add-account-check-your-bank-details-notification-content')";
            }

            public static class Button
            {
                public static readonly string Next = "id('add-account-submit')";
            }
        }
        #endregion

        public BankAccountDetails(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);
                Reporting.Log($"Bank account details page is Displayed");
                GetElement(XPath.Button.Next);
                Reporting.Log($"Next buttons is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Bank account details");
            Reporting.Log($"Page 3: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Verify the static labels displayed on the page
        /// </summary>
        private void VerifyUILabels()
        {
            Reporting.AreEqual(Constants.Label.AmountIncludeText, GetInnerText(XPath.Label.AmountIncludeText), $"the taxes/charges text '{Constants.Label.AmountIncludeText}' matching");
            Reporting.AreEqual(Constants.Label.ProvideBankDetailsText, GetInnerText(XPath.Label.ProvideBankDetailsText), $"the bank details entry text '{Constants.Label.ProvideBankDetailsText}' matching");

            Reporting.AreEqual(Constants.Label.BSB, GetInnerText(XPath.Label.BSB), $"the '{Constants.Label.BSB}' label is displayed on the page");
            Reporting.AreEqual(Constants.Label.AccountName, GetInnerText(XPath.Label.AccountName), $"the '{Constants.Label.AccountName}' label is displayed on the page");
            Reporting.AreEqual(Constants.Label.AccountNumber, GetInnerText(XPath.Label.AccountNumber), $"the '{Constants.Label.AccountNumber}' label is displayed on the page");

            // Notification card
            Reporting.AreEqual(Constants.NotificationCard.Heading, GetInnerText(XPath.NotificationCard.Heading), $"the notification card '{Constants.NotificationCard.Heading}' label is displayed on the page");
            Reporting.AreEqual(Constants.NotificationCard.Content, GetInnerText(XPath.NotificationCard.Content), $"the notification card '{Constants.NotificationCard.Content}' label is displayed on the page");
        }

        /// <summary>
        /// Verify error message displayed for not filling the mandatory field bsb, account number and account name
        /// </summary>
        public void VerifyInputWarningMessages()
        {
            Reporting.AreEqual(Constants.Warning.BSB, GetInnerText(XPath.Warning.BSB), $"the '{Constants.Warning.BSB}' warning text is displayed on the page");
            Reporting.AreEqual(Constants.Warning.AccountName, GetInnerText(XPath.Warning.AccountName), $"the '{Constants.Warning.AccountName}'  warning text  is displayed on the page");
            Reporting.AreEqual(Constants.Warning.AccountNumber, GetInnerText(XPath.Warning.AccountNumber), $"the '{Constants.Warning.AccountNumber}'  warning text  is displayed on the page");
        }

        /// <summary>
        /// Verify the static labels displayed on the page & stepper state
        /// </summary>
        public void VerifyDetailsUiCheck()
        {
            VerifyUILabels();
            VerifyActiveStepper();
        }

        /// <summary>
        /// Verify the expected refund amount displayed on the papge
        /// </summary>
        /// <param name="amount">Refund Amount</param>
        public void VerifyRefundAmount(string amount)
        {
            Reporting.AreEqual($"{Constants.Label.Amount}"+amount, GetInnerText(XPath.Label.Amount), $"the Refund amount displayed on the page");
        }

        /// <summary>
        /// Entering bsb, account number and name data supplied as part of test data
        /// Also verify the bsb details displayed
        /// </summary>
        public void EnterBankAccountDetails(RefundDetails testData)
        {

            Reporting.Log($"Inputting '{Constants.Label.BSB}': {testData.RefundBankAmount.Bsb}");
            WaitForTextFieldAndEnterText(XPath.Input.BSB, testData.RefundBankAmount.Bsb, false);

            Reporting.Log($"Inputting '{Constants.Label.AccountNumber}': {testData.RefundBankAmount.AccountNumber}");
            WaitForTextFieldAndEnterText(XPath.Input.AccountNumber, testData.RefundBankAmount.AccountNumber, false);

            Reporting.Log($"Inputting '{Constants.Label.AccountName}': {testData.RefundBankAmount.AccountName}");
            WaitForTextFieldAndEnterText(XPath.Input.AccountName, testData.RefundBankAmount.AccountName, false);

            _driver.WaitForElementToBeInvisible(By.XPath(XPath.Label.BSB_DETAILS_CHECKING), WaitTimes.T30SEC);
            Reporting.AreEqual(testData.RefundBankAmount.BankBranchState, GetInnerText(XPath.Label.BANK_BSB_DETAILS),
                $"BSB details matches the expected text '{testData.RefundBankAmount.BankBranchState}' for the BSB nuumber '{testData.RefundBankAmount.Bsb}'." +
                $" Actual Result:'{ GetInnerText(XPath.Label.BANK_BSB_DETAILS)}'");

            Reporting.Log($"Capturing populated fields before continuing.", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Checking the active stepper on the current page
        /// </summary>
        public override void VerifyActiveStepper()
        {
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterRefund).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterRefund} is disabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperOTP).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperOTP} is disabled on the page.");
            Reporting.IsTrue(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterBankDetails).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterBankDetails} is enabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperConfirmation).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperConfirmation} is disabled on the page.");
        }

        /// <summary>
        /// Select the "Next" button to navigate to the next page in the flow.
        /// </summary>
        public void ClickNext() => ClickControl(XPath.Button.Next);
    }
}
