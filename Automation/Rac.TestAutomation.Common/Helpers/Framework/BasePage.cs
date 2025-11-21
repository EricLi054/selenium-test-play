using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.SparkBasePage;

namespace Rac.TestAutomation.Common
{
    public static class FindHelpers
    {
        private const int SCREENSHOT_RETRIES = 3;

        /// <summary>
        /// Extended version of Selenium's FindElement, but returns a boolean success code
        /// instead of throwing an exception.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="criteria"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryFindElement(this IWebDriver driver, By criteria, out IWebElement element)
        {
            bool success = false;
            element = null;

            try
            {
                element = driver.FindElement(criteria);
                success = element != null;
            } catch(NoSuchElementException) { /* Consume exception as we'll return boolean success code */ }

            return success;
        }

        /// <summary>
        /// Returns boolean to indicate success of waiting for an element to be found in a 
        /// given time, instead of throwing an exception.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="criteria"></param>
        /// <param name="waitTimeSeconds"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryWaitForElement(this IWebDriver driver, By criteria, int waitTimeSeconds, out IWebElement element)
        {
            bool success = false;
            element = null;

            try
            {
                element = driver.WaitForElement(criteria, waitTimeSeconds);
                success = element != null;
            }
            catch(NoSuchElementException) { /* Consume exception as we'll return boolean success code */ }

            return success;
        }

        /// <summary>
        /// Returns boolean to indicate success of waiting for a visible element to be 
        /// found in a given time, instead of throwing an exception.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="criteria"></param>
        /// <param name="waitTimeSeconds"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryWaitForElementToBeVisible(this IWebDriver driver, By criteria, int waitTimeSeconds, out IWebElement element)
        {
            bool success = false;
            element = null;

            try
            {
                element = driver.WaitForElementToBeVisible(criteria, waitTimeSeconds);
                success = element != null;
            }
            catch(NoSuchElementException) { /* Consume exception as we'll return boolean success code */ }

            return success;
        }

        /// <summary>
        /// Searches for element to be found for the time given. Element does not need to be
        /// visible or enabled, just to be present in the DOM.
        /// </summary>
        /// <exception cref="NoSuchElementException">Throws if not found in time.</exception>
        public static IWebElement WaitForElement(this IWebDriver driver, By criteria, int waitTimeSeconds)
        {
            var endTime = DateTime.Now.AddSeconds(waitTimeSeconds);
            var found = false;
            IWebElement element = null;

            do
            {
                try
                {
                    element = driver.FindElement(criteria);
                    found = true;
                }
                catch(Exception ex) when (ex is NoSuchElementException || ex is StaleElementReferenceException)
                {
                    found = false;
                }                
            } while (DateTime.Now < endTime && !found);
            if (!found)
            {
                throw new NoSuchElementException($"Timed out waiting for element. See {driver.TakeSnapshot()}");
            }
            return element;
        }

        /// <summary>
        /// Searches for an element with the added requirement that it is visible
        /// (display property cannot be 'none').
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="criteria"></param>
        /// <param name="waitTimeSeconds"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException">Thrown if times out before element is found or does not become visible in time.</exception>
        public static IWebElement WaitForElementToBeVisible(this IWebDriver driver, By criteria, int waitTimeSeconds)
        {
            var endTime = DateTime.Now.AddSeconds(waitTimeSeconds);
            var found = false;
            IWebElement element = null;

            do
            {
                try
                {
                    element = driver.FindElement(criteria);
                    if (element.Displayed)
                    { found = true; }
                }
                catch (WebDriverException wde) // Traps all webdriver exceptions including StaleElement and NoSuchElement
                {
                    Reporting.Log($"Selenium exception waiting for element '{criteria.ToString()}' to become visible. We are in a retry loop. {wde.Message}");
                    found = false;
                }
                finally
                {
                    Thread.Sleep(SleepTimes.T500MS);
                }
            } while (DateTime.Now < endTime && !found);
            if (!found)
            {
                throw new NoSuchElementException($"Timed out waiting for element '{criteria.ToString()}'. See {driver.TakeSnapshot()}");
            }
            return element;
        }

        /// <summary>
        /// Wait for a visible element to become invisible before
        /// continuing. For example confirming that a Help Text 
        /// modal has been dismissed.
        /// </summary>
        /// <param name="criteria">Identify the element to check for [e.g. By.XPath(XPath.HelpText.Title)]</param>
        /// <param name="waitTimeSeconds">Maximum wait after which this method will throw an exception.</param>
        /// <exception cref="InvalidElementStateException">Element is still visible</exception>
        public static void WaitForElementToBeInvisible(this IWebDriver driver, By criteria, int waitTimeSeconds)
        {
            var endTime = DateTime.Now.AddSeconds(waitTimeSeconds);
            var visible = true;
            IWebElement element = null;

            do
            {
                Thread.Sleep(SleepTimes.T500MS);
                try
                {
                    element = driver.FindElement(criteria);
                    visible = element.Displayed;
                }
                catch(NoSuchElementException)
                {
                    // Exception accessing element means it is no longer present and test can continue.
                    visible = false;
                    Reporting.Log($"WaitForElementToBeInvisible confirmed element {criteria.ToString()}.Displayed = {visible} so test can continue");
                }
            } while (DateTime.Now < endTime && visible);
            if (visible)
            {
                throw new InvalidElementStateException($"Timed out waiting for element {criteria.ToString()} to disappear. See {driver.TakeSnapshot()}");
            }
        }

        /// <summary>
        /// This method waits for the element until it's enabled or the optional Constant.WaitTimes (default 5 seconds) parameter is exceeded.
        /// </summary>
        /// <param name="waitTimeSeconds">optional and defaulted to 5 seconds</param>
        /// <exception cref="NoSuchElementException">Element was not found, or not in correct state</exception>
        public static IWebElement WaitForElementToBeEnabled(this IWebDriver driver, By criteria, int waitTimeSeconds = WaitTimes.T5SEC)
        {
            var endTime = DateTime.Now.AddSeconds(waitTimeSeconds);
            var found = false;
            IWebElement element = null;

            do
            {
                try
                {
                    element = driver.FindElement(criteria);
                    if (element.Enabled)
                        found = true;
                }
                catch(NoSuchElementException)
                {
                    found = false;
                }
            } 
            while (DateTime.Now < endTime && !found);
            
            if (!found)
            {
                throw new NoSuchElementException(string.Format($"Timed out waiting for element {criteria.ToString()}. See {driver.TakeSnapshot()}"));
            }
            return element;
        }


        /// <summary>
        /// Take a screenshot of the page and generate a name for it with the syntax screenshot_yyyyMMddHHmmssfff.png
        /// Saves in a 'Report' folder with the ExtentReport.
        /// </summary>
        /// <param name="driver">No parameters required here.</param>
        /// <returns></returns>
        public static string TakeSnapshot(this IWebDriver driver)
        {
            var savePath = Reporting.ReportFolder;
            var attemptCounter = 0;

            Screenshot ss = null;
            // If screenshot fails (which has been occurring alot with React)
            // then just return null, which means no screenshot to add to log message.
            try
            { ss = ((ITakesScreenshot)driver).GetScreenshot(); }
            catch (WebDriverException)
            { return null; }

            string filename;

            do
            {
                attemptCounter++;

                filename = $"{savePath}screenshot_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png";
                try { ss.SaveAsFile(filename); }
                catch(IOException)
                {
                    Reporting.Log("Unable to save a screenshot due to file conflict. Another thread was trying to save to a file with the same name.");
                    filename = null;
                }
            } while (string.IsNullOrEmpty(filename) && (attemptCounter <= SCREENSHOT_RETRIES));
            return filename;
        }
        
    }
    

    abstract public class BasePage : IDisposable
    {
        public void Dispose() {}

        protected IWebDriver _driver;
        protected Browser    _browser;

        protected string     _savePath;

        #region XPATHS
        protected const string XPEXT_DROPDOWN_VALUE = "//span[starts-with(@class,'k-input')]"; //Used to amend xpath of dropdown to get value.
        private const string XP_CHECKMARK           = "//i[@class='icon-check']";
        #endregion

        public BasePage(Browser browser)
        {
            _browser = browser;
            _driver  = browser.Driver;
        }

        abstract public bool IsDisplayed();

        /// <summary>
        /// Calling method with Xpath
        /// will return the matching web element
        /// </summary>
        /// <param name="xpath"></param>        
        protected IWebElement GetElement(string xpath)
        {
            return _driver.FindElement(By.XPath(xpath));
        }

        /// <summary>
        /// Looks to return the current string value of a specified attribute
        /// from the DOM element identified by the XPath. This method will
        /// retry once if any selenium error occurs during the first attempt
        /// to read.
        /// NOTE: If you are after the "Value" attribute, we have an explicit
        /// method for that called 'GetValue()'
        /// </summary>
        /// <param name="attributeName">This is the name of the attribute you want the value from. It must match exactly.</param>
        /// <returns></returns>
        protected string GetAttribute(string xpath, string attributeName)
        {
            var success        = false;
            var attemptCount   = 0;
            var attributeValue = string.Empty;
            do
            {
                try
                {
                    var element = GetElement(xpath);
                    attributeValue = element.GetAttribute(attributeName);
                    success = true;
                }
                catch (StaleElementReferenceException)
                {
                    Reporting.Log($"StaleElementReference for xpath {xpath}, we will retry. Attempt #{attributeValue+1}");
                }
                finally
                { attemptCount++; }
            } while (!success && (attemptCount < 2));

            return attributeValue;
        }

        public void WaitForPage(int waitTimeSeconds = WaitTimes.T60SEC)
        {
            var loaded = false;
            var endTime = DateTime.Now.AddSeconds(waitTimeSeconds);
            do
            {
                loaded = IsDisplayed();
                if (!loaded) Thread.Sleep(500);
            } while (loaded == false && DateTime.Now < endTime);
            if (!loaded)
            {
                Reporting.Error($"{DateTime.Now.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS)} Time-out waiting for page. Waited {waitTimeSeconds}s");
            }
        }

        /// <summary>
        /// For legacy B2C/PCM: Supports searching for an address
        /// via the QAS search and selecting the first result.
        /// As the QAS search (via Shield) can be slow or even
        /// unresponsive, the code will ammend the address to
        /// prompt resubmitting the search.
        /// 
        /// Will attempt to find a result within 30seconds.
        /// </summary>
        /// <param name="xpathSearchField">XPath for search field</param>
        /// <param name="xpathQASSuggestions">XPath for search result list. If multiple matches, will select the first.</param>
        /// <param name="searchString"></param>
        /// <exception cref="Exception">If no result found in time.</exception>
        public void QASSearchForAddress(string xpathSearchField, string xpathQASSuggestions, string searchString)
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T60SEC);
            var success = false;
            do
            {
                Reporting.Log($"Entering string of '{searchString}' into QAS field to trigger search");
                // Set mailing address:
                WaitForTextFieldAndEnterText(xpathSearchField, searchString, false);

                // Sleep time to allow QAS to respond
                Thread.Sleep(10000);
                try
                {
                    var searchResultRows = _driver.FindElements(By.XPath(xpathQASSuggestions));

                    if (searchResultRows.Count > 0)
                    {
                        var desiredRow = searchResultRows.FirstOrDefault(x => string.Equals(x.Text, searchString, StringComparison.InvariantCultureIgnoreCase)) 
                                               ?? searchResultRows[0];
                        if (desiredRow.Size.Height > 0)
                        {
                            desiredRow.Click();
                            success = true;
                            break;
                        }
                    }
                }
                catch(WebDriverException wde)
                {
                    /* Error handling is managed by retries and success flag */
                    Reporting.Log($"Selenium exception trying to drive QAS search field {xpathSearchField}. We are in a retry loop. {wde.Message}");
                }
            } while (DateTime.Now < endTime && !success);

            if (!success)
            { Reporting.Error($"Did not get suggestions from QAS for address {searchString} within expected time."); }
        }

        /// <summary>
        /// To support cases where a control may exist in multiple forms, but
        /// only have one, or a few, visible for the current context without
        /// any clear indicator in the control's attributes.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns>list of relevant DOM elements that are visible</returns>
        protected List<IWebElement> GetElementsThatAreVisible(string xpath)
        {
            var visibleControls = new List<IWebElement>();
            var allControlInstances = _driver.FindElements(By.XPath(xpath));

            foreach (var control in allControlInstances)
            {
                if (control.Displayed)
                { visibleControls.Add(control); }
            }
            if (visibleControls.Count == 0) Reporting.Error($"Could not find any visible controls for xPath: {xpath}.");

            return visibleControls;
        }

        /// <summary>
        /// Method for fields where pre-existing text remains if the user clicks
        /// in them, and hence should be deleted before entering new text.
        /// Supports type-ahead option, where if true, method will press the
        /// (Keys.Return) / [enter] key to trigger selection of first option.
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="text"></param>
        /// <param name="hasTypeAhead"></param>
        protected void WaitForTextFieldAndEnterText(string xPath, string text, bool hasTypeAhead = true)
        {
            GenericTextFieldWithTypeAheadOrDropDown(xPath: xPath, text: text, hasTypeAhead: hasTypeAhead, clearExistingText: true);
        }

        /// <summary>
        /// This method is specifically for inputting the date of birth into the myRAC account registration form 
        /// as using existing methods results in only the year being filled.
        /// 
        /// It starts by selecting the element (which means the cursor is in the 'year' section) and then sends SHIFT+TAB twice to move
        /// he cursor to the 'day' section. It then inputs the date of birth as a string which should have been passed in 'ddMMyyyy' format.
        /// </summary>
        /// <param name="xPath">The control to input a string into</param>
        /// <param name="text">The date of birth to input in 'ddMMyyyy' format</param>
        protected void MyRACDateOfBirthEntry(string xPath, string text)
        {
            Actions actions = new Actions(_driver);
            
            Reporting.Log($"Pressing Shift+Tab twice to place the cursor in the DD part of the element.");
            actions.MoveToElement(GetElement(xPath)).Click().KeyDown(Keys.Shift).SendKeys(Keys.Tab).SendKeys(Keys.Tab).KeyUp(Keys.Shift).Perform();
            
            actions.SendKeys(text).Perform();
            Reporting.Log($"Capturing screen after inputting date of birth as '{text}'", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Method for text fields which clear themselves on focus. Typically for
        /// fields which provide a static list of options to pick from. Will
        /// emulate (Keys.Return) / [enter] key after text to trigger selection 
        /// of first option from resulting list that matches entered text.
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="text">text to type into field to trigger desired item from dropdown</param>
        /// <param name="waitTime">timw to wait to allow dropdown selections to appear before picking.</param>
        protected void WaitForSelectableAndPickByTyping(string xPath, string text, int waitTime = 1000)
        {
            GenericTextFieldWithTypeAheadOrDropDown(xPath: xPath, text: text, waitTime: waitTime, hasTypeAhead: true, clearExistingText: false);
        }

        /// <summary>
        /// Allows entry of text into a text field where a list of matching options is retrieved
        /// in a dropdown format, then selects the specific option that exactly matches the
        /// search text.
        /// </summary>
        /// <param name="xPath">XPath of the text field where the search text is to be entered</param>
        /// <param name="xPathDropdownOptions">XPath to identify the dropdown options that will be presented. Search text will be appended to this path.</param>
        /// <param name="text">Desired search text. Will be treated as exact match when assessing dropdown options</param>
        /// <param name="waitTime">Wait time for initial search text field to be available, in milliseconds</param>
        protected void WaitForSelectableFieldToSearchAndPickFromDropdown(string xPath, string xPathDropdownOptions, string text, int waitTime = SleepTimes.T1SEC)
        {
            GenericTextFieldWithTypeAheadOrDropDown(xPath: xPath, 
                                                    xPathDropdown: xPathDropdownOptions,
                                                    text: text,
                                                    waitTime: waitTime, 
                                                    hasTypeAhead: false, 
                                                    clearExistingText: true);
        }

        private void GenericTextFieldWithTypeAheadOrDropDown(string xPath, string text, string xPathDropdown = null, int waitTime = SleepTimes.T1SEC, bool hasTypeAhead = true, bool clearExistingText = true)
        {
            try
            {
                var field = _driver.WaitForElementToBeVisible(By.XPath(xPath), WaitTimes.T5SEC);
                var endTime = DateTime.Now.AddSeconds(5);
                do
                {
                    Thread.Sleep(500);
                } while (DateTime.Now < endTime && !field.Enabled);

                if (DateTime.Now > endTime)
                {
                    Reporting.Error("Timed out waiting for text field to be editable.");
                }
                //TODO RAI-328 - Find a way to support this for Mac platform
                if (_browser.DeviceName != TargetDevice.MacBook)
                {
                    ClickControl(xPath);
                }
                if (clearExistingText)
                {
                    field.Clear();
                    //Safari browser doesn't support Keys.Delete //TODO RAI-329 - Find a way to support this
                    if (_browser.BrowserName != TargetBrowser.Safari)
                    {
                        field.SendKeys($"{Keys.Control}a");
                        Thread.Sleep(500);
                        field.SendKeys($"{Keys.Delete}");
                        Thread.Sleep(500);
                    }
                } 

                field.SendKeys(text);

                if (!string.IsNullOrEmpty(xPathDropdown))
                {
                    var xpathSpecificOption = $"{xPathDropdown}[translate(text(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')=\"{text.ToLower()}\"]";
                    // We want to skip the force scroll as that can interfere with the dropdown UI library
                    ClickControl(xpathSpecificOption, waitTimeSeconds: WaitTimes.T30SEC, skipJSScrollLogic: true);
                }
                else if (hasTypeAhead)
                {
                    Thread.Sleep(waitTime);

                    field.SendKeys(Keys.Return);
                }
                // Inbuilt sleep as dropdown animation may be slow to close after hitting (Keys.Return) / [enter] or selecting dropdown value.
                // sleep allows time for dropdown to disappear.
                Thread.Sleep(500);
            }
            catch (Exception ex) when (ex is NoSuchElementException || ex is InvalidElementStateException || ex is StaleElementReferenceException)
            {
                Reporting.Error($"{DateTime.Now.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS)} Fault occurred waiting for, or driving, text field ({xPath})");
            }
        }

        protected void WaitForTextFieldTypeOverText(string xPath, string text, bool hasTypeAhead = true)
        {
            GenericTextFieldWhichBlocksClearingField(xPath: xPath, text: text, hasTypeAhead: hasTypeAhead);
        }

        private void GenericTextFieldWhichBlocksClearingField(string xPath, string text, string xPathDropdown = null, int waitTime = 1000, bool hasTypeAhead = true)
        {
            try
            {
                var field = _driver.WaitForElementToBeVisible(By.XPath(xPath), WaitTimes.T5SEC);
                var endTime = DateTime.Now.AddSeconds(5);
                do
                {
                    Thread.Sleep(500);
                } while (DateTime.Now < endTime && !field.Enabled);

                if (DateTime.Now > endTime)
                {
                    Reporting.Error("Timed out waiting for text field to be editable.");
                }

                ClickControl(xPath);

                field.SendKeys(Keys.Control + "a");
                
                field.SendKeys(text);

                if (!string.IsNullOrEmpty(xPathDropdown))
                {
                    var xpathSpecificOption = $"{xPathDropdown}[translate(text(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')=\"{text.ToLower()}\"]";
                    // We want to skip the force scroll as that can interfere with the dropdown UI library
                    ClickControl(xpathSpecificOption, waitTimeSeconds: WaitTimes.T30SEC, skipJSScrollLogic: true);
                }
                else if (hasTypeAhead)
                {
                    Thread.Sleep(waitTime);

                    field.SendKeys(Keys.Return);
                }
                // Inbuilt sleep as dropdown animation may be slow to close after hitting (Keys.Return) / [enter] or selecting dropdown value.
                // sleep allows time for dropdown to disappear.
                Thread.Sleep(500);
            }
            catch(NoSuchElementException)
            {
                Reporting.Error($"{DateTime.Now.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS)} Fault occurred waiting for, or driving, text field ({xPath})");
            }
            catch (StaleElementReferenceException)
            {
                GetElement(xPath).SendKeys(text);
            }

        }

        /// <summary>
        /// Used for dropdowns with no typing filter. Will screen out leading/trailing whitespace
        /// from both the provided text, as well as the dropdown values.
        /// </summary>
        /// <param name="xPathBaseControl">XPath to the base control to click to trigger dropdown display</param>
        /// <param name="xPathDropDown">XPath that will return the collection of elements forming the dropdown</param>
        /// <param name="optionText">test to match to find the specific desired dropdown element</param>
        /// <param name="partialMatching">if true, will select first dropdown option that contains the given optionText, otherwise it will only select an exact match</param>
        protected void WaitForSelectableAndPickFromDropdown(string xPathBaseControl, string xPathDropDown, string optionText, bool partialMatching = false)
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            var success = false;
            do
            {
                try
                {
                    ClickControl(xPathBaseControl, hasFailOver: true);

                    // As we can get some dropdown options with apostrophes, using XPath
                    // string literals to ensure they don't break syntax. This issue commonly
                    // occurs when working with contact names.
                    var xpathSpecificOption = $"{xPathDropDown}[contains(text(),\"{optionText}\")]";

                    // Wait for a first match, as caller is expecting at least one.
                    _driver.WaitForElementToBeVisible(By.XPath(xpathSpecificOption), WaitTimes.T5SEC);
                    Thread.Sleep(500); // Brief pause to allow for dropdown animations

                    // Get all matches and iterate until we get one that matches exactly, minus whitespace.
                    var allOptions = _driver.FindElements(By.XPath(xpathSpecificOption));

                    // We do the "Replace" on the provided optionText here because
                    // Selenium's ".Text" property normalises internal whitespace as well.
                    var desiredOptionText = optionText.RemoveDuplicateWhiteSpaceAndTrim();
                    foreach (var matchingOption in allOptions)
                    {
                        if ((!partialMatching && string.Equals(matchingOption.Text, desiredOptionText)) ||
                            (partialMatching  && matchingOption.Text.Contains(desiredOptionText)))
                        {
                            matchingOption.Click();
                            success = true;
                        }
                    }
                }
                catch (WebDriverException wde)
                {
                    /* Consume exception as success validation is done outside do-while loop */
                    Reporting.Log($"Selenium exception trying to drive dropdown {xPathBaseControl} and option '{optionText}'. We are in a retry loop. {wde.Message}");
                }
            } while (DateTime.Now < endTime && !success);

            if (!success)
            {
                Reporting.Error($"Either unable to drive base control ({xPathBaseControl}) or find desired dropdown option: {optionText}.");
            }

            // Small sleep to allow for dropdown collapse animation to complete.
            Thread.Sleep(700);
        }

        /// <summary>
        /// Base method to send key events to a given UI control. This method
        /// does not attempt to clear any existing text that might be in this
        /// control, it will just send the requested key events.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        protected void SendKeyPressesToField(string xpath, string text)
        {
            try
            {
                var field = GetElement(xpath);
                var endTime = DateTime.Now.AddSeconds(5);
                do
                {
                    Thread.Sleep(500);
                } while (DateTime.Now < endTime && !field.Enabled);

                if (DateTime.Now > endTime)
                {
                    Reporting.Error("Timed out waiting for text field to be editable.");
                }

                field.SendKeys(text);
            }
            catch(NoSuchElementException)
            {
                Reporting.Error(DateTime.Now.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS) + " Fault occurred waiting for, or driving, text field.");
            }
            catch (StaleElementReferenceException)
            {
                GetElement(xpath).SendKeys(text); 
            }
        }

        /// <summary>
        /// Base method to send key events to a given UI control. Will emulate
        /// CTRL+A and DEL events to attempt to clear any pre-existing text.
        /// Also occasionally useful because it does not send (Keys.Return) / [enter] 
        /// after the specified string.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        protected void SendKeyPressesAfterClearingExistingTextInField(string xpath, string text)
        {
            try
            {
                var field = GetElement(xpath);
                var endTime = DateTime.Now.AddSeconds(5);
                do
                {
                    Thread.Sleep(500);
                } while (DateTime.Now < endTime && !field.Enabled);

                if (DateTime.Now > endTime)
                {
                    Reporting.Error("Timed out waiting for text field to be editable.");
                }

                // Keypresses slowed to allow browser time to process.
                field.SendKeys($"{Keys.Control}a");
                Thread.Sleep(800);
                field.SendKeys($"{Keys.Delete}");
                Thread.Sleep(800);
                SendKeyPressesToField(xpath, text);
            }
            catch(NoSuchElementException)
            {
                Reporting.Error(DateTime.Now.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS) + " Fault occurred waiting for, or driving, text field.");
            }
        }

        /// <summary>
        /// Used to click a simple control. A inbuilt wait of 5seconds is also
        /// included if that control has conditional visibility from a previous
        /// action. If Selenium throws any exceptions in trying to operate on
        /// the requested control, the code will retry until either successful
        /// or time has expired.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="hasFailOver">TRUE means that the framework has fallback behaviour,
        /// and so do not log an error if this operation fails.</param>
        /// <param name="waitTimeSeconds">Time to keep retrying click operation</param>
        /// <param name="skipJSScrollLogic">If TRUE will not try to force a scroll into view.</param>
        /// <exception cref="NoSuchElementException">Thrown if the element can't be driven, or we don't find it in time.</exception>
        protected void ClickControl(string xpath, int waitTimeSeconds = WaitTimes.T5SEC, bool hasFailOver = false, bool skipJSScrollLogic = false)
        {
            PerformBaseClickOnControl(xpath, waitTimeSeconds, hasFailOver, skipJSScrollLogic);
        }

        private void PerformBaseClickOnControl(string xpath, int attemptTimeSeconds, bool hasFailOver = false, bool skipJSScrollLogic = false)
        {
            var endTime = DateTime.Now.AddSeconds(attemptTimeSeconds);
            var success = false;
            do
            {
                try
                {
                    var control = GetElement(xpath);
                    if (control.Displayed)
                    {
                        if (!skipJSScrollLogic)
                        { ScrollElementIntoView(xpath); }

                        control.Click();
                        success = true;
                        break;
                    }
                }
                catch (WebDriverException wde)
                {
                    /* Consume exception as success check is done outside do-while.
                     * Includes StateElementReference, noSuchElement and ElementClickIntercepted */
                    Reporting.Log($"Selenium exception trying to find and click {xpath}. We are in a retry loop. {wde.Message}");
                }

                Thread.Sleep(1000);
            } while (DateTime.Now < endTime);

            if (!success && !hasFailOver)
            { throw new NoSuchElementException($"Failed to perform action in given time. Targeting: {xpath}"); }
        }

        /// <summary>
        /// Attempts to inject a javascript operation to scroll a control into
        /// view. This method is to support automation as a large number of
        /// DOM elements cannot be driven if they aren't within the viewable
        /// area of the browser.
        /// </summary>
        /// <param name="xpath"></param>
        protected void ScrollElementIntoView(string xpath)
        {
            var control = GetElement(xpath);
            // JS operation to try to force scroll to centre of screen. 500ms sleep to allow for animation.
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({ block: 'center', inline: 'nearest'})", control);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Handles B2C binary toggles (e.g.: Yes / No), where 'class'
        /// attribute reflects any current selection by containing the
        /// term "checked".
        /// </summary>
        /// <param name="baseXPath">root DOM element that the relative xPaths will work from.</param>
        /// <param name="xPathForTrue">A relative xPath from the provided base to choose when 'value' is true.</param>
        /// <param name="xPathForFalse">A relative xPath from the provided base to choose when 'value' is false.</param>
        /// <param name="value">boolean flag to indicate which control to click.</param>
        protected void ClickBinaryToggle(string baseXPath, string xPathForTrue, string xPathForFalse, bool value)
        {
            var isClickToControlRequired = true;
            try
            {
                if (value == GetBinaryToggleState(baseXPath, xPathForTrue, xPathForFalse))
                {
                    isClickToControlRequired = false;
                }
            }
            catch(InvalidElementStateException)
            {
                // Do nothing. Means that no value had been set. So just click control.
            }

            if (isClickToControlRequired)
            {
                var desired_button_xpath = value ? xPathForTrue : xPathForFalse;
                ClickControl($"{baseXPath}{desired_button_xpath}");
            }
        }

        /// <summary>
        /// Method to retrieve a boolean representation of a toggle control. If the
        /// toggle has not yet had any value entered, then it throws an exception.
        /// 
        /// Expects that "checked" will be in the UI control's class attribute to indicate if checked.
        /// </summary>
        /// <param name="baseXPath">root DOM element that the relative xPaths will work from.</param>
        /// <param name="xPathForTrue">A relative xPath from the provided base, where if it is checked return TRUE.</param>
        /// <param name="xPathForFalse">A relative xPath from the provided base, where if it is checked will return FALSE.</param>
        /// <returns></returns>
        /// <exception cref="InvalidElementStateException">If no value had been set</exception>
        protected bool GetBinaryToggleState(string baseXPath, string xPathForTrue, string xPathForFalse)
        {
            // Check button for "TRUE" case.
            if (IsToggleButtonSelected($"{baseXPath}{xPathForTrue}"))
            { return true; }

            // Check button for "FALSE" case.
            if (IsToggleButtonSelected($"{baseXPath}{xPathForFalse}"))
            { return false; }

            throw new InvalidElementStateException($"A value has not yet been set for this binary control. Neither of the two options were marked as checked.");
        }

        /// <summary>
        /// Handles B2C tri-state toggles (e.g.: Yes / No / Unknown), where 'class'
        /// attribute reflects any current selection by containing the term "checked".
        /// Tri-state is set where True/False sets two primary options, and NULL sets
        /// a third option, such as "Unknown", "Unsure" etc.
        /// </summary>
        /// <param name="baseXPath">root DOM element that the relative xPaths will work from.</param>
        /// <param name="xPathForTrue">A relative xPath from the provided base to choose when 'value' is TRUE.</param>
        /// <param name="xPathForFalse">A relative xPath from the provided base to choose when 'value' is FALSE.</param>
        /// <param name="xPathForNull">A relative xPath from the provided base to choose when 'value' is NULL</param>
        /// <param name="value">nullable bool to indicate which control to click.</param>
        protected void ClickTriStateToggleWithNullableInput(string baseXPath, string xPathForTrue, string xPathForFalse, string xPathForNull, bool? value)
        {
            var isClickToControlRequired = true;
            try
            {
                if (value == GetNullableBinaryForTriStateToggle(baseXPath, xPathForTrue, xPathForFalse, xPathForNull))
                {
                    isClickToControlRequired = false;
                }
            }
            catch(InvalidElementStateException)
            {
                // Do nothing. Means that no value had been set. So just click control.
            }

            if (isClickToControlRequired)
            {
                var desired_button_xpath = xPathForNull;
                if (value.HasValue)
                    desired_button_xpath = value.Value ? xPathForTrue : xPathForFalse;
                ClickControl($"{baseXPath}{desired_button_xpath}");
            }
        }

        /// <summary>
        /// Returns a nullable bool as a representation of the currently selected
        /// control from a group of three buttons. This should only be applied to
        /// button groups where a true/false would normally select from either of
        /// two primary options, and a third option is selected if a NULL is
        /// provided.
        /// 
        /// Examples are "Yes/No/Unknown" button groups.
        /// 
        /// If no button is currently selected, then an exception is thrown.
        /// </summary>
        /// <param name="baseXPath">(optional) root XPath for button group</param>
        /// <param name="xPathForTrue"></param>
        /// <param name="xPathForFalse"></param>
        /// <param name="xPathForNull"></param>
        /// <param name="logContextMessage">(optional) to override exception message if a non-generic error message is desired.</param>
        /// <exception cref="InvalidElementStateException">No button was determined to have been selected.</exception>
        protected bool? GetNullableBinaryForTriStateToggle(string baseXPath, string xPathForTrue, string xPathForFalse, string xPathForNull, string logContextMessage = "requested")
        {
            // Check button for "TRUE" case.
            if (IsToggleButtonSelected($"{baseXPath}{xPathForTrue}"))
            { return true; }

            // Check button for "FALSE" case.
            if (IsToggleButtonSelected($"{baseXPath}{xPathForFalse}"))
            { return false; }

            // Check button for "null" (e.g. skip) case.
            if (IsToggleButtonSelected($"{baseXPath}{xPathForNull}"))
            { return null; }

            throw new InvalidElementStateException($"No option had yet been given for the {logContextMessage} control.");
        }

        /// <summary>
        /// Returns the value of the "class" attribute for the requested
        /// control.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        protected string GetClass(string xpath)
        {
            return GetAttribute(xpath, "class");
        }

        /// <summary>
        /// Returns the string from the "Value" attribute
        /// for the given HTML element. This does NOT return
        /// inner text.
        /// 
        /// This method is different from GetInnerText().
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        protected string GetValue(string xpath)
        {
            return GetAttribute(xpath, "value").Trim();
        }

        /// <summary>
        /// Returns the inner text of the given HTML element.
        /// 
        /// NOTE: This does NOT return the text of child elements. It only
        /// gets the text directly within the DOM element identified by the
        /// provided XPath.
        /// 
        /// This method is different from GetValue().
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        protected string GetInnerText(string xpath)
        {
            return _driver.WaitForElementToBeVisible(By.XPath(xpath), WaitTimes.T5SEC).Text.Trim();
        }

        /// <summary>
        /// Returns whether the control is enabled or not.
        /// NOTE: the effectiveness of thie call is based on whether
        /// the requested UI element was implemented with support for
        /// standard HTML enabled/disabled attributes. If things like
        /// custom CSS or other dev tricks are used to hide/disable
        /// a control, then Selenium won't be detecting it properly.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        protected bool IsControlEnabled(string xpath)
        {
            return GetElement(xpath).Enabled;
        }

        /// <summary>
        /// Returns whether the control is displayed or not.
        /// NOTE: the effectiveness of thie call is based on whether
        /// the requested UI element was implemented with styling
        /// support that sets the 'display' value in the 'style'
        /// attribute.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        protected bool IsControlDisplayed(string xpath)
        {
            IWebElement control = null;
            var isFound = _driver.TryFindElement(By.XPath(xpath), out control);

            return isFound ? control.Displayed : false;
        }

        private bool IsToggleButtonSelected(string xpath)
        {
            // For the SPARK platform, the @aria-pressed attribute will either be 'true'/'false'
            // to reflect whether it is checked.
            // For legacy B2C/PCM, the @class attribute will contains 'checked' if button is
            // currently selected. If not selected, then that value will be missing from @class.
            var field = GetElement(xpath);
            if (field.GetAttribute("aria-pressed") == "true" ||
                field.GetAttribute("class").Contains("checked"))
            { return true; }

            return false;
        }

        protected string GetSelectedTextFromDropDown(string xpath)
        {
            IWebElement comboBox = GetElement(xpath);
            SelectElement selectedValue = new SelectElement(comboBox);
            return selectedValue.SelectedOption.Text;
        }

    }
}
