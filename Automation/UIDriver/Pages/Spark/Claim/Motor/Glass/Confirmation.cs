using Newtonsoft.Json.Serialization;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using PercyIO.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.Motor.Glass
{
    public class Confirmation : SparkBasePage
    {
        #region CONSTANTS

        private class Constants
        {
            public const string ClaimReceived = "Claim received!";
            public const string ClaimOverToYou = "Claim received - over to you, ";
            public const string GotYourClaim = "We've got your claim, thanks!";

            public const string ConfirmationEmail = "You'll receive a confirmation email shortly.";
            public const string EmailText = "You'll receive an email shortly.";
            public const string ReviewClaim = "We'll review your claim and get back to you within one business day.";

            public const string ClaimNumberText = "Your claim number is ";
            public static string RepairText(MotorGlassRepairer repairer) => $"Your repairer is {repairer.GetDescription()}";
            public const string RepairLocation = "They'll confirm your locationWhether you're rural or metro, they'll fix your glass.";
            public const string AlreadyFixed_NextStep = "Your next stepsPlease submit your invoice for us to review.Or you can do this later. The email will have a link.";
            public const string RepairBooked_NextStep = "Your next stepsPlease give the repairer your claim number.The email will have more information on what to do next.";

        }

        #endregion


        #region XPATHS

        private class XPath
        {
            public class Header
            {
                public const string Message = "//h1[@data-testid='confirmation-header-title']";
                public const string Email = "//p[@data-testid='confirmation-header-subtitle']";
                public const string ClaimNumber = "//p[@data-testid='claimNumberDisplay']";
            }

            public class Repairer
            {
                public const string Deatils= "//h2[@data-testid='repairer-card-title']";
                public const string Location = "//div[@data-testid='notFoundCard']";
            }

            public class NextStep
            {
                public const string Text = "//div[@data-testid='next-steps-container']";
            }

            public class Button
            {
                public const string SubmitInvoice = "//a[@data-testid='invoice-upload-link-button']";
                public const string GoToHomePage = "//a[@data-testid='rac-home-page-link-button']";
            }

        }

        #endregion

        #region Settable properties and controls

        private string HeaderMessage => GetInnerText(XPath.Header.Message);

        private string EmailMessage => GetInnerText(XPath.Header.Email);

        private string ClaimNumberText => GetInnerText(XPath.Header.ClaimNumber);

        private string RepairerDetails => GetInnerText(XPath.Repairer.Deatils).StripLineFeedAndCarriageReturns(false);

        private string RepairerLocation => GetInnerText(XPath.Repairer.Location).StripLineFeedAndCarriageReturns(false);

        private string YourNextStepTexts => GetInnerText(XPath.NextStep.Text).StripLineFeedAndCarriageReturns(false);

        #endregion

        public Confirmation(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header.Message);
                GetElement(XPath.Header.ClaimNumber);
                GetElement(XPath.Button.GoToHomePage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Motor Glass Claim Page - Confirmation");
            return true;
        }

        public void VerifyConfirmationPage(ClaimCar claim)
        {
            if (claim.expectClaimPaymentBlock)
            {
                _browser.PercyScreenCheck(ClaimMotorGlass.Confirmation_PaymentBlock, GetPercyIgnoreCSSPaymentBlock());
                Reporting.AreEqual(Constants.GotYourClaim, HeaderMessage, "Header message on the Confirmation page");
                Reporting.AreEqual(Constants.ReviewClaim, EmailMessage, "Email message on the Confirmation page");
            }
            else
            {
                switch (claim.ClaimScenario)
                {
                    case MotorClaimScenario.GlassDamageNotFixed:

                        Reporting.AreEqual(Constants.ClaimReceived, HeaderMessage, "Header message on the Confirmation page");
                        Reporting.AreEqual(Constants.ConfirmationEmail, EmailMessage, "Email message on the Confirmation page");

                        var claimDetails = DataHelper.GetClaimDetails(claim.ClaimNumber);

                        if (claimDetails.ServiceProviderName().Contains(MotorGlassRepairer.Instant.GetDescription()))
                        {
                            Reporting.AreEqual(Constants.RepairText(MotorGlassRepairer.Instant), RepairerDetails, "Repairer details on the Confirmation page");
                        }
                        else if (claimDetails.ServiceProviderName().Contains(MotorGlassRepairer.OBrien.GetDescription()))
                        {
                            //For O'Brien repairer the confirmation page load progressively
                            Reporting.IsTrue(_driver.TryWaitForElementToBeVisible(By.XPath(XPath.Button.GoToHomePage), WaitTimes.T150SEC, out IWebElement control),
                                $"whether last section of the Confirmation page has loaded successfully when Service Provider is '{claimDetails.ServiceProviderName()}' " +
                                $"after allowing an additional 150 seconds");
                            Reporting.AreEqual(Constants.RepairText(MotorGlassRepairer.OBrien), RepairerDetails, "Repairer details on the Confirmation page");
                        }
                        else if (claimDetails.ServiceProviderName().Contains(MotorGlassRepairer.Novus.GetDescription()))
                        {
                            Reporting.AreEqual(Constants.RepairText(MotorGlassRepairer.Novus), RepairerDetails, "Repairer details on the Confirmation page");
                        }
                        else
                        {
                            Reporting.Error("Shield Motor Glass Service provider is not valid");
                        }
                        _browser.PercyScreenCheck(ClaimMotorGlass.Confirmation_GlassNotFixed, GetPercyIgnoreCSSRepairsAllocated());
                        Reporting.AreEqual(Constants.RepairLocation, RepairerLocation, "Repair location on the Confirmation page");
                        break;

                    case MotorClaimScenario.GlassDamageAlreadyFixed:

                        _browser.PercyScreenCheck(ClaimMotorGlass.Confirmation_GlassAlreadyFixed, GetPercyIgnoreCSSRepairsOtherOptions());

                        Reporting.AreEqual($"{Constants.ClaimOverToYou}{claim.Claimant.FirstName}!", HeaderMessage, ignoreCase: true, "Header message on the Confirmation page");
                        Reporting.AreEqual(Constants.EmailText, EmailMessage, "Email message on the Confirmation page");
                        Reporting.AreEqual(Constants.AlreadyFixed_NextStep, YourNextStepTexts, "Next steps on the Confirmation page");
                        break;

                    case MotorClaimScenario.GlassDamageRepairsBooked:

                        _browser.PercyScreenCheck(ClaimMotorGlass.Confirmation_GlassRepairBooked, GetPercyIgnoreCSSRepairsOtherOptions());

                        Reporting.AreEqual($"{Constants.ClaimOverToYou}{claim.Claimant.FirstName}!", HeaderMessage, ignoreCase: true, "Header message on the Confirmation page");
                        Reporting.AreEqual(Constants.EmailText, EmailMessage, "Email message on the Confirmation page");
                        Reporting.AreEqual(Constants.RepairBooked_NextStep, YourNextStepTexts, "Next steps on the Confirmation page");
                        break;
                    default:
                        throw new NotSupportedException($"{claim.ClaimScenario} is not valid for motor glass claim");
                }
            }

            Reporting.AreEqual($"{Constants.ClaimNumberText}{claim.ClaimNumber}", ClaimNumberText, "Claim number on the Confirmation page");

            ScrollElementIntoView(XPath.Button.GoToHomePage);
            Reporting.Log("Motor Glass Claim - Confirmation", _browser.Driver.TakeSnapshot());
        }

        public void ClickSubmitInvoice()
        {
            ClickControl(XPath.Button.SubmitInvoice);
        }

        private List<string> GetPercyIgnoreCSSRepairsAllocated() =>
          new List<string>()
          {
               "#claimNumberDisplay span",
               "#repairer-card-title",
               "div[data-testid='repairer-card'] img",
               "a[data-testid='repairer-card-phone-number-link']",
               "#repairer-card-body"
          };

        private List<string> GetPercyIgnoreCSSRepairsOtherOptions() =>
          new List<string>()
          {
              "#confirmation-header-title",
              "#claimNumberDisplay span"

          };

          private List<string> GetPercyIgnoreCSSPaymentBlock() =>
          new List<string>()
          {              
              "#claimNumberDisplay span"
          };

    }
}
