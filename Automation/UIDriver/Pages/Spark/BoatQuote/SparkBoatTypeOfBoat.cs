using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using UIDriver.Pages.Spark.BoatQuote;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

public class SparkBoatTypeOfBoat : BaseBoatPage
{
    #region XPATHS
    private class XPath
    {
        public const string PageHeader      = "//*[@id='header']";
        public const string ActiveStepper   = "//button[@aria-selected='true']";
        public const string SubLabel        = "id('CardChoiceInput-sublabel')";
        public class Button
        {
            public const string Powerboat   = "id('CardChoiceInput-cardgroup-0-PowerBoat')";
            public const string Sailboat    = "id('CardChoiceInput-cardgroup-1-SailBoat')";
            public const string NextPage    = "//button[@type='submit']";
        }
        public class AdviseUser
        { 
            public class FieldValidation
            {
                public const string BoatType = "//p[contains(text(),'Please select one')]";
            }
        }
    }
    #endregion

    #region Constants
    private class Constants
    {
        public const string PageHeader          = "Type of boat";
        public const string ActiveStepperLabel  = "Boat type";
        public const string SubLabel            = "Please choose an option.";
        public class AdviseUser
        {
            public class FieldDescription
            {
                public const string Powerboat   = "Powerboat\r\nRunabout, dinghy, bow rider, bass boat, " +
                                                "cuddy cabin, ski boat, cabin cruiser, " +
                                                "centre console boat";
                public const string Sailboat    = "Sailboat\r\nOff-beach craft, trailer sailer, trailed yacht";
            }
        }
    }
    #endregion

    #region Settable properties and controls

    #endregion
    public SparkBoatTypeOfBoat(Browser browser) : base(browser)
    { }

    public override bool IsDisplayed()
    {
        var isDisplayed = false;
        try
        {
            GetElement(XPath.PageHeader);
            isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
        }
        catch (NoSuchElementException)
        {
            return false;
        }

        if (isDisplayed)
        { Reporting.LogPageChange("Spark Boat Quote page ? - TypeOfBoat"); }

        return isDisplayed;
    }

    /// <summary>
    /// Triggers detailed verification of page content, only invoked when detailedUiChecking = true.
    /// </summary>
    public void VerifyPageContent()
    {
        Reporting.AreEqual(Constants.PageHeader,
            GetElement(XPath.PageHeader).Text, "expected page header with actual.");

        Reporting.AreEqual(Constants.SubLabel,
            GetElement(XPath.SubLabel).Text, "expected page sub-header with actual.");

        Reporting.AreEqual(Sidebar.Link.PdsUrl,
            GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

        VerifyStandardHeaderAndFooterContent();

        VerifyFieldValidation();

        VerifyHelpText();

        VerifyBoatFAQContent();
    }

    private void VerifyFieldValidation()
    {
        Reporting.LogMinorSectionHeading("Selecting the Next button to trigger field validation message");
        ClickControl(XPath.Button.NextPage);
        
        Reporting.Log("Capturing snapshot before evaluating.", _browser.Driver.TakeSnapshot());

        Reporting.AreEqual(AdviseUser.FieldValidation.SelectOne, 
            GetElement(XPath.AdviseUser.FieldValidation.BoatType).Text, "field validation message when no selection has been made for Boat Type against expected value");
    }

    private void VerifyHelpText()
    {
        Reporting.LogMinorSectionHeading("Validating descriptions of boat types available for selection");
        
        Reporting.AreEqual(Constants.AdviseUser.FieldDescription.Powerboat,
            GetElement(XPath.Button.Powerboat).Text, 
            "expected Powerboat description against page content");
        
        Reporting.AreEqual(Constants.AdviseUser.FieldDescription.Sailboat,
            GetElement(XPath.Button.Sailboat).Text, 
            "expected Sailboat description against page content");
    }

    public void SelectBoatType(QuoteBoat quoteBoat)
    {
        Reporting.Log($"Assigned boat type for selection = {quoteBoat.BoatTypeExternalCode.GetDescription()}");

        switch(quoteBoat.BoatTypeExternalCode)
        {
            case SparkBoatTypeExternalCode.L:
                ClickControl(XPath.Button.Sailboat); 
                break;
            case SparkBoatTypeExternalCode.P:
                ClickControl(XPath.Button.Powerboat);
                break;
            default:
                Reporting.Error($"Attempted to request a boat type that isn't supported; {quoteBoat.BoatTypeExternalCode.GetDescription()}");
                break;
        }
        
        Reporting.Log("Capturing state of screen after selection.", _browser.Driver.TakeSnapshot());
    }

    public void ContinueToYourBoat()
    {
        ClickControl(XPath.Button.NextPage);
    }
}
