using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;

namespace UIDriver.Pages.Spark.EFT
{
    public class ChooseSettlement : SparkBasePage
    {
        #region XPATHS
        private static class XPath
        {
            public static class General
            {
                public const string ClaimNumber = "//div[@id='stepper-content']//p[contains(@class,'gutterBottom ')]"; //TODO: SPK-4097 - When ID values have been added, update XPath
            }
            public static class CashSettlement
            {
                public const string Accept = "//label[@data-testid='accept']/span";
                public const string Decline = "//label[@data-testid='decline']/span";
            }
            public static class Button
            {
                public const string Confirm = "//button[@data-testid='submit']";
            }
        }
        #endregion XPATHS

        #region Settable properties and controls

        public string ClaimNumber
        {
            get
            {
                var urlRegex = new Regex(@"(\d+)");
                var rawText = GetElement(XPath.General.ClaimNumber).Text;
                Match match = urlRegex.Match(rawText);
                Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"claim number parsed from UI text: {rawText}");
                
                return match.Groups[1].Value;
            }
        }

        #endregion Settable properties and controls

        public ChooseSettlement(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.CashSettlement.Accept);
                GetElement(XPath.CashSettlement.Decline);
                GetElement(XPath.General.ClaimNumber);
                GetElement(XPath.Button.Confirm);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }


        public void DeclineCashSettlement()
        {
            ClickControl(XPath.CashSettlement.Decline);
            Reporting.Log("CSFS Page", _driver.TakeSnapshot());
            _browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.DeclineCashSettlement);
            ClickConfirmButton();
        }

        public void AcceptCashSettlement()
        {
            ClickControl(XPath.CashSettlement.Accept);
            Reporting.Log("CSFS Page", _driver.TakeSnapshot());
            _browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.AcceptCashSettlement);
            ClickConfirmButton();
        }

        public void ClickConfirmButton()
        {
            ClickControl(XPath.Button.Confirm);
        }
    }
}