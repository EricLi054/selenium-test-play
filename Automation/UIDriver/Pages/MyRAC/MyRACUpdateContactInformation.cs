using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;


namespace UIDriver.Pages.MyRAC
{
    public class MyRACUpdateContactInformation : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public class Masked
            {
                public class Headings
                {
                    public static readonly string PageHeader          = "Your contact details";
                    public static readonly string ContactDetailsPanel = "Contact details";
                    public static readonly string Telephone           = "Phone";
                    public static readonly string Mobile              = "Mobile";
                    public static readonly string Home                = "Home";
                    public static readonly string Work                = "Work";
                    public static readonly string Email               = "Contact Email";
                    public static readonly string Mail                = "Mailing Address";
                }
                public class Buttons
                {
                    public static readonly string EditDetails = "Edit";
                }
            }
            public class Editing
            {
                public class Headings
                {
                    public static readonly string PageHeader          = "Your contact details";
                    public static readonly string ContactDetailsPanel = "Contact details";
                    public static readonly string Telephone           = "Phone";
                    public static readonly string Mobile              = "Mobile";
                    public static readonly string HomePhone           = "Home phone";
                    public static readonly string WorkPhone           = "Work phone";
                    public static readonly string Email               = "Contact email";
                    public static readonly string EmailNote           = "This is the email we will use to contact you. " +
                                                                        "It may be different from your log-in email.";
                    public static readonly string MailingAddress      = "Mailing address *";
                }
                    
                public class Fields
                {
                    
                }
                public class Buttons
                {
                    public static readonly string UpdateContacts    = "Details are correct, continue";
                    public static readonly string Cancel            = "Cancel";
                }
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public class Masked
            {
                public class Headings
                {
                    public static readonly string PageHeader            = "Your contact details";
                    public static readonly string ContactDetailsPanel   = "Contact details";
                    public static readonly string Telephone             = "//p[@name='phone-label']";
                    public static readonly string Mobile                = "//p[contains(text(), 'Mobile')]";
                    public static readonly string Home                  = "//p[contains(text(), 'Home')]";
                    public static readonly string Work                  = "//p[contains(text(), 'Work')]";
                    public static readonly string Email                 = "//p[@name='email-label']";
                    public static readonly string Mail                  = "//p[@name='address-label']";
                }
                public class Display
                {
                    public static readonly string Mobile = "//p[@name='mobile-display']";
                    public static readonly string Home = "//p[@name='home-display']";
                    public static readonly string Work = "//p[@name='work-display']";
                    public static readonly string Email = "//p[@name='email-display']";
                    public static readonly string Mail = "//p[@name='address-display']";
                }
                public class Buttons
                {
                    public static readonly string EditDetails = "//button[contains(text(), 'Edit')]";
                }
            }
            public class Editing
            {
                public class Headings
                {
                    public static readonly string PageHeader            = "//h2[contains(text(),'our contact details')]";
                    public static readonly string ContactDetailsPanel   = "//h3[contains(text(),'ontact details')]";
                    public static readonly string Telephone             = "//label[@id='label-undefined' and contains(text(), 'Phone')]";
                    public static readonly string Mobile                = "//label[@id='label-undefined' and contains(text(), 'obile')]";
                    public static readonly string HomePhone             = "//label[@id='label-undefined' and contains(text(), 'ome phone')]";
                    public static readonly string WorkPhone             = "//label[@id='label-undefined' and contains(text(), 'ork phone')]";
                    public static readonly string Email                 = "//label[@id='label-undefined' and contains(text(), 'ontact email')]";
                    public static readonly string EmailNote             = "//label[@id='label-undefined' and contains(text(), 'your log-in email.')]";
                    public static readonly string MailingAddress        = "//label[@id='label-undefined' and contains(text(), 'ailing address')]";
                }

                public class Fields
                {
                    public static readonly string ExampleContainsText   = "//div[contains(text(),'select one')]";
                    public static readonly string Mobile                = "//input[@name='mobilePhone']";
                    public static readonly string HomePhone             = "//input[@name='homePhone']";
                    public static readonly string WorkPhone             = "//input[@name='workPhone']";
                    public static readonly string Email                 = "//input[@name='personalEmailAddress']";
                    public static readonly string MailingAddress        = "//input[@name='postalAddress']";
                }
                public class Dialog
                {
                    public static readonly string Title = "id('global-modal-title')";
                    public static readonly string Body  = "//*[contains(text(), 'RAC insurance policies')]";
                }
                public class Buttons
                {
                    public static readonly string UpdateContacts    = "//button[contains(text(), 'Update contacts')]";
                    public static readonly string Cancel            = "//button[contains(text(), 'Cancel')]";
                    public static readonly string DetailsCorrect    = "//a[contains(text(), 'Details are correct, continue')]";
                    public static readonly string DialogOkay        = "//button[contains(text(), 'Okay')]";
                }
            }
        }
        #endregion
        public MyRACUpdateContactInformation(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.Masked.Headings.Mail, GetInnerText(XPath.Masked.Headings.Mail));
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("myRAC Contact Details page");
            }
            return isDisplayed;
        }

        public void ClickEditDetails()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Masked.Buttons.EditDetails), WaitTimes.T150SEC);
            Reporting.Log($"Edit details button confirmed visible, selecting it to trigger MFA");
            ClickControl(XPath.Masked.Buttons.EditDetails);
        }
        
        public void EditMyRACFields(ClaimHome claim)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Editing.Fields.Email), WaitTimes.T150SEC);
            Reporting.Log($"Capturing snapshot of editable page", _browser.Driver.TakeSnapshot());

            if (!string.IsNullOrEmpty(claim.Claimant.NewMobilePhoneNumber))
             {
                Reporting.Log($"Updating Mobile to '{claim.Claimant.NewMobilePhoneNumber}'");
                WaitForTextFieldAndEnterText(XPath.Editing.Fields.Mobile, claim.Claimant.NewMobilePhoneNumber);
            }
            else
            {
                Reporting.Log($"No update to Mobile Telephone number as this is not indicated by test data.");
            }

            if (!string.IsNullOrEmpty(claim.Claimant.PrivateEmail.NewAddress))
            {
                Reporting.Log($"Updating Email to '{claim.Claimant.PrivateEmail.NewAddress}'");
                WaitForTextFieldAndEnterText(XPath.Editing.Fields.Email, claim.Claimant.PrivateEmail.NewAddress);
            }
            else 
            {
                Reporting.Log($"No update to Email as this is not indicated by test data.");
            }

            Reporting.Log($"Capturing snapshot of page before proceeding", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Editing.Buttons.UpdateContacts);

            DialogAdvisingInsurancePoliciesMayNeedUpdating();

            AssertDetailsCorrect();
        }

        public void DialogAdvisingInsurancePoliciesMayNeedUpdating()
         {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Editing.Dialog.Title), WaitTimes.T30SEC);
            Reporting.Log($"Capturing dialog displayed advising the member must update Insurance policies separately", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Editing.Buttons.DialogOkay);
        }

        public void AssertDetailsCorrect()
        {
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.Editing.Dialog.Title), WaitTimes.T5SEC);
            Reporting.Log($"Capturing screenshot immediately before confirming details are correct", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Editing.Buttons.DetailsCorrect);
        }
    }
}
