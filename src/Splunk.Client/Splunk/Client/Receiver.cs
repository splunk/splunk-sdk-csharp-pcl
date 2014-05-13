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
//// [ ] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides a class for sending events to Splunk.
    /// </summary>
    public class Receiver : Resource<Receiver>
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ns"></param>
        internal Receiver(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Receiver"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.  Use the <see cref=
        /// "Service.Receiver"/> property to access a <see cref="Receiver"/>.
        /// </remarks>
        public Receiver()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously sends a stream of raw events to Splunk.
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/> used to send events to Splunk.
        /// </returns>
        /// <remarks>
        /// This method the <a href="http://goo.gl/zFKzMp">POST 
        /// receivers/stream</a> endpoint to send raw events to Splunk as
        /// they become available on <paramref name="eventStream"/>.
        /// </remarks>
        public async Task SendAsync(Stream eventStream, ReceiverArgs args = null)
        {
            using (var content = new StreamContent(eventStream))
            {
                content.Headers.Add("x-splunk-input-mode", "streaming");

                using (var response = await this.Context.PostAsync(this.Namespace, StreamReceiver, content, args))
                {
                    await response.EnsureStatusCodeAsync(HttpStatusCode.NoContent);
                }
            }
        }

        /// <summary>
        /// Asynchronously send raw event text to Splunk.
        /// </summary>
        /// <param name="eventText">
        /// Raw event text.
        /// </param>
        /// <param name="args">
        /// Arguments identifying the event type and destination.
        /// </param>
        /// <returns>
        /// A <see cref="SearchResult"/> object representing the event created by
        /// Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/GPLUVg">POST 
        /// receivers/simple</a> endpoint to obtain the <see cref=
        /// "SearchResult"/> that it returns.
        /// </remarks>
        public async Task<SearchResult> SendAsync(string eventText, ReceiverArgs args = null)
        {
            using (var content = new StringContent(eventText))
            {
                using (var response = await this.Context.PostAsync(this.Namespace, SimpleReceiver, content, args))
                {
                    await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                    var reader = response.XmlReader;

                    await reader.ReadAsync();

                    if (reader.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        throw new InvalidDataException(); // TODO: diagnostics
                    }

                    foreach (var name in new string[] { "response", "results", "result" })
                    {
                        await reader.ReadAsync();

                        if (!(reader.NodeType == XmlNodeType.Element && reader.Name == name))
                        {
                            throw new InvalidDataException(); // TODO: diagnostics
                        }
                    }

                    var result = new SearchResult();
                    await result.ReadXmlAsync(reader);

                    foreach (var name in new string[] { "result", "results", "response" })
                    {
                        if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == name))
                        {
                            throw new InvalidDataException(); // TODO: diagnostics
                        }
                        await reader.ReadAsync();
                    }

                    return result;
                }
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("receivers");

        static readonly ResourceName SimpleReceiver = new ResourceName(ClassResourceName, "simple");
        static readonly ResourceName StreamReceiver = new ResourceName(ClassResourceName, "stream");

        #endregion
    }
}
