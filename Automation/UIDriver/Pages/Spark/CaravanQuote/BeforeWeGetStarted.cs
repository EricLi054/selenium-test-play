using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class BeforeWeGetStarted : SparkBeforeWeGetStarted
    {
        private new class XPath
        {
            public static class Popup
            {
                public const string DiscountMessage = "//button[@data-testid='discountAppliedButton']";
            }
        }

        public BeforeWeGetStarted(Browser browser) : base(browser)
        { }

        protected override void VerifyDiscountToast(MembershipTier membershipTier)
        {
            if (IsEligibleForDiscount(membershipTier))
            {
                string memberDiscountToastMapping = $"{MemberDiscountMappings[membershipTier].Caravan} {DISCOUNT_APPLIED}";

                Reporting.AreEqual(memberDiscountToastMapping, GetInnerText(XPath.Popup.DiscountMessage), "Expected discount toast message on Page 1 (Are you an RAC member) for the MainPH contact's Membership level was displayed");
            }
        }

    }
}
