using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BrowserStack;
using OpenQA.Selenium;
using PercyIO.Selenium;
using static PercyIO.Selenium.Percy;
using static Rac.TestAutomation.Common.Constants.General;
using Environment = System.Environment;

namespace Rac.TestAutomation.Common
{
    public class Browser : IDisposable
    {
        private IWebDriver _driver;
        private bool _isVisualTestingEnabled;

        private TargetDevice _deviceName = TargetDevice.Windows11;
        private TargetBrowser _browserName = TargetBrowser.Chrome;

        Local local = new Local();

        private const int MIN_WIDTH  = 1920;
        private const int MIN_HEIGHT = 1080;

        public IWebDriver Driver => _driver;
        
        public string PageTitle => _driver.Title;


        public TargetDevice DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

        public TargetBrowser BrowserName
        {
            get { return _browserName; }
            set { _browserName = value; }
        }

        public Browser()
        {
            _isVisualTestingEnabled = Config.Get().IsVisualTestingEnabled;            
        }

        public void OpenUrl(string url)
        {
            OpenUrl(Config.Get().GetBrowserType(), url, TargetDevice.Windows11, incognito: true);
        }

        public void OpenUrl(TargetBrowser webBrowser, string url, TargetDevice device, bool incognito = true)
        {            
            if (_driver == null)
            {
                if (Config.Get().IsCrossBrowserDeviceTestingEnabled)
                {
                    string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string browserStackBinaryPath = Path.Combine(homePath, ".browserstack\\BrowserStackLocal.exe");

                    if (!File.Exists(browserStackBinaryPath))
                    {
                        Reporting.Error($"Error: BrowserStackLocal binary file not found, please download and copy it to: {browserStackBinaryPath}");
                    }

                    List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>() 
                    {
                            new KeyValuePair<string, string>("key", Config.Get().BrowserStack.Automate.Key),
                            new KeyValuePair<string, string>("binarypath", browserStackBinaryPath),                            
                            new KeyValuePair<string, string>("forcelocal", "true")
                    };
                    // Starts the browserstack Local instance with the required arguments
                    local.start(bsLocalArgs);

                    DeviceName = device;
                    BrowserName = webBrowser;

                }
                _driver = DriverFactory.StartDriver(webBrowser, device, incognito);
            }

            _driver.Manage().Window.Maximize();

            try
            {
                Reporting.Log($"Opening <a href=\"{url}\">{url}</a>");
                _driver.Navigate().GoToUrl(url);

                 //Browserstack Automate doesn't allow to change the browser window size
                 //and it always open as a full screen
                if (!Config.Get().IsCrossBrowserDeviceTestingEnabled)
                {
                    // In case the test VMs are set to small screen resolutions, then force the
                    // Selenium client to use a larger view.
                    Reporting.Log($"Setting the new window size");

                    var sizeMaximised = _driver.Manage().Window.Size;
                    if (sizeMaximised.Width < MIN_WIDTH ||
                        sizeMaximised.Height < MIN_HEIGHT)
                    {
                        var newSize = new System.Drawing.Size(
                                          sizeMaximised.Width < MIN_WIDTH ? MIN_WIDTH : sizeMaximised.Width,
                                          sizeMaximised.Height < MIN_HEIGHT ? MIN_HEIGHT : sizeMaximised.Height);
                        _driver.Manage().Window.Size = newSize;
                    }
                }                
            }
            catch (WebDriverException ex)
            {
                Reporting.Error($"Page not open: {ex}", _driver.TakeSnapshot());
            }
        }

        public void PercyScreenCheck(string screenIdentifer, List<string> ignoreCSSEntries = null)
        {
            if (!_isVisualTestingEnabled) return;

            Options ignoreThese = null;
            var cssString = string.Empty;

            if (ignoreCSSEntries != null)
            {
                foreach (var cssItem in ignoreCSSEntries)
                {
                    cssString += $"{cssItem} {{ visibility: hidden; }}\r\n";
                }

                ignoreThese = new Options() { { "percyCSS", cssString } };
            }

            Percy.Snapshot((WebDriver)_driver, screenIdentifer, ignoreThese);           
        }

        public void CloseBrowser()
        {
            //Stop the browserstack local instance
            if (Config.Get().IsCrossBrowserDeviceTestingEnabled)
            {
                local.stop();
            }

            if (_driver != null) 
            { 
                DriverFactory.CloseDriver(); 
            }
            
            _driver = null;
        }

        public void Dispose()
        {                  
            CloseBrowser();
        }
    }
}
