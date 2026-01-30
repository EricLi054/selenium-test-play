using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DataModels;
using System.Linq;

namespace Tests.ActionsAndValidations
{
    public class VerifyMRO
    {

        /// <summary>
        /// Verify correct bank details added in the Shield
        /// </summary>
        public static void VerifyBankDetailsInShield(RefundDetails refundDetails)
        {

            var contactDetails = DataHelper.GetContactDetailsViaContactId(refundDetails.ContactId);
            var resultCount = contactDetails.BankAccounts.Count(x => x.Bsb == refundDetails.RefundBankAmount.Bsb &&
                                                            x.AccountNumber == refundDetails.RefundBankAmount.AccountNumber &&
                                                            x.AccountName == refundDetails.RefundBankAmount.AccountName);

            switch (resultCount)
            {
                case 0:
                    Reporting.Error("Provided bank account detail is not added in Shield");
                    break;
                case 1:
                    Reporting.IsTrue(true, "Provided bank account details added in Shield");
                    break;
                default:
                    Reporting.Error("Duplicate bank account details added in Shield");
                    break;
            }
        }
    }
}