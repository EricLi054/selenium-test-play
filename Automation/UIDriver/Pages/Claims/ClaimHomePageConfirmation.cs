using Rac.TestAutomation.Common;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Claims
{
    public class ClaimHomePageConfirmation : BaseClaimsConfirmation
    {
        #region XPATHS
        // our payment has been processed and will be available in your nominated bank account within 3 business days.
        protected const string XP_PAYMENT_PROCESSING_TIME_TEXT = "//div[@class='confirmationMessage']/p[contains(.,'bank account')]";
        protected const string XP_RECEIVE_EMAIL = "//div[@class='confirmationMessage']/p/span[contains(.,'receive an email')]";
        protected const string XP_EMAIL_EXPIRE = "//div[@class='confirmationMessage']/p/span[contains(.,'expire')]";

        #endregion

        #region Settable properties and controls
        public HomeClaimDamageType DetailsDamageType
        {
            get
            {
                var damageTypeString = GetElement(XP_DAMAGE_TYPE).Text.Trim();
                Reporting.IsTrue(HomeClaimDamageTypeNames.Any(x => x.Value.TextB2C.Equals(damageTypeString)), $"that damage Type shown in claim confirmation ({damageTypeString}) was recognised");

                return HomeClaimDamageTypeNames.First(x => x.Value.TextB2C.Equals(damageTypeString)).Key;
            }
        }

        /// <summary>
        /// Left panel, conditional paragraph text that
        /// provides information to the claimant about
        /// the time for processing a payment to them
        /// if they accepted an online settlement for
        /// fence damage.
        /// </summary>
        public string PaymentProcessingTimeParagraph
        {
            get => GetInnerText(XP_PAYMENT_PROCESSING_TIME_TEXT).StripLineFeedAndCarriageReturns();
        }

        /// <summary>
        /// Left panel, conditional paragraph text that
        /// provides information to the claimant about
        /// the email they received
        /// </summary>
        public string ReceiveEmailParagraph
        {
            get => GetInnerText(XP_RECEIVE_EMAIL).StripLineFeedAndCarriageReturns();
        }

        /// <summary>
        /// Left panel, conditional paragraph text that
        /// provides information to the claimant about
        /// the expire of link in the email
        /// </summary>
        public string EmailExpireParagraph
        {
            get => GetInnerText(XP_EMAIL_EXPIRE).StripLineFeedAndCarriageReturns();
        }

        #endregion

        public ClaimHomePageConfirmation(Browser browser) : base(browser) { }
    }
}
