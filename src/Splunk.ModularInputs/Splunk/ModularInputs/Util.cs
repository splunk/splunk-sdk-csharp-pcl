using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splunk.ModularInputs
{
    internal static class Util
    {
        internal static bool ParseSplunkBoolean(string input)
        {
            if (input == null)
                throw new NullReferenceException("Cannot parse a Splunk boolean from null");
            string s = input.ToLowerInvariant().Trim();
            if (s.Equals("true") || s.Equals("t") || s.Equals("on") ||
                s.Equals("yes") || s.Equals("y") || s.Equals("1"))
                return true;
            else if (s.Equals("false") || s.Equals("f") || s.Equals("off") ||
                s.Equals("no") || s.Equals("n") || s.Equals("0"))
                return false;
            else
                throw new FormatException("expected a string coercible to bool; found '" + s + "'");
        }
    }
}
