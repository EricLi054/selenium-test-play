using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.EFT
{
    public class EnterYourBankDetails : SparkPaymentPage
    {
        public EnterYourBankDetails(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPathPayment.Button.Submit);
                GetElement(XPathPayment.Bank.Bsb);
                GetElement(XPathPayment.Bank.AccountNumber);
                GetElement(XPathPayment.Bank.AccountName);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }
    }
}