using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

namespace UIDriver.Helpers
{
    public static class HelpersCreditCardPayment
    {
        /// <summary>
        /// For annual payments, as debit is taken immediately, the payment amount is
        /// shown in the credit card portal. This is collected for verification and
        /// returned to the caller.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="method"></param>
        /// <returns>The charged amount if an Annual payment, otherwise returns 0.</returns>
        public static void PerformCreditCardPayment(this Browser browser, Payment method)
        {
            using (var ccFrame = new WestpacQuickStream(browser))
            using (var spinner = new RACSpinner(browser))
            {
                ccFrame.WaitForPage();

                ccFrame.EnterCardDetails(method);
            }
        }
    }
}