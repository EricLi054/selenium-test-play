using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

public class SparkBoatConfirmation : SparkBasePage
{
    #region Constants
    private class Constants
    {
        public static readonly string PageHeader        = "Welcome aboard, ";
        public static readonly string ActiveStepper     = "Confirmation";
        public static readonly string EmailAdvice       = "You'll receive an email with your policy documents shortly.";
        public static readonly string MyRacHeader       = "Your myRAC account";
        public static readonly string MyRacSubHeader    = "With myRAC, you can manage your insurance policies any time you like.";

        public class Button
        {
            public static readonly string CallToActionMyRac = "Log in or register for myRAC";
            public static readonly string CallToActionHome  = "Go to RAC homepage";
            public static class Link
            {
                public static readonly string RacHomepageUrl  = "https://cdvnetd.ractest.com.au";
            }
        }
    }
    #endregion

    #region XPATHS
    private class XPath
    {
        public static readonly string PageHeader      = "//*[@id='header']";
        public static readonly string ActiveStepper   = "//button[@aria-selected='true']";
        public static readonly string EmailAdvice     = "id('subHeader')";
        public static readonly string PolicyNumber    = "//p[@data-testid='policyNumberLabel']"; //TODO Ask for an ID
        public static readonly string ReceiptNumber   = "//p[@data-testid='receiptNumberLabel']"; //TODO Ask for an ID? (SWAG)
        public static readonly string AmountPaid      = "//p[@data-testid='paymentAmount']"; //TODO Ask for an ID? (SWAG)
        public static readonly string MyRacHeader     = "id('myRacHeader')";
        public static readonly string MyRacSubHeader  = "myRacHeader"; //TODO Ask for an ID

        public class Button
        {
            public static readonly string CallToActionMyRac   = "//button[@id='loginOrRegisterLinkButton']";
            public static readonly string CallToActionHome    = "//button[@id='racHomePageLinkButton']";
            public class Link
            {
                public static readonly string MyRacUrl    = "//div[@id='loginOrRegisterLink']";
                public static readonly string HomepageUrl = "//div[@id='racHomePageLink']";
            }
        }

    }
    #endregion

    #region Settable properties and controls

    #endregion
    public SparkBoatConfirmation(Browser browser) : base(browser)
    { }

    public override bool IsDisplayed()
    {
        var isDisplayed = false;
        try
        {
            GetElement(XPaths.Header.PhoneNumber);
            Reporting.Log($"Found Confirmation XPaths.Header.PhoneNumber successfully");
            isDisplayed = string.Equals(Constants.EmailAdvice, GetInnerText(XPath.EmailAdvice));
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        if (isDisplayed)
        { Reporting.LogPageChange("Spark Boat Quote page ? - Confirmation"); }

        return isDisplayed;
    }

    public void VerifyPageContent(Browser browser, QuoteBoat quoteBoat)
    {
        Reporting.AreEqual(Constants.PageHeader + quoteBoat.CandidatePolicyHolders[0].FirstName + "!", 
            GetElement(XPath.PageHeader).Text, ignoreCase: true, "expected page header with actual (NOT case-sensitive).");

        Reporting.AreEqual(Constants.EmailAdvice,
            GetElement(XPath.EmailAdvice).Text, "expected email advice content against actual.");

        //Placeholders to be expanded upon.
    }

    public void CapturePolicyNumber(Browser browser, QuoteBoat quoteBoat)
    {
        var quoteData = quoteBoat.QuoteData;
        quoteData.QuoteNumber = GetInnerText(XPath.PolicyNumber).Remove(0, 14);
        Reporting.Log($"Policynumber taken from confirmation page and set as quoteData.QuoteNumber = {quoteData.QuoteNumber}");
    }

    /// <summary>
    /// Use the Shield API via GetPolicyDetails, GetContactDetailsViaContactId and (sometimes) GetContactDetailsViaExternalContactNumber
    /// to fetch information about this policy and compare with expected results.
    /// </summary>
    public void VerifyPolicyDetailsInShield(Browser browser, QuoteBoat quoteBoat)
    {
        Reporting.LogMinorSectionHeading("Verify Policy values in Shield");
        var quoteData = quoteBoat.QuoteData;
        var apiPolicyResponse = DataHelper.GetPolicyDetails(quoteData.QuoteNumber);
        var apiContactResponse = DataHelper.GetContactDetailsViaContactId(apiPolicyResponse.Policyholder.Id.ToString());

        Reporting.AreEqual(quoteData.QuoteNumber, apiPolicyResponse.PolicyNumber, 
            $"expected Policy Number against the value returned by the Shield API");
        Reporting.AreEqual(Status.Policy.GetDescription(), apiPolicyResponse.Status, "Status in Shield");

        Reporting.LogMinorSectionHeading("About you");

        Reporting.Log($"apiPolicyResponse.Policyholder.Id.ToString() = {apiPolicyResponse.Policyholder.Id.ToString()}");
        Reporting.Log($"apiContactResponse.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH) = {apiContactResponse.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), apiContactResponse.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
            $"expected Date of Birth against the value returned from Shield Contact API related to {apiPolicyResponse.Policyholder.Id.ToString()} on {quoteData.QuoteNumber}");

        string skippersTicketExpectedApiValue = SparkBoatYourQuote.TranslateTestDataToExpectedApiValueForSkippersTicket(quoteBoat);
        Reporting.AreEqual(skippersTicketExpectedApiValue, apiPolicyResponse.BoatAsset.SkipperExperience,
            $"expected SkipperExperience against the value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.LogMinorSectionHeading("Boat type");

        Reporting.AreEqual(quoteBoat.BoatTypeExternalCode.ToString(), apiPolicyResponse.BoatAsset.BoatType,
            $"expected EXTERNAL CODE for {quoteBoat.BoatTypeExternalCode.GetDescription()} Boat Type against the value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.LogMinorSectionHeading("Your boat");

        Reporting.AreEqual(quoteBoat.BoatMake.GetDescription(), apiPolicyResponse.BoatAsset.Make,
            $"expected Make against the value returned from Shield API related to {quoteData.QuoteNumber}");
        if (quoteBoat.BoatMake == BoatMake.Other)
        {
            Reporting.AreEqual(UnlistedInputs.OtherBoatMake.ToString(), apiPolicyResponse.BoatAsset.OtherMake,
            $"expected OtherMake against value returned from Shield API related to {quoteData.QuoteNumber} when 'Other' is selected for boat Make.");
        }
        Reporting.AreEqual(quoteBoat.BoatYearBuilt.ToString(), apiPolicyResponse.BoatAsset.BuiltYear,
            $"expected BuiltYear against the value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.SparkBoatHull.ToString(), apiPolicyResponse.BoatAsset.HullConstruction.ToString(),
            $"expected EXTERNAL CODE for Hull Construction material against the value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.InsuredAmount, apiPolicyResponse.Covers[0].SumInsured,
            $"expected Sum Insured against value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.IsFinanced.ToString(), apiPolicyResponse.BoatAsset.IsFinanced, ignoreCase: true,
            $"expected IsFinanced against the value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");
        if (quoteBoat.IsFinanced)
        {
            var apiFinancierContactResponse = DataHelper.GetContactDetailsViaExternalContactNumber(apiPolicyResponse.BoatAsset.FinancierExternalContactId[0]);
            if (string.Equals(quoteBoat.Financier, UnlistedInputs.FinancierNotFound))
            {
                Reporting.AreEqual("Other financier", apiFinancierContactResponse.LegalEntityName.ToString(), ignoreCase: true,
                    $"expected Financier when financier is not available for selection from our list against the value returned from Shield Contact API related to Contact Id {apiPolicyResponse.BoatAsset.FinancierExternalContactId[0].ToString()} on {quoteData.QuoteNumber} (NOT case-sensitive)");
            }
            else
            {
                Reporting.AreEqual(quoteBoat.Financier.ToString(), apiFinancierContactResponse.LegalEntityName.ToString(), ignoreCase: true,
                    $"expected Financier against the value returned from Shield Contact API related to Contact Id {apiPolicyResponse.BoatAsset.FinancierExternalContactId[0].ToString()} on {quoteData.QuoteNumber} (NOT case-sensitive)");
            }

        }
        else
        {
            Reporting.IsNull(apiPolicyResponse.BoatAsset.FinancierExternalContactId, 
                $"no value for Financier returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} as Financier was not set");
        }

        Reporting.LogMinorSectionHeading("More about your boat");

        Reporting.AreEqual("West Perth", apiPolicyResponse.BoatAsset.Risk_suburb, ignoreCase: true, //TODO: SPK-3809 generate random data and make this conditional
            $"expected Risk_suburb against the value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual("6005", apiPolicyResponse.BoatAsset.Risk_postcode,
            $"expected Risk_postcode against the value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.IsGaraged.ToString(), apiPolicyResponse.BoatAsset.IsGaraged, ignoreCase: true,
            $"expected IsGaraged against the value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");
        Reporting.AreEqual(quoteBoat.SparkBoatMotorType.ToString(), apiPolicyResponse.BoatAsset.MotorType,
            $"expected MotorType against the value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.SecurityAlarmGps.ToString(), apiPolicyResponse.BoatAsset.IsSecurityAlarmGps, ignoreCase: true,
            $"expected IsSecurityAlarmGps against the value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");
        Reporting.AreEqual(quoteBoat.SecurityNebo.ToString(), apiPolicyResponse.BoatAsset.IsSecurityNebo, ignoreCase: true,
            $"expected IsSecurityNebo against the value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");
        Reporting.AreEqual(quoteBoat.SecurityHitch.ToString(), apiPolicyResponse.BoatAsset.IsSecurityHitch, ignoreCase: true,
            $"expected IsSecurityHitch against the value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");

        Reporting.LogMinorSectionHeading("Start Date");

      
        Reporting.AreEqual(quoteBoat.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
            apiPolicyResponse.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
            $"expected Policy Start against value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.LogMinorSectionHeading("Your details");
        
        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].Title.ToString(), apiContactResponse.Title.ToString(),
            $"expected Policyholder Title against value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].FirstName.ToString(), apiContactResponse.FirstName.ToString(),
            ignoreCase: true, $"expected Policyholder FirstName against value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].Surname.ToString(), apiContactResponse.Surname.ToString(),
            ignoreCase: true, $"expected Policyholder Surname against value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MobilePhoneNumber.ToString(), apiContactResponse.MobilePhoneNumber.ToString(),
            $"expected Policyholder MobilePhoneNumber against value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].PrivateEmail.Address, apiContactResponse.PrivateEmail.Address,
            ignoreCase: true, $"expected Policyholder PrivateEmail against value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MailingAddress.StreetNumber.ToString(), apiContactResponse.MailingAddress.StreetNumber.ToString(),
            ignoreCase: true, $"expected Mailing Address StreetNumber against value returned from Shield API related to {quoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MailingAddress.Suburb.ToString(), apiContactResponse.MailingAddress.Suburb.ToString(),
            ignoreCase: true, $"expected Mailing Address Suburb against value returned from Shield API related to {quoteData.QuoteNumber} (NOT case-sensitive)");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MailingAddress.PostCode.ToString(), apiContactResponse.MailingAddress.PostCode.ToString(),
            $"expected Mailing Address PostCode against value returned from Shield API related to {quoteData.QuoteNumber}");

        Reporting.LogMinorSectionHeading("Registration");
        if (quoteBoat.BoatRego == null)
        {
            Reporting.IsNullOrEmptyString(apiPolicyResponse.BoatAsset.BoatRegistration,
                    $"no Boat Registration value returned in the Shield API response related to {quoteBoat.QuoteData.QuoteNumber} as Boat registration was not set");
        }
        else 
        {
            Reporting.AreEqual(quoteBoat.BoatRego.ToUpper(), apiPolicyResponse.BoatAsset.BoatRegistration.ToString(),
                $"expected Boat Registration against the value returned from Shield API related to {quoteData.QuoteNumber}");
        }
            
        if (quoteBoat.BoatTrailerRego == null)
        {
            Reporting.IsNullOrEmptyString(apiPolicyResponse.BoatAsset.TrailerRegistration,
                $"no Trailer Registration value returned in the Shield API response related to {quoteBoat.QuoteData.QuoteNumber} as Trailer Registration was not set");
        }
        else
        {
            Reporting.AreEqual(quoteBoat.BoatTrailerRego.ToUpper(), apiPolicyResponse.BoatAsset.TrailerRegistration.ToString(),
            $"expected Trailer Registration against the value returned from Shield API related to {quoteData.QuoteNumber}");
        }

        VerifyShieldDatabasePolicyValues(browser, quoteBoat);
        
        if (quoteBoat.PayMethod.IsAnnual)
        {
            Reporting.LogMinorSectionHeading("Annual Premium Breakdown");
            Reporting.AreEqual(quoteBoat.QuoteData.AnnualPremium.ToString("0.00"), apiPolicyResponse.AnnualPremium.Total.ToString("0.00"),
                $"expected Total Annual Premium against the value returned from Shield API related to {quoteData.QuoteNumber}");

            Reporting.AreEqual(quoteBoat.QuoteData.PremiumBreakdownBasic.ToString("0.00"), apiPolicyResponse.AnnualPremium.BaseAmount.ToString("0.00"),
                $"expected Base Amount included in the Annual Premium against the value returned from Shield API related to {quoteData.QuoteNumber}");
            
            Reporting.AreEqual(quoteBoat.QuoteData.PremiumBreakdownStamp.ToString("0.00"), apiPolicyResponse.AnnualPremium.StampDuty.ToString("0.00"),
                $"expected Stamp Duty included in the Annual Premium against the value returned from Shield API related to {quoteData.QuoteNumber}");
            
            Reporting.AreEqual(quoteBoat.QuoteData.PremiumBreakdownGST.ToString("0.00"), apiPolicyResponse.AnnualPremium.Gst.ToString("0.00"),
                $"expected GST included in the Annual Premium against the value returned from Shield API related to {quoteData.QuoteNumber}");
        }

        if (quoteBoat.PayMethod.IsMonthly)
        {
            Reporting.LogMinorSectionHeading("Monthly Premium Instalment amount");
            Reporting.AreEqual(quoteBoat.QuoteData.MonthlyPremium.ToString("0.00"), apiPolicyResponse.NextPendingInstallment().Amount.Total.ToString("0.00"),
                $"expected pending instalment against the value returned from Shield API related to {quoteData.QuoteNumber}");
        }
    }
    public void VerifyShieldDatabasePolicyValues(Browser browser, QuoteBoat quoteBoat)
    {
        Reporting.LogMinorSectionHeading("Database value checks");
        var dataPolicyResponse = ShieldBoatDB.FetchBoatPolicyDetails(quoteBoat.QuoteData.QuoteNumber);
        
        Reporting.AreEqual(quoteBoat.BasicExcess.ToString(), dataPolicyResponse.BasicExcess,
            $"expected Basic Excess for the boat against the value returned from Shield DB related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.HasWaterSkiingAndFlotationDeviceCover.ToString(), dataPolicyResponse.HasWaterSkiingAndFlotationDeviceCover.ToString(),
            $"expected WaterSkiingAndFlotationDeviceCover flag against the value returned from Shield DB related to {quoteBoat.QuoteData.QuoteNumber}");

        if (quoteBoat.BoatTypeExternalCode.Equals(SparkBoatTypeExternalCode.L))
        {
            Reporting.AreEqual(quoteBoat.HasRacingCover.ToString(), dataPolicyResponse.HasRacingCover.ToString(),
            $"expected WaterSkiingAndFlotationDeviceCover flag against the value returned from Shield DB related to {quoteBoat.QuoteData.QuoteNumber}");
        }
        else
        {
            Reporting.AreEqual("False", dataPolicyResponse.HasRacingCover.ToString(),
                $"expected Racing Cover flag returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} is false as boat is not a Sailboat");
        }
        
        Reporting.AreEqual(UpdateUser.UserName, dataPolicyResponse.PolicyUpdateUser, 
            $"expected Update User is recorded in the Shield DB related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual("B2C", dataPolicyResponse.OriginalChannel, 
            $"expected Original Channel recorded against {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual("No discount", dataPolicyResponse.Discount, 
            $"expected Discount recorded against {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual("Basis", dataPolicyResponse.Ncb, 
            $"expected No Claim Bonus recorded against {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.IsNullOrEmptyString(dataPolicyResponse.OldHasAlarmShouldBeNull, 
            $"no value saved in the deprecated has_alarm field of the Shield database against {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.IsNullOrEmptyString(dataPolicyResponse.OldHasGpsShouldBeNull,
            $"no value saved in the deprecated has_gps_tracking_device field of the Shield database against {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.IsNullOrEmptyString(dataPolicyResponse.VehicleUsageDscShouldBeNull,
            $"no value saved in the Vehicle Useage field of the Shield database against {quoteBoat.QuoteData.QuoteNumber}");
    }

    /// <summary>
    /// Ignore CSS for dynamic fields in visual testing
    /// </summary>
    public List<string> GetPercyIgnoreCSS() =>
      new List<string>()
      {
              "#header",
              "[data-testid='policyNumberLabel']",
              "[data-icon='thumbs-up'] path"
      };
}
