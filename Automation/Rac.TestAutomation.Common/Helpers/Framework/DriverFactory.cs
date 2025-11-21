using System;
using System.Threading;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;


using static Rac.TestAutomation.Common.Constants.General;
using NUnit.Framework;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using OpenQA.Selenium.Safari;
using Environment = System.Environment;

namespace Rac.TestAutomation.Common
{
    public static class DriverFactory
    {
        /// <summary>
        /// The WebDriver (from v128 onwards) started loggin diagnostic
        /// information to the console, which included messages about
        /// the driver starting, the URL opened, local connections, etc.
        /// These are excessive noise in Azure pipeline logs. This value
        /// being true will ensure that information is hidden.
        /// </summary>
        private static readonly bool HideWebDriverDiagnosticInfo = true;

        private class ChromeBrowserOptions
        {            
            public static string Incognito = "--incognito";
            public static string Headless  = "headless";
        }

        private class FirefoxBrowserOptions
        {
            public static string Private            = "-private";
            public static string PrivateAlwaysStart = "browser.privatebrowsing.autostart";
            public static string Headless           = "--headless";
        }

        private class EdgeBrowserOptions
        {
            public static string DisableGPU        = "disable-gpu";
            public static string UserDataDirectory = "user-data-dir";
            public static string Headless          = "headless";
        }

        private static ThreadLocal<IWebDriver> _driverStored = new ThreadLocal<IWebDriver>();

        private static IWebDriver DriverStored
        {
            get
            {
                if (_driverStored == null || _driverStored.Value == null)
                { Reporting.Error("Need to call 'StartDriver' method before attempting to use it."); }

                return _driverStored.Value;
            }
            set
            {
                _driverStored.Value = value;
            }
        }
        public static IWebDriver StartDriver(TargetBrowser webBrowser, TargetDevice device, bool incognito = false)
        {
            switch (webBrowser)
            {
                case TargetBrowser.Chrome:               
                    DriverStored = GetChromeDriver(incognito, webBrowser, device);
                    break;
                case TargetBrowser.Firefox:
                    DriverStored = GetFirefoxBrowser(incognito);
                    break;
                case TargetBrowser.Edge:
                    DriverStored = GetEdgeBrowser(incognito,  webBrowser, device);
                    break;
                case TargetBrowser.Safari:                
                    DriverStored = GetSafariDriver(webBrowser, device);
                    break;
                default:
                    throw new NotImplementedException($"That web browser ({webBrowser.GetDescription()}) is not supported. Try chrome/firefox/edge.");
            }

            Reporting.LogMinorSectionHeading($"Starting browser type: {webBrowser.GetDescription()}");

            return DriverStored;
        }

        private static Dictionary<string, object> BrowserStackOptions(TargetDevice deviceName)
        {
            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();

            browserstackOptions.Add("buildName", TestContext.CurrentContext.Test.Name);
            browserstackOptions.Add("userName", Config.Get().BrowserStack.Automate.UserName);
            browserstackOptions.Add("accessKey", Config.Get().BrowserStack.Automate.Key);
            browserstackOptions.Add("idleTimeout", 300);
            browserstackOptions.Add("video", "true");
            browserstackOptions.Add("interactiveDebugging", "true");
            browserstackOptions.Add("local", "true");

            switch (deviceName)
            {
                case TargetDevice.iPhone14:
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("osVersion", "16");
                    browserstackOptions.Add("deviceName", "iPhone 14");
                    break;
                case TargetDevice.iPad10:
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("osVersion", "16");
                    browserstackOptions.Add("deviceName", "iPad 10th");
                    break;
                case TargetDevice.MacBook:
                    browserstackOptions.Add("osVersion", "Monterey");
                    break;
                case TargetDevice.Windows11:
                    browserstackOptions.Add("osVersion", "Windows 11");
                    break;
                case TargetDevice.GalaxyS21:
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("osVersion", "12.0");
                    browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                    break;
                default:
                    throw new NotSupportedException($"{deviceName.GetDescription()} is not supported for cross browser and device testing");
            }

            return browserstackOptions;
        }


        private static IWebDriver GetChromeDriver(bool incognito, TargetBrowser browser, TargetDevice device)
        {
            var config = Config.Get();
            var options = new ChromeOptions();

            if (config.IsCrossBrowserDeviceTestingEnabled)
            {
                var browserstackOptions = BrowserStackOptions(device);
                options.AddAdditionalOption("bstack:options", browserstackOptions);
                return new RemoteWebDriver(new Uri(config.BrowserStack.Automate.URL), options);
            }
            else
            {               
                // For Release builds, we run headless in the Azure pipelines so that we can
                // set our desired screen resolution without being constrained by host settings.
#if DEBUG
#else
                options.AddArgument(ChromeBrowserOptions.Headless);                
#endif
                if (incognito) options.AddArgument(ChromeBrowserOptions.Incognito);

                // This is to ensure that cross domain cookies are allowed. Needed to allow
                // Windows AD issued cookies work for NPE Spark platform
                options.AddUserProfilePreference("profile.cookie_controls_mode", 0);
                var chromeDriverSvc = ChromeDriverService.CreateDefaultService();
                chromeDriverSvc.SuppressInitialDiagnosticInformation = HideWebDriverDiagnosticInfo;
                return new ChromeDriver(chromeDriverSvc, options);
            }
        }

        private static IWebDriver GetFirefoxBrowser(bool incognito)
        {
            var options = new FirefoxOptions();

            options.AcceptInsecureCertificates = true;

            // For Release builds, we run headless in the Azure pipelines so that we can
            // set our desired screen resolution without being constrained by host settings.
#if DEBUG
#else
            options.AddArgument(FirefoxBrowserOptions.Headless);
#endif

            if (incognito)
            {
                options.AddArgument(FirefoxBrowserOptions.Private);
                options.SetPreference(FirefoxBrowserOptions.PrivateAlwaysStart, true);
            }

            return new FirefoxDriver(options);
        }

        private static IWebDriver GetEdgeBrowser(bool incognito, TargetBrowser browser, TargetDevice device)
        {
            var config = Config.Get();
            var options = new EdgeOptions();

            if (config.IsCrossBrowserDeviceTestingEnabled)
            {
                var browserstackOptions = BrowserStackOptions(device);
                options.AddAdditionalOption("bstack:options", browserstackOptions);
                return new RemoteWebDriver(new Uri(config.BrowserStack.Automate.URL), options);
            }
            else
            {
                // For Release builds, we run headless in the Azure pipelines so that we can
                // set our desired screen resolution without being constrained by host settings.
#if DEBUG
#else
            options.AddArgument(EdgeBrowserOptions.Headless);
#endif

                if (incognito)
                {
                    options.AddArguments("-inprivate");
                }

                var edgeDriverSvc = EdgeDriverService.CreateDefaultService();
                edgeDriverSvc.SuppressInitialDiagnosticInformation = HideWebDriverDiagnosticInfo;
                return new EdgeDriver(edgeDriverSvc, options);
            }
        }

        private static IWebDriver GetSafariDriver(TargetBrowser browser, TargetDevice device)
        {
            var config = Config.Get();
            var options = new SafariOptions();           

            // This is to ensure that cross domain cookies are allowed. Needed o allow
            // Windows AD issued cookies work for NPE Spark platform

            if (config.IsCrossBrowserDeviceTestingEnabled)
            {
                var browserstackOptions = BrowserStackOptions(device);
                options.BrowserVersion = "latest";                
                options.AddAdditionalOption("bstack:options", browserstackOptions);
                return new RemoteWebDriver(new Uri(config.BrowserStack.Automate.URL), options);
            }
            else
            {
                return new SafariDriver(SafariDriverService.CreateDefaultService(), options);
            }
        }


        public static IWebDriver GetDriver()
        {
            return DriverStored;
        }

        public static void CloseDriver()
        {
            try
            {
                var driver = DriverStored;
                driver.Quit();
                driver.Dispose();
            }
            catch
            {
                // eat exception. Probably trying to close a driver that was never initialised.
            }
            finally
            {
                DriverStored = null;
            }
        }
    }
}
