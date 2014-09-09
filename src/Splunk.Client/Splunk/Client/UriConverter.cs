/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

//// TODO:
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Provides a converter to convert a string to a relative or absolute
    /// <see cref="Uri"/> instance.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ValueConverter{System.Uri}"/>
    sealed class UriConverter : ValueConverter<Uri>
    {
        /// <summary>
        /// The default <see cref="UriConverter"/> instance.
        /// </summary>
        public static readonly UriConverter Instance = new UriConverter();

        /// <summary>
        /// Converts the string representation of the <paramref name="input"/>
        /// object to a <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent a <see cref="Uri"/>.
        /// </exception>
        public override Uri Convert(object input)
        {
            var value = input as Uri;

            if (value != null)
            {
                return value;
            }

            if (Uri.TryCreate(input.ToString(), UriKind.RelativeOrAbsolute, out value))
            {
                if (Purifier != null)
                {
                    Purifier.Purify(value);
                }

                return value;
            }

            throw NewInvalidDataException(input);
        }

        #region Privates/internals

        static readonly IPurifier Purifier = Type.GetType("Mono.Runtime") != null ? new MonoPurifier() : null;

        #endregion

        #region Types

        /// <summary>
        /// Interface for purifier.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ValueConverter{System.Uri}"/>
        interface IPurifier
        {
            void Purify(Uri uri);
        }

        /// <summary>
        /// A mono purifier.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.UriConverter.IPurifier"/>
        class MonoPurifier : IPurifier
        {
            #region Methods

            /// <summary>
            /// Purifies the given document.
            /// </summary>
            /// <param name="uri">
            /// URI of the document.
            /// </param>
            public void Purify(Uri uri)
            {
                if (!uri.IsAbsoluteUri)
                {
                    return;
                }

                var source = (string)Source.GetValue(uri);
                var info = new UriInfo(uri, source);

                Path.SetValue(uri, info.Path);
                Query.SetValue(uri, info.Query);
                Fragment.SetValue(uri, info.Fragment);
                CachedToString.SetValue(uri, info.Source);
                CachedAbsoluteUri.SetValue(uri, info.Source);
            }

            #endregion

            #region Privates/internals

            static readonly FieldInfo Source;
            static readonly FieldInfo Path;
            static readonly FieldInfo Query;
            static readonly FieldInfo Fragment;
            static readonly FieldInfo CachedToString;
            static readonly FieldInfo CachedAbsoluteUri;

            static MonoPurifier()
            {
                IEnumerable<FieldInfo> fields = typeof(Uri).GetRuntimeFields();

                foreach (var field in fields)
                {
                    switch (field.Name)
                    {
                        case "source":
                            Source = field;
                            break;
                        case "path":
                            Path = field;
                            break;
                        case "query":
                            Query = field;
                            break;
                        case "fragment":
                            Fragment = field;
                            break;
                        case "cachedToString":
                            CachedToString = field;
                            break;
                        case "cachedAbsoluteUri":
                            CachedAbsoluteUri = field;
                            break;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Information about the uri.
        /// </summary>
        class UriInfo
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the Splunk.Client.UriConverter.UriInfo
            /// class.
            /// </summary>
            /// <param name="uri">
            /// URI of the document.
            /// </param>
            /// <param name="source">
            /// The source.
            /// </param>
            public UriInfo(Uri uri, string source)
            {
                var pathStart = source.IndexOf(uri.Authority, StringComparison.Ordinal) + uri.Authority.Length;
                var queryStart = source.IndexOf("?", StringComparison.Ordinal);
                var fragmentStart = source.IndexOf("#", StringComparison.Ordinal);

                var pathEnd = queryStart == -1 ? fragmentStart : queryStart;

                if (pathEnd == -1)
                {
                    pathEnd = source.Length + 1;
                }

                this.fragment = fragmentStart == -1 ? "" : source.Substring(fragmentStart);
                this.path = queryStart > -1 ? source.Substring(pathStart, pathEnd - pathStart) : source.Substring(pathStart);

                this.query = fragmentStart > -1 
                    ? source.Substring(queryStart, fragmentStart - queryStart)
                    : queryStart > -1
                    ? source.Substring(queryStart, source.Length - queryStart) 
                    : "";

                //// NOTE: We must fixup the source because Uri.ToString incorrectly represents some Uri instances,
                //// depending on how they are created. Here is an example. 
                ////
                ////     var uri1 = new Uri("http://localhost/");
                ////     var uri2 = new Uri(uri1, "?auth=12345");
                ////     Console.WriteLine(uri2.ToString());
                ////  
                //// Result: http://localhost?auth=12345

                this.source = source;

                if (pathStart < source.Length - 1 && source[pathStart] != '/')
                {
                    this.source = string.Concat("/", source);
                }
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the fragment.
            /// </summary>
            /// <value>
            /// The fragment.
            /// </value>
            public string Fragment
            {
                get { return this.fragment; }
            }

            /// <summary>
            /// Gets the full pathname of the file.
            /// </summary>
            /// <value>
            /// The full pathname of the file.
            /// </value>
            public string Path 
            { 
                get { return this.path; } 
            }

            /// <summary>
            /// Gets the query.
            /// </summary>
            /// <value>
            /// The query.
            /// </value>
            public string Query
            { 
                get { return this.query; }
            }

            /// <summary>
            /// Gets the source for the.
            /// </summary>
            /// <value>
            /// The source.
            /// </value>
            public string Source
            { 
                get { return this.source; }
            }

            #endregion

            #region Privates/internals

            readonly string fragment;
            readonly string path;
            readonly string query;
            readonly string source;

            #endregion
        }

        #endregion
    }
}
