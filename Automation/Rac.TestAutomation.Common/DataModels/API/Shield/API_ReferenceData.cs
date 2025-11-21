using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common.API
{
    /// <summary>
    /// Supports the Shield Home Insurance Address Status API calls.
    /// </summary>
    public class ApiAddressStatus
    {
        // Shield indicating if the property's wind band rating puts the home in cyclone risk.
        public bool isCycloneProneArea { get; set; }
        // Shield indicating if the property is in a temporary 'decline new coverage' list
        public bool isDeclined { get; set; }
    }
}
