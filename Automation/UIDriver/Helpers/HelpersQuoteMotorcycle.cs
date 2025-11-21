using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Helpers
{
    public static class HelpersQuoteMotorcycle
    {
        /// <summary>
        /// Specific for SPARK pages, returning whether the red highlight is present
        /// on a given text input box.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="xpath"></param>
        /// <param name="nameOfField"></param>
        /// <returns></returns>
        public static bool IsTextBoxHighlightedError(this IWebDriver driver, string xpath, string nameOfField)
        {
            var hasErrorHighlight = false;
            var inputField = driver.FindElement(By.XPath(xpath));

            switch (inputField.GetAttribute("aria-invalid"))
            {
                case "true":
                    hasErrorHighlight = true;
                    break;
                case "false":
                    hasErrorHighlight = false;
                    break;
                default:
                    Reporting.Error($"Unexpected value in 'aria-invalid' when checking {nameOfField} text box.");
                    break;
            }
            return hasErrorHighlight;
        }

        public static bool IsTextBoxErrorMessagePresent(this IWebDriver driver, string xpath, string expectedErrorText)
        {
            IWebElement field;
            var hasErrorNotice = driver.TryFindElement(By.XPath($"{xpath}/../../p[contains(@class,'Mui-error')]"), out field);
            if (hasErrorNotice)
                Reporting.AreEqual(field.Text, expectedErrorText, $"field validation error for an invalid value");

            return hasErrorNotice;
        }

        /// <summary>
        /// Relevant for Here's Your Quote page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="xpath"></param>
        /// <param name="expectedErrorText"></param>
        /// <returns></returns>
        public static bool IsSumInsuredErrorMessagePresent(this IWebDriver driver, string xpath, string expectedErrorText)
        {
            IWebElement field;
            var hasErrorNotice = driver.TryFindElement(By.XPath($"{xpath}/../../../div[contains(@class,'MuiFormHelperText-root') and contains(@class,'Mui-error')]"), out field);
            if (hasErrorNotice)
                Reporting.AreEqual(field.Text, expectedErrorText, $"field validation error when an invalid Sum Insured value is input");
            return hasErrorNotice;
        }
    }
}
