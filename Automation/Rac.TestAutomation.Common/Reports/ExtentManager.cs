using System;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using NUnit.Framework;

namespace Rac.TestAutomation.Common
{
    public static class ExtentManager
    {
        private static readonly Lazy<ExtentReports> _lazy = new Lazy<ExtentReports>(() => new ExtentReports());
        public static ExtentReports Instance { get { return _lazy.Value; } }

        static ExtentManager()
        {
            var config = Config.Get();

            var reporter = new ExtentHtmlReporter(Reporting.ReportFolder);
            reporter.Config.Theme = Theme.Dark;
            Instance.AddSystemInfo("Environment", config.B2C.Url);
            Instance.AddSystemInfo("User Name", "RACI QA Team");
            Instance.AttachReporter(reporter);
        }
    }

}