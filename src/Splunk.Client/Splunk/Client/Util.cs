using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splunk.Client
{
    public static class Util
    {
        public static Task IgnoreSyncContext(this Task t)
        {
            t.ConfigureAwait(false);
            return t;
        }

        public static Task<TResult> IgnoreSyncContext<TResult>(this Task<TResult> t)
        {
            t.ConfigureAwait(false);
            return t;
        }
    }
}
