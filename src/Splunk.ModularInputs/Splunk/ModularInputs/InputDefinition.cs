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

namespace Splunk.ModularInputs
{
    using System;
    using System.Collections.Generic;
    using Splunk.Client;

    /// <summary>
    /// Represents the specification of a modular input instance.
    /// </summary>
    public class InputDefinition : IDisposable
    {
        #region Properties

        /// <summary>
        /// The name of this instance.
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// A dictionary of all the parameters and their values for this instance.
        /// </summary>
        public IDictionary<string, Parameter> Parameters
        { get; set; }

        /// <summary>
        /// The hostname of the splunkd server that invoked this program.
        /// </summary>
        public string ServerHost
        { get; set; }

        /// <summary>
        /// The URI to reach the REST API of the splunkd instance that invoked this program.
        /// </summary>
        public string ServerUri
        { get; set; }

        /// <summary>
        /// A directory to write state that needs to be shared between executions of this program.
        /// </summary>
        public string CheckpointDirectory
        { get; set; }

        /// <summary>
        /// A REST API session key allowing the instance to make REST calls.
        /// </summary>
        public string SessionKey
        { get; set; }

        /// <summary>
        /// A Service instance connected to the Splunk instance that invoked this program.
        /// </summary>
        public Service Service
        {
            get
            {
                if (ServerUri == null)
                {
                    throw new InvalidOperationException("Cannot get a Service object without a ServerUri");
                }

                if (this.service == null)
                {
                    lock (gate)
                    {
                        if (this.service == null)
                        {
                            Uri uri;

                            if (!Uri.TryCreate(ServerUri, UriKind.Absolute, out uri))
                            {
                                throw new FormatException("Invalid server URI");
                            }

                            Splunk.Client.Scheme scheme;

                            if (uri.Scheme.Equals("https"))
                            {
                                scheme = Splunk.Client.Scheme.Https;
                            }
                            else if (uri.Scheme.Equals("http"))
                            {
                                scheme = Splunk.Client.Scheme.Http;
                            }
                            else
                            {
                                var text = "Invalid URI scheme: " + uri.Scheme + "; expected http or https";
                                throw new FormatException(text);
                            }

                            Service service = new Service(scheme, uri.Host, uri.Port);
                            service.SessionKey = this.SessionKey;
                            this.service = service;
                        }
                    }
                }

                return this.service;
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.service != null)
            {
                this.service.Dispose();
            }
        }

        #endregion

        #region Privates

        object gate = new object();
        Service service;

        #endregion
    }
}
