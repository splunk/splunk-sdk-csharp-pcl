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
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for sending events to Splunk.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Endpoint"/>
    /// <seealso cref="T:Splunk.Client.ITransmitter"/>
    public class Transmitter : Endpoint, ITransmitter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Transmitter"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal Transmitter(Service service)
            : this(service.Context)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transmitter"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        protected internal Transmitter(Context context)
            : base(context, Namespace.Default, ClassResourceName)
        { }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task SendAsync(Stream eventStream, string indexName = null, TransmitterArgs args = null)
        {
            using (var content = new StreamContent(eventStream))
            {
                content.Headers.Add("x-splunk-input-mode", "streaming");
                var arguments = Enumerable.Empty<Argument>();
                
                if (indexName != null)
                {
                    arguments = arguments.Concat(new Argument[] { new Argument("index", indexName) });
                }

                if (args != null)
                {
                    arguments = arguments.Concat(args);
                }

                using (var response = await this.Context.PostAsync(this.Namespace, StreamReceiver, content, arguments).ConfigureAwait(false))
                {
                    await response.EnsureStatusCodeAsync(HttpStatusCode.NoContent).ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc/>
        public virtual async Task<SearchResult> SendAsync(string eventText, string indexName = null, TransmitterArgs args = null)
        {
            using (var content = new StringContent(eventText))
            {
                var arguments = Enumerable.Empty<Argument>();

                if (indexName != null)
                {
                    arguments = arguments.Concat(new Argument[] { new Argument("index", indexName) });
                }

                if (args != null)
                {
                    arguments = arguments.Concat(args);
                }

                using (var response = await this.Context.PostAsync(this.Namespace, SimpleReceiver, content, arguments).ConfigureAwait(false))
                {
                    await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                    var reader = response.XmlReader;

                    reader.Requires(await reader.MoveToDocumentElementAsync("response").ConfigureAwait(false));
                    await reader.ReadElementSequenceAsync("results", "result").ConfigureAwait(false);

                    var result = new SearchResult(SearchResultMetadata.Missing);

                    await result.ReadXmlAsync(reader).ConfigureAwait(false);
                    await reader.ReadEndElementSequenceAsync("result", "results", "response").ConfigureAwait(false);

                    return result;
                }
            }
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Name of the class resource.
        /// </summary>
        internal static readonly ResourceName ClassResourceName = new ResourceName("receivers");

        static readonly ResourceName SimpleReceiver = new ResourceName(ClassResourceName, "simple");
        static readonly ResourceName StreamReceiver = new ResourceName(ClassResourceName, "stream");

        #endregion
    }
}
