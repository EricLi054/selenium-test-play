using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System;
using System.Collections.Generic;
using System.Data;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.MotorcycleQuote;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Spark.NewBusiness
{
    [TestFixture]
    [Property("Functional", "Motorcycle tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MotorcycleFieldValidation : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Regression tests for motorcycle new business.");
        }
        
        /// <summary>
        /// Verifying field validation on the "Are you a member?"
        /// form for motorcycle online quotes.
        /// </summary>
        /// <param name="testContact"></param>
        [Test, Description("MCO AreYouAMember FieldValidations"), TestCaseSource("BuildTestDataAreYouAMemberPageFieldValidations")]
        [Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle)]
        public void INSU_T325_MCO_AreYouAMember_FieldValidations(Contact testContact)
        {
            /***********************************************************
             * Open SPARK launch page.
             ***********************************************************/
            LaunchPage.OpenSparkMotorcycleLandingPage(_browser);

            /***********************************************************
             * Select RAC Member asNo
             ***********************************************************/

            try
            {
                ActionsQuoteMotorcycle.SelectRACMember(_browser, testContact);
            }
            catch (ReadOnlyException)
            {
                // We'll eat this exception as this is expected for invalid data
            }

            VerifyBeforeWeGetStartedFields(testContact);
        }

        /// <summary>
        /// Verifying the sum insured limits on the premium quote page
        /// for motorcycle online. Verifying that +-30% is successful,
        /// but validation is triggered at +-31%.
        /// </summary>
        /// <param name="testQuoteData"></param>
        [Test, TestCaseSource("BuildTestDataQuotePageSumInsuredFieldValidations")]
        [Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle)]
        public void MCO_QuotePage_SumInsured_FieldValidations(QuoteMotorcycle testQuoteData)
        {
            // Using the following min and max values, to ensure that we will get a motorcycle
            // which will still be within the acceptable limit of 500-200,000 after a +/-31%
            // variation to value.
            var randomBike = MotorCycleBuilder.GetRandomVehicle(725, 152671);
            testQuoteData.Make        = randomBike.Make;
            testQuoteData.Model       = randomBike.Model;
            testQuoteData.Year        = randomBike.Year;
            testQuoteData.EngineCC    = randomBike.EngineCC;
            testQuoteData.MarketValue = randomBike.MarketValue;

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, testQuoteData);

            ActionsQuoteMotorcycle.AdjustQuoteParametersRemainOnQuotePage(_browser, testQuoteData);

            VerifyQuoteSumInsuredFields(testQuoteData);
        }

        private static IEnumerable<TestCaseData> BuildTestDataAreYouAMemberPageFieldValidations()
        {
            // NOTE: statically coded data is used here due to the way
            // NUnit works with coded data sources. Randomised data
            // will prevent the test runner from running and identifying
            // individual elements in a dataset.


            yield return new TestCaseData(new Contact()
            {
                FirstName = null,
                DateOfBirth = DateTime.MinValue,
                MobilePhoneNumber = null,
                PrivateEmail = null,
                MembershipNumber = "123",
                MembershipTier = MembershipTier.Red
            }).SetName("MCO_AreYouAMember_FieldValidation_BlankFieldValidation");

            yield return new TestCaseData(new Contact()
            {
                FirstName = "a123",
                DateOfBirth = DateTime.Now.AddYears(-16).AddDays(1),
                MobilePhoneNumber = "000",
                PrivateEmail = new Email("bob"),
                MembershipNumber = "123",
                MembershipTier = MembershipTier.Red
            }).SetName("MCO_AreYouAMember_FieldValidation_InvalidValueValidation");

            yield return new TestCaseData(new Contact()
            {
                FirstName = "a_b \"cd\"",
                DateOfBirth = DateTime.Now.AddYears(-101),  // Workaround for PROD bug: SPK-2056
                MobilePhoneNumber = "1400123123",
                PrivateEmail = new Email("bob@cat"),
                MembershipNumber = "123",
                MembershipTier = MembershipTier.Red
            }).SetName("MCO_AreYouAMember_FieldValidation_SpecialCharecterValidation");
        }

        private static IEnumerable<TestCaseData> BuildTestDataQuotePageSumInsuredFieldValidations()
        {
            // NOTE: statically coded data is used here due to the way
            // NUnit works with coded data sources. Randomised data
            // will prevent the test runner from running and identifying
            // individual elements in a dataset.

            // Motorcycle details are not being detailed here as they
            // need to be fetched at runtime for market value.
            // TEST MUST OVERRIDE BIKE DETAILS.

            // Setup test data
            var basePH = new Contact()
            {
                FirstName = "Fred",
                DateOfBirth = DateTime.Now.AddYears(-30),
                MobilePhoneNumber = "0400100100",
                MailingAddress = new Address()
                {
                    Suburb = "West Perth",
                    PostCode = "6005"
                },
                PrivateEmail = null
            };

            var bikeRating = new QuoteMotorcycle()
            {
                CoverType = MotorCovers.MFCO,
                UsageType = MotorcycleUsage.Private,
                AnnualKm = AnnualKms.UpTo10000,
                IsGaraged = false,
                ParkingAddress = basePH.MailingAddress,
                Financier = null,
                Drivers = new List<Driver>()
                {
                    new Driver()
                    {
                        Details = basePH,
                        LicenseTime = "3+"
                    }
                },
                InsuredVariance = 31
            };

            yield return new TestCaseData(bikeRating).SetName("MCO_QuotePage_SumInsured_FieldValidation_VarienceOf31PercentOverValueShouldTriggerErrorValidation");

            bikeRating.InsuredVariance = 30;
            yield return new TestCaseData(bikeRating).SetName("MCO_QuotePage_SumInsured_FieldValidation_VarienceOf30PercentOverValueShouldNOTTriggerErrorValidation");

            bikeRating.InsuredVariance = -31;
            yield return new TestCaseData(bikeRating).SetName("MCO_QuotePage_SumInsured_FieldValidation_VarienceOf31PercentUnderValueShouldTriggerErrorValidation");

            bikeRating.InsuredVariance = -30;
            yield return new TestCaseData(bikeRating).SetName("MCO_QuotePage_SumInsured_FieldValidation_VarienceOf30PercentUnderValueShouldNotTriggerErrorValidation");
        }

        private void VerifyBeforeWeGetStartedFields(Contact inputContact)
        {
            using (var pageControl = new BeforeWeGetStarted(_browser))
            {
                if (DataHelper.NameStringHasInvalidCharacters(inputContact.FirstName))
                    Reporting.IsTrue(pageControl.IsFirstNameErrorValidationTriggered(), $"field validation to trigger for contact name: \"{inputContact.FirstName}\"");
                else
                    Reporting.IsTrue(pageControl.IsFirstNameErrorValidationTriggered(), $"contact name: \"{inputContact.FirstName}\" is accepted.");

                if (inputContact.IsValidAgeAsPolicyholder())
                    Reporting.IsTrue(pageControl.IsDoBErrorValidationTriggered(), $"field validation to trigger for birthdate: \"{inputContact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}\"");
                else
                    Reporting.IsTrue(pageControl.IsDoBErrorValidationTriggered(), $"birthdate: \"{inputContact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}\" is accepted.");

                if (DataHelper.MobileNumberHasInvalidCharactersOrInvalidlength(inputContact.MobilePhoneNumber))
                    Reporting.IsTrue(pageControl.IsMobileNumberErrorValidationTriggered(), $"field validation to trigger for mobile number: \"{inputContact.MobilePhoneNumber}\"");
                else
                    Reporting.IsTrue(pageControl.IsMobileNumberErrorValidationTriggered(), $"mobile number: \"{inputContact.MobilePhoneNumber}\" is accepted.");

                var email = inputContact.PrivateEmail != null ? inputContact.PrivateEmail.Address : "";
                if (DataHelper.IsEmailInvalidFormat(inputContact.PrivateEmail))
                    Reporting.IsTrue(pageControl.IsEmailErrorValidationTriggered(), $"field validation to trigger for email: \"{email}\"");
                else
                    Reporting.IsTrue(pageControl.IsEmailErrorValidationTriggered(), $"email: \"{email}\" is accepted.");
            }
        }

        private void VerifyQuoteSumInsuredFields(QuoteMotorcycle quoteDetails)
        {
            using(var pageControl = new HeresYourQuote(_browser))
            {
                if (quoteDetails.InsuredVariance > 30 ||
                    quoteDetails.InsuredVariance < -30)
                {
                    Reporting.IsTrue(pageControl.IsSumInsuredValidationTriggered(quoteDetails.CoverType, quoteDetails.MarketValue), $"field validation to triggered for variance of: \"{quoteDetails.InsuredVariance}\"");
                }
                else
                {
                    Reporting.IsFalse(pageControl.IsSumInsuredValidationTriggered(quoteDetails.CoverType, quoteDetails.MarketValue), $"sum insured variance of: \"{quoteDetails.InsuredVariance}\" is accepted.");
                }
            }
        }
    }
}
