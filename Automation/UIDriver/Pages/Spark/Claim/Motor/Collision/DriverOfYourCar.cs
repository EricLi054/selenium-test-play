using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class DriverOfYourCar : SparkPersonalInformationPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static string HeaderText => "Driver of your car";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";
            public static readonly string ActiveStepperLabel = "Driver of your car";

            public class Label
            {
                public static readonly string WereYouTheDriver = "Were you the driver?";
            }
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('driver-of-your-car-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";

            public class Button
            {
                public static readonly string Yes = "//button[@aria-label='Yes']";
                public static readonly string No = "//button[@aria-label='No']";
                public static readonly string Next = "id('submit-button')";
            }

            public class Toggle
            {
                public static readonly string WereYouTheDriver = "id('is-driver')";
            }
            public class Label
            {
                public static readonly string WereYouTheDriver = "//label[@for='is-driver']";
            }
            public class RadioButton
            {
                public static string WhoWasDriver(string driverName) => $"//label[contains(.,\"{driverName}\")]/span[1]";
                public static readonly string SomeoneElse = "//label[.='Someone else']/span[1]";
            }
            public static readonly string DriversName = "//div[@role='radiogroup']/div[contains(@id, 'policy-driver')]";
            public static readonly string MiddleName = "id('other-driver-middle-name')";
            public static readonly string DateOfBirth = "id('other-driver-date-of-birth')";
            public static readonly string Address = "id('address')";
            public static readonly string FirstAddress = "id('address-listbox')/li[@id = 'address-option-0']";
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);
        private bool WereYouTheDriver
        {
            get => GetBinaryToggleState(XPath.Toggle.WereYouTheDriver, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Toggle.WereYouTheDriver, XPath.Button.Yes, XPath.Button.No, value);
        }
        private string WhoWasTheDriver  {
            set => ClickControl(XPath.RadioButton.WhoWasDriver(value));
        }

        private string MiddleName
        {
            get => GetValue(XPath.MiddleName);
            set => WaitForTextFieldAndEnterText(XPath.MiddleName, value);
        }

        private string InputDateOfBirth
        {
            get => GetInnerText(XPath.DateOfBirth);

            set
            {
                ClickControl(XPath.DateOfBirth);
                WaitForTextFieldAndEnterText(XPath.DateOfBirth, value, hasTypeAhead: false);
            }
        }
        private string Address
        {
            get => GetValue(XPath.Address);
            set => QASSearchForAddress(XPath.Address, XPath.FirstAddress, value);             
        }

        #endregion


        public DriverOfYourCar(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("Driver of your car");
            Reporting.Log("Driver of your car immediately after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.Label.WereYouTheDriver, GetInnerText(XPath.Label.WereYouTheDriver), "Were you the driver question label");
        }

        //TO DO: Currently we only covering claimant is the driver scenario, there is SPK-4963 raised for covering other scenarios 
        public void ChooseDriverOfYourCar(ClaimCar claim)
        {
            Reporting.AreEqual(Constants.HeaderText, Header, "Page header");
            Reporting.AreEqual(Constants.ClaimNumberText(claim.ClaimNumber), ClaimNumber, "Claim number");

            WereYouTheDriver = claim.IsClaimantDriver;
            
            //if claimnt is not the driver then select the driver
            if (!WereYouTheDriver)
            {
                claim.Driver.expectedClaimDrivers = GetExpectedClaimDrivers(claim);
                
                //if we expect there is more than 1 driver added in the policy
                if (claim.Driver.expectedClaimDrivers.Count() > 0)
                {
                    VerifyDriversList(claim);

                    if (claim.Driver.isNewContact())
                    {
                        ClickControl(XPath.RadioButton.SomeoneElse);
                    }
                    else
                    {
                        WhoWasTheDriver = claim.Driver.DriverDetails.FirstName;
                    }
                }

                //Enter the new driver details
                if (claim.Driver.isNewContact())
                {
                    FirstName = claim.Driver.DriverDetails.FirstName;
                    MiddleName = claim.Driver.DriverDetails.MiddleName;
                    LastName = claim.Driver.DriverDetails.Surname;
                    InputDateOfBirth = claim.Driver.DriverDetails.DateOfBirth.ToString("dd/MM/yyyy");
                    ContactNumber = claim.Driver.DriverDetails.MobilePhoneNumber;
                    Address = claim.Driver.DriverDetails.MailingAddress.StreetSuburbState();
                    Reporting.Log($"Snapshot after inputting other driver details", _driver.TakeSnapshot());
                }
            }

            Reporting.Log("Driver of your car - Before clicking Next Button", _driver.TakeSnapshot());
            ClickNext();
        }

        /// <summary>
        /// Verify all the drivers full name
        /// </summary>        
        public void VerifyDriversList(ClaimCar claim)
        {
            var elements = _driver.FindElements(By.XPath(XPath.DriversName));
            List<string> actualDriverNames = new List<string>();

            for (int i = 0; i < elements.Count - 1; i++)
            {
                Reporting.IsTrue(claim.Driver.expectedClaimDrivers.Any(x => x.Equals(elements[i].Text, StringComparison.OrdinalIgnoreCase)), $"additional driver name {elements[i].Text}");
            }            
        }


        /// <summary>
        /// Return all the policy holders and additional drivers full name
        /// excluding the claimant's full name
        /// </summary>       
        /// <returns></returns>
        private List<string> GetExpectedClaimDrivers(ClaimCar claim)
        {
            List<string> policyHolderExternalContactNumber = new List<string>();
            List<string> claimDrivers = new List<string>();
            var policyHolders = claim.Policy.PolicyHolders.FindAll(x => x.ContactRoles.FirstOrDefault() != ContactRole.AuthParty);
            //Get all the policy holders external contact number
            foreach (var policyHolder in policyHolders)
            {
                if (!policyHolder.ContactRoles.Contains(ContactRole.AuthParty))
                {
                    policyHolderExternalContactNumber.Add(policyHolder.ExternalContactNumber);
                }
               
            }
            var policyDrivers = DataHelper.GetPolicyDetails(claim.Policy.PolicyNumber).MotorAsset.Drivers;

            //Add the additional drivers full names, excluing the policy holders
            foreach (var driver in policyDrivers)
            {
                if (!policyHolderExternalContactNumber.Contains(driver.ContactExternalNumber))
                {
                    var contacts = DataHelper.GetContactDetailsViaExternalContactNumber(driver.ContactExternalNumber);
                    claimDrivers.Add(contacts.GetFullName());
                }
            }

            //Add all the policy holders full name, excluding the claimsnt
            foreach (var policyHolder in policyHolders)
            {
                if (!policyHolder.ContactRoles.Contains(ContactRole.AuthParty))
                {
                    if (policyHolder.ExternalContactNumber != claim.Claimant.ExternalContactNumber)
                    {
                        claimDrivers.Add(policyHolder.GetFullName());
                    }
                } 
            }

            return claimDrivers;
        }

        new public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        public List<string> GetPercyIgnoreCSS() =>
         new List<string>()
         {
               "#claimNumberDisplay span"
         };

    }
}
