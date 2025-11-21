using OpenQA.Selenium;
using UIDriver.Pages.Spark.CaravanQuote;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace Rac.TestAutomation.Common
{
    abstract public class SparkPersonalInformationPage : SparkBasePage
    {
        #region XPATHS
        public static class XPathPersonalInfo
        {
            public static readonly string Header = FORM + "//h2[contains(text(),'Tell us more about you')]";
            public static class Policyholder
            {
                public static class Personal
                {
                    // Personal information of policyholders
                    public static readonly string GenderMale            = "//button[@data-testid='maleButton' or @data-testid='toggle-button-male' " +
                                                                          "or @id='toggle-button-group-gender-Male']";
                    public static readonly string GenderFemale          = "//button[@data-testid='femaleButton' or @data-testid='toggle-button-female' " +
                                                                          "or @id='toggle-button-group-gender-Female']";
                    public static readonly string TitleMr               = "//div[contains(@id,'title')]/button[text()='Mr']";
                    public static readonly string TitleMrs              = "//div[contains(@id,'title')]/button[text()='Mrs']";
                    public static readonly string TitleMiss             = "//div[contains(@id,'title')]/button[text()='Miss']";
                    public static readonly string TitleMs               = "//div[contains(@id,'title')]/button[text()='Ms']";
                    public static readonly string TitleMx               = "//div[contains(@id,'title')]/button[text()='Mx']";
                    public static readonly string TitleDr               = "//div[contains(@id,'title')]/button[text()='Dr']";
                    public static readonly string TitleButtonSelected   = "//div[@id='titleButtonGroup']/button[contains(@class,'Mui-selected')]/span";
                    public static readonly string MailingAddress        = FORM + "//input[@id='mailingAddress-input']";
                    public static readonly string FirstAddress          = "id('mailingAddress-input-listbox')/li[not(contains(text(),'Loading'))]";
                    public static readonly string FirstAddressPopupDs   = "id('mailingAddress-input-field-popup')/li[not(contains(text(),'Loading'))]";
                    public static readonly string FirstNameInput        = "id('firstName')";
                    public static readonly string LastNameInput         = "id('lastName')";
                    public static readonly string ContactNumberInput    = "id('contactNumber')";
                    public static readonly string EmailInput            = "id('email')";
                }
                public static class Details
                {
                    // Policyholder details for Single Match, Multi Match and No Match
                    public static readonly string MailingAddressLabel   = FORM + "//label[contains(text(),'Mailing address') or contains(text(),'mailing address')]";
                    // Policyholder details for Multi Match and No Match
                    public static readonly string TitleLabel            = FORM + "//label[text()='Title']";
                    public static readonly string TitleButtonGroup      = FORM + "//div[@id='toggle-button-group-title' or @id='titleButtonGroup']";
                    public static readonly string GenderLabel           = FORM + "//label[text()='What gender do you most identify with?' or text()='Gender']";
                    public static readonly string GenderButtonGroup     = FORM + "//div[@id='toggle-button-group-gender' or @name='gender']";
                    public static readonly string LastNameLabel         = FORM + "//label[text()='Last name']";
                    // Policyholder details for No Match
                    public static readonly string FirstNameLabel        = FORM + "//label[text()='First name']";
                    public static readonly string ContactNumberLabel    = FORM + "//label[text()='Contact number']";
                    public static readonly string EmailLabel            = FORM + "//label[text()='Email']";
                }
            }
            public static class Buttons
            {
                public static readonly string Next = FORM + "//button[@data-testid='submit']";
            }
        }

        #endregion

        public SparkPersonalInformationPage(Browser browser) : base(browser) { }

        public string FirstName
        {
            get => GetValue(XPathPersonalInfo.Policyholder.Personal.FirstNameInput);

            set => WaitForTextFieldAndEnterText(XPathPersonalInfo.Policyholder.Personal.FirstNameInput, value, false);
        }

        public string LastName
        {
            get => GetValue(XPathPersonalInfo.Policyholder.Personal.LastNameInput);

            set => WaitForTextFieldAndEnterText(XPathPersonalInfo.Policyholder.Personal.LastNameInput, value, false);
        }

        public string ContactNumber
        {
            get => GetValue(XPathPersonalInfo.Policyholder.Personal.ContactNumberInput);

            set => WaitForTextFieldAndEnterText(XPathPersonalInfo.Policyholder.Personal.ContactNumberInput, value, false);
        }

        public string Email
        {
            get => GetValue(XPathPersonalInfo.Policyholder.Personal.EmailInput);

            set => WaitForTextFieldAndEnterText(XPathPersonalInfo.Policyholder.Personal.EmailInput, value, false);
        }

        public Gender Gender
        {
            get
            {
                if (GetElement(XPathPersonalInfo.Policyholder.Personal.GenderMale).GetAttribute("Pressed").Equals(true))
                { return Gender.Male; }
                if (GetElement(XPathPersonalInfo.Policyholder.Personal.GenderFemale).GetAttribute("Pressed").Equals(true))
                { return Gender.Female; }
                throw new InvalidSelectorException("No Gender has been selected");
            }
            set
            {
                switch (value)
                {
                    case Gender.Male:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.GenderMale);
                        break;
                    case Gender.Female:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.GenderFemale);
                        break;
                    default:
                        Reporting.Error("Caller passed an unsupported value for Gender");
                        break;
                }
            }
        }

        public Title Title
        {
            get => DataHelper.GetValueFromDescription<Title>(GetInnerText(XPathPersonalInfo.Policyholder.Personal.TitleButtonSelected));

            set
            {
                switch (value)
                {
                    case Title.Dr:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleDr);
                        break;
                    case Title.Miss:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleMiss);
                        break;
                    case Title.Mr:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleMr);
                        break;
                    case Title.Mrs:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleMrs);
                        break;
                    case Title.Ms:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleMs);
                        break;
                    case Title.Mx:
                        ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleMx);
                        break;
                    default:
                        Reporting.Error("Select a valid value for Title");
                        break;
                }
            }
        }

        public void DisplayGenderField()
        {
            ClickControl(XPathPersonalInfo.Policyholder.Personal.TitleMx);
        }

        protected string MailingAddress
        {
            get => GetValue(XPathPersonalInfo.Policyholder.Personal.MailingAddress);

            set => SetMailingAddress(value);
        }

        protected void SetMailingAddress(string value)
        {
            QASSearchForAddress(XPathPersonalInfo.Policyholder.Personal.MailingAddress, XPathPersonalInfo.Policyholder.Personal.FirstAddress, value);
        }

        protected void SetTitleWithGender(Contact policyHolder)
        {
            Reporting.Log($"Desired value for title is '{policyHolder.Title}'");
            Title = policyHolder.Title;

            //Ask user to enter Gender only if the Title (Dr or Mx) cannot imply the Gender. 'Miss', 'Mrs' and 'Ms' maps to Female and 'Mr' maps to Male.
            if (policyHolder.Title == Title.Dr || policyHolder.Title == Title.Mx)
            {
                _driver.WaitForElementToBeVisible(By.XPath(XPathPersonalInfo.Policyholder.Details.GenderButtonGroup), WaitTimes.T5SEC);
                Reporting.IsTrue(IsControlDisplayed(XPathPersonalInfo.Policyholder.Details.GenderButtonGroup) 
                    && IsControlDisplayed(XPathPersonalInfo.Policyholder.Details.GenderLabel), 
                    "Tell us more about you page displays 'Gender button group' and 'Gender label' since Title could not imply the gender");
                Gender = policyHolder.Gender;
                Reporting.Log($"Gender set to '{policyHolder.Gender}' from test data.");
            }
        }

        public void VerifyAnyPremiumChangePopup(QuoteCaravan quoteData)
        {
            using (var premiumChangePopup = new PremiumChangePopup(_browser))
            {
                premiumChangePopup.WaitForPremiumChangePopup(PremiumChangeTrigger.MEMBER_MATCH, quoteData);
                premiumChangePopup.VerifyPopupContent(PremiumChangeTrigger.MEMBER_MATCH, quoteData);
                premiumChangePopup.VerifyPremiumChange(_browser, quoteData, SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO);
            }
        }

        public void  VerifyNoPremiumPopupIsDisplayed()
        {
            using (var premiumChangePopup = new PremiumChangePopup(_browser))
            {             
                Reporting.IsFalse(premiumChangePopup.IsDisplayed(), "Premium change pop-up is not expected on this scenario");
            }
        }

        protected void ClickNext()
        {
            Reporting.Log("Capturing screen before selecting Next :", _browser.Driver.TakeSnapshot());
            ClickControl(XPathPersonalInfo.Buttons.Next);
        }
    }
}
