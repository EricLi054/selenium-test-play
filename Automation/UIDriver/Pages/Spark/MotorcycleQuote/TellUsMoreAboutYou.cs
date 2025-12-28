using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Linq;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class TellUsMoreAboutYou : SparkPersonalInformationPage
    {
        #region XPATHS

        public static class XPath
        {
            public static class StepperLabels
            {
                public const string ConfirmDetails = "//div[contains(@class,'MuiStepper-vertical')]//div[contains(@class,'MuiStep-root')]//span[contains(text(),'Confirm policy details')]";
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

            set => SetMailingAddress(value);
        }

        #endregion

        public TellUsMoreAboutYou(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            try
            {
                // If matched existing member, these will be the only presented fields:
                GetElement(XPathPersonalInfo.Header);
                GetElement(XPathPersonalInfo.Policyholder.Personal.MailingAddress);
                GetElement(XPathPersonalInfo.Buttons.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Tell Us More About You");
            return true;
        }

        /// <summary>
        /// Fills all the fields
        /// </summary>
        /// <param name="quoteDetails"></param>
        public void FillTellUsMoreAboutYou(QuoteMotorcycle quoteDetails)
        {
            if (!quoteDetails.Drivers[0].Details.IsRACMember || quoteDetails.Drivers[0].Details.SkipDeclaringMembership)
            {
                SetTitleWithGender(quoteDetails.Drivers.FirstOrDefault().Details);
                FirstName = quoteDetails.Drivers.FirstOrDefault().Details.FirstName;
                LastName = quoteDetails.Drivers.FirstOrDefault().Details.Surname;
                ContactNumber = quoteDetails.Drivers.FirstOrDefault().Details.MobilePhoneNumber;
                Email = quoteDetails.Drivers.FirstOrDefault().Details.PrivateEmail.Address;
            }
            MailingAddress = quoteDetails.Drivers.FirstOrDefault().Details.MailingAddress.StreetSuburbState();
            Reporting.Log($"Capturing Screenshot after entering driver details", _browser.Driver.TakeSnapshot());
            ClickNext();

            using (var spinner = new SparkSpinner(_browser))
                spinner.WaitForSpinnerToFinish();
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

        public void ClickConfirmDetailsStep()
        {
            ClickControl(XPath.StepperLabels.ConfirmDetails);
        }
    }
}
