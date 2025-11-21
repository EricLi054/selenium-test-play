using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Linq;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class TellUsMoreAboutYou : SparkPersonalInformationPage
    {
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
    }
}
