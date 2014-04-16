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

// TODO:
// [ ] Contracts
// [ ] Documentation
// [ ] Properties & Methods
// [ ] Refactor Resource<TResource> to eliminate all abstract members because
//     there are too many places where they aren't relevant. Move them into
//     Entity<TEntity> and EntityCollection<TCollection, TEntity>

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

        internal Receiver(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

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
        /// they become available on <see cref="stream"/>.
        /// </remarks>
        public async Task SendAsync(Stream eventStream, ReceiverArgs args = null)
        {
            using (var content = new StreamContent(eventStream))
            {
                content.Headers.Add("x-splunk-input-mode", "streaming");

                using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, content, args))
                {
                    await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                }
            }
        }

        /// <summary>
        /// Asynchronously send raw event text to Splunk.
        /// </summary>
        /// <param name="eventText">
        /// Raw event text.
        /// <param name="args">
        /// Arguments identifying the event type and destination.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> object representing the event created by
        /// Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/GPLUVg">POST 
        /// receivers/simple</a> endpoint to obtain the <see cref="Result"/>
        /// that it returns.
        /// </remarks>
        public async Task<Result> SendAsync(string eventText, ReceiverArgs args = null)
        {
            using (var content = new StringContent(eventText))
            {
                using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, content, args))
                {
                    await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                    var reader = response.XmlReader;

                    foreach (var name in new string[] { "response", "results", "result" })
                    {
                        await reader.ReadAsync();

                        if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "results"))
                        {
                            throw new InvalidDataException(); // TODO: diagnostics
                        }
                    }

                    var result = new Result();
                    await result.ReadXmlAsync(reader);

                    foreach (var name in new string[] { "result", "results", "response" })
                    {
                        if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "results"))
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

        #endregion
    }
}
