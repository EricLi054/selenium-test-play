using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.BoatQuote
{
    public class PremiumChangePopup : SparkBasePage
    {
        #region XPATHS
        public class XPath
        {
            public static readonly string PremiumChangePopup = "//div[@role='dialog' and @aria-labelledby='dialog-box-title']";
            public static readonly string PremiumChangeTitle    = "id('dialog-box-title')";
            public static readonly string PremiumChangeBody     = "id('dialog-box-body-text')";
            public static readonly string FrequencyText         = "id('card-frequency-text')";
            public static readonly string TotalPrice            = "id('card-frequency-price')";
            public static readonly string TotalAnnualComparison = "id('card-total-annual-price')";

            public class Button
            {
                public static readonly string BreakdownAccordion    = "id('card-breakdown-accordion')";
                public static readonly string ClosePopup            = PremiumChangePopup + "//button[@data-testid='dialog-box-button']";
            }
            public class PremiumBreakdown
            {
                public static readonly string Basic                 = "id('card-breakdown-basic-price')";
                public static readonly string GovernmentCharges     = "id('card-breakdown-gov-price')";
                public static readonly string GST                   = "id('card-breakdown-gst-price')";
            }
        }
        #endregion
        #region Constants
        public class Constants
        {
            public static readonly string PremiumChangeTitle = "We've updated your quote";
            public static readonly string PremiumChangeBody  = "We've updated some things since your quote was created and your price has changed.";
            public class FrequencyText
            {
                public static readonly string Annual  = "New annual price";
                public static readonly string Monthly = "New monthly price";
            }
        }
        #endregion


        #region Settable properties and controls 

        #endregion

        public PremiumChangePopup(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Button.ClosePopup);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }
    }
}