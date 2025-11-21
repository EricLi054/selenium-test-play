using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        /// <summary>
        /// These are the Test Categories
        /// </summary>
        public class EFT
        {
            /// <summary>
            /// Valid mobile number that is reserved for test/entertainment use
            /// by ACMA: https://www.acma.gov.au/phone-numbers-use-tv-shows-films-and-creative-works
            /// </summary>
            public static readonly string SafeTestMobileNumber = "0491570006";
        }
    }
}
