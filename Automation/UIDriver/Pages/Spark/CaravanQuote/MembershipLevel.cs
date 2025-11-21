using OpenQA.Selenium;
using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.Contacts;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class MembershipLevel : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string Header = FORM + "//h2[text()='One more question']";
            }
            public static class Button
            {
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
            public static class MembershipLevel
            {
                private const string _Selector = "//ul[@role='listbox']";

                public const string Label = FORM + "//label[contains(text(),'What RAC membership level are you?')]";
                public const string Input = FORM + "//div[@id='select-membership-level']";
                public const string Options = _Selector + "/li";
            }
        }
        // Membership details for Multi Match



        #endregion

        public string Membership
        {
            get => GetInnerText(XPath.MembershipLevel.Input);
            set => WaitForSelectableAndPickFromDropdown(XPath.MembershipLevel.Input, XPath.MembershipLevel.Options, value);
        }

        public MembershipLevel(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.Button.Next);
                GetElement(XPath.MembershipLevel.Label);
                GetElement(XPath.MembershipLevel.Input);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }

        public void SetMembershipAndClickNext(MembershipTier membershipTier)
        {
            Membership = membershipTier.GetDescription();
            ClickControl(XPath.Button.Next);
        }
    }
}