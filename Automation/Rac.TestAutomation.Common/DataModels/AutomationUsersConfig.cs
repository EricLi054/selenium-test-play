using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    /// <summary>
    /// Supports loading the JSON config file used when
    /// running the automation that creates Shield users.
    /// </summary>
    public class AutomationUsersConfigElement
    {
        public int StartIndexInclusive { get; set; }
        public int EndIndexInclusive { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
    }
}
