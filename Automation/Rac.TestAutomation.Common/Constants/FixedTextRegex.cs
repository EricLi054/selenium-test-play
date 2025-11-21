
namespace Rac.TestAutomation.Common
{
    public static class FixedTextRegex
    {
         public const string POLICY_RENEWAL_SUCCESS_REGEX = @"^\s*Thank you\s*" +
            @"You've successfully renewed your policy.$";

        public const string POLICY_CHANGE_WHERE_I_KEEP_MY_CAR_SUCCESS_REGEX = @"^\s*Thank you\s*" +
            @"You have successfully updated the address where you keep your car.\s* " +
            @"Other policies\?\s* " +
            @"If you have any other policies please change your address on each individual policy or call us on 13 17 03.\s* " +
            @"Mailing address\s* " +
            @"Do you need to update your policy mailing address with this new address\? Please edit your mailing address in My details.\s*$";

        public const string POLICY_CHANGE_MY_CAR_SUCCESS_REGEX = @"^\s*Thank you\s*" +
            @"You have successfully updated your policy with your new car information.\s* " +
            @"If this change also applies to your roadside assistance, please call us on 13 17 03.\s*$";

        public const string POLICY_EMAIL_CERTIFICATE_OF_CURRENCY_SUCCESS_REGEX = @"^Thank you\s*" +
            @"The requested document has been successfully sent to (\S+(\.\S+)*@\S+(\.\S+)*)\.\s*" +
            @"If you do not receive an email within one hour, please check your spam folder.\s*Back to My policies";

        public const string CLAIMS_CONSULTANT_CONTACT_REGEX = @"^\s*One of our friendly claims consultants will contact you within 2 business hours.\s*" +
            @"Please note that during busy claim periods such as storm events, it may take us up to 1 business day to get in touch.\s*";

        public const string CLAIMS_FAQ_TEXT_CLAIM_LODGED_REGEX = @"^\s*For more information, please see claims FAQ's or view your claim on My Insurance.\s*$";

        public const string CLAIMS_ONLINE_SETTLEMENT_PROCESSING_TIME_REGEX = @"^\s*Your payment has been processed and will be available in your nominated bank account within 3 business days.\s*";

        public const string CLAIMS_ONLINE_SETTLEMENT_RECEIVE_EMAIL = @"^\s*You(’|')ll receive an email shortly outlining your settlement options\s*";

        public const string CLAIMS_ONLINE_SETTLEMENT_EMAIL_LINK_EXPIRE = @"^\s*Please note the link will expire within 72 hours\s*";

        public const string POLICY_CHANGE_MY_HOME_DETAILS_SUCCESS_REGEX = @"^\s*Thank you\s*" +
            @"You have successfully updated your home details.\s* \s*" +
            @"Other policies\?\s* If you have any other policies please change your address on each individual policy or call us on 13 17 03.\s* \s*" +
            @"Mailing address\s* Do you need to update your policy mailing address with this new address\? Please edit your mailing address in (myRAC)*|(My details)*\.\s*$";

        public const string ANNUAL_POLICY_ENDORSEMENT_CARD_PAYMENT_AUTHORISATION_REGEX = @"^Card payment authorisation terms RAC Insurance can deduct this payment from this account. " +
             @"You're authorised to operate the nominated account.\s*";

        public const string ANNUAL_CARD_PAYMENT_AUTHORISATION_REGEX = @"^\s*Authorisation By clicking 'Purchase policy' you are committing to purchase this policy and we will complete your payment through a secure payment gateway.\s*";

        public const string ANNUAL_BANK_MONTHLY_CARD_BANK_PAYMENT_AUTHORISATION_REGEX = "^Direct debit authorisation terms "+
            @"RAC Insurance can deduct all future payments from this account. " +
            @"This includes when your policy is renewed. If payment is due on a non-business day, we may deduct the payment on either the business day before the due date or on the next business day. " +
            @"You're authorised to operate the nominated account. You've read and understood the Product Disclosure Statement and Direct Debit Request Service " +
            @"Agreement which have important information about direct debits. RAC Insurance Pty Limited's financial institution will arrange this debit or charge. " +
            @"This will be made through the Bulk Electronic Clearing System Framework \(BECS\) from your nominated account. " +
            @"It will be subject to the terms and conditions of the Direct Debit Request Service Agreement.";

        public const string DIRECT_DEBIT_AUTHORISATION_TERMS_AGREE_REGEX = "^I've read and agree to the direct debit authorisation terms.";

        public const string CARD_PAYMENT_AUTHORISATION_TERMS_AGREE_REGEX = "^I've read and agree to the card payment authorisation terms.";

        public const string QUOTE_UPDATED_POPUP_MESSAGE_REGEX = @"^\s*We've updated your quote based on the information you changed.\s*";

        public const string QUOTE_HELP_DIALOGUE_BANNER_TEXT_OMCO = @"^\s*Our most popular cover for a reason. With the highest level of protection and the most benefits.\s*";

        public const string QUOTE_HELP_DIALOGUE_BANNER_TEXT_OMTP = @"^\s*Great if your main concern is damage to someone else(’|')s property or your budget.\s*";

        public const string QUOTE_HELP_DIALOGUE_BANNER_TEXT_OMTO = @"^\s*Offers some protection for your bike and damage to other people(’|')s property.\s*";

        public const string QUOTE_COVER_DECLINED_TEXT = @"We.re sorry but we can.t provide you with an online quote due to you not meeting our eligibility requirements.\s*" +
            @"You have the right to request the information relied upon in making this decision. You can do so by contacting us on 13 17 03.\s*" +
            @"To help you find appropriate insurance cover, the Insurance Council of Australia \(ICA\) has compiled a list of insurance providers who offer general insurance products.\s*" +
            @"The .Find an Insurer. service is available via www.findaninsurer.com.au.";

        public const string QUOTE_COVER_DECLINED_VEH_USAGE_TEXT = @"Unfortunately, we are unable to offer insurance for this type of vehicle usage.\s*" +
            @"You have the right to request the information relied upon in making this decision. You can do so by contacting us on 13 17 03.\s*" +
            @"To help you find appropriate insurance cover, the Insurance Council of Australia \(ICA\) has compiled a list of insurance providers who offer general insurance products.\s*" +
            @"The .Find an Insurer. service is available via www.findaninsurer.com.au.";

        public const string PAYMENT_ANNUAL_CREDIT_CARD_AUTHORISATION_REGEX = @"^\s*Authorisation\s*By clicking 'Pay now' you are committing to purchase this policy and we will complete your payment through a secure payment gateway.\s*$";
        public const string PAYMENT_ANNUAL_CREDIT_CARD_AUTHORISATION_PCM_REGEX = @"^\s*Authorisation\s*By clicking 'Pay now' you are committing to pay for this policy and we will complete your payment through a secure payment gateway.\s*$";

        public const string PAYMENT_ANNUAL_BANK_MONTHLY_CREDIT_CARD_BANK_AUTHORISATION_REGEX = @"^\s*Direct debit authorisation terms\s*RAC Insurance can deduct all future payments from this account. "+
            @"This includes when your policy is renewed. If payment is due on a non-business day, we may deduct the payment on either the business day before the due date or on the next business day.\s*" +
            @"You(’|')re authorised to operate the nominated account.\s*" +
            @"You(’|')ve read and understood the Product Disclosure Statement and Direct Debit Request Service Agreement which have important information about direct debits.\s*" +
            @"RAC Insurance Pty Limited(’|')s financial institution will arrange this debit or charge. This will be made through the Bulk Electronic Clearing System Framework \(BECS\) from your nominated account. " +
            @"It will be subject to the terms and conditions of the Direct Debit Request Service Agreement.\s*";

        public const string EFT_CONFIRMATION_MESSAGE_REGEX = @"^\s*A payment will be made into your bank account within four to seven business days. However, during busy claim periods, we may take a little longer.\s*";

        public const string CSFS_DECLINE_CASH_CONFIRMATION_MESSAGE_REGEX = @"^\s*We(’|')ll be in touch within five business days to arrange next steps. However, during busy claim periods, we may take a little longer.\s*";

        public const string CSFS_ACCEPT_CASH_CONFIRMATION_MESSAGE_REGEX = @"^\s*A payment will be made into your bank account within four to seven business days.\s*";

        public const string CSFS_ERROR_HEADING_REGEX = @"^\s*We couldn(’|')t take you to your destination.\s*";
        public const string CSFS_ERROR_MESSAGE_REGEX = @"^\s*It looks like your link has expired or you(’|')ve already provided this information. Please call us on  13 17 03  if you need assistance.\s*";

        public const string CLAIM_FENCE_SETTLEMENT_LENGTH = @"^\s*(\d+) panels claimed";
        public const string CLAIM_FENCE_SETTLEMENT_COVER_PER_UNIT = @"panels claimed x \$(\d+\.\d+) \(cost per";

        public const string BANK_ACCOUNT_MASKING = @"\*{3}\s\d{3}\Z";
        public const string CREDIT_CARD_MASKING = @"\*{4}\s\*{4}\s\*{4}\s\d{4}\Z";
    }
}