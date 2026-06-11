using OpenQA.Selenium;
using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class TellUsMoreAboutYou : SparkPersonalInformationPage
    {
        #region XPATHS

        public static class XPath
        {
            public static class StepperLabels
            {
                public const string StorageAndUse = "id('storage-and-use-step')";
            }

            public static class MatchedMember
            {
                public const string ConfirmAddressLabel = FORM + "//label[contains(text(),'Please confirm your mailing address')]";
            }

            public static class DuplicateAlert
            {
                public const string Base    = "/html/body/div[starts-with(@class,'k-widget k-window')]";
                public const string Title   = Base + "//span[@id='simple-dialog_wnd_title']";
                public const string Content = Base + "//div[@id='simple-dialog']";
                public const string Close   = Base + "//div[@class='cluetip-close']/a";
            }
        }

        #endregion

        #region Settable properties and controls

        public new string MailingAddress
        {
            get => GetValue(XPathPersonalInfo.Policyholder.Personal.MailingAddress);

            set
            {
                if (MailingAddress != value)
                { SetMailingAddress(value); }
            }
        }

        #endregion

        public TellUsMoreAboutYou(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPathPersonalInfo.Header);
                GetElement(XPathPersonalInfo.Buttons.Next);
                GetElement(XPathPersonalInfo.Policyholder.Details.MailingAddressLabel);
                GetElement(XPathPersonalInfo.Policyholder.Personal.MailingAddress);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Tell Us More About You");
            return true;
        }

        public void VerifyPageContent(Contact policyHolder, RetrieveQuoteType? retrieveQuote)
        {
            //When member single matched upfront
            if (policyHolder.MemberMatchRule != MemberMatchRule.None && !policyHolder.IsMultiMatchRSAMember && !policyHolder.SkipDeclaringMembership && !retrieveQuote.HasValue)
            {
                bool valid = IsControlDisplayed(XPath.MatchedMember.ConfirmAddressLabel) && IsControlDisplayed(XPathPersonalInfo.Policyholder.Personal.MailingAddress) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.LastNameLabel, XPathPersonalInfo.Policyholder.Personal.LastNameInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.FirstNameLabel, XPathPersonalInfo.Policyholder.Personal.FirstNameInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.MiddleNameLabel, XPathPersonalInfo.Policyholder.Personal.MiddleNameInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.ContactNumberLabel, XPathPersonalInfo.Policyholder.Personal.ContactNumberInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.EmailLabel, XPathPersonalInfo.Policyholder.Personal.EmailInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.TitleLabel, XPathPersonalInfo.Policyholder.Details.TitleButtonGroup);

                Reporting.IsTrue(valid, "Tell us more about you page displays only the labels and fields applicable only for single match users");
            }
            //When member multi matched upfront
            else if (policyHolder.IsMultiMatchRSAMember && !policyHolder.SkipDeclaringMembership && !retrieveQuote.HasValue)
            {
                bool valid = IsMultiMatchFormShown() &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.FirstNameLabel, XPathPersonalInfo.Policyholder.Personal.FirstNameInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.ContactNumberLabel, XPathPersonalInfo.Policyholder.Personal.ContactNumberInput) &&
                             IsHidden(XPathPersonalInfo.Policyholder.Details.EmailLabel, XPathPersonalInfo.Policyholder.Personal.EmailInput);

                Reporting.IsTrue(valid, "Tell us more about you page displays only the labels and fields applicable only for multi match users");
            }
            else
            {
                bool valid = IsMultiMatchFormShown() &&
                             IsShown(XPathPersonalInfo.Policyholder.Details.FirstNameLabel, XPathPersonalInfo.Policyholder.Personal.FirstNameInput) &&
                             IsShown(XPathPersonalInfo.Policyholder.Details.MiddleNameLabel, XPathPersonalInfo.Policyholder.Personal.MiddleNameInput) &&
                             IsShown(XPathPersonalInfo.Policyholder.Details.ContactNumberLabel, XPathPersonalInfo.Policyholder.Personal.ContactNumberInput) &&
                             IsShown(XPathPersonalInfo.Policyholder.Details.EmailLabel, XPathPersonalInfo.Policyholder.Personal.EmailInput);

                Reporting.IsTrue(valid, "Tell us more about you page displays all the labels and fields applicable for no match users");
            }
        }

        private bool IsShown(string labelXPath, string inputXPath) =>
            IsControlDisplayed(labelXPath) && IsControlDisplayed(inputXPath);

        private bool IsHidden(string labelXPath, string inputXPath) =>
            !IsControlDisplayed(labelXPath) && !IsControlDisplayed(inputXPath);

        private bool IsMultiMatchFormShown() =>
            IsShown(XPathPersonalInfo.Policyholder.Details.MailingAddressLabel, XPathPersonalInfo.Policyholder.Personal.MailingAddress) &&
            IsShown(XPathPersonalInfo.Policyholder.Details.TitleLabel, XPathPersonalInfo.Policyholder.Details.TitleButtonGroup) &&
            IsShown(XPathPersonalInfo.Policyholder.Details.LastNameLabel, XPathPersonalInfo.Policyholder.Personal.LastNameInput);

        public void FillPersonalInformation(Contact policyHolder)
        {
            if (policyHolder.IsMultiMatchRSAMember && !policyHolder.SkipDeclaringMembership)
            {
                SetTitleWithGender(policyHolder);
                LastName = policyHolder.Surname;
            }
            else if ((policyHolder.MemberMatchRule == MemberMatchRule.None) || policyHolder.SkipDeclaringMembership)
            {
                SetTitleWithGender(policyHolder);
                FirstName = policyHolder.FirstName;
                if (!string.IsNullOrEmpty(policyHolder.MiddleName))
                {
                    MiddleName = policyHolder.MiddleName;
                }
                LastName = policyHolder.Surname;
                ContactNumber = policyHolder.MobilePhoneNumber;
                Email = policyHolder.PrivateEmail.Address;
            }

            MailingAddress = policyHolder.MailingAddress.StreetSuburbState();

            ClickNext();

            using (var spinner = new SparkSpinner(_browser))
                spinner.WaitForSpinnerToFinish();
            
            // After spinner finishes, duplicate alert may appear (takes precedence over premium popup)
            // Check for it here so caller can handle it appropriately
            // Note: This check is non-blocking - if alert doesn't appear, execution continues normally
        }
        public bool IsDuplicateAlertVisible()
        {
            IWebElement dialogTitle;
            return _driver.TryWaitForElementToBeVisible(By.XPath(XPath.DuplicateAlert.Title), WaitTimes.T10SEC, out dialogTitle);
        }

        public string GetDuplicateAlertTitle()
        {
            return GetInnerText(XPath.DuplicateAlert.Title);
        }

        public string GetDuplicateAlertContent()
        {
            return GetInnerText(XPath.DuplicateAlert.Content);
        }

        public void CloseDuplicateAlertDialog()
        {
            ClickControl(XPath.DuplicateAlert.Close);
        }

        public void ClickStorageAndUseStep()
        {
            ClickControl(XPath.StepperLabels.StorageAndUse);
        }
    }
}