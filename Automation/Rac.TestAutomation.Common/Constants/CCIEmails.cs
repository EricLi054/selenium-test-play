using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public static class CCIEmails
    {       
        public const string SupplierEnquiryEmail_Subject =      "Enquiry Email for your claim ";
        public const string SupplierEnquiryEmail_Body =         @"Hi Claims,As Western Building have been asked to provide a desktop quote for the above claim could you 
                                                                please send us a copy of the original builder scope of works 
                                                                in order to provide a apples for apples quote.Thanks";

        public const string CreditHireEmail_Subject =           "Carbiz: 39402 Your reference number:";
        public const string CreditHireEmail_Body =              @"Good morning Please advise your position on this claim and whether this claim has been reviewed for payment. 
                                                                Kind Regards
                                                                Eric Ngo​​​​
                                                                Claims And Recoveries Officer";

        public const string ProofOfLossEmail_Subject =          "RAC attached our proof of loss documents.";
        public const string ProofOfLossEmail_Body =             @"Hi team Please find attached our proof of loss documents. These documents include quotes and 
                                                                invoices regarding the cost of damages to vehicle and advise the process of recovery of these costs directly 
                                                                from yourself. We require your response regarding the requested payment within 14 days from the date of this 
                                                                email.Please note that all quotes issued are reviewed and authorised to ensure the costs of repairs are 
                                                                fair and reasonable.Should you have any queries or require assistance regarding payment 
                                                                please contact us during business hours or alternatively you can reply directly to this email 
                                                                keeping the subject line as the claim reference number stated. 
                                                                Thank you ";

        public const string TCUEmail_Subject =                  "Sample TCU email subject ";
        public const string TCUEmail_Body =                     @"Hi team , This is a sample TCU email body, Thanks, RAC member 
                                                                Thank you ";

        public const string MemberEnquiryEmail_Subject =        "Re: RAC Claim Lodgement Enquiry ";
        public const string MemberEnquiryEmail_Body =           @"Good morning May i know what are the information and document you need raise my claim .Thank you Jacqueline ";

        public const string MemberInvoiceEmail_Subject =        "Invocie for the Claim";
        public const string MemberInvoiceEmail_Body =           @"Hi Please find attached invoice for mob vaccum cleaner.
                                                                Now Godfrey store is closed and we cant get it repaired.Regards Syed";     
    }
}
