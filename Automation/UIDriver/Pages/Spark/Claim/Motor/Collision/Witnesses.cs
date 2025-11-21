using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class Witnesses : BaseMotorClaimPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string ActiveStepperLabel = "Witnesses";
            public static readonly string HeaderText = "Witnesses";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "id('header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static class Witness1
            {
                public static readonly string FirstName = "id('witnessesQuestion0-first-name')";
                public static readonly string LastName = "id('witnessesQuestion0-last-name')";
                public static readonly string ContacdtNumber = "id('witnessesQuestion0-contact-number')";
                public static readonly string Email = "id('witnessesQuestion0-email')";
            }

            public static class Witness2
            {
                public static readonly string FirstName = "id('witnessesQuestion1-first-name')";
                public static readonly string LastName = "id('witnessesQuestion1-last-name')";
                public static readonly string ContacdtNumber = "id('witnessesQuestion1-contact-number')";
                public static readonly string Email = "id('witnessesQuestion1-email')";
            }

            public static class Button
            {
                public static readonly string AddWitness = "id('witnessesQuestion-add-witness')";
                public static readonly string Next = "id('submit')";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);

        #endregion


        public Witnesses(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Witnesses");
            Reporting.Log("Witnesses", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.HeaderText, Header, "Page header");
        }


        /// <summary>
        /// Add number of witnesses generated for this test on the Witnesses page
        /// If there is no witness to be added then it will click Next.
        /// Otherwise it will input First Name, Surname, Mobile phone number and Email address information for up to two witnesses.
        public void AddWitnesses(ClaimCar claim)
        {
            Reporting.AreEqual(Constants.ClaimNumberText(claim.ClaimNumber), ClaimNumber, "Claim number");

            //Only add a new witness when Witness is not null
            if (claim.Witness?.Any() == true)
            {
                var firstWitness = claim.Witness.First();
                ClickControl(XPath.Button.AddWitness);
                WaitForTextFieldAndEnterText(XPath.Witness1.FirstName, firstWitness.FirstName, hasTypeAhead: false);
                if (!string.IsNullOrEmpty(firstWitness.Surname))
                {
                    WaitForTextFieldAndEnterText(XPath.Witness1.LastName, firstWitness.Surname, hasTypeAhead: false);
                }
                if (!string.IsNullOrEmpty(firstWitness.MobilePhoneNumber) || !string.IsNullOrEmpty(firstWitness.HomePhoneNumber))
                {
                    var contactNumber = string.IsNullOrEmpty(firstWitness.MobilePhoneNumber) ? firstWitness.HomePhoneNumber : firstWitness.MobilePhoneNumber;
                    WaitForTextFieldAndEnterText(XPath.Witness1.ContacdtNumber, contactNumber, hasTypeAhead: false);
                }
                if (!string.IsNullOrEmpty(firstWitness.PrivateEmail.Address))
                {
                    WaitForTextFieldAndEnterText(XPath.Witness1.Email, firstWitness.PrivateEmail.Address, hasTypeAhead: false);
                }

                //Add 2nd witness
                if (claim.Witness.Count() > 1)
                {
                    ClickControl(XPath.Button.AddWitness);
                    var seondWitness = claim.Witness[1];
                    WaitForTextFieldAndEnterText(XPath.Witness2.FirstName, seondWitness.FirstName, hasTypeAhead: false);
                    if (!string.IsNullOrEmpty(seondWitness.Surname))
                    {
                        WaitForTextFieldAndEnterText(XPath.Witness2.LastName, seondWitness.Surname, hasTypeAhead: false);
                    }
                    if (!string.IsNullOrEmpty(seondWitness.MobilePhoneNumber) || !string.IsNullOrEmpty(seondWitness.HomePhoneNumber))
                    {
                        var contactNumber = string.IsNullOrEmpty(seondWitness.MobilePhoneNumber) ? seondWitness.HomePhoneNumber : seondWitness.MobilePhoneNumber;
                        WaitForTextFieldAndEnterText(XPath.Witness2.ContacdtNumber, contactNumber, hasTypeAhead: false);
                    }
                    if (!string.IsNullOrEmpty(seondWitness.PrivateEmail.Address))
                    {
                        WaitForTextFieldAndEnterText(XPath.Witness2.Email, seondWitness.PrivateEmail.Address, hasTypeAhead: false);
                    }                       
                }
            }

            Reporting.Log("Witnesses - Before clicking Next Button", _driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }
    }


}