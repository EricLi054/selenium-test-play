using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// The cancellation confirmation page shows members 
    /// an acknowledgement that their policy has been 
    /// cancelled.  It provides information about the
    /// next steps in the cancellation process.
    /// </summary>
    public class CancellationConfirmationPage: SparkBasePage
    {

        #region CONSTANTS
        public class Constants
        {
            public class Roadside
            {
                public const string Title = "Existing roadside assistance";
                public const string ExplantoryText = "This vehicle may be covered by a Roadside Assistance product.";
                public const string ActionText = "Please call 1300 138 021 if you'd like to discuss your Roadside Assistance coverage options.";
            }

            public class RefundInformation
            {
                public const string Heading = "You're getting a refund!";
                public const string AmountLabel = "Refund amount";
                public const string DestinationLabelAccount = "To account:";
                public const string DestinationLabelCard = "To card:";
                public const string ProcessingTime = "Processing time: 3–7 business days";  // contains an em dash
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Root = "id('root')";

            public class PageInformation
            {
                public const string Title = "id('pco-confirmation-page-title')";
                public const string SubTitle = "id('pco-confirmation-page-subtitle')";
            }

            public class Navigation
            {
                public const string BackToMyRac = "id('racHomePageLink')";
            }

            public class RoadsideAssist
            {
                public const string Root = "id('roadsideWarningTitleId')";
                public const string CardTitle = "id('roadsideWarningTitleId-title')";
                public const string CardParagraph1 = "id('roadsideWarningParagraph1')";
                public const string CardParagraph2 = "id('roadsideWarningParagraph2')";
                public const string Phone = "//a[@id='roadsideWarningPhone' and @href='tel:1300138021']";
            }

            public class Refund
            {
                public const string Heading = "//h3[contains(text(),\"" + Constants.RefundInformation.Heading + "\")]";
                public const string MoneyIcon = Heading + "/*[local-name()='svg' and @data-icon='money-bill-wave']";
                public const string AmountLabel = "id('pco-confirmation-refund-title')";
                public const string Amount = "id('pco-confirmation-refund-amount')";
                public const string Destination = "id('pco-confirmation-refund-destination')";
                public const string ProcessingTime = "id('pco-confirmation-refund-message')";
            }
        }
        #endregion

        public string PageTitle() => GetInnerText(XPath.PageInformation.Title);
        public string PageSubTitle() => GetInnerText(XPath.PageInformation.SubTitle);

        // Roadside Assistance
        public bool IsRoadsideAssistanceCardDisplayed => _driver.TryWaitForElementToBeVisible(
            By.XPath(XPath.RoadsideAssist.Root), WaitTimes.T5SEC, out IWebElement element);
        public bool IsRoadsideAssistanceCallLinkPresent => _driver.TryWaitForElementToBeVisible(
            By.XPath(XPath.RoadsideAssist.Phone), WaitTimes.T5SEC, out IWebElement element);
        public string RoadsideAssistanceTitle => GetInnerText(XPath.RoadsideAssist.CardTitle);
        public string RoadsideAssistanceFirstParagraph => GetInnerText(XPath.RoadsideAssist.CardParagraph1);
        public string RoadsideAssistanceSecondParagraph => GetInnerText(XPath.RoadsideAssist.CardParagraph2);

        // Refund Related
        public string RefundHeading() => GetInnerText(XPath.Refund.Heading);
        public bool IsMoneyIconDisplayed => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.Refund.MoneyIcon),
            WaitTimes.T5SEC, out IWebElement _);
        public string RefundLabel() => GetInnerText(XPath.Refund.AmountLabel);
        public string RefundAmount() => GetInnerText(XPath.Refund.Amount);
        public string RefundDestination() => GetInnerText(XPath.Refund.Destination);
        public string RefundProcessingTime() => GetInnerText(XPath.Refund.ProcessingTime);

        public CancellationConfirmationPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XPath.Root);
                GetElement(XPath.Navigation.BackToMyRac);
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        /// <summary>
        /// The confirmation page contains elements that are identified 
        /// using the policy number.  This is because the policy details
        /// is a shared component and in other situations multiple policy cards
        /// could exist on a page.
        /// </summary>
        /// <param name="policyNumber">Policy number used to identify elements</param>
        /// <returns></returns>
        public bool IsDisplayed(string policyNumber)
        {
            var rendered = false;
            try
            {
                if (IsDisplayed())
                {
                    GetElement(XPath.PageInformation.Title);
                    GetElement(XPath.PageInformation.SubTitle);
                    GetElement(XPath.Navigation.BackToMyRac);
                    rendered = true;
                }
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }
    }
}
