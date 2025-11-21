using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Motor
{
    public abstract class BaseMotorClaimPage : SparkBasePage
    {
        #region CONSTANTS
        public class ConstantBaseMotorClaim
        {
            public readonly static string NotificationCard = "Please check this is correctYou can't change this online once you progress.";
        }

        #endregion

        #region XPATHS
        protected class XPathBaseMotorClaim
        {
            public class NotificationCard
            {
                public static readonly string Body = "//div[@data-testid='notFoundCard']";
            }
        }

        #endregion

        public BaseMotorClaimPage(Browser browser) : base(browser)
        { }

        /// <summary>
        /// Verify motor claim online notification card content
        /// </summary>
        public void VerifyMotorClaimOnlineNotificationCard()
        {
            Reporting.AreEqual(ConstantBaseMotorClaim.NotificationCard, GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "expected Notification Card content is displayed");
        }
    }
}