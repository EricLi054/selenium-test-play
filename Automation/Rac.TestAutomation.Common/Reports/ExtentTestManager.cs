using AventStack.ExtentReports;
using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public class ExtentTestManager
    {
        [ThreadStatic]
        private static ExtentTest _parentTest;

        [ThreadStatic]
        private static ExtentTest _childTest;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest CreateParentTest(string fixtureName, string description = null)
        {
            ILoggerRepository loggerRepository = LogManager.CreateRepository($"{fixtureName}Repository");
            ThreadContext.Properties["LogName"] = fixtureName;
            log4net.Config.XmlConfigurator.Configure(loggerRepository);

            _parentTest = ExtentManager.Instance.CreateTest(fixtureName, description);
            return _parentTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest CreateTest(string testName, string description = null)
        {
            ILoggerRepository loggerRepository = LogManager.CreateRepository($"{testName}Repository");
            ThreadContext.Properties["LogName"] = testName;
            log4net.Config.XmlConfigurator.Configure(loggerRepository);

            _childTest = _parentTest.CreateNode(testName, description);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest GetTest()
        {
            return _childTest ?? _parentTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ILog GetLogStream(string testName)
        {
            return LogManager.GetLogger($"{testName}Repository", "TestLogger");
        }
    }
}