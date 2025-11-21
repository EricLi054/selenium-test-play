using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public class PaymentMethodAndTerm
    {
        /// <summary>
        /// String representation of the Shield IDs used for different
        /// collection methods of payment
        /// </summary>
        public string CollectionMethod { get; set; }
        /// <summary>
        /// String representation of the Shield IDs used for different
        /// payment terms
        /// </summary>
        public string PaymentTerm { get; set; }
    }
}
