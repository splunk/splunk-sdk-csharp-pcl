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
    public class InputDefinition
    {
        /// <summary>
        /// The name of this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A dictionary of all the parameters and their values for this instance.
        /// </summary>
        public IDictionary<string, Parameter> Parameters { get; set; }

        /// <summary>
        /// The hostname of the splunkd server that invoked this program.
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// The URI to reach the REST API of the splunkd instance that invoked this program.
        /// </summary>
        public string ServerUri { get; set; }

        /// <summary>
        /// A directory to write state that needs to be shared between executions of this program.
        /// </summary>
        public string CheckpointDirectory { get; set; }

        /// <summary>
        /// A REST API session key allowing the instance to make REST calls.
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// A Service instance connected to the Splunk instance that invoked this program.
        /// </summary>
        public Service Service
        {
            get
            {
                //// TODO: Getters should always return the same reference value
                //// 1. Either pre-allocate (e.g., in constructor) or create and then cache the service object here.
                //// 2. Convert this property to the GetService method => no question that the user owns it and is
                ////    responsible for calling Dispose
                //// Number two may be the better option

                if (ServerUri == null)
                    throw new NullReferenceException("Cannot get a Service object without ServerUri");
                Uri parsedServerUri;
                if (Uri.TryCreate(ServerUri, UriKind.Absolute, out parsedServerUri))
                {
                    Splunk.Client.Scheme scheme;
                    if (parsedServerUri.Scheme.Equals("https"))
                        scheme = Splunk.Client.Scheme.Https;
                    else if (parsedServerUri.Scheme.Equals("http"))
                        scheme = Splunk.Client.Scheme.Http;
                    else
                        throw new FormatException("Invalid URI scheme: " + parsedServerUri.Scheme + "; expected http or https");
                    Service service = new Service(
                        scheme: scheme,
                        host: parsedServerUri.Host,
                        port: parsedServerUri.Port
                    );
                    service.SessionKey = SessionKey;
                    return service;
                }
                else
                {
                    throw new FormatException("Invalid server URI");
                }
            }
        }
    }
}
