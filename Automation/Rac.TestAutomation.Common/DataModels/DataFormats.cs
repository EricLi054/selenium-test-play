using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public class DataFormats
    {
        public const string DATE_FORMAT_FORWARD_HYPHENS = "dd-MM-yyyy";
        public const string DATE_FORMAT_REVERSE_HYPHENS = "yyyy-MM-dd";

        public const string DATE_FORMAT_FORWARD_FORWARDSLASH = "dd/MM/yyyy";
        public const string DATE_FORMAT_FORWARD_FORWARDSLASH_WITHSPACE = "dd / MM / yyyy";
        public const string DATE_ABBREVIATED_MONTH_FORWARD_FORWARDSLASH = "d/MMM/yyyy";
        public const string DATE_MONTH_YEAR_FORWARDSLASH = "MM/yy";

        public const string DATE_ABBREVIATED_MONTH_FORWARD_WHITESPACE = "d MMM yyyy";
        public const string DATE_FULL_MONTH_WITH_YEAR = "MMMM yyyy";

        public const string DATE_CALENDAR_PICKER_B2C = "dddd, MMMM dd, yyyy";
        public const string DATE_FULL_DATE_WITH_DAY = "dddd d MMMM yyyy";

        public const string TIME_FORMAT_12HR = "hhmmtt";
        public const string TIME_FORMAT_24HR = "HH:mm";
        public const string TIME_FORMAT_24HR_WITH_SECONDS = "HH:mm:ss";
    }
}
