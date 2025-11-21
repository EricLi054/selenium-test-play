using OpenQA.Selenium;
using Rac.TestAutomation.Common;


namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class BeforeYouStart : SparkBeforeYouStart
    {

        #region CONSTANTS
        private static class Constant
        {
            public static readonly string Header = "Car accident claim";
        }

        #endregion

        #region XPATHS

        private new class XPath : SparkBeforeYouStart.XPath
        {
            public static readonly string Header = "//h1[text()=\"Car accident claim\"]";
            public static class Card
            {
                public static readonly string HopeYouAreOkay = "//p[text()=\"Firstly, we hope you're okay.\"]";
                public static readonly string BestOfYourKnowledge = "//p[text()=\"Fill in the form to the best of your knowledge.\"]";
                public static readonly string Take15Min = "//p[text()=\"This may take around 15 minutes.\"]";
                public static readonly string DriverDetails = "//p[text()=\"You'll need the full name and date of birth of the driver of your car.\"]";
                public static readonly string ReportsNotRequired = "//p[text()=\"You don't need a police or crash report to start a claim.\"]";               
                public static readonly string ExcessDetails = "//p[text()=\"You can find details of your excess in your policy documents. All personal information is collected according to our\"]";
            }

            public static class Button
            {
                public static readonly string StartClaim = "id('start-claim-btn')";
            }
        }

        #endregion

        public BeforeYouStart(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.SubHeader);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Before You Start page");
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constant.Header, GetInnerText(XPath.Header), "Header Text");
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.HopeYouAreOkay), "Firstly, we hope you're okay text is displayed");
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.Take15Min), "This may take around 15 minutes text is displayed");
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.DriverDetails), "You'll need the full name and date of birth of the driver of your car");
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.ReportsNotRequired), "You don't need a police or crash report to start a claim text is displayed");            
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.ExcessDetails), "You can find details of your excess in your policy documents");
            //for iPhone we need to scroll down to get element in the view
            if (_browser.DeviceName == Constants.General.TargetDevice.iPhone14)
            {
                ScrollElementIntoView(XPath.NextButton);
            }
        }

        public void ClickStartClaimDialogBox()
        {
            ClickControl(XPath.Button.StartClaim);
        }
    }
}