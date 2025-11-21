using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class WhereAndHowItHappened : BaseMotorClaimPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string ActiveStepperLabel = "Where and how";
            public static readonly string HeaderText = "Where and how it happened";
            public static readonly string ClaimNumberText = "Your claim number is ";

            public class Label
            {
                public static readonly string WhereAccidentHappened = "Provide an address or location";                
                public static readonly string HowAccidentHappened = "Please describe what happened";               
            }
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('where-and-how-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public class Field
            {
                public class Label
                {
                    public static readonly string WhereAccidentHappened = "id('label-description-location')";
                    public static readonly string HowAccidentHappened = "id('label-how-the-incident-happened')";
                }
                public class Input
                {
                    public static readonly string WhereAccidentHappened = "id('description-location')";
                    public static readonly string HowAccidentHappened = "id('how-the-incident-happened')";
                    public static readonly string PoliceReportNumber = "id('police-report-number')";
                }
                public static class Toggle
                {
                    public static readonly string WasThePoliceInvolved = "id('were-police-involved')";
                }
            }           
            public class Button
            {
                public static readonly string Yes = "//button[@aria-label='Yes']";
                public static readonly string No = "//button[@aria-label='No']";
                public static readonly string NotSure = "//button[@aria-label=\"I'm not sure\"]";

                public static readonly string UseMap = "//button[@aria-label='Use a map']";
                public static readonly string DescribeLocation = "//button[@aria-label='Describe location']";

                public static readonly string Next = "id('submit-button')";
            }
        }

        #endregion

        #region Settable properties and controls

        private string HeaderText => GetInnerText(XPath.Header);
        private string ClaimNumber
        {
            get
            {
                var claimNumber = new String(GetElement(XPath.ClaimNumber).Text.
                    Where(x => Char.IsDigit(x)).ToArray());
                return claimNumber;
            }
        }
        private string WhereAccidentHappenedQuestionLabel => GetInnerText(XPath.Field.Label.WhereAccidentHappened).StripLineFeedAndCarriageReturns();
        private string WhereAccidentHappened
        {
            get => GetInnerText(XPath.Field.Input.WhereAccidentHappened);
            set => WaitForTextFieldAndEnterText(XPath.Field.Input.WhereAccidentHappened, value);
        }
        private string HowAccidentHappenedQuestionLabel => GetInnerText(XPath.Field.Label.HowAccidentHappened).StripLineFeedAndCarriageReturns();
        private string HowAccidentHappened
        {
            get => GetInnerText(XPath.Field.Input.HowAccidentHappened);
            set => WaitForTextFieldAndEnterText(XPath.Field.Input.HowAccidentHappened, value);
        }
        private bool? WasThePoliceInvolved
        {
            get => GetNullableBinaryForTriStateToggle(XPath.Field.Toggle.WasThePoliceInvolved, XPath.Button.Yes, XPath.Button.No, XPath.Button.NotSure);
            set => ClickTriStateToggleWithNullableInput(XPath.Field.Toggle.WasThePoliceInvolved, XPath.Button.Yes, XPath.Button.No, XPath.Button.NotSure, value);
        }
        private string PoliceReportNumber
        {
            get => GetInnerText(XPath.Field.Input.PoliceReportNumber);
            set => WaitForTextFieldAndEnterText(XPath.Field.Input.PoliceReportNumber, value);
        }
        #endregion


        public WhereAndHowItHappened(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("Where and how it happened");
            Reporting.Log("Where and how it happened immediately after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking(string claimNumber)
        {
            ClickControl(XPath.Button.DescribeLocation);

            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.HeaderText, HeaderText, "Page header text");
            Reporting.AreEqual(Constants.Label.WhereAccidentHappened, WhereAccidentHappenedQuestionLabel, "Where accident happened question text");
            Reporting.AreEqual(Constants.Label.HowAccidentHappened, HowAccidentHappenedQuestionLabel, "How accident happened question text");
        }

        public void EnterWhereAndHowAccidentHappened(ClaimCar claimCar)
        {
            claimCar.ClaimNumber = ClaimNumber;

            ClickControl(XPath.Button.DescribeLocation);

            WhereAccidentHappened = claimCar.EventLocation;
            HowAccidentHappened = claimCar.AccountOfAccident;
            WasThePoliceInvolved = claimCar.IsPoliceInvolved;
            if (claimCar.IsPoliceInvolved == true)
            {
                if (!string.IsNullOrEmpty(claimCar.PoliceReportNumber))
                {
                    PoliceReportNumber = claimCar.PoliceReportNumber;
                }
            }
            Reporting.Log("Where and how it happened - Before clicking Next Button", _driver.TakeSnapshot());

            ClickNext();
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "#claimNumberDisplay span"
          };

    }
}
