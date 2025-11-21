using Rac.TestAutomation.Common;
using System;
using UIDriver.Helpers;
using UIDriver.Pages;
using UIDriver.Pages.Shield;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.EFT;
using static Rac.TestAutomation.Common.ClaimContact;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations
{
    public static class ActionEFT
    {
        public static string RetrieveEFTLinkFromEmailAndOpenInBrowser(Browser browser, ClaimContact claimData)
        {
            var eftUrl = DataHelper.GetClaimEFTLinkFromCashSettlementEmail(claimData.ClaimNumber);
            LaunchPage.OpenEFTLink(browser, eftUrl, claimData);
            return eftUrl;
        }

        /// <summary>
        /// Supports Spark EFT Payment
        /// Enter the bank details and
        /// Click Submit button
        /// </summary>
        public static void ProvideBankDetails(Browser browser, BankAccount bankDetails)
        {
            using (var bankDetail = new EnterYourBankDetails(browser))
            {
                bankDetail.WaitForPage();

                browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.EnterYourBankDetails);
                bankDetail.ClickSubmitButton();
                browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.EnterYourBankDetailsFieldValidation);

                bankDetail.EnterBankDetailsAndClickSubmit(bankDetails);
            }
        }

        /// <summary>
        /// Accept Cash Settlement Offer
        /// Supports Spark EFT Payment
        /// </summary>
        public static void AcceptCashSettlement(Browser browser)
        {
            using (var settlementChoice = new ChooseSettlement(browser))
            {
                settlementChoice.WaitForPage();

                browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.ChoosePreferredOptions);
                settlementChoice.ClickConfirmButton();
                browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.ChoosePreferredOptionsFieldValidation);

                settlementChoice.AcceptCashSettlement();
            }
        }

        /// <summary>
        /// Fetch the OTP (One Time Password) from SMS and enter that
        /// code into the EFT form. Because OTP delivery can vary in 
        /// delivery time, there is a 5min period for retry and even
        /// requesting a new code to be sent.
        /// Once the code has been successfully entered, the bank 
        /// details are submitted for EFT.
        /// </summary>
        public static void EnterOneTimePasscodeAndProvideBankDetails(Browser browser, BankAccount bankDetails, bool detailUiChecking=false)
        {
            var isOTPSuccessful = false;
            var endTime = DateTime.Now.AddMinutes(5);
            do
            {
                ///If the IsBypassOTPEnabled() feature toggle is enabled then the OTP should be 000000
                ///otherwise, if the toggle is not enabled then we retrive the OTP from mailosaur
                var oneTimePasscode = "000000";
                if (!Config.Get().IsBypassOTPEnabled())
                {
                    oneTimePasscode = DataHelper.GetOTPFromSMS();
                }               

                // If we didn't find an OTP, don't cancel, it may still be pending delivery.
                if (string.IsNullOrEmpty(oneTimePasscode))
                { continue; }

                using (var oneTimePasscodePage = new OneTimePasscodeScreen(browser))
                using(var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish();
                    oneTimePasscodePage.WaitForPage();
                    browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.VerifyItsYou);
                    isOTPSuccessful = oneTimePasscodePage.EnterOneTimePasscodeAndVerify(oneTimePasscode);
                    if (!isOTPSuccessful)
                    { oneTimePasscodePage.RequestNewOTPCode(); }
                }
            } while (!isOTPSuccessful && (endTime > DateTime.Now));

            if (!isOTPSuccessful)
            { 
                Reporting.Error("We either never got an OTP, or not the correct one. Verify system config as OTP service may be disabled or throttled."); 
            }
            else
            {
                Reporting.Log("OTP should be successful",browser.Driver.TakeSnapshot());
            }

            if (detailUiChecking)
            {
                using (var bankDetail = new EnterYourBankDetails(browser))
                {
                    bankDetail.WaitForPage();
                    bankDetail.EnterInvalidNoMatchBSBAndCheckErrorMessage(payMethod:null);
                }
            }
            ProvideBankDetails(browser, bankDetails);
        }

        /// <summary>
        /// Decline Cash Settlement Offer        
        /// Click Submit button
        /// </summary>
        public static void DeclineCashSettlement(Browser browser)
        {
            using (var settlementChoice = new ChooseSettlement(browser))
            {
                settlementChoice.WaitForPage();
                settlementChoice.DeclineCashSettlement();
            }
        }

        /// <summary>
        /// Update the cash settlement liability in Shield
        /// Create a Cash Settlement payment with Pending status
        /// This will automatically trigger the CSFS email
        /// </summary>
        public static void UpdateLiabilityCreateCSFSPaymentInShield(Browser browser, string ClaimNumber, ClaimContact.ClaimsEFTFLow claimsEFTFlow)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDamageDetails = new DamageDetails(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageHome.SlowSearchByClaimNumber(ClaimNumber);
                pageClaimDetails.WaitForPage();

                pageClaimDetails.DependenciesTree.ActOnPolicyholdersDamageScenario(DependenciesTree.Action.Edit);
                pageDamageDetails.WaitForPage();
                Reporting.Log($"Capturing damage Scenario", browser.Driver.TakeSnapshot());
                AddCashSettlementLiability(browser, ClaimNumber);
                Reporting.Log($"Capturing after Cash Settlement Liability", browser.Driver.TakeSnapshot());
                CreatePayment(browser, claimsEFTFlow);
                Reporting.Log($"Capturing after Create Payment", browser.Driver.TakeSnapshot());
                MaintainEventFinish(browser);
            }
            Reporting.Log($"Capturing state before logging out of Shield and closing browser", browser.Driver.TakeSnapshot());
            browser.LogoutShieldAndCloseBrowser();
        }

        /// <summary>
        /// Create a payment for EFT flow
        /// </summary>
        public static void CreateEFTPaymentInShield(Browser browser, string ClaimNumber, ClaimContact.ClaimsEFTFLow claimsEFTFlow)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDamageDetails = new DamageDetails(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageHome.SlowSearchByClaimNumber(ClaimNumber);
                pageClaimDetails.WaitForPage();

                CreatePayment(browser, claimsEFTFlow);
                MaintainEventFinish(browser);
            }
            Reporting.Log($"Capturing state before logging out of Shield and closing browser", browser.Driver.TakeSnapshot());
            browser.LogoutShieldAndCloseBrowser();
        }

        /// <summary>
        /// Update the cash settlement liability in Shield     
        /// </summary>
        private static void AddCashSettlementLiability(Browser browser, string ClaimNumber)
        {
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDamageDetails = new DamageDetails(browser))
            using (var pageNewLiabilityReserve = new AddLiabilityReserve(browser))
            {
                pageDamageDetails.AddLiabilityReserve();
                pageNewLiabilityReserve.WaitForPage();
                pageNewLiabilityReserve.Reason = AddLiabilityReserve.AUTHORISED_QUOTE;
                pageNewLiabilityReserve.CashSettlementAmount = AddLiabilityReserve.CASH_SETTLEMENT_CREDIT_AMOUNT;                

                pageNewLiabilityReserve.ClickOK();
                pageDamageDetails.WaitForPage();

                pageDamageDetails.ClickOK();
                pageDamageDetails.WaitForOkToDisappear();

                pageClaimDetails.WaitForPage();
                Reporting.IsTrue(true, $"Cash Settlement Liability added successfully in Shield for Claim Number {ClaimNumber}");
            }

        }

        /// <summary>
        /// Create a Cash Settlement payment with Pending status
        /// This will automatically trigger the CSFS email
        /// </summary>

        private static void CreatePayment(Browser browser, ClaimContact.ClaimsEFTFLow claimsEFTFLow)
        {
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDamageDetails = new DamageDetails(browser))
            using (var pageClaimPayment = new ShieldClaimCreatePayment(browser))
            using (var pageMaintainEvent = new MaintainEvent(browser))
            using (var updateEmailAddress = new MaintainContactEmail(browser))
            {
                pageClaimDetails.DependenciesTree.ActOnPolicyholdersDamageScenario(DependenciesTree.Action.Select);
                pageClaimDetails.DependenciesTree.ClickPayment();
                pageClaimPayment.WaitForPage();

                pageClaimPayment.PaymentType = ShieldClaimCreatePayment.CLAIM_PAYMENT_TYPE;
                pageClaimPayment.PayTo = ShieldClaimCreatePayment.CLAIM_PAY_TO;
                
                if (claimsEFTFLow == ClaimContact.ClaimsEFTFLow.CSFS)
                {
                    pageClaimPayment.CSFSRequired = "Yes";
                    pageClaimPayment.SelectRepairs();
                }
                else
                {
                    pageClaimPayment.CSFSRequired = "No";
                    pageClaimPayment.DismissGeneralNoticeDialog();
                    pageClaimPayment.CSFSReasons = "50/50 Fence";
                    pageClaimPayment.SelectEFTConfirmationRequired();
                    pageClaimPayment.WaitForPage();
                    //Bank account should be "Select" if the EFT Confirmation Checkbox is checked
                    pageClaimPayment.BankAccount = "Select";
                }
                pageClaimPayment.ControlAmount = ShieldClaimCreatePayment.CONTROL_AMOUNT;
                pageClaimPayment.PolicyHolderITCEPercentage = ShieldClaimCreatePayment.PH_ITCE_PERCENTAGE;
                pageClaimPayment.CalculateAndEnterClaimPaymentAmount();
                pageClaimPayment.ClaimTransactionStatus = ShieldClaimCreatePayment.CLAIM_TRANSACTION_STATUS;
                Reporting.Log($"Capturing screen before selecting Next button", browser.Driver.TakeSnapshot());
                pageClaimPayment.ClickNext();
                Reporting.Log($"Cash Settlement payment should have been created successfully in Shield");
            }
        }

        /// <summary>
        /// Click the Finish button on MaintainEvent screen and Wait for Confirmation
        /// </summary>
        private static void MaintainEventFinish(Browser browser)
        {
            
            using (var pageMaintainEvent = new MaintainEvent(browser))
            {
                pageMaintainEvent.ClickApply();
                Reporting.Log($"Capturing screen before selecting Finish button", browser.Driver.TakeSnapshot());
                pageMaintainEvent.ClickFinishUpdate();
                pageMaintainEvent.WaitForAndClearGenericConfirmationDialog();
                Reporting.Log($"Cash Settlement email should have been sent successfully");
            }
        }

        /// <summary>
        /// Send EFT claim communication email
        /// </summary>
        public static void SendEFTForm(Browser browser, string ClaimNumber)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageClaimCommunication = new ShieldClaimManualAction(browser))           
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageHome.SlowSearchByClaimNumber(ClaimNumber);
                pageClaimDetails.WaitForPage();

                pageClaimDetails.DependenciesTree.ActOnPolicyholdersDamageScenario(DependenciesTree.Action.Select);
                pageClaimDetails.DependenciesTree.ClickManualAction();

                pageClaimCommunication.WaitForPage();
                pageClaimCommunication.SelectEFTForm();
            }

            MaintainEventFinish(browser);
            Reporting.Log($"Capturing state before logging out of Shield and closing browser", browser.Driver.TakeSnapshot());
            browser.LogoutShieldAndCloseBrowser();

        }

    }
}
