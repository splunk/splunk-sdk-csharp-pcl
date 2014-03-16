namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Eai : ExpandoAdapter
    {
        internal Eai(ExpandoObject expandoObject)
            : base(expandoObject)
        { }

        public Acl Acl
        {
            get { return this.GetValue("Acl", AclConverter.Default); }
        }
    }
}
