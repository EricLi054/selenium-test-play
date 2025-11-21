using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants.AdviseUser;


namespace UIDriver.Pages.Spark.BoatQuote
{
    public abstract class BaseBoatPage : SparkBasePage
    {
        #region XPATH
        public class XPathBaseBoat
        {
            public class AdviseUser
            {
                public class BoatFaqCard
                {
                    public readonly static string Head = "id('faq-header')";
                    public readonly static string Body = "id('faq-body')";
                    public readonly static string Link = "id('faq-link')";
                }
            }
        }
        #endregion
        public BaseBoatPage (Browser browser) : base(browser) { }


        /// <summary>
        /// Verify the content of the FAQ card being added to Boat New Business as an experiment by UX (see SPK-4495).
        /// </summary>
        public void VerifyBoatFAQContent()
        {
            Reporting.AreEqual(BoatFaqCard.Head, GetInnerText(XPathBaseBoat.AdviseUser.BoatFaqCard.Head),
                "expected FAQ card Header text with actual value on page");
            Reporting.AreEqual(BoatFaqCard.Body, GetInnerText(XPathBaseBoat.AdviseUser.BoatFaqCard.Body),
                "expected FAQ card Body text with actual value on page");
            Reporting.AreEqual(BoatFaqCard.Link, GetElement(XPathBaseBoat.AdviseUser.BoatFaqCard.Link).GetAttribute("href"),
                "expected FAQ card link URL with actual value on page");
            Reporting.LogMinorSectionHeading("End Boat FAQ Card verification");
        }
    }
}