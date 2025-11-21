using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using System;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;


namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class Confirmation : SparkBasePage
    {
        #region CONSTANTS

        private static class Constants
        {
            public static readonly string ClaimReceived = "Claim received - thanks, ";
            public static readonly string EmailText = "You'll receive an email shortly.";
            public static readonly string ClaimNumberText = "Your claim number is ";

            public static readonly string NoExcessAmountDisplayed = "Your excessThe email will confirm your excess and how to pay it.";
            public static readonly string ZeroExcessAmountDisplayed = "You don't have to pay an excessGiven the information you provided, you don't have to pay an excess for this claim.";
            public static readonly string PayExcess = "Please pay your excess as soon as you can.";
            public static string ExcessAmountDisplayed(double excessAmount) => $"Your total excess is ${excessAmount}The email will have more information on the excess and how to pay it.";
            public static readonly string HireCarCover = "You've got hire car coverWhen you need a hire car, call us.";
            public static readonly string CarTowed = "If your car was towedIt's important you call us on 13 17 03 as there may be costs that we don't cover.";
            public static readonly string NextSteps = "Next stepsWe'll contact you within two business hours about your next steps. During busy periods, it can take a bit longer.";
            public static readonly string WhatsNexWithSubmitReceipt = "What's nextYou'll receive an email with your next steps shortly. If we need to, we'll call you.You can submit receipts for towing or taxis if you have them.We'll let you know if we need images of the damageSubmit receipts now";
            public static readonly string WhatsNextWithOutSubmitReceipt = "What's nextYou'll receive an email with your next steps shortly. If we need to, we'll call you.";
            public static readonly string GetQuote = "Please get a repair quoteThe email will explain how to do this in more detail.We'll review your claim and will let you know if we need more information.";
            public static string RepairerAssigned(string repairerName, string contactNumber) => $"Please call {repairerName}Book your car in on {contactNumber}.We'll review your claim and will let you know if we need more information.";
            public static readonly string NotificationText = "We'll let you know if we need images of the damage";

        }

        #endregion


        #region XPATHS

        private static class XPath
        {
            public static readonly string ConfirmationMessage = "//h2[@data-testid='confirmation-header-title']";
            public static readonly string Email = "//p[@data-testid='confirmation-header-subtitle']";
            public static readonly string ClaimNumber = "//p[@data-testid='claimNumberDisplay']";

            public static readonly string NextStep = "//div[@data-testid='next-steps-container']";
            public static readonly string Excess = "//div[@data-testid='excess-card-container']";
            public static readonly string HireCar = "//div[@data-testid='hire-car-cover-card-container']";
            public static readonly string CarTowed = "//div[@data-testid='car-towed-card-container']";
            public static readonly string NotificationCard = "id('damage-images-notification-card')";

            public static readonly string GoToRACHomePage = "id('rac-home-page-link-button')";

            public static class Button
            {
                public static readonly string SubmitReceipts = "id('submit-receipts-now')";
            }
        }

        #endregion

        public Confirmation(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.GoToRACHomePage);
                GetElement(XPath.ClaimNumber);
                GetElement(XPath.ConfirmationMessage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Confirmation");
            Reporting.Log("Confirmation after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public GetClaimResponse VerifyConfirmationPage(ClaimCar claim)
        {
            var claimResponse = DataHelper.GetClaimDetails(claim.ClaimNumber);

            // If the vehicle is drivable and there is no Payment Block on the claim,
            // then Shield should return IsCompleteLodgementDone as true, otherwise it will be false
            claim.expectClaimPaymentBlock = claimResponse.IsBlockedPayments;
            claim.ExpectCompleteLodgementDone = IsOneStepCollisionClaimScenario(claim);

            Reporting.AreEqual($"{Constants.ClaimReceived}{claim.Claimant.FirstName}", GetInnerText(XPath.ConfirmationMessage), "Claim received message on the Confirmation page");
            
            if ((!claim.OnlyClaimDamageToTP && claim.IsVehicleDriveable == true && !claim.IsRepairerAllocationExhausted)
                || (claim.OnlyClaimDamageToTP && claim.ShieldDamageType != ShieldClaimScenario.UninsuredMotoristExtension))
            {
                Reporting.AreEqual(Constants.EmailText, GetInnerText(XPath.Email), "Email message on the Confirmation page");
            }

            Reporting.AreEqual($"{Constants.ClaimNumberText}{claim.ClaimNumber}", GetInnerText(XPath.ClaimNumber), "Claim number on the Confirmation page");

            /// If the vehicle is not drivable OR
            /// If the repairer allocation tool is exhausted
            /// then we check Whats Next card            
            if (!claim.OnlyClaimDamageToTP)
            {
                //when car is not drivable
                if (claim.IsVehicleDriveable != true)
                {
                    Reporting.AreEqual(Constants.WhatsNexWithSubmitReceipt, GetInnerText(XPath.NextStep).StripLineFeedAndCarriageReturns(false), "Whats Next message on the Confirmation page");
                }
                //when car is drivable and
                //repairer allocation got exhausted
                else if (claim.IsVehicleDriveable == true && claim.IsRepairerAllocationExhausted)
                {
                    Reporting.AreEqual(Constants.WhatsNextWithOutSubmitReceipt, GetInnerText(XPath.NextStep).StripLineFeedAndCarriageReturns(false), "Whats Next message on the Confirmation page");
                }
                // User choose a get repairer quote
                // then we check Please get a repairer quote card
                else if (claim.IsVehicleDriveable == true && claim.RepairerOption == RepairerOption.GetQuote)
                {
                    Reporting.AreEqual(Constants.GetQuote, GetInnerText(XPath.NextStep).StripLineFeedAndCarriageReturns(false), "Get quote message on the Confirmation page");
                }
                // If User choose repairer option 1 or 2
                // then we check repairer card
                else if (claim.IsVehicleDriveable == true && claim.RepairerOption == RepairerOption.First ||
                    claim.IsVehicleDriveable == true && claim.RepairerOption == RepairerOption.Second)
                {
                    var text = GetInnerText(XPath.NextStep).StripLineFeedAndCarriageReturns(false);
                    Reporting.AreEqual(Constants.RepairerAssigned(claim.AssignedRepairer.BusinessName, claim.AssignedRepairer.ContactNumber), Regex.Replace(text, @"(?<=\d+)\s+(?=\d+)", ""), "Repairer assigned message on the Confirmation page");
                }
            }

            //if the claim type is UME (uninsured motor extention)
            if (claim.ShieldDamageType == ShieldClaimScenario.UninsuredMotoristExtension)
            {
                Reporting.AreEqual(Constants.WhatsNextWithOutSubmitReceipt, GetInnerText(XPath.NextStep).StripLineFeedAndCarriageReturns(false), "Whats Next message on the Confirmation page");
                Reporting.AreEqual(Constants.CarTowed, GetInnerText(XPath.CarTowed).StripLineFeedAndCarriageReturns(false), "Car towed message on the Confirmation page");
            }

            var claimExcess = claimResponse.ClaimContacts.Find(x => x.ClaimantSide == "PH").CalculatedExcess;
            if (claim.ExpectCompleteLodgementDone && claimExcess > 0)
            {
                var actualText = GetInnerText(XPath.Excess).StripLineFeedAndCarriageReturns(false);
                if (claim.OnlyClaimDamageToTP)
                {
                    var expectedExcessText = $"{Constants.ExcessAmountDisplayed(claimExcess)}{Constants.PayExcess}";
                    Reporting.AreEqual(expectedExcessText, Regex.Replace(actualText, @"(?<=\d+)\s+(?=\d+)", ""), "Excess text on the Confirmation page");
                }
                else
                {
                    var expectedExcessText = Constants.ExcessAmountDisplayed(claimExcess);
                    Reporting.AreEqual(expectedExcessText, Regex.Replace(actualText, @"(?<=\d+)\s+(?=\d+)", ""), "Excess text on the Confirmation page");
                }
            }
            else if (claim.ExpectCompleteLodgementDone && claimExcess == 0)
            {
                Reporting.AreEqual(Constants.ZeroExcessAmountDisplayed, GetInnerText(XPath.Excess).StripLineFeedAndCarriageReturns(false), "Excess text on the Confirmation page");
            }
            else
            {
                if (claim.OnlyClaimDamageToTP && claim.ShieldDamageType != ShieldClaimScenario.UninsuredMotoristExtension)
                {
                    var expectedExcessText = $"{Constants.NoExcessAmountDisplayed}{Constants.PayExcess}";
                    Reporting.AreEqual(expectedExcessText, GetInnerText(XPath.Excess).StripLineFeedAndCarriageReturns(false), "Excess text on the Confirmation page");
                }
                else
                {
                    Reporting.AreEqual(Constants.NoExcessAmountDisplayed, GetInnerText(XPath.Excess).StripLineFeedAndCarriageReturns(false), "Excess text on the Confirmation page");
                }                
            }

            if (claim.Policy.HasHireCarCover && !claim.OnlyClaimDamageToTP)
            {
                Reporting.AreEqual(Constants.HireCarCover, GetInnerText(XPath.HireCar).StripLineFeedAndCarriageReturns(false), "Hire car message on the Confirmation page");
            }

            if (claim.IsVehicleDriveable == true)
            {
                Reporting.AreEqual(Constants.NotificationText, GetInnerText(XPath.NotificationCard).StripLineFeedAndCarriageReturns(false), "Notification message on the Confirmation page");
            }

            return claimResponse;
        }

        /// <summary>
        /// This should be invoked only when we expect to see the option to submit receipts for the 
        /// motor collision claim (e.g. Vehicle has been towed).
        /// </summary>
        /// <param name="claimData">Claim data generated for this test</param>
        public void SubmitReceiptsNow(ClaimCar claimData)
        {
            Reporting.Log($"Selecting 'Submit receipts now' button.");
            ClickControl(XPath.Button.SubmitReceipts);
        }
            


        /// <summary>
        /// This method identifies if the claim is eligible for One step
        /// </summary>
        private static bool IsOneStepCollisionClaimScenario(ClaimCar claimData)
        {
            bool isOneStepClaimScenario = false;
            //If there is payment block on the claim, then disqualified from one-step claims
            if (claimData.expectClaimPaymentBlock)
            {
                isOneStepClaimScenario = false;
                Reporting.Log("The claim is not eligible for One Step because of payment block");
            }
            // If it's an Uninsured Motorist Extension claim, then disqualified from one-step claims
            else if (claimData.ShieldDamageType == ShieldClaimScenario.UninsuredMotoristExtension)
            {
                isOneStepClaimScenario = false;
                Reporting.Log("The claim is not eligible for One Step because it's a Uninsured Motorist Extension claim");
            }
            //If it's a liability only claim and it's not an Uninsured Motorist Extension claim
            //then it's eligible for one-step claims
            else if (claimData.OnlyClaimDamageToTP)
            {
                isOneStepClaimScenario = true;
                Reporting.Log("The claim is eligible for One Step as it's a libality only claim");
            }
            //If repairer plan exhausted, then disqualified from one-step claims 
            else if (claimData.IsRepairerAllocationExhausted)
            {
                isOneStepClaimScenario = false;
                Reporting.Log("The claim is not eligible for One Step because referral plan is exhausted");
            }            
            else
            {
                switch (claimData.DamageType)
                {
                    case MotorClaimDamageType.SingleVehicleCollision:
                        isOneStepClaimScenario = true;
                        Reporting.Log("The claim is eligible for One Step as it's a single vehicle collision claim " +
                            "which is not expected to trigger a Payment Block, and repairer allocation is not exhausted.");
                        break;
                    case MotorClaimDamageType.MultipleVehicleCollision:
                                               
                        var scenario = claimData.ClaimScenario;
                        if (scenario == MotorClaimScenario.WhileDrivingMyCarHitAParkedCar ||
                            scenario == MotorClaimScenario.WhileReversingHitParkedCar ||
                            scenario == MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar ||
                            scenario == MotorClaimScenario.WhileParkedAnotherCarHitMyCar ||
                            scenario == MotorClaimScenario.WhileDrivingOtherVehicleHitRearOfMyCar ||
                            scenario == MotorClaimScenario.WhileReversingHitByAnotherCar ||
                            scenario == MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar)
                        {
                            isOneStepClaimScenario = true;
                            Reporting.Log("The claim is eligible for One Step as it's a multi vehicle collision claim and claim scenarios is" +
                                "either 'TP hit PH whilst parked' or 'PH hit TP whilst parked' or 'TP hit PH in rear' or 'TP hit PH in rear'");
                        }
                        break;
                    default:
                        throw new NotSupportedException($"{claimData.DamageType.GetDescription()} is not supported");
                }
            }
            return isOneStepClaimScenario;
        }
    }
}