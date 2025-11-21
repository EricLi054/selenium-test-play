using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Data;
using System.Threading;
using UIDriver.Helpers;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace UIDriver.Pages.Spark
{
    abstract public class SparkBeforeWeGetStarted : SparkBasePage
    {
        #region XPATHS
        public static class XPath
        {
            public static readonly string Header = "//h2[contains(text(),'Before we get started')]";
            public static readonly string FindMyDetails = "//button[@data-testid='submit']";
            public static readonly string HeaderPhoneNumber = "//a[@id='header-phone-button']";
            public static class FormLabel
            {
                public static readonly string AreYouRacMember = "//label[@data-testid='formlabelAreYouAnRACMember']";
            }

            public static class Member
            {
                public static readonly string MembershipLabel = "//span[text()='Member details']";
                public static readonly string AreYouRacMember = "id('areYouAMemberButtonGroup')";
                public static readonly string HelpText = "//div[@id='areYouAnRACMemberToolTip-message']//p";
                public static readonly string FirstName = "id('firstName')";
                public static readonly string FirstNameLabel = "id('firstName')/../../label";
                public static readonly string DateOfBirth = "id('dateOfBirth')";
                public static readonly string MobileNumber = "id('contactNumber')";
                public static readonly string MobileNumberLabel = "id('contactNumber')/../../label";
                public static readonly string Email = "id('email')";
                public static readonly string EmailLabel = "id('email')/../../label";
                public static readonly string BirthdayAnimation = "id('birthdayConfettiContainer')";
            }
            public static class Button
            {
                public static readonly string Yes = "//button[@data-testid='yesButton']";
                public static readonly string No = "//button[@data-testid='noButton']";
                public static readonly string Skip = "//button[@data-testid='skipButton']";
                public static readonly string Help = "//button[@data-testid='areYouAnRACMemberToolTipButton']";
                public static readonly string HelpClose = "//button[@aria-label='close']";
            }

            public static class CaravanSpecific
            {
                public readonly static string PhoneNumber = "//a[@id='InsurancePhoneNumberButtonId']";
                public static readonly string LeftPaneProductHeader = "//h1[text()='Caravan Insurance']";
                public static readonly string GiveFeedback = "//button[@id='feedback-button' and text()='Give feedback']";
            }
        }
        #endregion

        #region Constants
        public const string DISCOUNT_APPLIED       = "discount applied";
        private const string ARE_YOU_AN_RAC_MEMBER = "Are you an RAC member?\r\nWe'll use this to pre-fill your details and apply eligible discounts.";
        private const string HELP_TEXT = "If you've any of these RAC products you're a member: Roadside Assistance, Insurance, FREE2go, Life Insurance, Health Insurance, Pet Insurance, Finance, Security Monitoring or Rewards Membership.";
        #endregion

        #region Settable properties and controls
        public bool? AreYouAnRACMember
        {
            get
            {
                return GetNullableBinaryForTriStateToggle(XPath.Member.AreYouRacMember, XPath.Button.Yes, XPath.Button.No, XPath.Button.Skip, "Are you a member?");
            }
            set
            {
                ClickTriStateToggleWithNullableInput(XPath.Member.AreYouRacMember, XPath.Button.Yes, XPath.Button.No, XPath.Button.Skip, value);
            }
        }

        public string FirstName
        {
            get => GetValue(XPath.Member.FirstName);

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ClickControl(XPath.Member.FirstName);        // If no value just click in text box
                    ClickControl(XPath.Member.FirstNameLabel);  // Then click label to trigger blur event and force validation.
                }
                else
                    WaitForTextFieldAndEnterText(XPath.Member.FirstName, value, false);
            }
        }

        public string DateOfBirth
        {
            get => GetValue(XPath.Member.DateOfBirth);
 
            set
            {
                var dobfield = GetElement(XPath.Member.DateOfBirth);
                if (string.IsNullOrEmpty(value))
                    ClickControl(XPath.Member.DateOfBirth);  // If no value just click in text box
                else
                    // User typeAhead flag to use return key to trigger blur event
                    WaitForTextFieldAndEnterText(XPath.Member.DateOfBirth, value, true);
            }
        }

        public string MobileNumber
        {
            get => GetValue(XPath.Member.MobileNumber);

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ClickControl(XPath.Member.MobileNumber);        // If no value just click in text box
                    ClickControl(XPath.Member.MobileNumberLabel);  // Then click label to trigger blur event and force validation.
                }
                else
                    WaitForTextFieldAndEnterText(XPath.Member.MobileNumber, value, false);
            }
        }

        public string Email
        {
            get => GetValue(XPath.Member.Email);

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ClickControl(XPath.Member.Email);        // If no value just click in text box
                    ClickControl(XPath.Member.EmailLabel);  // Then click label to trigger blur event and force validation.
                }
                else
                    WaitForTextFieldAndEnterText(XPath.Member.Email, value, false);
            }
        }
        #endregion

        public SparkBeforeWeGetStarted(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Member.AreYouRacMember);
                GetElement(XPath.FormLabel.AreYouRacMember);
                GetElement(XPath.Button.Help);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Quote page 1 - Before We Get Started");

            return true;
        }

        /// <summary>
        /// Supports Spark version of B2C Caravan
        /// Verify different texts and label names of the
        /// 'Page 1: Before we get started' page
        /// </summary>
        public void VerifyGeneralPageContent()
        {
            Reporting.AreEqual(Header.Link.RACI_TELEPHONE_NUMBER, GetElement(XPath.CaravanSpecific.PhoneNumber).GetAttribute("href"), "Help telephone number at the top right hand corner");

            Reporting.AreEqual(ARE_YOU_AN_RAC_MEMBER, GetInnerText(XPath.FormLabel.AreYouRacMember), "Are you an RAC member. We'll use this to pre-fill your..... text");

            Reporting.IsTrue(GetClass(XPath.Member.MembershipLabel).Contains("active"), "Membership label on the left side is active");

            //Launch help test and verify the help text
            ClickControl(XPath.Button.Help);
            Reporting.AreEqual(HELP_TEXT, GetInnerText(XPath.Member.HelpText), "Help text when ? icon is clicked");
            ClickControl(XPath.Button.HelpClose);        
        }

        /// <summary>
        /// Supports Spark version of B2C Caravan
        /// Verify caravan specific texts and label names of the
        /// 'Page 1: Before we get started' page
        /// </summary>
        public void VerifyCaravanPageContent()
        {
            Reporting.AreEqual(true, GetElement(XPath.CaravanSpecific.LeftPaneProductHeader).Displayed, "Product name text on the top left side is visible");
            Reporting.AreEqual(true, GetElement(XPath.CaravanSpecific.GiveFeedback).Displayed, "Give Feedback button on the bottom left side is visible");            
        }

        /// <summary>
        /// Completes initial form (Before we get started: Are you an RAC member),
        /// when beginning Spark B2C motorcycle/caravan quote process.
        /// The method includes submitting the form to proceed to the next page.
        /// </summary>
        /// <param name="contact"></param>
        /// <exception cref="ReadOnlyException">This occurs if the "Find My Details" button could not be clicked.</exception>
        public void SelectAreYouAnRACMember(Contact contact)
        {
            if (contact.IsMultiMatchRSAMember && !contact.SkipDeclaringMembership)
                AreYouAnRACMember = true;
            else
                AreYouAnRACMember = contact.SkipDeclaringMembership ? (bool?)null : contact.IsRACMember;

            if ((contact.IsRACMember && !contact.SkipDeclaringMembership) || (contact.IsMultiMatchRSAMember && !contact.SkipDeclaringMembership))
            {
                FirstName    = contact.FirstName;
                DateOfBirth  = contact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH);

                if (DateTime.Now.Day == contact.DateOfBirth.Day &&
                    DateTime.Now.Month == contact.DateOfBirth.Month)
                {
                    Reporting.IsTrue(WaitForBirthdayNotificationToAppearAndClear(), "'happy birthday' animation shown");
                }

                MobileNumber = contact.MobilePhoneNumber;
                Email        = contact.PrivateEmail != null && contact.PrivateEmail.Address != null ?
                                   contact.PrivateEmail.Address.ToLower() :
                                   ""; 
                Reporting.Log($"Capturing screenshot of information submitted for Upfront Match ", _browser.Driver.TakeSnapshot());
                ClickFindMyDetailsButton();
                using (var spinner = new SparkSpinner(_browser))
                {
                    spinner.WaitForSpinnerToFinish();
                }

                if (!contact.IsMultiMatchRSAMember) VerifyDiscountToast(contact.MembershipTier);
            }
        }

        /// <summary>
        /// Supports Spark version of B2C Caravan
        /// Attempt to click the 'Find My Details' button in 'Before we get started' page
        /// </summary>
        /// <exception cref="ElementNotInteractableException">Thrown if button is present but disabled.</exception>
        public void ClickFindMyDetailsButton()
        {
            if (IsControlEnabled(XPath.FindMyDetails))
            {
                ClickControl(XPath.FindMyDetails);
            }
            else
            {
                throw new ElementNotInteractableException("Button is currently disabled and not clickable. Check input values.");
            }
        }

        public bool IsFirstNameErrorValidationTriggered()
        {
            var hasErrorHighlight = _driver.IsTextBoxHighlightedError(XPath.Member.FirstName, "First Name");
            var hasErrorNotice    = _driver.IsTextBoxErrorMessagePresent(XPath.Member.FirstName, "Please enter a valid first name");

            if (hasErrorHighlight != hasErrorNotice)
            { Reporting.Error($"Inconsistent error state of hasErrorHighlight ({hasErrorHighlight}) and hasErrorNotice ({hasErrorNotice})."); }

            return hasErrorHighlight;
        }

        public bool IsDoBErrorValidationTriggered()
        {
            var hasErrorHighlight = _driver.IsTextBoxHighlightedError(XPath.Member.DateOfBirth, "Date of Birth");
            var hasErrorNotice    = IsTextBoxErrorMessagePresentDob(XPath.Member.DateOfBirth, "You must be aged between 16 and 100");

            if (hasErrorHighlight != hasErrorNotice)
            { Reporting.Error($"Inconsistent error state of hasErrorHighlight ({hasErrorHighlight}) and hasErrorNotice ({hasErrorNotice})."); }

            return hasErrorHighlight;
        }

        public bool IsMobileNumberErrorValidationTriggered()
        {
            var hasErrorHighlight = _driver.IsTextBoxHighlightedError(XPath.Member.MobileNumber, "Mobile Number");
            var hasErrorNotice = _driver.IsTextBoxErrorMessagePresent(XPath.Member.MobileNumber, "For a mobile, enter your 10 digit number or for a landline, please include your area code");

            if (hasErrorHighlight != hasErrorNotice)
            { Reporting.Error($"Inconsistent error state of hasErrorHighlight ({hasErrorHighlight}) and hasErrorNotice ({hasErrorNotice})."); }

            return hasErrorHighlight;
        }

        public bool IsEmailErrorValidationTriggered()
        {
            var hasErrorHighlight = _driver.IsTextBoxHighlightedError(XPath.Member.Email, "Email");
            var hasErrorNotice = _driver.IsTextBoxErrorMessagePresent(XPath.Member.Email, "Please enter a valid email");

            if (hasErrorHighlight != hasErrorNotice)
            { Reporting.Error($"Inconsistent error state of hasErrorHighlight ({hasErrorHighlight}) and hasErrorNotice ({hasErrorNotice})."); }

            return hasErrorHighlight;
        }

        private bool WaitForBirthdayNotificationToAppearAndClear()
        {
            var birthdayNotificationSeen = _driver.WaitForElementToBeVisible(By.XPath(XPath.Member.BirthdayAnimation), WaitTimes.T5SEC) != null;

            // Now wait for dialog to disappear.
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
            do
            {
                IWebElement birthdayPopUp = null;
                var isBirthdayDialogStillPresent = _driver.TryFindElement(By.XPath(XPath.Member.BirthdayAnimation), out birthdayPopUp);

                if (isBirthdayDialogStillPresent)
                    Thread.Sleep(500);
                else
                    break;
            } while (DateTime.Now < endTime);

            return birthdayNotificationSeen;
        }

        protected abstract void VerifyDiscountToast(MembershipTier membershipTier);
    }
}
