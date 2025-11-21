using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class TellUsMoreAboutYourJointPH : SparkPersonalInformationPage
    {
        #region XPATHS
        public static class XPath
        {
            public static readonly string Header                    = FORM + "//h2[contains(text(),'Tell us more about your joint policyholder')]";
            public static readonly string SameMailingAddrLabel      = FORM + "//legend[contains(text(),'Is their mailing address the same as yours')]";
            public static readonly string SameMailAddrButtonGroup   = FORM + "//div[@id='input-mailAddressSameAsPrimary']";
            public static readonly string SameMailAddrNoButton      = SameMailAddrButtonGroup + "/button[@value='no']";
        }

        #endregion

        public TellUsMoreAboutYourJointPH(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPathPersonalInfo.Policyholder.Details.TitleLabel);
                GetElement(XPathPersonalInfo.Policyholder.Details.TitleButtonGroup);
                GetElement(XPathPersonalInfo.Policyholder.Details.GenderLabel);
                GetElement(XPathPersonalInfo.Policyholder.Details.GenderButtonGroup);
                GetElement(XPathPersonalInfo.Policyholder.Details.FirstNameLabel);
                GetElement(XPathPersonalInfo.Policyholder.Personal.FirstNameInput);
                GetElement(XPathPersonalInfo.Policyholder.Details.LastNameLabel);
                GetElement(XPathPersonalInfo.Policyholder.Personal.LastNameInput);
                GetElement(XPathPersonalInfo.Policyholder.Details.ContactNumberLabel);
                GetElement(XPathPersonalInfo.Policyholder.Personal.ContactNumberInput);
                GetElement(XPathPersonalInfo.Policyholder.Details.EmailLabel);
                GetElement(XPathPersonalInfo.Policyholder.Personal.EmailInput);
                GetElement(XPath.SameMailingAddrLabel);
                GetElement(XPath.SameMailAddrButtonGroup);
                GetElement(XPathPersonalInfo.Buttons.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Tell Us More About Your Joint PH");
            return true;
        }

        /// <summary>
        /// Answers all the joint policyholder personal information questions on the 
        /// 'Tell us more about your joint policyholder' page.
        /// We always answer 'No' to the 'Is their mailing address the same as yours' question.
        /// </summary>
        /// <param name="jointPolicyHolder"></param>
        public void FillPersonalInformation(Contact jointPolicyHolder)
        {
            SetTitleWithGender(jointPolicyHolder);
            FirstName = jointPolicyHolder.FirstName;
            LastName = jointPolicyHolder.Surname;
            ContactNumber = jointPolicyHolder.MobilePhoneNumber;
            Email = jointPolicyHolder.PrivateEmail.Address;
            ClickControl(XPath.SameMailAddrNoButton);

            MailingAddress = jointPolicyHolder.MailingAddress.StreetSuburbState();

            ClickNext();

            using (var spinner = new SparkSpinner(_browser))
            { spinner.WaitForSpinnerToFinish(); }
        }
    }
}