using System;
using System.Collections.Generic;
using System.Text;

namespace NameParser
{
    [Flags]
    public enum Prefer
    {
        Default = 0,

        /// <summary>
        ///  For Issue #20, when the parser detects a Title and a Last with prefixes (eg, "Mr. Del Richards"), 
        ///  convert the prefix to a first name.
        ///  
        /// This can cause incorrect flipping of prefix to first (eg, "Mr. Van Rossum"), so you should use
        /// this flag only when you know your data has a first name.
        /// </summary>
        FirstOverPrefix = 1,
    }
}
