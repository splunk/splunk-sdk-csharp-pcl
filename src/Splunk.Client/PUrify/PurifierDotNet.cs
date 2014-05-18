﻿#if !WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PUrify
{
    class PurifierDotNet : IPurifier
    {
        private static FieldInfo flagsField;
        private static FieldInfo infoField;
        private static FieldInfo stringField;
        private static FieldInfo infoStringField;
        private static FieldInfo moreInfoField;
        private static FieldInfo moreInfoAbsoluteUri;
        private static FieldInfo moreInfoPath;
        private static FieldInfo moreInfoQuery;

        static PurifierDotNet()
        {
            var uriType = typeof(Uri);
			/*
            flagsField = uriType.GetField("m_Flags", BindingFlags.NonPublic | BindingFlags.Instance);
            stringField = uriType.GetField("m_String", BindingFlags.NonPublic | BindingFlags.Instance);
            infoField = uriType.GetField("m_Info", BindingFlags.NonPublic | BindingFlags.Instance);
            var infoFieldType = infoField.FieldType;
            infoStringField = infoFieldType.GetField("String", BindingFlags.Public | BindingFlags.Instance);
            moreInfoField = infoFieldType.GetField("MoreInfo", BindingFlags.Public | BindingFlags.Instance);
            var moreInfoType = moreInfoField.FieldType;
            moreInfoAbsoluteUri = moreInfoType.GetField("AbsoluteUri", BindingFlags.Public | BindingFlags.Instance);
            moreInfoQuery = moreInfoType.GetField("Query", BindingFlags.Public | BindingFlags.Instance);
            moreInfoPath = moreInfoType.GetField("Path", BindingFlags.Public | BindingFlags.Instance);
*/
}

        //Code inspired by Rasmus Faber's solution in this post: http://stackoverflow.com/questions/781205/getting-a-url-with-an-url-encoded-slash
        public void Purify(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return;

            string paq = uri.PathAndQuery; // need to access PathAndQuery
            var abs = uri.AbsoluteUri; //need to access this as well the MoreInfo prop is initialized.
            ulong flags = (ulong)flagsField.GetValue(uri);
            flags &= ~((ulong)0x30); // Flags.PathNotCanonical|Flags.QueryNotCanonical
            flagsField.SetValue(uri, flags);
            object info = infoField.GetValue(uri);
            var source = (string)stringField.GetValue(uri);
            infoStringField.SetValue(info, source);
            object moreInfo = moreInfoField.GetValue(info);
            moreInfoAbsoluteUri.SetValue(moreInfo, source);
            var uriInfo = new UriInfo(uri, source);
            moreInfoPath.SetValue(moreInfo, uriInfo.Path);
            moreInfoQuery.SetValue(moreInfo, uriInfo.Query);
        }
    }
}
#endif

