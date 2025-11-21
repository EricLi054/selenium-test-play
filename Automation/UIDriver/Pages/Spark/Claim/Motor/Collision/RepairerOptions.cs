using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class RepairerOptions : BaseMotorClaimPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string ActiveStepperLabel = "Repairer options";
            public static readonly string HeaderText = "Repairer options";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";

            public static class Label
            {
                public static readonly string GetQuoteHeader = "Get a quote from your own repairer";
                public static readonly string GetQuoteInfo = "Please tell your repairer to send us the quote for us to review.";
            }
            
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "id('repairer-options-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";

            public static class FirstRepairer
            {
                public static readonly string RepairerName = "id('repairer-0-selection-card-title')";
                public static readonly string RepairerAddress = "id('repairer-0-selection-card-fine-print')";
                public static readonly string ElectricVehicleRepairer = "id('repairer-0-selection-card-content')//div[.='Electric vehicle repairer']";
                public static readonly string LifetimeGuarantee = "id('repairer-0-selection-card-content')//div[.='Lifetime guarantee on repairs']";
                public static readonly string QualityRepairer = "id('repairer-0-selection-card-content')//div[.='Quality repair facility']";
                public static readonly string FasttrackAssessment = "id('repairer-0-selection-card-content')//div[.='Fast-tracked assessments']";
                public static readonly string HireCar = "id('repairer-0-selection-card-content')//div[.='Complimentary compact hire car (if available)']";
                public static readonly string Select = "id('repairer-0-selection-card-select-btn')";
            }
            public static class SecondRepairer
            {
                public static readonly string RepairerName = "id('repairer-1-selection-card-title')";
                public static readonly string RepairerAddress = "id('repairer-1-selection-card-fine-print')";
                public static readonly string ElectricVehicleRepairer = "id('repairer-1-selection-card-content')//div[.='Electric vehicle repairer']";
                public static readonly string LifetimeGuarantee = "id('repairer-1-selection-card-content')//div[.='Lifetime guarantee on repairs']";
                public static readonly string QualityRepairer = "id('repairer-1-selection-card-content')//div[.='Quality repair facility']";
                public static readonly string FasttrackAssessment = "id('repairer-1-selection-card-content')//div[.='Fast-tracked assessments']";
                public static readonly string HireCar = "id('repairer-1-selection-card-content')//div[.='Complimentary compact hire car (if available)']";
                public static readonly string Select = "id('repairer-1-selection-card-select-btn')";
            }
            public static class GetQuote
            {
                public static readonly string Title = "id('get-a-quote-selection-card-title')";
                public static readonly string Info = "id('get-a-quote-selection-card-content')";               
                public static readonly string Select = "id('get-a-quote-selection-card-select-btn')";
            }

            public static class Button
            {
                public static readonly string SubmitClaim = "id('submit')";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);
        
        
        #endregion


        public RepairerOptions(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.SubmitClaim);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Repairer options");
            Reporting.Log("Repairer options after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.Label.GetQuoteHeader, GetInnerText(XPath.GetQuote.Title), "Get Quote title text");
            Reporting.AreEqual(Constants.Label.GetQuoteInfo, GetInnerText(XPath.GetQuote.Info), "Get Quote info text");
            Reporting.IsTrue(IsControlDisplayed(XPath.GetQuote.Select), "Select button displayed");
        }
       
        public void VerifyRepairerDetails(ClaimCar claimCar, List<ServiceProvider> serviceProviders)
        {
            Reporting.AreEqual(Constants.HeaderText, Header, "Page header");
            Reporting.AreEqual(Constants.ClaimNumberText(claimCar.ClaimNumber), ClaimNumber, "Claim number");

            var firstServiceProvider = serviceProviders.First();
            Reporting.AreEqual(firstServiceProvider.RepairerName, GetInnerText(XPath.FirstRepairer.RepairerName), "First repairer name");
            Reporting.AreEqual(firstServiceProvider.DetailRepairerAddress.StreetSuburbStatePostcode().Trim(), GetInnerText(XPath.FirstRepairer.RepairerAddress), "First repairer address");

            // To check when the vehicle type is EV and the EV Repairer options found in the service provider list
            if (claimCar.Policy.Vehicle.IsElectricVehicle)
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.FirstRepairer.ElectricVehicleRepairer), "Electric Vehicle repairer displayed for first repairer");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.FirstRepairer.ElectricVehicleRepairer), "Electric Vehicle repairer displayed for first repairer");
            }

            Reporting.IsTrue(IsControlDisplayed(XPath.FirstRepairer.LifetimeGuarantee), "Lifetime guarantee of repairs option displayed for first repairer");
            Reporting.IsTrue(IsControlDisplayed(XPath.FirstRepairer.QualityRepairer), "Quality repair facility option displayed for first repairer");


            if (firstServiceProvider.IsRapidRepairer)
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.FirstRepairer.FasttrackAssessment), "Fast-tracked assessments option displayed for first repairer");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.FirstRepairer.FasttrackAssessment), "Fast-tracked assessments option should not be displayed for first repairer");
            }

            if (firstServiceProvider.IsReadyDrive && !claimCar.Policy.HasHireCarCover)
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.FirstRepairer.HireCar), "Complimentary compact hire car (if available) option displayed for first repairer");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.FirstRepairer.HireCar), "Complimentary compact hire car(if available) option should not be displayed for first repairer");
            }

            Reporting.AreEqual("Selected", GetInnerText(XPath.FirstRepairer.Select), "First repairer select button should be selected");

            if (serviceProviders.Count() > 1)
            {
                var secondServiceProvider = serviceProviders[1];

                Reporting.AreEqual(secondServiceProvider.RepairerName, GetInnerText(XPath.SecondRepairer.RepairerName), "Second repairer name");
                Reporting.AreEqual(secondServiceProvider.DetailRepairerAddress.StreetSuburbStatePostcode().Trim(), GetInnerText(XPath.SecondRepairer.RepairerAddress), "Second repairer address");

                Reporting.IsTrue(IsControlDisplayed(XPath.SecondRepairer.LifetimeGuarantee), "Lifetime guarantee of repairs option displayed for Second repairer");
                Reporting.IsTrue(IsControlDisplayed(XPath.SecondRepairer.QualityRepairer), "Quality repair facility option displayed for Second repairer");

                if (secondServiceProvider.IsRapidRepairer)
                {
                    Reporting.IsTrue(IsControlDisplayed(XPath.SecondRepairer.FasttrackAssessment), "Fast-tracked assessments option displayed for Second repairer");
                }
                else
                {
                    Reporting.IsFalse(IsControlDisplayed(XPath.SecondRepairer.FasttrackAssessment), "Fast-tracked assessments option should not be displayed for second repairer");
                }

                if (secondServiceProvider.IsReadyDrive && !claimCar.Policy.HasHireCarCover)
                {
                    Reporting.IsTrue(IsControlDisplayed(XPath.SecondRepairer.HireCar), "Complimentary compact hire car (if available) option displayed for Second repairer");
                }
                else
                {
                    Reporting.IsFalse(IsControlDisplayed(XPath.SecondRepairer.HireCar), "Complimentary compact hire car(if available) option should not be displayed for second repairer");
                }
            }
        }
        /// <summary>
        /// This method to select the repairer option based on the reapirer input
        /// It also store the repairer name and contact number
        /// </summary>
        /// <param name="serviceProviders"> It's a mandatory parameter and it's sourced from SearchServiceProvider API call</param>
        public void SelectRepairer(ClaimCar claimCar, List<ServiceProvider> serviceProviders)
        {
            switch (claimCar.RepairerOption)
            {
                case RepairerOption.None:
                    break;
                case RepairerOption.First:
                case RepairerOption.Second when serviceProviders.Count() == 1:                
                    var firstRepairer = serviceProviders.FirstOrDefault();
                    claimCar.AssignedRepairer.BusinessName = firstRepairer.RepairerName;
                    claimCar.AssignedRepairer.ContactNumber = firstRepairer.RepairerPhoneNumber;
                    if (claimCar.RepairerOption == RepairerOption.Second)
                    {
                        Reporting.Log($"Selected the first repairer option as second repairer option is not available for this claim");
                    }
                    break;
                case RepairerOption.Second when serviceProviders.Count() > 1:
                    var secondRepairer = serviceProviders[1];
                    claimCar.AssignedRepairer.BusinessName = secondRepairer.RepairerName;
                    claimCar.AssignedRepairer.ContactNumber = secondRepairer.RepairerPhoneNumber;
                    ClickControl(XPath.SecondRepairer.Select);
                    break;
                case RepairerOption.GetQuote:
                    ClickControl(XPath.GetQuote.Select);
                    break;
                default:
                    throw new System.NotSupportedException($"{claimCar.RepairerOption} is not a valid repairer option");
            }

            Reporting.Log("Repairer options - Before clicking submit claim button", _driver.TakeSnapshot());
            ClickSubmitClaim();
        }
       
        public void ClickSubmitClaim()
        {
            ClickControl(XPath.Button.SubmitClaim);
        }

    }
}
