using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Endorsements;
using UIDriver.Pages;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Spark.Endorsements
{
    [Property("Functional", "Spark Update How You Pay")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class UpdateHowYouPay : BaseUITest
    {

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Update How You Pay Tests");
        }

        #region TestCases
        /// <summary>      
        /// Covers the scenario of a member who has a motor policy paid monthly by Credit Card, 
        /// and reschedules the next instalment paid by new credit card.
        /// </summary>
        [Test(Description = "Reschedule Payment for existing policy paid by credit card")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.InsuranceContactService)]
        public void INSU_T111_MotorPolicyRescheduleFromOldCreditCardToNewCreditCard()
        {
            var testData = BuildTestDataForMotorPolicyReschedule_CreditCard();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);

            VerifyUpdateHowYouPay.VerifyDetailsPageLoaded(_browser, testData);

            ActionsUpdateHowYouPay.UpdateNextPaymentDate(_browser, testData.NextPaymentDate);
            ActionsUpdateHowYouPay.UpdateEmailAddress(_browser, testData.ActivePolicyHolder);
            // Existing credit card is recorded at CreditCards[1] - a newly generated credit card occupies [0] 
            ActionsUpdateHowYouPay.AddNewCreditCard(_browser, testData.ActivePolicyHolder.CreditCards[0]);

            // Checking the authorisation text after adding new payment.
            VerifyUpdateHowYouPay.VerifyAuthorisationText(_browser);

            ActionsUpdateHowYouPay.AcceptTermsAndConditionsThenConfirm(_browser);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);

            VerifyUpdateHowYouPay.VerifyConfirmationPageWhenUsingCreditCard(_browser, testData);
            // Existing credit card is recorded at CreditCards[1] - a newly generated credit card occupies [0] 
            VerifyUpdateHowYouPay.VerifyUpdateOccursInShieldWithCreditCardAsSource(testData, testData.ActivePolicyHolder.CreditCards[0]);

            _browser.CloseBrowser();

            Reporting.LogMinorSectionHeading("Log back into payments view to verify changes are displayed.");
            // Check that updates are reflected when the page is loaded again, with the new credit card occupying CreditCards[0]
            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            VerifyUpdateHowYouPay.VerifyDetailsPageAfterUpdate(_browser, testData, testData.ActivePolicyHolder.CreditCards[0]);
        }

        /// <summary>       
        /// Covers the scenario of a member who has a home policy paid monthly by Bank Debit,
        /// and reschedules the next instalment by new bank account with BSB validation.
        /// </summary>
        [Test(Description = "Reschedule Payment for existing home policy and update bank account direct debit")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay), 
            Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.MultiFactorAuthentication)]
        public void INSU_T112_HomePolicyRescheduleFromOldBankAccountToNewBankAccount()
        {
            var testData = BuildTestDataForHomePolicyReschedule_BankAccount();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            VerifyUpdateHowYouPay.VerifyDetailsPageLoaded(_browser, testData);

            ActionsUpdateHowYouPay.UpdateNextPaymentDate(_browser, testData.NextPaymentDate);
            ActionsUpdateHowYouPay.UpdateEmailAddress(_browser, testData.ActivePolicyHolder,detailUIChecking:true);

            // Adds the randomly generated account by ContactBuilder at BankAccounts[0] as the new payment source 
            ActionsUpdateHowYouPay.AddNewBankAccount(_browser, testData.ActivePolicyHolder.BankAccounts[0], detailUIChecking:true);

            // Checking the authorisation text after adding new payment.
            VerifyUpdateHowYouPay.VerifyAuthorisationText(_browser);

            ActionsUpdateHowYouPay.AcceptTermsAndConditionsThenConfirm(_browser);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, detailUiChecking: true);
            
            VerifyUpdateHowYouPay.VerifyConfirmationPageWhenUsingBankAccount(_browser, testData);
            // Confirms the changes are updated in shield, the new randomly generated account is held at BankAccounts[0] 
            VerifyUpdateHowYouPay.VerifyUpdateOccursInShieldWithBankAccountAsSource(_browser, testData, testData.ActivePolicyHolder.BankAccounts[0]);

            _browser.CloseBrowser();

            Reporting.LogMinorSectionHeading("Log back into payments view to verify changes are displayed.");
            // Confirms accessing the site again includes the new randomly generated bank account
            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            VerifyUpdateHowYouPay.VerifyDetailsPageAfterUpdate(_browser, testData, testData.ActivePolicyHolder.BankAccounts[0]);
        }

        /// <summary>
        /// Cover the scenario where only the date is updated.
        /// </summary>
        [Test(Description = "Reschedule Payment for existing home policy paid by bank account direct debit")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay), Category(TestCategory.Home), Category(TestCategory.Spark)]
        public void INSU_T110_HomePolicyRescheduleChangeDateOnly()
        {
            var testData = BuildTestDataForHomePolicyReschedule_BankAccount();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);

            VerifyUpdateHowYouPay.VerifyDetailsPageLoaded(_browser, testData);

            ActionsUpdateHowYouPay.UpdateNextPaymentDate(_browser, testData.NextPaymentDate);
            ActionsUpdateHowYouPay.UpdateEmailAddress(_browser, testData.ActivePolicyHolder);

            ActionsUpdateHowYouPay.AcceptTermsAndConditionsThenConfirm(_browser);
            VerifyUpdateHowYouPay.VerifyConfirmationPageWhenUsingBankAccount(_browser, testData);
            // Existing bank account is recorded at BankAccounts[1] - a newly generated account occupies [0] 
            VerifyUpdateHowYouPay.VerifyUpdateOccursInShieldWithBankAccountAsSource(_browser, testData, testData.ActivePolicyHolder.BankAccounts[1]);

            _browser.CloseBrowser();

            Reporting.LogMinorSectionHeading("Log back into payments view to verify changes are displayed.");
            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            VerifyUpdateHowYouPay.VerifyDetailsPageAfterUpdate(_browser, testData, testData.ActivePolicyHolder.BankAccounts[1]);
        }

        /// <summary>
        /// For caravan policy updates the payment method from a bank account to a credit card 
        /// in addition to changing the date.
        /// </summary>
        [Test(Description = "Reschedule Payment for existing caravan policy paid by bank account direct debit to credit card")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        public void INSU_T109_CaravanPolicyChangeFromBankAccountToCreditCardAndReschedule()
        {
            var testData = BuildTestDataCaravanPolicyPaidViaBankAccountAndNewInstalmentDate();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);

            VerifyUpdateHowYouPay.VerifyDetailsPageLoaded(_browser, testData);

            ActionsUpdateHowYouPay.UpdateNextPaymentDate(_browser, testData.NextPaymentDate);
            ActionsUpdateHowYouPay.UpdateEmailAddress(_browser, testData.ActivePolicyHolder);

            // Adds the randomly generated account by ContactBuilder at CreditCard[0] as the new payment source 
            ActionsUpdateHowYouPay.AddNewCreditCard(_browser, testData.ActivePolicyHolder.CreditCards[0]);

            // Checking the authorisation text after adding new payment.
            VerifyUpdateHowYouPay.VerifyAuthorisationText(_browser);

            ActionsUpdateHowYouPay.AcceptTermsAndConditionsThenConfirm(_browser);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);

            VerifyUpdateHowYouPay.VerifyConfirmationPageWhenUsingCreditCard(_browser, testData);
            VerifyUpdateHowYouPay.VerifyUpdateOccursInShieldWithCreditCardAsSource(testData, testData.ActivePolicyHolder.CreditCards[0]);

            _browser.CloseBrowser();

            Reporting.LogMinorSectionHeading("Log back into payments view to verify changes are displayed.");
            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            VerifyUpdateHowYouPay.VerifyDetailsPageAfterUpdate(_browser, testData, testData.ActivePolicyHolder.CreditCards[0]);
        }

        /// <summary>
        /// Motor Cycle policy updates the payment method from a credit card to a bank account  
        /// in addition to changing the date and email id.
        /// </summary>
        [Test(Description = "Reschedule Payment for existing motor cycle policy paid by credit card to bank account direct debit")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay), Category(TestCategory.Motorcycle), Category(TestCategory.Spark)]
        public void INSU_T113_MotorCyclePolicyChangeFromCreditCardToBankAccountAndReschedule()
        {
            var testData = BuildTestDataForMotorCyclePolicyReschedule_CreditCardToBankAccount();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);

            VerifyUpdateHowYouPay.VerifyDetailsPageLoaded(_browser, testData);

            ActionsUpdateHowYouPay.UpdateNextPaymentDate(_browser, testData.NextPaymentDate);
            ActionsUpdateHowYouPay.UpdateEmailAddress(_browser, testData.ActivePolicyHolder);
          
            // Adds the randomly generated account by ContactBuilder at BankAccount[0] as the new payment source 
            ActionsUpdateHowYouPay.AddNewBankAccount(_browser, testData.ActivePolicyHolder.BankAccounts[0]);

            // Checking the authorisation text after adding new payment.
            VerifyUpdateHowYouPay.VerifyAuthorisationText(_browser);

            ActionsUpdateHowYouPay.AcceptTermsAndConditionsThenConfirm(_browser);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);

            VerifyUpdateHowYouPay.VerifyConfirmationPageWhenUsingBankAccount(_browser, testData);

            // Confirms the changes are updated in shield, the new randomly generated account is held at BankAccounts[0] 
            VerifyUpdateHowYouPay.VerifyUpdateOccursInShieldWithBankAccountAsSource(_browser, testData, testData.ActivePolicyHolder.BankAccounts[0]);

            _browser.CloseBrowser();

            Reporting.LogMinorSectionHeading("Log back into payments view to verify changes are displayed.");
            // Confirms accessing the site again includes the new randomly generated bank account
            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            VerifyUpdateHowYouPay.VerifyDetailsPageAfterUpdate(_browser, testData, testData.ActivePolicyHolder.BankAccounts[0]);
        }
        /// <summary>
        /// For motor policy change annual direct debit from bank account to new credit card
        /// </summary>
        [Test(Description = "Change Payment for existing motor policy paid by bank account direct debit to credit card")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay), Category(TestCategory.Motor), Category(TestCategory.Spark)]
        public void INSU_T108_MotorPolicyChangeAnnualDirectDebitFromBankAccountToCreditCard()
        {
            var testData = BuildTestDataMotorPolicyAnnualDirectDebit();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);

            VerifyUpdateHowYouPay.VerifyDetailsPageLoaded(_browser, testData);

            // Adds the randomly generated account by ContactBuilder at CreditCard[0] as the new payment source 
            ActionsUpdateHowYouPay.AddNewCreditCard(_browser, testData.ActivePolicyHolder.CreditCards[0]);
            ActionsUpdateHowYouPay.UpdateEmailAddress(_browser, testData.ActivePolicyHolder);

            // Checking the authorisation text after adding new payment.
            VerifyUpdateHowYouPay.VerifyAuthorisationText(_browser);

            ActionsUpdateHowYouPay.AcceptTermsAndConditionsThenConfirm(_browser);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, retryOTP: true);

            VerifyUpdateHowYouPay.VerifyConfirmationPageWhenUsingCreditCard(_browser, testData);
            VerifyUpdateHowYouPay.VerifyUpdateOccursInShieldWithCreditCardAsSource(testData, testData.ActivePolicyHolder.CreditCards[0]);
        }


        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay)]
        [Test(Description = "MotorPolicyInRenewal_Redirect_To_RenewalPage, Update how you for a motor policy in renewal, it will redirect to the motor renewal flow and complete the renewal process")]
        public void INSU_T114_MotorPolicyInRenewal_Redirect_To_RenewalPage()
        {
            var testData = BuildMotorRenewalFromMonthlyCardMfcoToConvertToAnnualCash();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            ActionsUpdateHowYouPay.ReviewAndRenew(_browser, testData);
            ActionsMotorEndorsement.MotorRenewalFlowAfterLaunch(_browser, testData);

            //MFA for bank account input
            bool expectBankMFA = Config.Get().IsSparkEndorsementBankMultiFactorAuthenticationExpected();
            if (expectBankMFA)
            {
                ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, detailUiChecking: true);
            }
            else
            {
                Reporting.Log($"{SparkFeatureToggles.SparkEndorsementsBankExpectMultiFactorAuthentication.GetDescription()} = {expectBankMFA} so we won't expect it here.");
            }

            ActionsMotorEndorsement.VerifyConfirmationPage(_browser, testData);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyDetailsInShield(testData);

            //Verifying renewal details on PCM
            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseMotor.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.UpdateHowYouPay)]
        [Test(Description = "HomePolicyInRenewal_Block_Access_To_RenewalPage, Open update how you pay for home policy in renewal, it will display the error page and not allow to complete online")]
        public void INSU_T115_HomePolicyInRenewal_Block_Access_To_RenewalPage()
        {
            var testData = BuildHomeRenewal_Annual_Block_UHIP_Flow();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            LaunchPage.OpenUpdateHowYouPayByURL(_browser, testData);
            //Currently only motor policies are eligible for online renewal
            //for other policy types, it will display the blocking page
            VerifyUpdateHowYouPay.VerifyNotEligibleForOnlineUpdate(_browser, testData);
        }

        #endregion

        #region Test Case Helper Methods

        private EndorseCar BuildTestDataMotorPolicyAnnualDirectDebit()
        {
            var candidatePolicy = ShieldMotorDB.FindMotorPolicyNotInRenewal(false);


            var testData = new MotorEndorsementBuilder().InitialiseMotorCarWithDefaultData(candidatePolicy.PolicyNumber)
                                                        .Build();

            var currentPaymentAccount = ShieldPolicyDB.FetchNextInstalmentDetails(candidatePolicy.PolicyNumber);
            testData.PayMethod.Payer.BankAccounts.Add(currentPaymentAccount.BankAccount);
            testData.PayMethod = testData.PayMethod.BankAccount().Annual();

            return testData;
        }

        private EndorseCar BuildTestDataForMotorPolicyReschedule_CreditCard()
        {
            EndorseCar testData = null;
            var policies = ShieldPolicyDB.FindPolicyPaidMonthlyViaCreditCardForChangeHowYouPay(ShieldProductType.MGP);

            foreach (var policy in policies)
            {
                try
                {
                    Reporting.Log($"Evaluating candidate policy '{policy}'");
                    var policyDetails = DataHelper.GetPolicyDetails(policy);
                    var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                    if (policyHolder != null && policyHolder.MobilePhoneNumber != null)
                    {
                        testData = new MotorEndorsementBuilder().InitialiseMotorCarWithDefaultData(policy, policyHolder)
                                                            .WithNextPaymentDate(4)
                                                            .Build();

                        var currentPaymentAccount = ShieldPolicyDB.FetchNextInstalmentDetails(policy);
                        testData.PayMethod.Payer.CreditCards.Add(currentPaymentAccount.CreditCard);
                        testData.PayMethod = testData.PayMethod.CreditCard().Monthly();
                        break;
                    }
                }
                catch { /* If we have unusable data, throw away and move to next. */ }
            }

            Reporting.IsNotNull(testData, "Didn't find valid test data");
            return testData;
        }

        private EndorseMotorCycle BuildTestDataForMotorCyclePolicyReschedule_CreditCardToBankAccount()
        {
            EndorseMotorCycle testData = null;
            var policies = ShieldPolicyDB.FindPolicyPaidMonthlyViaCreditCardForChangeHowYouPay(ShieldProductType.MGC);

            foreach (var policy in policies)
            {
                try
                {
                    Reporting.Log($"Evaluating candidate policy '{policy}'");
                    var policyDetails = DataHelper.GetPolicyDetails(policy);
                    var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                    if (policyHolder != null && policyHolder.MobilePhoneNumber != null)
                    {
                        testData = new MotorCycleEndorsementBuilder().InitialiseMotorCycleWithDefaultData(policy, policyHolder)
                                                  .WithNextPaymentDate(5)
                                                  .Build();

                        var currentPaymentAccount = ShieldPolicyDB.FetchNextInstalmentDetails(policy);
                        testData.PayMethod.Payer.CreditCards.Add(currentPaymentAccount.CreditCard);
                        testData.PayMethod = testData.PayMethod.CreditCard().Monthly();
                        break;
                    }
                }
                catch { /* If we have unusable data, throw away and move to next. */ }
            }

            Reporting.IsNotNull(testData, "Didn't find valid test data");
            return testData;
        }

        private EndorseHome BuildTestDataForHomePolicyReschedule_BankAccount()
        {
            EndorseHome testData = null;
            var policies = ShieldPolicyDB.FindPolicyPaidMonthlyViaBankDebit(ShieldProductType.HGP);

            foreach (var policy in policies)
            {
                try
                {
                    Reporting.Log($"Evaluating candidate policy '{policy}'");
                    var policyDetails = DataHelper.GetPolicyDetails(policy);
                    var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                    if (policyHolder != null && policyHolder.MobilePhoneNumber != null)
                    {
                        testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policy, policyHolder)
                                                           .WithNextPaymentDate(3)
                                                           .Build();

                        var currentPaymentAccount = ShieldPolicyDB.FetchNextInstalmentDetails(policy);
                        testData.PayMethod.Payer.BankAccounts.Add(currentPaymentAccount.BankAccount);
                        testData.PayMethod = testData.PayMethod.BankAccount().Monthly();
                        break;
                    }
                }
                catch { /* If we have unusable data, throw away and move to next. */ }
            }

            Reporting.IsNotNull(testData, "Didn't find valid test data");
            return testData;
        }

        private EndorseCaravan BuildTestDataCaravanPolicyPaidViaBankAccountAndNewInstalmentDate()
        {
            EndorseCaravan testData = null;
            var policies = ShieldPolicyDB.FindPolicyPaidMonthlyViaBankDebit(ShieldProductType.MGV);

            foreach (var policy in policies)
            {
                try
                {
                    Reporting.Log($"Evaluating candidate policy '{policy}'");
                    var policyDetails = DataHelper.GetPolicyDetails(policy);
                    var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                    if (policyHolder != null && policyHolder.MobilePhoneNumber != null)
                    {
                        testData = new CaravanEndorsementBuilder().InitialiseCaravanWithDefaultData(policy, policyHolder)
                                                              .WithNextPaymentDate(5)
                                                              .Build();

                        var currentPaymentAccount = ShieldPolicyDB.FetchNextInstalmentDetails(policy);
                        testData.PayMethod.Payer.BankAccounts.Add(currentPaymentAccount.BankAccount);
                        testData.PayMethod = testData.PayMethod.BankAccount().Monthly();
                        break;
                    }
                }
                catch { /* If we have unusable data, throw away and move to next. */ }
            }

            Reporting.IsNotNull(testData, "Didn't find valid test data");
            return testData;
        }

        private EndorseCar BuildMotorRenewalFromMonthlyCardMfcoToConvertToAnnualCash()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForRenewal(motorCover: MotorCovers.MFCO, hasRiskAddress: true, paymentScenarios: PaymentScenario.MonthlyCard);

            var paymentAccount = new PaymentV2()
                .BankAccount(new BankAccount().InitWithRandomValues())
                .Monthly()
                .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(false)
                .WithRandomNewCar()
                .WithExcess("$1500")
                .WithAnnualKms(AnnualKms.UpTo10000)                
                .WithUsage(usage: VehicleUsage.Private)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseHome BuildHomeRenewal_Annual_Block_UHIP_Flow()
        {
            EndorseHome testData = null;
            var policies = ShieldHomeDB.FindHomePolicyForRenewal(PaymentFrequency.Annual);
            foreach (var policy in policies)
            {
                if (!ShieldPolicyDB.IsPolicySuitableForEndorsements(policy))
                { continue; }

                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (policyHolder != null)
                {
                    testData = new HomeEndorsementBuilder()
                                .InitialiseHomeWithDefaultData(policy, policyHolder)
                                .WithExpectedImpactOnPremium(PremiumChange.NotApplicable)                                
                                .Build();
                    break;
                }
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }
        #endregion
    }
}
