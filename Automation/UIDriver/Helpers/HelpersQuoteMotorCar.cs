using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

namespace UIDriver.Helpers
{
    public static class HelpersQuoteMotorCar
    {
        public static void LaunchPageBeginNewMotorQuote(this Browser browser)
        {
            using (var launchPage = new LaunchPage(browser))
            {
                launchPage.ClickNewMotorQuote();
            }

            using (var quotePage1 = new MotorQuote1Details(browser))
            {
                quotePage1.WaitForPage();
            }
        }

        public static void MotorQuoteEditCoverPage2(this Browser browser, QuoteCar vehicle)
        {
            using (var quotePage2 = new MotorQuote2Quote(browser))
            using (var spinner = new RACSpinner(browser))
            {
                quotePage2.ClickTab(vehicle.CoverType);
                var quoteNumber = quotePage2.QuoteReference;
                var quoteDetails = DataHelper.GetQuoteDetails(quoteNumber);
                // TODO: INSU-286 Remove as the following line of code as this will no longer be required when the story is actioned
                // Check if the cover type is TPO and the product version is greater than or equal to MotorProductVersionIdWithExcessNcbChanges
                vehicle.IsMotorQuoteWithExcessChanges = (((vehicle.CoverType == Constants.PolicyMotor.MotorCovers.TPO) || (vehicle.CoverType == Constants.PolicyMotor.MotorCovers.TFT)) && (quoteDetails.ProductVersionAsInteger >= Constants.PolicyMotor.MotorProductVersionIdWithExcessNcbChanges));

                if (!string.IsNullOrEmpty(vehicle.Excess))
                {
                    // TODO: INSU-286 Remove the "if" check as this is no longer required when the story is actioned
                    if (vehicle.IsMotorQuoteWithExcessChanges)
                    {
                        vehicle.Excess = Constants.PolicyMotor.MOTOR_COVER_TPO_TFT_DEFAULT_EXCESS_POST_VERSION_46;
                    }
                   quotePage2.QuoteExcess = vehicle.Excess;
                    spinner.WaitForSpinnerToFinish();
                }

                if (vehicle.StartDate.Date != DateTime.Now.Date)
                {
                    quotePage2.QuoteStartDate = vehicle.StartDate;
                    spinner.WaitForSpinnerToFinish();
                }

                // Buffer details:
                var quotePrice  = vehicle.PayMethod.IsAnnual ? quotePage2.QuotePriceAnnual : quotePage2.QuotePriceMonthly;
                var quoteExcess = quotePage2.QuoteExcess;
                var quoteDate   = quotePage2.QuoteStartDate.ToString(DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_FORWARDSLASH);

                Reporting.Log($"Quote ({quoteNumber}) successfully generated. Quoted price: {quotePrice} Set Excess: {quoteExcess} Start Date: {quoteDate}", browser.Driver.TakeSnapshot());
            }
        }

        public static void MotorQuoteClickBuyPage2(this Browser browser)
        {
            using (var quotePage2 = new MotorQuote2Quote(browser))
            using (var quotePage3 = new MotorQuote3Policy(browser))
            using (var spinner = new RACSpinner(browser))
            {
                quotePage2.ClickBuyOnlineButton();
                spinner.WaitForSpinnerToFinish(nextPage: quotePage3);
            }
        }

        public static void MotorQuoteAddedDetailsPage3(this Browser browser, QuoteCar vehicle)
        {
            using (var quotePage3 = new MotorQuote3Policy(browser))
            using (var quoteSummary = new MotorQuote3Summary(browser))
            using (var spinner = new RACSpinner(browser))
            {
                // Complete the additional car details
                Reporting.IsTrue(quotePage3.FillInAddedVehicleDetails(vehicle), "successfully completed additional vehicle details");
                Reporting.Log($"Moving on to input driver details", browser.Driver.TakeSnapshot());
                quotePage3.ClickCarDetailsContinueButton();

                // B2C may interchange some drivers, so we use a temporary list
                // to search and eliminate from.
                var workingDriversList = new List<Driver>(vehicle.Drivers);
                // Complete the additional driver details
                for (int i = 0; i < vehicle.Drivers.Count; i++)
                {
                    quotePage3.WaitForDriverDetails(i);
                    quotePage3.FillInDriverDetails(i, workingDriversList, vehicle.ParkingAddress, browser);
                    quotePage3.ClickDriverContinueButton(i);
                }
                spinner.WaitForSpinnerToFinish(nextPage: quoteSummary); 
            }
        }

        /// <summary>
        /// This alternative flow is just to support tests checking the Field Length Validation
        /// under test name Anonymous_MotorPolocy_NameFields_TC01.
        /// Will only serve for validation errors against the first driver on a policy, and 
        /// has no awareness to confirm that quotePage3 is displayed.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="vehicle"></param>
        public static void MotorQuoteAddedDetailsPage3FieldLengthValidations(this Browser browser, QuoteCar vehicle)
        {
            using (var quotePage3 = new MotorQuote3Policy(browser))
            using (var quoteSummary = new MotorQuote3Summary(browser))
            using (var spinner = new RACSpinner(browser))
            {
                // Complete the additional car details
                Reporting.IsTrue(quotePage3.FillInAddedVehicleDetails(vehicle), "successfully completed additional vehicle details");
                quotePage3.ClickCarDetailsContinueButton();

                // B2C may interchange some drivers, so we use a temporary list
                // to search and eliminate from.
                var workingDriversList = new List<Driver>(vehicle.Drivers);
                // Complete the additional driver details
                for (int i = 0; i < vehicle.Drivers.Count; i++)
                {
                    quotePage3.WaitForDriverDetails(i);
                    quotePage3.FillInDriverDetails(i, workingDriversList, vehicle.ParkingAddress, browser);
                    quotePage3.ClickDriverContinueButton(i);
                }
                //Check the Field Length Validations and end test
                quotePage3.VerifyNameFieldLengthValidationErrors();
            }
        }
    }
}
