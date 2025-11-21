using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// Confirmation Page for Update How You Pay. Acknowledgement and summary of the 
    /// member's submission for updates related to payment source and instalment date.
    /// </summary>
    public class UpdateHowYouPayConfirmationPage : SparkBasePage
    {
        #region CONSTANTS
        public class Constants
        {
            public static readonly string Title = "Update done!";
            public static readonly string MotorDateChangePrefix = "You'll be paying your car insurance";
            public static readonly string HomeDateChangePrefix = "You'll be paying your home insurance";
            public static readonly string CaravanDateChangePrefix = "You'll be paying your caravan and trailer insurance";
            public static readonly string MotorCycleDateChangePrefix = "You'll be paying your motorcycle insurance";
            public static readonly string SourceSuffixCreditCard = " from a credit card.";
            public static readonly string SourceSuffixBankAccount = " from a bank account.";
            public static readonly string MyPoliciesLinkText = "Back to my policy";
        }
        #endregion

        #region XPATHS
        public class XPath
        {
            public class Confirmation
            {
                public static readonly string Container = "id('change-how-you-pay-successful-confirmation')";
                public static readonly string ThumbsUp = Container + "//*[local-name()='svg' and @data-icon='thumbs-up']";
                public static readonly string Title = "//h2[@id='change-how-you-pay-successful-confirmation-header' and contains(text(),'Update done!')]";
                public static readonly string Paragraph = "//div[@id='change-how-you-pay-successful-confirmation-message']/p";
                public static readonly string BackToPolicy = "id('racPcmLink')";
            }
        }
        #endregion

        public UpdateHowYouPayConfirmationPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Confirmation.Title);
                GetElement(XPath.Confirmation.Paragraph);
                GetElement(XPath.Confirmation.ThumbsUp);
                Reporting.LogPageChange("Spark Update How You Pay - Confirmation page");
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Check the confirmation page contains the right message in regards to the 
        /// product type, next payment date and the payment source (credit card vs bank account)
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="isNewPaymentBankAccount"></param>
        public void VerifyUpdateConfirmationMessage(EndorsementBase testData, bool isNewPaymentBankAccount)
        {
            var payment = isNewPaymentBankAccount ? "bank account" : "credit card";
            var suffixForPaymentSource = isNewPaymentBankAccount ? Constants.SourceSuffixBankAccount : Constants.SourceSuffixCreditCard;
            var prefixForProductType = string.Empty;
            var logMessage = string.Empty;

            if(testData.GetType() == typeof(EndorseCar))
            {
                prefixForProductType = Constants.MotorDateChangePrefix;
                logMessage = "car";
            } 
            else if (testData.GetType() == typeof(EndorseCaravan))
            {
                prefixForProductType = Constants.CaravanDateChangePrefix;
                logMessage = "caravan and trailer";
            } 
            else if (testData.GetType() == typeof(EndorseHome))
            {
                prefixForProductType = Constants.HomeDateChangePrefix;
                logMessage = "home";
            }
            else if (testData.GetType() == typeof(EndorseMotorCycle))
            {
                prefixForProductType = Constants.MotorCycleDateChangePrefix;
                logMessage = "motorcycle";
            }

            if (testData.PayMethod.IsAnnual && testData.PayMethod.IsPaymentByBankAccount)
            {
                Reporting.AreEqual($"{prefixForProductType}{suffixForPaymentSource}", GetInnerText(XPath.Confirmation.Paragraph),
                $"confirmation message contains includes {logMessage}, next instalment date and {payment} as payment source");
            }
            else
            {
                Reporting.AreEqual($"{prefixForProductType} on {testData.NextPaymentDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, CultureInfo.InvariantCulture)}{suffixForPaymentSource}",
                GetInnerText(XPath.Confirmation.Paragraph), $"confirmation message contains includes {logMessage}, next instalment date and {payment} as payment source");
            }
        }

        /// <summary>
        /// Confirm there is a link back to PCM and link contains the current policy for the member
        /// </summary>
        /// <param name="policyNumber"></param>
        public void VerifyBackToMyPolicy(string policyNumber)
        {
            Reporting.AreEqual(Constants.MyPoliciesLinkText, GetInnerText(XPath.Confirmation.BackToPolicy), "link text for return back to member's policies");
            Reporting.IsTrue(GetElement(XPath.Confirmation.BackToPolicy).GetAttribute("href").Contains($"?policyNumber={policyNumber}"),
                "link back to my policies include current policy in the url");
        }
    }
}
