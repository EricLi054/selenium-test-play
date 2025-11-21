using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;


namespace UIDriver.Pages.Spark.Claim.Motor.Glass
{
    public class BeforeYouStart : SparkBeforeYouStart
    {
        #region XPATHS

        private new class XPath : SparkBeforeYouStart.XPath
        {
            public class Card
            {
                public const string NoClaimBonus = "//p[text()=\"This claim won't affect your no claim bonus.\"]";
                public const string ExcessForChipRepairs = "//p[text()=\"There's no excess for chip repairs.\"]";
                // TODO: SPK-6704 to remove ExcessForReplacementsWhenNCBToggledOff when the NCB/Excess toggle is removed, and re-name ExcessForReplacementsWhenNCBToggledOn
                public const string ExcessForReplacementsWhenNCBToggledOff = "//p[text()=\"Excess is $100 for replacements.*\"]";
                public const string ExcessForReplacementsWhenNCBToggledOn = "//p[text()=\"There's an excess for replacements.\"]";
            }

            public const string NoExcessCondition = "//p[text()=\"*Unless you\'ve paid extra and have no excess.\"]";
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
            catch(NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motor Glass Claim Page - Before You Start");
            return true;
        }

        public void VerifyText()
        {
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.ExcessForChipRepairs), "There's no excess for chip repairs card");
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.ExcessForReplacementsWhenNCBToggledOn), "There's an excess for replacements.");
            Reporting.IsFalse(IsControlDisplayed(XPath.Card.NoClaimBonus), "No Claim bonus text is not displayed.");
            Reporting.IsFalse(IsControlDisplayed(XPath.Card.ExcessForReplacementsWhenNCBToggledOff), "Excess is $100 for replacements.* card is not displayed.");
            Reporting.IsFalse(IsControlDisplayed(XPath.NoExcessCondition), "Unless you've paid extra and have no excess. text");
        }
    }


}
