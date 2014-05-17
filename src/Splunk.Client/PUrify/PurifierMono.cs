#if !WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PUrify
{
    class PurifierMono : IPurifier
    {
        private static Type uriType = typeof(Uri);
        private static FieldInfo mono_sourceField;
        private static FieldInfo mono_queryField;
        private static FieldInfo mono_pathField;
        private static FieldInfo mono_cachedToStringField;
        private static FieldInfo mono_cachedAbsoluteUriField;

        static PurifierMono()
        {
			mono_sourceField = uriType.GetRuntimeFields().Single(f=>f.Name ==  "source");
			mono_queryField = uriType.GetRuntimeFields().Single(f=>f.Name == "query");
			mono_pathField = uriType.GetRuntimeFields().Single(f=>f.Name == "path");
			mono_cachedToStringField = uriType.GetRuntimeFields().Single(f=>f.Name == "cachedToString");
			mono_cachedAbsoluteUriField = uriType.GetRuntimeFields().Single(f=>f.Name == "cachedAbsoluteUri");
        }

        public void Purify(Uri uri)
        {
            var source = (string)mono_sourceField.GetValue(uri);
            var uriInfo = new UriInfo(uri, source);
            mono_pathField.SetValue(uri, 
                uriInfo.Path.StartsWith("/") 
                    ? uriInfo.Path 
                    : "/" + uriInfo.Path
            );
            mono_queryField.SetValue(uri, uriInfo.Query);
            mono_cachedToStringField.SetValue(uri, uriInfo.Source);
            mono_cachedAbsoluteUriField.SetValue(uri, uriInfo.Source);
        }
    }
}
#endif
