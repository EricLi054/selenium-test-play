using Rac.TestAutomation.Common;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.CaravanQuote;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace Tests.ActionsAndValidations
{
    public static class ActionsQuoteCaravan
    {
        //This is the percentage variance allowed from the market value,
        //when the user provides a custom value for the 'Choose insured value' on 'Here's your quote' page.
        public const int MAX_SUM_INSURED_PERCENTAGE     = 30;

        /// <summary>
        /// Supports Spark Caravan
        /// Creates a new caravan quote on spark caravan and set the quote number and premium values,
        /// into the test data object (under child property QuoteData),
        /// which will be visible to caller after this method exits
        /// </summary>
        public static void CreateNewCaravanQuote(Browser browser, QuoteCaravan quote)
        {
            LaunchPage.OpenSparkCaravanLandingPage(browser);

            //Page 1: Before we get started
            SelectRACMember(browser, quote.PolicyHolders[0]);

            //Page 2: Let's start with your caravan
            LetsStartWithYourCaravan(browser, quote);
            ProvideValueOfCaravan(browser, quote);

            //Page 3: Tell us more about your <Caravan>
            TellUsMoreAboutYourCaravan(browser, quote);

            //Page 4: Now, a bit about the policyholders
            NowABitAboutThePolicyholders(browser, quote);

            //Page 5: Here's your quote
            HeresYourQuote(browser, quote);
            
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Change the Excess (if available), Vehicle Sum Insured and Contents Sum Insured values in the Here's Your Quote page
        /// </summary>
        public static void ChangeQuoteParametersAndComparePremiums(Browser browser, QuoteCaravan quote)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                heresYourQuote.PaymentFrequency = quote.PayMethod.PaymentFrequency;

                heresYourQuote.ChangeExcess(quote);

                heresYourQuote.ChangeInsuredValueBasedOnPercentage(quote.MarketValue, quote.InsuredVariance);

                heresYourQuote.ChangeContentsInsuredValue(quote.ContentsSumInsured);
            }
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Click 'Get Policy' button on 'Here's your quote' page,
        /// to confirm the quote customizations
        /// </summary>
        public static void ConfirmQuote(Browser browser)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                Reporting.Log("Capturing 'Your quote' before selecting Get Policy :", browser.Driver.TakeSnapshot());
                heresYourQuote.ClickGetPolicy();
            }
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Verification of content in 'Before We Get Started' page
        /// </summary>
        public static void PageVerificationBeforeWeGetStarted(Browser browser)
        {
            LaunchPage.OpenSparkCaravanLandingPage(browser);

            using (var beforeWeGetStarted = new BeforeWeGetStarted(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish();
                }

                beforeWeGetStarted.WaitForPage();

                beforeWeGetStarted.VerifyGeneralPageContent();

                beforeWeGetStarted.VerifyCaravanPageContent();
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Answers the "Are you a member?" question.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact"></param>
        public static void SelectRACMember(Browser browser, Contact contact)
        {
            using (var beforeWeGetStarted = new BeforeWeGetStarted(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: beforeWeGetStarted);
                }

                browser.PercyScreenCheck(CaravanNewBusiness.SelectRACMember);
                beforeWeGetStarted.SelectAreYouAnRACMember(contact);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Answers the Page 2: "Let's Start With Your Caravan" questions.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteCaravan"></param>
        public static void LetsStartWithYourCaravan(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var letsStartWithYourCaravan = new LetsStartWithYourCaravan(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: letsStartWithYourCaravan);
                browser.PercyScreenCheck(CaravanNewBusiness.LetsStartWithYourCaravan);
                letsStartWithYourCaravan.SearchForCaravan(quoteCaravan);
                quoteCaravan.ModelDescription = letsStartWithYourCaravan.ModelSelected.ToUpper();
                Reporting.Log("Capturing 'Let's Start With Your Caravan' before selecting Next :", browser.Driver.TakeSnapshot());
                letsStartWithYourCaravan.ClickNext();
            }
        }

        public static void ProvideValueOfCaravan(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var letsStartWithYourCaravan = new LetsStartWithYourCaravan(browser))
            {
                var valueOfCaravanFieldDisplayed = quoteCaravan.MarketValue < CARAVAN_MIN_SUM_INSURED_VALUE;

                if (valueOfCaravanFieldDisplayed)
                    letsStartWithYourCaravan.EnterValueOfCaravan(quoteCaravan.SumInsuredValue);
                else
                    letsStartWithYourCaravan.ClickNext();
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Answers the 'value of caravan' question on
        /// Page 2: "Let's Start With Your Caravan" with invalid boundary values
        /// (to verify error messages) and finally a valid value
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteCaravan"></param>
        public static void ProvideValueOfCaravanValidAndInvalid(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var letsStartWithYourCaravan = new LetsStartWithYourCaravan(browser))
            {
                //Enter invalid lower boundary value to verify validation message
                letsStartWithYourCaravan.EnterValueOfCaravan(CARAVAN_MIN_SUM_INSURED_VALUE - 1);
                letsStartWithYourCaravan.VerifyValueOfCaravanValidationMessages();

                //Enter invalid upper boundary value to verify 'Please call us' message
                letsStartWithYourCaravan.EnterValueOfCaravan(CARAVAN_MAX_SUM_INSURED_VALUE + 1);
                letsStartWithYourCaravan.VerifyValueOfCaravanCallUsToContinueMessage();

                //Enter a valid value
                letsStartWithYourCaravan.EnterValueOfCaravan(quoteCaravan.SumInsuredValue);

                Reporting.Log("Page 2: Let's Start With Your Caravan: Enter a valid value for the 'Value of caravan' text field", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Answers the Page 3: "Tell Us More About Your <Caravan>" questions.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteCaravan"></param>
        public static void TellUsMoreAboutYourCaravan(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var tellUsMoreAboutYourCaravan = new TellUsMoreAboutYourCaravan(browser))
            {
                tellUsMoreAboutYourCaravan.WaitForPage();
                browser.PercyScreenCheck(CaravanNewBusiness.TellUsMoreAboutYourCaravan, tellUsMoreAboutYourCaravan.GetPercyIgnoreCSS());
                tellUsMoreAboutYourCaravan.FillAdditionalInformationAndVerifyKnockouts(quoteCaravan);
                Reporting.Log("Capturing 'Tell us more about your (Caravan Make)' before selecting Next :", browser.Driver.TakeSnapshot());
                tellUsMoreAboutYourCaravan.ClickNext();
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Answers the Page 4: "Now, a bit about the policyholders" questions.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteCaravan"></param>
        public static void NowABitAboutThePolicyholders(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var nowABitAboutThePolicyholders = new NowABitAboutThePolicyholders(browser))
            {
                nowABitAboutThePolicyholders.WaitForPage();
                browser.PercyScreenCheck(CaravanNewBusiness.NowABitAboutThePolicyholder);
                nowABitAboutThePolicyholders.FillPolicyholderInformation(quoteCaravan);
                Reporting.Log("Capturing 'About the policyholders' immediately before selecting View Quote", browser.Driver.TakeSnapshot());
                nowABitAboutThePolicyholders.ClickNext();
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Page 5: "Here's your quote" page.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteCaravan"></param>
        public static void HeresYourQuote(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                heresYourQuote.WaitForPage();
                browser.PercyScreenCheck(CaravanNewBusiness.HeresYourQuote, heresYourQuote.GetPercyIgnoreCSS());
                heresYourQuote.CloseOffRoadCoverPopupIfDisplayed();
                heresYourQuote.VerifyQuotePageLabels(quoteCaravan);
                ChangeQuoteParametersAndComparePremiums(browser, quoteCaravan);
                heresYourQuote.SaveQuoteDetailsAndComparePremiums(quoteCaravan);
                if (quoteCaravan.RetrieveQuote.HasValue && (quoteCaravan.RetrieveQuote.Value == RetrieveQuoteType.Email))
                {
                    spinner.WaitForSpinnerToFinish();
                    heresYourQuote.SendEmailQuote(quoteCaravan);
                }                

                //If the policy holder is a existing member then skip the validation
                if (!quoteCaravan.PolicyHolders[0].IsRACMember && !quoteCaravan.PolicyHolders[0].IsMultiMatchRSAMember)
                {
                    VerifyQuoteCaravan.VerifyQuoteViaShieldAPI(quoteCaravan, SparkBasePage.QuoteStage.AFTER_QUOTE);
                }
            }
        }

        public static void ProceedWithQuoteToPurchase(Browser browser, QuoteCaravan quoteCaravan, bool detailUIChecking = false)
        {
            ConfirmQuote(browser);

            //Page 6: Great, let's set a start date and handles the Popup (Your quote has been updated)
            SetPolicyStartDate(browser, quoteCaravan);

            //Page 7.1: Tell us more about you
            ProvidePersonalInformationMainPH(browser, quoteCaravan);

            //Page 7.2: Tell us more about your joint policyholder
            if (quoteCaravan.PolicyHolders.Count > 1)
            {
                //Provide personal information for Joint PH, only if one exists
                ProvidePersonalInformationJointPH(browser, quoteCaravan);
            }

            VerifyQuoteCaravan.VerifyQuoteViaShieldAPI(quoteCaravan, SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO);
                        
            //Page 8: Payment
            ProvidePaymentDetails(browser, quoteCaravan, detailUIChecking);
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Answers 'When would you like your policy to start?' question
        /// in 'Great, let's set a start date.' page.
        /// </summary>
        public static void SetPolicyStartDate(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var greatLetsSetAStartDate = new GreatLetsSetAStartDate(browser))
            {
                greatLetsSetAStartDate.WaitForPage();
                browser.PercyScreenCheck(CaravanNewBusiness.SetStartDate, greatLetsSetAStartDate.GetPercyIgnoreCSS());
                greatLetsSetAStartDate.VerifyPageContent();
                greatLetsSetAStartDate.SetStartDate(quoteCaravan);
            }
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Answers personal information questions in 'Tell us more about you' page for the main Policyholder.
        /// A premium change popup will occur when the quote has only one policyhlder, and when the member (Single or Multi Matched),
        /// has a membership tier that makes them eligible for a discount (i.e. Gold, Silver or Bronze),
        /// skipped declaring membership on Page 1 (Are you an RAC member), but gets matched (Single or Multi) on 'Tell us more about you' page.
        /// </summary>
        public static void ProvidePersonalInformationMainPH(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var tellUsMoreAboutYou = new TellUsMoreAboutYou(browser))
            using (var membershipLevel    = new MembershipLevel(browser))
            using (var beforeWeGetStarted = new BeforeWeGetStarted(browser))
            {
                browser.PercyScreenCheck(CaravanNewBusiness.TellUsMoreAboutYou);
                var mainPH = quoteCaravan.PolicyHolders[0];

                bool isMainPHEligibleForDiscountButSkippedDeclaringMembership = mainPH.SkipDeclaringMembership && beforeWeGetStarted.IsEligibleForDiscount(mainPH.MembershipTier);

                tellUsMoreAboutYou.WaitForPage();

                tellUsMoreAboutYou.VerifyPageContent(mainPH, quoteCaravan.RetrieveQuote);

                tellUsMoreAboutYou.FillPersonalInformation(mainPH);

                if (mainPH.SkipDeclaringMembership && mainPH.IsMultiMatchRSAMember)
                {
                    //When a Multi-Match main policyholder, skipped declaring membership on Page 1 ('Are you an RAC member'),
                    //but gets Multi-Matched on this page, then we ask the membership tier to determine the applicable discounts.
                    membershipLevel.SetMembershipAndClickNext(mainPH.MembershipTier);

                    using (var spinner = new SparkSpinner(browser))
                    { spinner.WaitForSpinnerToFinish(); }
                }

                if (isMainPHEligibleForDiscountButSkippedDeclaringMembership)
                {
                    //Business Rule:->'Display the premium change pop-ups on every premium change'
                    tellUsMoreAboutYou.VerifyAnyPremiumChangePopup(quoteCaravan);
                }
                else 
                {
                    //Where the user declares membership in "before we get started" and is eligible for discount; and do not have a joint policy holder
                    tellUsMoreAboutYou.VerifyNoPremiumPopupIsDisplayed();
                }
            }
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Answers personal information questions in 'Tell us more about your joint policyholder' page for the Joint Policyholder.
        /// A premium change popup will occur on this page based on the following rules:
        /// Rule 1-> 'Where there are 2 PH's then do not display the premium change until we have both PH's details.'
        ///          This rule is implemented in 'ProvidePersonalInformationMainPH' method as well as in this method.
        /// Rule 2-> 'After entering both policyholders details if the premium increases then show the annual or monthly increased premium change box'.
        ///          (This rule is not applicable for Automation since we don't cover browser back button clicks in automation)
        /// Rule 3-> 'After entering both policyholders details if the premium reduces for PH1 as they have matched(single or multi) with a discount tier then show annual or monthly and the PH1 reduced premium change box.'
        /// Rule 4-> 'After entering both policyholders details if the premium reduces for PH2 as they have matched(single or multi) with a discount tier that is higher than PH1 then show annual or monthly the PH2 reduced premium change box'
        /// Rule 5-> 'If the premium has been reduced and the member selected annual on the quote page then show the annual premium reduced modal for the correct policyholder.'
        /// Rule 6-> 'If the premium has been reduced and the member selected monthly on the quote page then show the monthly premium reduced modal for the correct policyholder.'
        /// Rule 7-> 'If there is no change to the premium after both members have entered their personal details then continue onto the payment step.
        ///          This rule is implemented in this method.    
        /// Rule 8-> 'If the premium has decreased as a result of PH2 membership tier then include the text '<PH2 first name> qualified for a member discount so your policy price has reduced'
        /// </summary>
        public static void ProvidePersonalInformationJointPH(Browser browser, QuoteCaravan quoteCaravan)
        {
            using (var tellUsMoreAboutYourJointPH = new TellUsMoreAboutYourJointPH(browser))
            using (var membershipLevel = new MembershipLevel(browser))
            using (var beforeWeGetStarted = new BeforeWeGetStarted(browser))
            {
                var mainPolicyholder = quoteCaravan.PolicyHolders[0];
                var jointPolicyholder = quoteCaravan.PolicyHolders[1];
                
                bool isMainPHEligibleForDiscount  = mainPolicyholder.SkipDeclaringMembership && beforeWeGetStarted.IsEligibleForDiscount(mainPolicyholder.MembershipTier);
                bool isJointPHEligibleForDiscount = beforeWeGetStarted.IsEligibleForDiscount(jointPolicyholder.MembershipTier) && ((int)jointPolicyholder.MembershipTier > (int)mainPolicyholder.MembershipTier);

                tellUsMoreAboutYourJointPH.WaitForPage();

                tellUsMoreAboutYourJointPH.FillPersonalInformation(jointPolicyholder);

                if (jointPolicyholder.IsMultiMatchRSAMember && mainPolicyholder.MembershipTier != MembershipTier.Gold)
                {
                    //When the Joint policyholder is a Multi-Match member, we ask their membership tier on this page to determine the applicable discounts,
                    //if the main policyholder is not already having the highest (i.e. Gold) membership.
                    membershipLevel.SetMembershipAndClickNext(jointPolicyholder.MembershipTier);

                    using (var spinner = new SparkSpinner(browser))
                    { spinner.WaitForSpinnerToFinish(); }
                }

                if (isJointPHEligibleForDiscount && jointPolicyholder.MembershipTier > mainPolicyholder.MembershipTier)
                {
                    //If either of the policyholders is eligible for a discount, we display the premium change popup.
                    //This also caters for the Rule 1: 'Where there are 2 PH's then do not display the premium change until we have both PH's details.'
                    tellUsMoreAboutYourJointPH.VerifyAnyPremiumChangePopup(quoteCaravan);
                }
            }
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Answers payment questions in 'Payment' page.
        /// </summary>
        public static void ProvidePaymentDetails(Browser browser, QuoteCaravan quoteCaravan,bool detailUIChecking = false)
        {
            using (var paymentDetails = new PaymentDetails(browser))
            {
                paymentDetails.WaitForPage();
                browser.PercyScreenCheck(CaravanNewBusiness.EnterPaymentDetailsAndPurchasePolicy, paymentDetails.GetPercyIgnoreCSS());

                paymentDetails.VerifyPolicySummary(quoteCaravan);

                paymentDetails.VerifyPaymentAmount(quoteCaravan);

                if (detailUIChecking)
                {
                    paymentDetails.EnterInvalidNoMatchBSBAndCheckErrorMessage(quoteCaravan.PayMethod);
                }

                paymentDetails.EnterPaymentDetailsAndPurchasePolicy(Vehicle.Caravan, quoteCaravan.PayMethod);
            }
        }

        /// <summary>
        /// Verify Retrieved Quote
        /// Discounts | Excess defaults or no excess panel | Payment Frequency | Premium | Agreed value | Contents cover
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quote"></param>
        public static void VerifyHeresYourQuoteAfterRetrieve(Browser browser, QuoteCaravan quote)
        {
            using(var heresYourQuote = new HeresYourQuote(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: heresYourQuote);
                heresYourQuote.CloseOffRoadCoverPopupIfDisplayed();
                heresYourQuote.VerifyQuoteValuesAfterRetrieve(quote, browser);
            }
        }
        public static void CloseBrowserAndWait(Browser browser, int waitTimes = 5000)
        {
            browser.CloseBrowser();
            Thread.Sleep(waitTimes);
        }

        public static string GetPolicyNumberFromConfirmationPage(Browser browser)
        {
            string policyNo;
            using (var confirmationPage = new ConfirmationPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: confirmationPage);
                confirmationPage.WaitForPage();
                Reporting.Log("Confirmation Page:", browser.Driver.TakeSnapshot());
                browser.PercyScreenCheck(CaravanNewBusiness.ConfirmationPage, confirmationPage.GetPercyIgnoreCSS());
                policyNo = confirmationPage.PolicyNumber;
            }

            return policyNo;
        }
    }
}
