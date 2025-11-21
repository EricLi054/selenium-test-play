using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class BeforeWeGetStarted : SparkBeforeWeGetStarted
    {
        private class XPathBeforeStarted
        {
            public static class Popup
            {
                public const string DiscountMessage = "//div[@id='toastNotification']//p[contains(.,'discount applied')]";
            }
        }

        public BeforeWeGetStarted(Browser browser) : base(browser)
        { }

        protected override void VerifyDiscountToast(MembershipTier membershipTier)
        {
            if (IsEligibleForDiscount(membershipTier))
            {
                string memberDiscountToastMapping = $"{MemberDiscountMappings[membershipTier].Motorcycle} {DISCOUNT_APPLIED}";

                Reporting.AreEqual(memberDiscountToastMapping, GetInnerText(XPathBeforeStarted.Popup.DiscountMessage), "Expected discount toast message on Page 1 (Are you an RAC member) for the MainPH contact's Membership level was displayed");
            }
        }

    }
}
